using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Android.Graphics;
using IWantTo.Client.Android.Utils;
using IWantTo.Client.Core.DataStorage;
using IWantTo.Client.Core.DataStorage.Model;
using IWantTo.Client.Core.Utils;

namespace IWantTo.Client.Android.Services
{
    /// <summary>
    /// Image services downloads images from web and store them into cache. This class is singleton.
    /// Cache is implemented in database as LRU cache. In Database we are persist full image and also scaled image.
    /// Scaled image is defined by SCALED_IMAGE_WIDTH and SCALED_IMAGE_HEIGHT parameters.
    /// </summary>
    public class ImageService
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _lock = new object();

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly ImageService _instance;

        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static ImageService Instance
        {
            get { return _instance; }
        }

        static ImageService()
        {
            _instance = new ImageService();
        }

        /// <summary>
        /// Constructs Image Service.
        /// </summary>
        protected ImageService()
        {
            // set SSL parameters
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
        }

        /// <summary>
        /// Event that fires whenever image is downloaded from the web or it fail.
        /// </summary>
        public event EventHandler ImageDownloaded;

        /// <summary>
        /// Fires event ImagesDownloaded.
        /// </summary>
        /// <param name="e"></param>
        protected void OnImageDownloaded(EventArgs e)
        {
            if (ImageDownloaded != null)
            {
                ImageDownloaded(this, e);
            }
        }

        /// <summary>
        /// Download image from the web. 
        /// </summary>
        /// <param name="imageUri">Images URI to be downloaded.</param>
        /// <param name="addData">Additional data. Note: it is not used in this time.</param>
        public void DownloadImage(string imageUri, long addData)
        {
            _log.InfoFormat("Downloading image: '{0}' with addData:{1}", imageUri, addData);

            // load image from web
            var webClientImageDownloader = new WebClient();
            webClientImageDownloader.OpenReadCompleted += webClientImageDownloader_OpenReadCompleted;
            webClientImageDownloader.BaseAddress = imageUri;
            // we are providing additional data in UserState
            webClientImageDownloader.OpenReadAsync(new Uri(imageUri, UriKind.Absolute), addData);
        }

        /// <summary>
        /// Occured when image is downloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webClientImageDownloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            lock (_lock)
            {
                try
                {
                    var client = sender as WebClient;
                    if (client != null)
                    {
                        var url = client.BaseAddress;

                        // get full image from web
                        var opts = new BitmapFactory.Options { InPurgeable = true, InInputShareable = true, InSampleSize = 1 };
                        var fullImage = BitmapFactory.DecodeStream(e.Result, new Rect(-1, -1, -1, -1), opts);

                        // get scaled image from full image
                        using (var previewImage = ImageUtils.Instance.GeneratePreviewBitmapFromBitmap(fullImage))
                        {
                            // retrieve additional data from user state
                            var addData = (long)e.UserState;

                            // insert into cache
                            InsertImageToCache(url, fullImage, previewImage, addData);

                            // dispose preview image
                            previewImage.Recycle();
                        }

                        // dispose full image
                        fullImage.Recycle();
                    }
                    else
                    {
                        _log.WarnFormat("Failed to load image from unknown reason!");
                    }
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("Failed to load image.", ex);
                }

                // throw ImageLoaded event if image is already downloaded or not
                OnImageDownloaded(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets full or preview image from cache. If missing null is returned.
        /// </summary>
        /// <param name="key">Image key, it can be URL or image Path.</param>
        /// <param name="preview">True for requesting preview image, false if we need full size image.</param>
        /// <returns>Bitmap or null if image doesn't exist in cache.</returns>
        public Bitmap TryGetImage(string key, bool preview)
        {
            Bitmap retBitmap = null;

            var record = IWantToDatabase.GetFileLruCache(key);
            if (record != null)
            {
                _log.InfoFormat("Image with key: '{0}' already stored in cache.", key);
                var data = (preview) ? record.Preview : record.Data;
                var bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                retBitmap = bitmap;
            }
            else
            {
                _log.WarnFormat("Image with key: '{0}' missing in cache.", key);
            }

            return retBitmap;
        }

        /// <summary>
        /// Stores downloaded image to database LruCache.
        /// </summary>
        /// <param name="key">Image key, it can be URL or Path.</param>
        /// <param name="fullImage">Full Bitmap.</param>
        /// <param name="previewImage">Preview Bitmap.</param>
        /// <param name="addData">Additional data. Note: it is not used in this time.</param>
        public void InsertImageToCache(string key, Bitmap fullImage, Bitmap previewImage, long addData)
        {
            var cacheRecord = new FileLruCache(key, ImageUtils.Instance.TransformBitmapToByteArray(fullImage), ImageUtils.Instance.TransformBitmapToByteArray(previewImage), addData);
            IWantToDatabase.InsertFileLruCache(cacheRecord);
        }

        /// <summary>
        /// Certificate validation callback.
        /// </summary>
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (error == SslPolicyErrors.None)
            {
                return true;
            }

            _log.WarnFormat("X509Certificate [{0}] Policy Error: '{1}'", cert.Subject, error.ToString());

            // Note: certifikate is always ok
            return true;
        }
    }
}
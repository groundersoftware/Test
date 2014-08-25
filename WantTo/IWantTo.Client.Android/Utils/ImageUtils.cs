using System.IO;
using Android.Graphics;
using Android.Util;

namespace IWantTo.Client.Android.Utils
{
    public class ImageUtils
    {
        private const int SCALED_ATTACHMENT_IMAGE_WIDTH = 800;
        private const int SCALED_ATTACHMENT_IMAGE_HEIGHT = 800;

        private const int SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_LDPI = 300;
        private const int SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_LDPI = 300;

        private const int SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_MDPI = 400;
        private const int SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_MDPI = 400;

        private const int SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_HDPI = 600;
        private const int SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_HDPI = 600;

        private const int SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_XHDPI = 800;
        private const int SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_XHDPI = 800;

        /// <summary>
        /// Following parameters are set according to android screen density.
        /// </summary>
        private int _previewWidth;
        private int _previewHeight;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly ImageUtils _instance;

        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static ImageUtils Instance
        {
            get { return _instance; }
        }

        static ImageUtils()
        {
            _instance = new ImageUtils();
        }

        /// <summary>
        /// Constructs Image Utils.
        /// </summary>
        protected ImageUtils()
        {
            // set default MDPI screen resolution
            _previewWidth = SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_MDPI;
            _previewHeight = SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_MDPI;
        }

        /// <summary>
        /// Set screen density for ImageUtils. According density parameters
        /// are all images resized for message list.
        /// </summary>
        /// <param name="density"></param>
        public void SetScreenDensity(DisplayMetricsDensity density)
        {
            switch (density)
            {
                case DisplayMetricsDensity.Low:
                    _previewWidth = SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_LDPI;
                    _previewHeight = SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_LDPI;
                    break;
                case DisplayMetricsDensity.Medium:
                    _previewWidth = SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_MDPI;
                    _previewHeight = SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_MDPI;
                    break;
                case DisplayMetricsDensity.High:
                    _previewWidth = SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_HDPI;
                    _previewHeight = SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_HDPI;
                    break;
                case DisplayMetricsDensity.Xhigh:
                    _previewWidth = SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_XHDPI;
                    _previewHeight = SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_XHDPI;
                    break;
                default:
                    _previewWidth = SCALED_MESSAGE_PREVIEW_IMAGE_WIDTH_MDPI;
                    _previewHeight = SCALED_MESSAGE_PREVIEW_IMAGE_HEIGHT_MDPI;
                    break;
            }
        }

        /// <summary>
        /// Generates scaled Bitmap from local Attachments (e.g. from gallery images, camera and signature component).
        /// It is used for resizing big image after taken by camera, gallery and signature.
        /// In default is resized to 600 x 600 pixels.
        /// </summary>
        /// <param name="path">Path to stored local Image file.</param>
        /// <returns>Scaled Bitmap.</returns>
        public Bitmap GenerateFullBitmapFromLocalAttachment(string path)
        {
            return DecodeSampledBitmapFromFile(path, SCALED_ATTACHMENT_IMAGE_WIDTH, SCALED_ATTACHMENT_IMAGE_HEIGHT);
        }

        /// <summary>
        /// Generates scaled Preview Bitmap from local Attachments (e.g. from gallery images, camera and signature component).
        /// It is used in Messages List.
        /// In default is resized to 400 x 400 pixels.
        /// </summary>
        /// <param name="path">Path to stored local Image file.</param>
        /// <returns>Scaled Bitmap.</returns>
        public Bitmap GeneratePreviewBitmapFromWebAttachment(string path)
        {
            return DecodeSampledBitmapFromFile(path, _previewWidth, _previewHeight);
        }

        /// <summary>
        /// Generates scaled Preview Bitmap from provided Full Bitmap (it can be image downloaded from web and also resized local file)
        /// It is used in Messages List.
        /// In default is resized to 400 x 400 pixels.
        /// </summary>
        /// <param name="image">Bitmap image.</param>
        /// <returns>Scaled Bitmap.</returns>
        public Bitmap GeneratePreviewBitmapFromBitmap(Bitmap image)
        {
            return DecodeSampledBitmapFromByteArray(TransformBitmapToByteArray(image), _previewWidth, _previewHeight);
        }

        /// <summary>
        /// Transform Bitmap to Byte Array.
        /// </summary>
        /// <param name="image">Bitmap Image.</param>
        /// <returns>Bitmap Byte Array.</returns>
        public byte[] TransformBitmapToByteArray(Bitmap image)
        {
            byte[] bitmapByteArray;
            using (var stream = new MemoryStream())
            {
                image.Compress(Bitmap.CompressFormat.Png, 0, stream);
                bitmapByteArray = stream.ToArray();
            }

            return bitmapByteArray;
        }

        /// <summary>
        /// Decodes large bitmap from file efficiently.
        /// </summary>
        /// <remarks>
        /// See <see cref="http://docs.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently/">Load Large Bitmaps Efficiently</see> for more information.
        /// </remarks>
        /// <param name="path">Bitmap location.</param>
        /// <param name="reqWidth">Requested width.</param>
        /// <param name="reqHeight">Requested height.</param>
        /// <returns>Scaled version of the input bitmap.</returns>
        private Bitmap DecodeSampledBitmapFromFile(string path, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new BitmapFactory.Options { InJustDecodeBounds = true };

            using (var dispose = BitmapFactory.DecodeFile(path, options)) { }

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeFile(path, options);
        }

        /// <summary>
        /// Decodes large bitmap from stream efficiently.
        /// </summary>
        /// <remarks>
        /// See <see cref="http://docs.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently/">Load Large Bitmaps Efficiently</see> for more information.
        /// </remarks>
        /// <param name="data">Byte array Bitmap.</param>
        /// <param name="reqWidth">Requested width.</param>
        /// <param name="reqHeight">Requested height.</param>
        /// <returns>Scaled version of the input bitmap.</returns>
        private Bitmap DecodeSampledBitmapFromByteArray(byte[] data, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new BitmapFactory.Options { InJustDecodeBounds = true };

            using (var dispose = BitmapFactory.DecodeByteArray(data, 0, data.Length, options)) { }

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeByteArray(data, 0, data.Length, options);
        }

        /// <summary>
        /// Calculates sample size.
        /// </summary>
        /// <param name="options">Decoding options.</param>
        /// <param name="reqWidth">Requested width.</param>
        /// <param name="reqHeight">Requested height.</param>
        /// <returns>Sample size.</returns>
        private int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = (float)options.OutHeight;
            var width = (float)options.OutWidth;
            var inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                inSampleSize = width > height ? height / reqHeight : width / reqWidth;
            }

            return (int)inSampleSize;
        }
    }
}
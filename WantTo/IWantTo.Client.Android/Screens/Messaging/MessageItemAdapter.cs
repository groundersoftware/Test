using System.Collections.Generic;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using IWantTo.Client.Android.Base;
using IWantTo.Client.Android.Services;
using IWantTo.Client.Core.DataStorage.Model;
using Object = Java.Lang.Object;

namespace IWantTo.Client.Android.Screens.Messaging
{
    public class MessageItemAdapter : BaseAdapter
    {
        /// <summary>List of messages</summary>
        private readonly List<ChatMessage> _items;

        /// <summary>Activity from which is adapter created</summary>
        private readonly BaseActivity _activity;

        /// <summary>Image service</summary>
        private readonly ImageService _imagesService;

        /// <summary>User name</summary>
        private readonly string _userName;

        /// <summary>
        /// Creates adapter with all messages. Messages are ordered by created date.
        /// </summary>
        /// <param name="activity">Activity from which is adapter created</param>
        /// <param name="messages">Ordered list of messages.</param>
        public MessageItemAdapter(BaseActivity activity, List<ChatMessage> messages) : base() 
        {
            _items = messages;
            _activity = activity;
            _imagesService = ImageService.Instance;

            // refresh list after all images downloaded
            _imagesService.ImageDownloaded += (sender, e) => _activity.RunOnUiThread(NotifyDataSetChanged);

            _userName = "unknown";

        }

        public override Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public ChatMessage this[int position]
        {
            get
            {
                return _items[position];
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this[position];

            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.MessagingListRowLayout, null);

            // if message another user ?
            var rightAlignment = (item.User.Equals(_userName));

            // Layout parameters for complete message widget
            var wholeMessageLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            // Layout parameters for name text
            var userNameLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            // Layout parameters for timestamp text
            var userTimestampLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            // Layouts
            var mainLayout = view.FindViewById<LinearLayout>(Resource.Id.MessagingListRow_MainLayout);
            var bubbleTextLayout = view.FindViewById<LinearLayout>(Resource.Id.MessagingListRow_BubbleTextLayout);
            var bubbleImageLayout = view.FindViewById<RelativeLayout>(Resource.Id.MessagingListRow_BubbleImageLayout);
            var wholeMessageLayout = view.FindViewById<LinearLayout>(Resource.Id.MessagingListRow_WholeMessageLayout);
            var messageImageView = view.FindViewById<ImageView>(Resource.Id.MessagingListRow_MessageImage);

            if (rightAlignment)
            {
                wholeMessageLayoutParams.Gravity = GravityFlags.Right;
                mainLayout.SetPadding(DpToPx(40), 0, DpToPx(10), 0);
                bubbleTextLayout.SetBackgroundResource(Resource.Drawable.chat_bubble_right);
                userNameLayoutParams.Gravity = GravityFlags.Right;
                userTimestampLayoutParams.Gravity = GravityFlags.Right;
            }
            else
            {
                wholeMessageLayoutParams.Gravity = GravityFlags.Left;
                mainLayout.SetPadding(DpToPx(10), 0, DpToPx(40), 0);
                bubbleTextLayout.SetBackgroundResource(Resource.Drawable.chat_bubble_left);
                userNameLayoutParams.Gravity = GravityFlags.Left;
                userTimestampLayoutParams.Gravity = GravityFlags.Left;
            }

            wholeMessageLayout.LayoutParameters = wholeMessageLayoutParams;
            bubbleImageLayout.LayoutParameters = wholeMessageLayoutParams;

            // Message timestamp
            var messageTimeStamp = view.FindViewById<TextView>(Resource.Id.MessagingListRow_MessageTimeStampText);
            messageTimeStamp.Text = item.Created;
            messageTimeStamp.SetTextColor(_activity.Resources.GetColor((rightAlignment) ? Resource.Color.messagelistrow_bubble_datetime_right : Resource.Color.messagelistrow_bubble_datetime_left));
            messageTimeStamp.LayoutParameters = userTimestampLayoutParams;

            // Message user name
            var userName = view.FindViewById<TextView>(Resource.Id.MessagingListRow_UserName);
            userName.Text = item.User;
            userName.LayoutParameters = userNameLayoutParams;
            if (rightAlignment)
            {
                userName.SetPadding(0, 0, DpToPx(5), DpToPx(2));
            }
            else
            {
                userName.SetPadding(DpToPx(6), 0, 0, DpToPx(2));
            }

            // Message text
            var messageText = view.FindViewById<TextView>(Resource.Id.MessagingListRow_MessageText);
            messageText.Text = item.Message;
            messageText.SetTextColor(_activity.Resources.GetColor((rightAlignment) ? Resource.Color.messagelistrow_bubble_text_right : Resource.Color.messagelistrow_bubble_text_left));

            // show or hide text/image
            if (string.IsNullOrEmpty(item.AttachmentUrl))
            {
                bubbleTextLayout.Visibility = ViewStates.Visible;
                bubbleImageLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                // Layout parameters for timestamp text
                var imageTimestampLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                imageTimestampLayoutParams.AddRule((rightAlignment) ? LayoutRules.AlignRight : LayoutRules.AlignLeft, messageImageView.Id);
                imageTimestampLayoutParams.AddRule(LayoutRules.CenterInParent);
                imageTimestampLayoutParams.AddRule(LayoutRules.AlignBottom, messageImageView.Id);

                var imageMessageTimeStamp = view.FindViewById<TextView>(Resource.Id.MessagingListRow_MessageTimeStampImage);
                imageMessageTimeStamp.LayoutParameters = imageTimestampLayoutParams;
                if (rightAlignment)
                {
                    imageMessageTimeStamp.SetPadding(0, 0, DpToPx(20), DpToPx(8));
                }
                else
                {
                    imageMessageTimeStamp.SetPadding(DpToPx(20), 0, 0, DpToPx(10));
                }

                bubbleTextLayout.Visibility = ViewStates.Gone;
                bubbleImageLayout.Visibility = ViewStates.Visible;

                if (!string.IsNullOrEmpty(item.AttachmentUrl))
                {
                    // try load image from cache
                    var image = _imagesService.TryGetImage(item.AttachmentUrl, true);

                    if (image != null)
                    {
                        // generate and show message image
                        GenerateMessageImage(image, messageImageView, rightAlignment);

                        imageMessageTimeStamp.Text = item.Created;
                    }
                    else
                    {
                        // set 'downloading' icon
                        messageImageView.SetImageResource(Resource.Drawable.ic_missing_image);


                        // starts loading image in background
                        _imagesService.DownloadImage(item.AttachmentUrl, -1);       // additional data are not used now
                    }
                }
            }

            return view;
        }

        /// <summary>
        /// Generates message image in messages layout.
        /// </summary>
        /// <param name="image">Message image</param>
        /// <param name="messageImageView">ImageView in layout as place for image.</param>
        /// <param name="rightAlignment">True if message is on right side, false if image will be on left side.</param>
        private void GenerateMessageImage(Bitmap image, ImageView messageImageView, bool rightAlignment)
        {
            // compose image with bubble image together
            var r = _activity.Resources;
            var layers = new Drawable[2];
            layers[0] = new BitmapDrawable(r, image);
            layers[1] = (rightAlignment) ? r.GetDrawable(Resource.Drawable.chat_bubble_right_images) : r.GetDrawable(Resource.Drawable.chat_bubble_left_images);
            var layerDrawable = new LayerDrawable(layers);

            // set image
            messageImageView.SetImageDrawable(layerDrawable);
        }

        /// <summary>
        /// Converts DP dimensions to Pixels dimensions according to device density.
        /// </summary>
        /// <param name="dp">Dimension in DP.</param>
        /// <returns>Dimension in pixels.</returns>
        private int DpToPx(int dp)
        {
            return (int)(dp * _activity.Resources.DisplayMetrics.Density + 0.5f);
        }

        public override int Count
        {
            get
            {
                return _items.Count;
            }
        }
    }
}
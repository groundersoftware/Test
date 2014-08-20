using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using IWantTo.Client.Android.Base;
using IWantTo.Client.Core.DataStorage.Model;
using IWantTo.Client.Core.Utils;
using Java.Interop;

namespace IWantTo.Client.Android.Screens.Messaging
{
    [Activity(Name = "iwantto.activity.messagingactivity")]
    public class MessagingActivity : BaseActivity
    {
        /// <summary>Logger</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>List view with messages.</summary>
        private ListView _messagesListView;

        private ImageButton _sendButton;
        private ImageButton _attachmentButton;
        private EditText _msgEdit;
        private TextView _emptyMessage;

        #region Overriden methods

        /// <inheritdoc />
        protected override void OnCreate(Bundle bundle)
        {
            MenuItemsMask = ((uint)MenuItemEnum.Settings | (uint)MenuItemEnum.About);
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MessagingLayout);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _messagesListView = FindViewById<ListView>(Resource.Id.MessagingLayout_MessageList);
            _messagesListView.ItemClick += OnListItemClick;

            Title = Resources.GetString(Resource.String.MessagingLayout_Title);

            _msgEdit = FindViewById<EditText>(Resource.Id.MessagingLayout_AddCommentMessage);
            _msgEdit.AfterTextChanged += (sender, args) => UpdateSendAndAttachmentButton();

            _emptyMessage = FindViewById<TextView>(Resource.Id.MessagingLayout_EmptyMessage);
            _sendButton = FindViewById<ImageButton>(Resource.Id.MessagingLayout_SendButton);
            _attachmentButton = FindViewById<ImageButton>(Resource.Id.MessagingLayout_AttachmentButton);
            
            UpdateSendAndAttachmentButton();
        }

        /// <summary>
        /// Reload Job from Database and actualize Activity
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            // refresh activity
            RefreshActivity();
        }

        #endregion

        [Export]
        public void OnSend(View button)
        {
            if (!string.IsNullOrEmpty(_msgEdit.Text))
            {
                // send message here
                _log.WarnFormat("Sending mesaages is not implemneted yet.");
            }

            // clears message
            _msgEdit.Text = String.Empty;

            // refresh activity
            RefreshActivity();
        }

        protected void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                var mla = listView.Adapter as MessageItemAdapter;
                if (mla != null)
                {
                    var m = mla[e.Position];

                    _log.InfoFormat("Click on {0} message.", m);
                }
            }
        }

        /// <summary>
        /// Refresh whole activity. E.g. reloads Job Messages from database, updates all UI widgets.
        /// </summary>
        private void RefreshActivity()
        {
            var messages = new List<ChatMessage>();
            messages.Add(new ChatMessage("1", "unknown", "", "Test message 1", DateTime.UtcNow));
            messages.Add(new ChatMessage("2", "Majga", "", "Test message 2", DateTime.UtcNow));

            // Updates UI in UI Thread
            RunOnUiThread(() => UpdateUI(messages));

        }

        /// <summary>
        /// Updates UI objects. 
        /// Note: Call only from UI Thread !
        /// </summary>
        private void UpdateUI(List<ChatMessage> messages)
        {
            var adapter = new MessageItemAdapter(this, messages);
            _messagesListView.Adapter = adapter;
            _emptyMessage.Visibility = (adapter.Count > 0) ? ViewStates.Invisible : ViewStates.Visible;
            _messagesListView.SetSelection(adapter.Count - 1);
        }

        /// <summary>
        /// Hide or Shows Send button and Attachment button.
        /// </summary>
        private void UpdateSendAndAttachmentButton()
        {
            _attachmentButton.Visibility = (string.IsNullOrEmpty(_msgEdit.Text)) ? ViewStates.Visible : ViewStates.Gone;
            _sendButton.Visibility = (!string.IsNullOrEmpty(_msgEdit.Text)) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
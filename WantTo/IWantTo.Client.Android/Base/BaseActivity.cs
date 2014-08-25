using Android.App;
using Android.Content;
using Android.OS;
using IWantTo.Client.Android.Screens.About;
using IWantTo.Client.Android.Screens.Messaging;
using IWantTo.Client.Android.Screens.Preferences;
using IWantTo.Client.Android.Services;
using IWantTo.Client.Core.Utils;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;

namespace IWantTo.Client.Android.Base
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : SherlockFragmentActivity
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Mask for Menu items in Options.</summary>
        protected uint MenuItemsMask { get; set; }

        /// <summary>Broadcast receiver for each activity.</summary>
        private Receiver _broadcastReceiver;
        private bool _isReceiverRegistered;

        /// <summary>Menu.</summary>
        private static readonly MenuItemEntry[] MENU_ENTRIES =
        {
            new MenuItemEntry(MenuItemEnum.Exit, Resource.String.MenuExit, 0)
        };

        /// <summary>
        /// Constructor for Job Dispatch Activity.
        /// </summary>
        public BaseActivity()
        {
            MenuItemsMask = 0xFFFFFFFF;
        }

        protected override void OnCreate(Bundle bundle)
        {
            _log.InfoFormat("Creating '{0}' Activity.", LocalClassName);
            base.OnCreate(bundle);

            // start broadcast receiver in start, don't move it from here
            _broadcastReceiver = new Receiver(this);
        }

        protected override void OnStart()
        {
            _log.InfoFormat("Starting '{0}' Activity", LocalClassName);
            base.OnStart();

        }

        protected override void OnResume()
        {
            _log.InfoFormat("Resuming '{0}' Activity", LocalClassName);
            base.OnResume();

            // register broadcast receiver
            if (!_isReceiverRegistered)
            {
                RegisterReceiver(_broadcastReceiver, new IntentFilter(AppConstants.Broadcast));
                _isReceiverRegistered = true;
            }
        }

        protected override void OnPause()
        {
            _log.InfoFormat("Pausing '{0}' Activity", LocalClassName);
            base.OnPause();

            // register broadcast receiver
            if (_isReceiverRegistered)
            {
                UnregisterReceiver(_broadcastReceiver);
                _isReceiverRegistered = false;
            }
        }

        /// <summary>
        /// Stops Flurry session.
        /// Note: if method is overridden in activity don't forgot call this base method.
        /// </summary>
        protected override void OnStop()
        {
            _log.InfoFormat("Stopping '{0}' Activity", LocalClassName);
            base.OnStop();

        }

        /// <summary>
        /// Also UnBound Activity from Job Dispatch Service and removing created handlers.
        /// Note: if method is overrided in activity don't forgot call this base method.
        /// </summary>
        protected override void OnDestroy()
        {
            _log.InfoFormat("Destroying '{0}' Activity", LocalClassName);
            base.OnDestroy();

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (!PressMenuItem((MenuItemEnum)item.ItemId))
            {
                // close activity in other case - it is Back button implementation on Action Bar
                Finish();
            }

            return true;
        }

        /// <summary>
        /// Activate a particular menu.
        /// </summary>
        /// <param name="menuItem">Menu Item.</param>
        /// <returns>True if menu was found and activated, otherwise false.</returns>
        protected bool PressMenuItem(MenuItemEnum menuItem)
        {
            switch (menuItem)
            {
                case MenuItemEnum.About:
                {
                    var intent = new Intent(this, typeof(AboutActivity));
                    StartActivity(intent);
                    return true;
                }
                case MenuItemEnum.Settings:
                {
                    var intent = new Intent(this, typeof(PreferencesActivity));
                    StartActivity(intent);
                    return true;
                }
                case MenuItemEnum.Chat:
                {
                    var intent = new Intent(this, typeof(MessagingActivity));
                    StartActivity(intent);
                    return true;
                }
                case MenuItemEnum.Exit:
                {
                    MenuExit();
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var order = 0;
            foreach (var e in MENU_ENTRIES)
            {
                if (((uint)e.Id & MenuItemsMask) != 0)
                {
                    var item = menu.Add(0, (int)e.Id, order, e.Label);
                    item.SetIcon(e.Icon);
                }
                order++;
            }
            return true;
        }

        protected virtual void MenuExit()
        {
            StopService(new Intent(this, typeof(BackgroundService)));

            var returnIntent = new Intent();
            returnIntent.PutExtra("OnExit", true);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }

        /// <summary>
        /// Receive new GPS location.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        protected virtual void OnLocationUpdate(double latitude, double longitude)
        {
            // override in base class if you need
        }

        /// <summary>
        /// Receiver for all notifications.
        /// </summary>
        [BroadcastReceiver]
        private sealed class Receiver : BroadcastReceiver
        {
            private readonly BaseActivity _activity;

            public Receiver()
            {

            }

            public Receiver(BaseActivity activity)
            {
                _activity = activity;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (_activity != null)
                {
                    if (intent.Extras != null)
                    {
                        if (intent.Extras.ContainsKey(AppConstants.LocationUpdateLatitude))
                        {
                            var latitude = intent.GetDoubleExtra(AppConstants.LocationUpdateLatitude, 0);
                            var longitude = intent.GetDoubleExtra(AppConstants.LocationUpdateLongitude, 0);

                            // inform also about User Authorization after successful connection
                            _activity.OnLocationUpdate(latitude, longitude);
                        }
                    }
                }
            }
        }
    }
}
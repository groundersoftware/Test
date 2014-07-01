using Android.App;
using Android.Views;
using Android.Widget;
using IWantTo.Client.Android.Screens.Base;
using Object = Java.Lang.Object;

namespace IWantTo.Client.Android.Screens.Drawer
{
    public class DrawerListAdapter : BaseAdapter
    {
        /// <summary>List of drawer entries.</summary>
        private static readonly DrawerItem[] DRAWER_ENTRIES =
        {
            new DrawerItem(MenuItemEnum.Settings, Resource.String.MenuSettings, Resource.Drawable.ic_drawer_settings),
            new DrawerItem(MenuItemEnum.About, Resource.String.MenuAbout, Resource.Drawable.ic_drawer_about),
            new DrawerItem(MenuItemEnum.Exit, Resource.String.MenuExit, Resource.Drawable.ic_drawer_logout)
        };

        /// <summary>Activity where the adapter is used.</summary>
        private readonly Activity _activity;

        /// <summary>
        /// Creates Drawer List Adapter.
        /// </summary>
        /// <param name="activity">Activity where the adapter is used.</param>
        public DrawerListAdapter(Activity activity) : base()
        {
            _activity = activity;
        }

        /// <inheritdoc />
        public override Object GetItem(int position)
        {
            return DRAWER_ENTRIES[position];
        }

        /// <inheritdoc />
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <inheritdoc />
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.DrawerListItem, null);
            var icon = view.FindViewById<ImageView>(Resource.Id.DrawerListRow_Icon);
            var label = view.FindViewById<TextView>(Resource.Id.DrawerListRow_Label);
            var drawerItem = DRAWER_ENTRIES[position];

            icon.SetImageResource(drawerItem.Icon);
            label.SetText(drawerItem.Label);

            return view;
        }

        /// <inheritdoc />
        public override int Count
        {
            get { return DRAWER_ENTRIES.Length; }
        }
    }
}
using Android.App;
using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using IWantTo.Client.Android.Screens.Base;
using IWantTo.Client.Android.Screens.Drawer;
using IWantTo.Client.Core.Services;

namespace IWantTo.Client.Android.Screens.Main
{
    [Activity(Name = "iwantto.activity.mainactivity", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {
        /// <summary>Map fragment</summary>
        private MapFragment _mapFragment;

        /// <summary>Menu drawer</summary>
        private DrawerLayout _drawer;
        private ListView _drawerList;
        private MyActionBarDrawerToggle _drawerToggle;

        protected override void OnCreate(Bundle bundle)
        {
            // set application menu according to particular activity
            MenuItemsMask = ((uint)MenuItemEnum.Exit);

            base.OnCreate(bundle);

            SupportActionBar.SetDisplayShowTitleEnabled(false);

            _mapFragment = MapFragment.NewInstance();
            var tx = FragmentManager.BeginTransaction();
            tx.Add(Resource.Id.map, _mapFragment);
            tx.Commit();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerList = FindViewById<ListView>(Resource.Id.left_drawer);
            _drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

            _drawerList.Adapter = new DrawerListAdapter(this);
            _drawerList.ItemClick += (sender, args) => SelectDrawerMenuItem(args.Position);

            // DrawerToggle is the animation that happens with the indicator next to the ActionBar icon. 
            _drawerToggle = new MyActionBarDrawerToggle(this, _drawer, Resource.Drawable.ic_navigation_drawer, Resource.String.DrawerOpen, Resource.String.DrawerClose);
            _drawer.SetDrawerListener(_drawerToggle);


            var dbVersion = ConfigurationService.Instance.DatabaseVersion;
        }

        /// <inheritdoc />
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        protected override void OnResume()
        {

            var mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            var map = mapFrag.Map;
            if (map != null)
            {
                // The GoogleMap object is ready to go.
                System.Console.WriteLine("Mam mapu!!!");
            }


            base.OnResume();
        }

        /// <summary>
        /// Handler for selecting menu item in drawer.
        /// </summary>
        /// <param name="position">Position of selected menu item.</param>
        private void SelectDrawerMenuItem(int position)
        {
            _drawerList.SetItemChecked(position, true);
            _drawer.CloseDrawers();

            // find activated menu item and start a particular action
            var selectedItem = (DrawerItem)_drawerList.GetItemAtPosition(position);
            PressMenuItem(selectedItem.Id);
        }

    }
}


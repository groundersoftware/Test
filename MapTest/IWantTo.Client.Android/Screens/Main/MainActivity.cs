﻿using Android.App;
using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using IWantTo.Client.Android.Base;
using IWantTo.Client.Android.Drawer;
using IWantTo.Client.Android.Services;
using IWantTo.Client.Core.Services;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;

namespace IWantTo.Client.Android.Screens.Main
{
    [Activity(Name = "iwantto.activity.mainactivity", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {
        /// <summary>Location service.</summary>
        private LocationService _locationService;

        /// <summary>Map fragment</summary>
        private MapFragment _mapFragment;

        /// <summary>Menu drawer</summary>
        private DrawerLayout _drawerLayout;
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

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);
            _drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

            _drawerList.Adapter = new DrawerListAdapter(this);
            _drawerList.ItemClick += (sender, args) => SelectDrawerMenuItem(args.Position);

            // DrawerToggle is the animation that happens with the indicator next to the ActionBar icon. 
            _drawerToggle = new MyActionBarDrawerToggle(this, _drawerLayout, Resource.Drawable.ic_navigation_drawer, Resource.String.DrawerOpen, Resource.String.DrawerClose);
            _drawerLayout.SetDrawerListener(_drawerToggle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            var dbVersion = ConfigurationService.Instance.DatabaseVersion;

            // creates location service
            _locationService = new LocationService(this);
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

            // enable position tracking if activity is visible
            _locationService.EnablePositionTracking(true);
            base.OnResume();
        }

        protected override void OnPause()
        {
            // disable position tracking if activity is not visible
            _locationService.EnablePositionTracking(false);
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            _locationService.EnablePositionTracking(false);
            base.OnDestroy();
        }

        /// <summary>
        /// Overriden method for handling Home button for left drawer menu.
        /// </summary>
        /// <param name="item">Seleected menu item.</param>
        /// <returns>True if selected menu item was processed or not.</returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // handle Home button in this activity for closing and opening menu drawer
            if (item.ItemId == global::Android.Resource.Id.Home)
            {
                if (_drawerLayout.IsDrawerOpen((int)GravityFlags.Left))
                {
                    _drawerLayout.CloseDrawer((int)GravityFlags.Left);
                }
                else
                {
                    _drawerLayout.OpenDrawer((int)GravityFlags.Left);
                }

                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Handler for selecting menu item in drawer.
        /// </summary>
        /// <param name="position">Position of selected menu item.</param>
        private void SelectDrawerMenuItem(int position)
        {
            _drawerList.SetItemChecked(position, true);
            _drawerLayout.CloseDrawer((int)GravityFlags.Left);

            // find activated menu item and start a particular action
            var selectedItem = (DrawerItem)_drawerList.GetItemAtPosition(position);
            PressMenuItem(selectedItem.Id);
        }

    }
}


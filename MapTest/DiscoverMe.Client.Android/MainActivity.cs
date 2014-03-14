using System;

using Android.App;
using Android.Gms.Maps.Model;
using Android.Content;
using Android.Gms.Maps;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DiscoverMe.Client.Android;

namespace DiscoverMe.Client.Android
{
    [Activity(Label = "MapTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        /// <summary>Map fragment</summary>
        private MapFragment _mapFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _mapFragment = MapFragment.NewInstance();
            var tx = FragmentManager.BeginTransaction();
            tx.Add(Resource.Id.map, _mapFragment);
            tx.Commit();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
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
    }
}


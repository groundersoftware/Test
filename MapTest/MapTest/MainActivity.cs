using System;

using Android.App;
using Android.Gms.Maps.Model;
using Android.Content;
using Android.Gms.Maps;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MapTest
{
    [Activity(Label = "MapTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity 
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var _myMapFragment = MapFragment.NewInstance();
            FragmentTransaction tx = FragmentManager.BeginTransaction();
            tx.Add(Resource.Id.map, _myMapFragment);
            tx.Commit();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //var mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
            //var map = mapFrag.Map;
            //if (map != null)
            //{
            //    // The GoogleMap object is ready to go.
            //}
        }
    }
}


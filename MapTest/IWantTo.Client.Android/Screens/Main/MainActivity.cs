using Android.App;
using Android.Gms.Maps;
using Android.OS;
using IWantTo.Client.Android.Screens.Base;
using IWantTo.Client.Core.DataStorage;

namespace IWantTo.Client.Android.Screens.Main
{
    [Activity(Name = "iwantto.activity.mainactivity", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {
        /// <summary>Map fragment</summary>
        private MapFragment _mapFragment;

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


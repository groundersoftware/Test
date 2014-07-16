using Android.App;
using Android.OS;
using Android.Widget;
using IWantTo.Client.Android.Screens.Base;

namespace IWantTo.Client.Android.Screens.About
{
    [Activity(Name = "iwantto.activity.aboutactivity")]
    public class AboutActivity : BaseActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // set application menu according to particular activity
            MenuItemsMask = (uint) MenuItemEnum.None;

            // set activity title
            Title = Resources.GetString(Resource.String.AboutLayout_Title);

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AboutLayout);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var productValue = FindViewById<TextView>(Resource.Id.AboutProductValue);
            var versionValue = FindViewById<TextView>(Resource.Id.AboutVersionValue);
            var copyrightValue = FindViewById<TextView>(Resource.Id.AboutCopyrightValue);

            var packageManager = PackageManager.GetPackageInfo(PackageName, 0);

            productValue.Text = Resources.GetString(Resource.String.ApplicationName);
            versionValue.Text = packageManager.VersionName;
            copyrightValue.Text = "© 2014 Grounder Software, a.s.";
        }
    }
}
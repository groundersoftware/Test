using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using IWantTo.Client.Core.Utils;

using Xamarin.ActionbarSherlockBinding.App;

namespace IWantTo.Client.Android.Screens.Base
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : SherlockFragmentActivity
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
        }
    }
}
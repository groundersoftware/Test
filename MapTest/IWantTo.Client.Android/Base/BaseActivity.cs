using Android.App;
using Android.Content;
using Android.OS;
using IWantTo.Client.Android.Screens.About;
using IWantTo.Client.Android.Screens.Preferences;
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
            base.OnCreate(bundle);

            // Create your application here
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
            var returnIntent = new Intent();
            returnIntent.PutExtra("OnExit", true);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }
    }
}
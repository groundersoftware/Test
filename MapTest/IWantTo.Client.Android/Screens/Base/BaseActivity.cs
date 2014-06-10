using Android.App;
using Android.Content;
using Android.OS;

using IWantTo.Client.Core.Utils;

using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;

namespace IWantTo.Client.Android.Screens.Base
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
            switch ((MenuItemEnum)item.ItemId)
            {
                case MenuItemEnum.Exit:
                {
                    MenuExit();
                    return true;
                }
                default:
                {
                    // close activity in other case - it is Back button implementation on Action Bar
                    Finish();
                    return true;
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
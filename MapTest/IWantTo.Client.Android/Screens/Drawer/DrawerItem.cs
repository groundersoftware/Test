using IWantTo.Client.Android.Screens.Base;

namespace IWantTo.Client.Android.Screens.Drawer
{
    public class DrawerItem : Java.Lang.Object
    {
        /// <summary>Gets Id of drawer item.</summary>
        public MenuItemEnum Id { get; private set; }

        /// <summary>Resource ID of String associated with item.</summary>
        public int Label { get; private set; }

        /// <summary>Gets Resource Id of drawer item icon.</summary>
        public int Icon { get; private set; }

        /// <summary>
        /// Construct Drawer menu item.
        /// </summary>
        /// <param name="id">Drawer Item Id.</param>
        /// <param name="label">Resource ID of String associated with item.</param>
        /// <param name="icon">Resource ID of Icon associated with item.</param>
        public DrawerItem(MenuItemEnum id, int label, int icon)
        {
            Id = id;
            Label = label;
            Icon = icon;
        }
    }
}
namespace IWantTo.Client.Android.Screens.Base
{
    public class MenuItemEntry
    {
        public MenuItemEnum Id { get; private set; }
        public int Label { get; private set; }
        public int Icon { get; private set; }

        /// <summary>
        /// Construct Menu Entry.
        /// </summary>
        /// <param name="id">Menu Item Id.</param>
        /// <param name="label">Label.</param>
        /// <param name="icon">Icon.</param>
        public MenuItemEntry(MenuItemEnum id, int label, int icon)
        {
            Id = id;
            Label = label;
            Icon = icon;
        }
    }
}
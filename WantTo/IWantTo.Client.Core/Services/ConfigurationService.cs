using IWantTo.Client.Core.DataStorage;
using IWantTo.Client.Core.Utils;

namespace IWantTo.Client.Core.Services
{
    /// <summary>
    /// Configuration class.
    /// </summary>
    public class ConfigurationService : BaseDatabaseService
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Singleton instance.</summary>
        private static volatile ConfigurationService _instance;

        /// <summary>Database Version.</summary>
        public int DatabaseVersion
        {
            get { return ReadConfigurationValue<int>("DatabaseVersion"); }
            set { UpdateConfigurationValue("DatabaseVersion", value); }
        }

        /// <summary>Language.</summary>
        public string Language
        {
            get { return ReadConfigurationValue<string>("Language"); }
            set { UpdateConfigurationValue("Language", value); }
        }

        /// <summary>True if notification sounds is enabled, otherwise false.</summary>        
        public bool NotificationSound
        {
            get { return ReadConfigurationValue<bool>("NotificationSound"); }
            set { UpdateConfigurationValue("NotificationSound", value); }
        }

        /// <summary>True if notification vibrating is enabled, otherwise false.</summary>        
        public bool NotificationVibrating
        {
            get { return ReadConfigurationValue<bool>("NotificationVibrating"); }
            set { UpdateConfigurationValue("NotificationVibrating", value); }
        }

        /// <summary>
        /// Returns Configuration instance. It is singleton.
        /// </summary>
        public static ConfigurationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigurationService();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Default constructor is deprecated.
        /// </summary>
        private ConfigurationService() : base(_log)
        {
        }

        /// <summary>
        /// Read configuration value from database.
        /// </summary>
        /// <param name="columnName">Configuration column name.</param>
        /// <returns>Value from user configuration.</returns>
        private T ReadConfigurationValue<T>(string columnName)
        {
            var ret = default(T);

            UseDatabase(() =>
            {
                ret = IWantToDatabase.GetConfiguration<T>(columnName);
                _log.InfoFormat("Readed Configuration '{0}' = '{1}'", columnName, ret);
            });

            return ret;
        }

        /// <summary>
        /// Insert configuration value to database.
        /// </summary>
        /// <param name="columnName">Configuration column name.</param>
        /// <param name="value">New value.</param>
        private void UpdateConfigurationValue<T>(string columnName, T value)
        {

            UseDatabase(() =>
            {
                IWantToDatabase.UpdateConfiguration(columnName, value);
                _log.InfoFormat("Written Configuration '{0}' = '{1}'", columnName, value);
            });
        }
    }
}

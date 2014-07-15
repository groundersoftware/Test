using IWantTo.Client.Core.DataStorage.Contract;
using IWantTo.Client.Core.DataStorage.SQLite;

namespace IWantTo.Client.Core.DataStorage.Model
{
    /// <summary>
    /// Configuration parameters for whole application. This table contains configuration for all users. Configuration pre user are stored in UserConfiguration table.
    /// </summary>
    [Table("Configuration")]
    public class Configuration : BusinessEntityBase
    {
        #region Constructors

        public Configuration() { }

        /// <summary>
        /// Creates Configuration with default settings.
        /// </summary>
        /// <param name="databaseVersion">Database version.</param>
        public Configuration(int databaseVersion)
        {
            DatabaseVersion = databaseVersion;

            // default values
            Language = "en";
            NotificationSound = true;
            NotificationVibrating = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>Actual version of database model.</summary>
        [NotNull]
        public int DatabaseVersion { get; set; }

        /// <summary>Language.</summary>
        [NotNull]
        public string Language { get; set; }

        /// <summary>True if notification sounds is enabled, otherwise false.</summary>      
        [NotNull]
        public bool NotificationSound { get; set; }

        /// <summary>True if notification vibrating is enabled, otherwise false.</summary>        
        [NotNull]
        public bool NotificationVibrating { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator ==(Configuration left, Configuration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Configuration left, Configuration right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(IBusinessEntity other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.GetType() != typeof(Configuration))
            {
                return false;
            }

            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Configuration))
            {
                return false;
            }

            return Equals(obj as Configuration);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Id.GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("Configuration[Id={0},DatabaseVersion={1},Language='{2}',NotificationSound='{3}',NotificationVibrating='{4}']", Id, DatabaseVersion, Language, NotificationSound, NotificationVibrating);
        }

        #endregion Methods
    }
}

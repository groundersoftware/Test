using IWantTo.Client.Core.DataStorage.SQLite;

namespace IWantTo.Client.Core.DataStorage.Contract
{
    /// <summary>
    /// Business entity base class. Provides the Id property.
    /// </summary>
    public abstract class BusinessEntityBase : IBusinessEntity
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <remarks>
        /// Due to our <see cref="Orm.SqlType">change</see> in SQLite.NET (ORM) this column will essentially become INTEGER PRIMARY KEY which is an alias for ROWID.
        /// In addition it will be auto incremented. See <see cref="http://www.sqlite.org/autoinc.html"/>.
        /// </remarks>
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public static bool operator ==(BusinessEntityBase left, BusinessEntityBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BusinessEntityBase left, BusinessEntityBase right)
        {
            return !Equals(left, right);
        }

        public virtual bool Equals(IBusinessEntity other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
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

            if (obj.GetType() != typeof(IBusinessEntity))
            {
                return false;
            }

            return Equals(obj as BusinessEntityBase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Id.GetHashCode();
            }
        }
    }
}

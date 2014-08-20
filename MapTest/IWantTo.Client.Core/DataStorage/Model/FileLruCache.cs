using System;
using IWantTo.Client.Core.DataStorage.Contract;
using IWantTo.Client.Core.DataStorage.SQLite;

namespace IWantTo.Client.Core.DataStorage.Model
{
    [Table("FileLruCache")]
    public class FileLruCache : BusinessEntityBase
    {
        #region Constructors

        public FileLruCache() { }

        /// <summary>
        /// Construct FileLruCache record in database.
        /// </summary>
        /// <param name="key">File key.</param>
        /// <param name="data">File data.</param>
        /// <param name="preview">Preview file data if exist or null.</param>
        /// <param name="addData">Additional data if necessary. Note: not used now.</param>
        public FileLruCache(string key, byte[] data, byte[] preview, long addData)
        {
            long dataLength = data.Length;
            long previewLength = (preview == null) ? 0 : preview.Length;

            Key = key;
            Data = data;
            Preview = preview;
            Length = dataLength + previewLength;
            Timestamp = DateTime.UtcNow;
            AddData = addData;
        }

        #endregion Constructors

        #region Properties

        /// <summary>File Key.</summary>
        [NotNull]
        [Indexed(Unique = true)]
        public string Key { get; protected set; }

        /// <summary>Full Image Data.</summary>
        [NotNull]
        public byte[] Data { get; protected set; }

        /// <summary>Preview Image Data used in Messages List.</summary>
        public byte[] Preview { get; protected set; }

        /// <summary>File Length.</summary>
        [NotNull]
        public long Length { get; protected set; }

        /// <summary>Timestamp of file.</summary>
        [NotNull]
        public DateTime Timestamp { get; protected set; }

        /// <summary>Additional data if necessary. Note: not used now.</summary>
        public long AddData { get; protected set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return string.Format("FileLruCache[Key:'{0}',Length:{1},Timestamp:{2},AddData:{3}]", Key, Length, Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"), AddData);
        }

        #endregion Methods
    }
}

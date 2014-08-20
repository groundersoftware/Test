using System;
using IWantTo.Client.Core.DataStorage.Contract;
using IWantTo.Client.Core.DataStorage.SQLite;

namespace IWantTo.Client.Core.DataStorage.Model
{
    /// <summary>
    /// Chat Message table.
    /// </summary>
    [Table("ChatMessage")]
    public class ChatMessage : BusinessEntityBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChatMessage() { }

        /// <summary>
        /// Creates Job Message.
        /// </summary>
        /// <param name="uniqueId">Message Unique Id. This Id is the same on server and device.</param>
        /// <param name="user">Name of user who creates a message.</param>
        /// <param name="attachmentUrl">Link to web place from where attachment will be loaded.</param>
        /// <param name="message">Text of message.</param>
        /// <param name="createdDate">Created Date.</param>
        public ChatMessage(string uniqueId, string user, string attachmentUrl, string message, DateTime createdDate)
        {
            UniqueId = uniqueId;
            User = user;
            AttachmentUrl = attachmentUrl;
            Message = message;
            CreatedDate = createdDate;
        }

        #endregion Constructors

        #region Properties

        /// <summary>Message Unique Id. This Id is the same on server and device.</summary>
        [Unique]
        public string UniqueId { get; set; }

        /// <summary>User name who created a message.</summary>
        public string User { get; set; }

        /// <summary>Link to web place from where attachment will be loaded.</summary>
        public string AttachmentUrl { get; set; }

        /// <summary>Text of message.</summary>
        public string Message { get; set; }

        /// <summary>Created Date.</summary>
        public DateTime CreatedDate { get; set; }

        [Ignore]
        public string Created
        {
            get
            {
                var createdUtcDate = DateTime.SpecifyKind(CreatedDate, DateTimeKind.Utc);
                var createdLocalDate = createdUtcDate.ToLocalTime();
                return createdLocalDate.ToString("dd. MM. yyyy - HH:mm");
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            var createdDate = CreatedDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return string.Format("ChatMessage[Id:{0},UniqueId:'{1}',User:'{2}',AttachmentUrl:'{3}',Message='{4}',UTC CreatedDate='{5}']", Id, UniqueId, User, AttachmentUrl, Message, createdDate);
        }

        #endregion Methods
    }
}

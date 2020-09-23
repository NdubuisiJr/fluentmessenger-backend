namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The structure of a returned Notification
    /// </summary>
    public class NotificationDto {
        /// <summary>
        /// The Notification Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The owner's Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Text of the notification
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Indicates if the user has read it or not
        /// </summary>
        public bool IsViewed { get; set; }
    }
}

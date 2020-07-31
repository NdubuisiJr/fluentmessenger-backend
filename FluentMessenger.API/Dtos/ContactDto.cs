namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The object that represents a contact
    /// </summary>
    public class ContactDto {
        /// <summary>
        /// The contact's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The contact's full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The mobile number associated with the contacts
        /// </summary>
        public string Number { get; set; }
    }
}

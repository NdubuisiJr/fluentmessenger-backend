namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// An object that represent a message template
    /// </summary>
    public class TemplateDto {

        /// <summary>
        /// The template Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The templated message
        /// </summary>
        public string Message { get; set; }
    }
}

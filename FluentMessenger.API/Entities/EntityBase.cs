using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Entities {
    public class EntityBase {
        [Key]
        public int Id { get; set; }
    }
}

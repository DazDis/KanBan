using System.ComponentModel.DataAnnotations;

namespace Server.Database.Entities
{
    public class UserEntity
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // связь
        public List<TaskEntity?> Tasks { get; set; } = new();
        public List<TeamEntity?> Teams { get; set; } = new();
    }
}
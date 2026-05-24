using System.ComponentModel.DataAnnotations;

namespace Server.Database.Entities
{
    public class TeamEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Color { get; set; }
        public List<UserEntity?> Users { get; set; } = new();
        public List<TaskEntity> Tasks { get; set; } = new();
    }
}
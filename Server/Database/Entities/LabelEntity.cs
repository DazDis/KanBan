using System.ComponentModel.DataAnnotations;

namespace Server.Database.Entities
{
    public class LabelEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Color { get; set; }
        // связь
        public List<TaskEntity> Tasks { get; set; } = new();
    }
}
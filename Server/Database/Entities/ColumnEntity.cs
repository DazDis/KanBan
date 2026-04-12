using System.ComponentModel.DataAnnotations;

namespace Server.Database.Entities
{
    public class ColumnEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } 
    }
}
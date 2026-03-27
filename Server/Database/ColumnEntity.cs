using System.ComponentModel.DataAnnotations;

namespace Server.DataBase
{
    public class ColumnEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
    }
}
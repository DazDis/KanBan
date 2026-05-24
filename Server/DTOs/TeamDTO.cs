using System.ComponentModel.DataAnnotations;

namespace Server.DataBase
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Color { get; set; }
        public List<int> UserIds { get; set; } = new();


    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaClient.DataBase
{
    public class TeamModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> UserIds { get; set; } = new();

    }
}
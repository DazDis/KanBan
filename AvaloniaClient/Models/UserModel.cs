using System.Collections.Generic;

namespace AvaloniaClient.DataBase
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<int> TeamIds { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPlanner.Model
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public List<User> Collaborators { get; set; } = new List<User>();
        public User Creator { get; set; }
        public List<string> Participants {get; set;} = new List<string>();
    }
}
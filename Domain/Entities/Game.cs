using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Platform { get; set; }
        public string Status { get; set; }
        public double? Rating { get; set; }
        public int HoursPlayed { get; set; }
    }
}

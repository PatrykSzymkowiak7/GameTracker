using GameTracker.Domain.Enums;

namespace GameTracker.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public GameStatus Status { get; set; }
        public double? Rating { get; set; }
        public int HoursPlayed { get; set; }
        public int? DeveloperId { get; set; }
        public Developer? Developer { get; set; }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<GamePlatform> Platforms { get; set; } = new List<GamePlatform>();
    }
}

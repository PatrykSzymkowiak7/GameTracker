using GameTracker.Domain.Enums;

namespace GameTracker.Application.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public Platform Platform { get; set; }

        public GameStatus Status { get; set; }

        public double? Rating { get; set; }

        public int HoursPlayed { get; set; }
    }
}

using GameTracker.Domain.Enums;

namespace GameTracker.Application.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public double? Rating { get; set; }

        public int HoursPlayed { get; set; }

        public GameStatus Status { get; set; }

        public DeveloperDto? Developer { get; set; }

        public IEnumerable<GenreDto> Genres { get; set; } = [];

        public IEnumerable<GamePlatformDto> Platforms { get; set; } = [];
    }
}

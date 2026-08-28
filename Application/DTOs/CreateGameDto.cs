using GameTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GameTracker.Application.DTOs
{
    public class CreateGameDto
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public int? DeveloperId { get; set; }

        public List<int> GenreIds { get; set; } = [];

        public List<int> PlatformIds { get; set; } = [];

        public GameStatus Status { get; set; }

        [Range(0,10)]
        public double? Rating { get; set; }

        [Range(0,int.MaxValue)]
        public int HoursPlayed { get; set; }
    }
}

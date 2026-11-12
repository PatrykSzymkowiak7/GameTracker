using GameTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GameTracker.Application.DTOs
{
    public class CreateGameDto
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int? DeveloperId { get; set; }

        [Required]
        public List<int> GenreIds { get; set; } = [];

        [Required]
        public List<int> PlatformIds { get; set; } = [];

        [Required]
        public GameStatus Status { get; set; }

        [Range(0,10)]
        public double? Rating { get; set; }

        [Range(0,int.MaxValue)]
        public int HoursPlayed { get; set; }
    }
}

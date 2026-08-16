using GameTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.DTOs
{
    public class UpdateGameDto
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Genre { get; set; } = string.Empty;

        public Platform Platform { get; set; }

        public GameStatus Status { get; set; }

        [Range(0, 10)]
        public double? Rating { get; set; }

        [Range(0, int.MaxValue)]
        public int HoursPlayed { get; set; }
    }
}

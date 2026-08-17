using GameTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Application.DTOs
{
    public class GameQueryDto
    {
        public GameStatus? Status { get; set; }
        public Platform? Platform { get; set; }
        public string? Genre { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

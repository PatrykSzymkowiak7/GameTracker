using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Domain.Entities
{
    public class GamePlatform
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private readonly List<Game> _games = [];
        public IReadOnlyCollection<Game> Games => _games.AsReadOnly();

        private GamePlatform() { }

        public GamePlatform(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Platform name can't be empty");

            Name = name;
        }
    }
}

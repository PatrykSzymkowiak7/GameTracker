using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Domain.Entities
{
    public class Developer
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private readonly List<Game> _games = [];
        public ICollection<Game> Games => _games.AsReadOnly();

        private Developer() { }

        public Developer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Developer name cannot be empty");

            Name = name;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Developer name cannot be empty");

            Name = name;
        }
    }
}

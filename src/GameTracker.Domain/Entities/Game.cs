using GameTracker.Domain.Enums;
using System.Collections;

namespace GameTracker.Domain.Entities
{
    public class Game
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public GameStatus Status { get; private set; }
        public double? Rating { get; private set; }
        public int HoursPlayed { get; private set; }
        public int? DeveloperId { get; private set; }
        public Developer? Developer { get; private set; }
        public int UserId { get; private set; }

        private readonly List<Genre> _genres = new();
        public IReadOnlyCollection<Genre> Genres => _genres.AsReadOnly();

        private readonly List<GamePlatform> _platforms = new();
        public IReadOnlyCollection<GamePlatform> Platforms => _platforms.AsReadOnly();

        private Game() { }

        public Game(string title, GameStatus status, double? rating, int hoursPlayed, Developer? developer,
            IEnumerable<Genre> genres, IEnumerable<GamePlatform> platforms, int userId)
        {
            Title = title;
            Status = status;
            Rating = rating;
            HoursPlayed = hoursPlayed;
            Developer = developer;
            DeveloperId = developer?.Id ?? null;
            _genres.AddRange(genres);
            _platforms.AddRange(platforms);
            UserId = userId;
        }

        public void AddGenres(IEnumerable<Genre> genres)
        {
            foreach(var genre in genres)
                AddGenre(genre);
        }

        public void AddGenre(Genre genre)
        {
            ArgumentNullException.ThrowIfNull(genre);

            if (!_genres.Any(g => g.Id == genre.Id))
                _genres.Add(genre);
        }

        public void AddPlatforms(IEnumerable<GamePlatform> platforms)
        {
            foreach (var platform in platforms)
                _platforms.Add(platform);
        }

        public void AddPlatform(GamePlatform platform)
        {
            ArgumentNullException.ThrowIfNull(platform);

            if (!_platforms.Any(p => p.Id == platform.Id))
                _platforms.Add(platform);
        }

        public void SetGenres(IEnumerable<Genre> genres)
        {
            ArgumentNullException.ThrowIfNull(genres);

            if (genres.Count() == 0)
                throw new ArgumentException("Genres cannot be empty");

            _genres.Clear();
            _genres.AddRange(genres);
        }

        public void SetPlatforms(IEnumerable<GamePlatform> platforms)
        {
            ArgumentNullException.ThrowIfNull(platforms);

            if (Platforms.Count() == 0)
                throw new ArgumentException("Platforms cannot be empty");

            _platforms.Clear();

            foreach(var platform in platforms)
                AddPlatform(platform);
        }

        public void Rename(string title)
        {
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title.Trim();
        }

        public void SetRating(double? rating)
        {
            if (rating < 0 || rating > 10)
                throw new ArgumentOutOfRangeException(nameof(rating));
        }

        public void ChangeStatus(GameStatus status)
        {
            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException(nameof(status));

            Status = status;
        }

        public void SetHoursPlayed(int hoursPlayed)
        {
            if (hoursPlayed < 0)
                throw new ArgumentOutOfRangeException(nameof(hoursPlayed));

            HoursPlayed = hoursPlayed;
        }

        public void UpdateDetails(
            string title,
            GameStatus status,
            double? rating,
            int hoursPlayed)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null");

            ArgumentNullException.ThrowIfNull(status);

            if (hoursPlayed < 0)
                throw new ArgumentOutOfRangeException(nameof(hoursPlayed));

            if(rating != null && (rating < 0 || rating > 10))
                    throw new ArgumentOutOfRangeException(nameof(rating));

            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException(nameof(status));

            Title = title.Trim();
            Status = status;
            Rating = rating;
            HoursPlayed = hoursPlayed;
        }

        public void SetDeveloper(Developer? developer)
        {
            DeveloperId = developer?.Id;
            Developer = developer;
        }
    }
}

namespace GameTracker.Application.Exceptions
{
    public class GameConflictException : Exception
    {
        public GameConflictException(string message) 
            : base(message)
        { 
        }
    }
}

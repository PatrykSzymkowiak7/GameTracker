namespace GameTracker.Application.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(int id) 
            : base($"Game with id {id} was not found.")
        {
        }
    }
}

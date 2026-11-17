namespace Application.CustomExceptions
{
    public class ConnectionStringNotFoundException : Exception
    {
        public ConnectionStringNotFoundException(string message) : base(message) { }
    }
}

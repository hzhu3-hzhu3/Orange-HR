namespace HRManagement.Application.Exceptions;
public class InvalidStateTransitionException : Exception
{
    public InvalidStateTransitionException(string message) : base(message)
    {
    }
    public InvalidStateTransitionException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

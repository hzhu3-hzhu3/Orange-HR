namespace HRManagement.Application.Exceptions;
public class CircularManagerReferenceException : Exception
{
    public CircularManagerReferenceException(string message) : base(message)
    {
    }
    public CircularManagerReferenceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

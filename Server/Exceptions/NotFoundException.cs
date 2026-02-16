namespace Server.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
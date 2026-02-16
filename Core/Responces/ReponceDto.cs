namespace Core.Responces;

public class ReponceDto
{
    public required StatusReponce StatusReponce { get; init; }
    public required string ErrorMessage { get; init; }

    public override string ToString()
    {
        if (StatusReponce == StatusReponce.Ok)
        {
            return "Ok";    
        }
        else if (StatusReponce == StatusReponce.NotFound)
        {
            return $"NotFound, {ErrorMessage}";
        }
        else if (StatusReponce == StatusReponce.Error)
        {
            return $"Error, {ErrorMessage}";
        }

        return string.Empty;
    }
}
using Core.Enums;

namespace Core.Reponces;

public class ReponceDto
{
    public required TypeResponces TypeResponce { get; init; }
    public required string Error { get; init; }

    public override string ToString()
    {
        return $"{TypeResponce}, {Error}";
    }
}

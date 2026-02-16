using Core.Enums;

namespace Core.Requests;

public class RequestDto
{
    public required TypeCommands TypeCommand { get; init; }
    public required string Body { get; init; }
}

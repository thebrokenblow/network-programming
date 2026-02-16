using Core.Model;
using System.Text;
using System.Text.Json;

namespace Client.Common;

public class RequestBuilder
{
    private readonly StringBuilder stringRequestBuilder = new();

    public RequestBuilder AddTypeCommand(TypeCommand typeCommand)
    {
        stringRequestBuilder.AppendLine(typeCommand.ToString());

        return this;
    }

    public RequestBuilder AddBody<T>(T objectBody)
    {
        var serializeObjectBody = JsonSerializer.Serialize(objectBody);
        stringRequestBuilder.Append(serializeObjectBody);

        return this;
    }

    public string Build()
    {
        return stringRequestBuilder.ToString();
    }
}
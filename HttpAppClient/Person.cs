namespace HttpAppClient;

public class Person
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string SecondName { get; set; }

    public override string ToString()
    {
        return $"FirstName: {FirstName}, SecondName: {SecondName}";
    }
}

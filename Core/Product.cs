namespace Core;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Description: {Description}, Price: {Price}";
    }
}
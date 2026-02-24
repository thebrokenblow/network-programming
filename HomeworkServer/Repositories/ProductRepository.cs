using HomeworkServer.Model;

namespace HomeworkServer.Repositories;

public class ProductRepository
{
    private int maxId = 0;
    private readonly List<Product> _products = [];
    
    public void Add(Product product)
    {
        product.Id = ++maxId;
        _products.Add(product);
    }

    public List<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public bool Update(Product updatedProduct)
    {
        var existingPerson = GetById(updatedProduct.Id);

        existingPerson.Name = updatedProduct.Name;
        existingPerson.Description = updatedProduct.Description;
        existingPerson.Price = updatedProduct.Price;


        return true;
    }

    public bool Delete(int id)
    {
        var product = GetById(id);
        return _products.Remove(product);
    }

    public void DeleteAll()
    {
        _products.Clear();
    }
}
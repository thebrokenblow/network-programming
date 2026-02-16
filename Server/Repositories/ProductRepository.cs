using Core;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Repositories;

public class ProductRepository : IDisposable
{
    private readonly ApplicationContext _context;
    
    public ProductRepository(ApplicationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var products = await _context.Products.AsNoTracking()
                                              .ToListAsync();

        return products;
    }

    public async Task CreateAsync(Product product)
    {
        await _context.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        var updatingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

        updatingProduct.Name = product.Name;
        updatingProduct.Description = product.Description;
        updatingProduct.Price = product.Price;

        _context.Update(updatingProduct);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        _context.Remove(product);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
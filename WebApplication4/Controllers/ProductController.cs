﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Model;

namespace WebApplication4.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController: ControllerBase
{
    private readonly DataContext _context;

    public ProductController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
    {
        return await _context.Product.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct([FromBody] Product product)
    {
       
        _context.Product.Add(product);
        await _context.SaveChangesAsync();
        return Created();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);
        
        _context.Product.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Product>> UpdatePatchProduct(int id, [FromBody] float price)
    {
        var product = await _context.Product.FindAsync(id);
        product.Price = price;
        await _context.SaveChangesAsync();
        return Ok(product);
    }
    //<-------------------------------------------------------------------------------------------->
    [HttpGet("sorted")]
    public async Task<ActionResult<IEnumerable<Product>>> GetSortedProducts([FromBody] string order)
    {
        if (order.ToLower() == "desc")
        {
            return await _context.Product.OrderByDescending(p => p.Price).ToListAsync();
        }
        return await _context.Product.OrderBy(p => p.Price).ToListAsync();
    }
    
    [HttpDelete("range-delete")]
    public async Task<ActionResult> DeleteProducts([FromBody] List<int> ids)
    {
        var products = await _context.Product.Where(p => ids.Contains(p.Id)).ToListAsync();

        if (products == null || products.Count == 0)
        {
            return NotFound();
        }

        _context.Product.RemoveRange(products);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    [HttpGet("search")]
    public async Task<ActionResult<Product>> GetProductByDescriptionWord([FromBody] string keyword)
    {
        var product = await _context.Product.FirstOrDefaultAsync(p => p.Discription.Contains(keyword));

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }
    
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByUser([FromBody] int userId)
    {
        var user = await _context.User.Include(u => u.Products).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.Products);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Product.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }
    
    
    
    
    
    
    
    
    
    
    
}
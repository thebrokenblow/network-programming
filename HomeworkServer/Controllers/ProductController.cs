using HomeworkServer.Model;
using HomeworkServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeworkServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository repository) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var people = repository.GetAll();

        return Ok(people);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var person = repository.GetById(id);
        return Ok(person);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product person)
    {
        repository.Add(person);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }


    [HttpPut]
    public IActionResult Update([FromBody] Product person)
    {
        repository.Update(person);

        return Ok(person);
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        repository.Delete(id);

        return NoContent();
    }

    [HttpDelete]
    public IActionResult DeleteAll()
    {
        repository.DeleteAll();

        return NoContent();
    }
}
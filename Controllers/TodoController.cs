using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuTodo.Controllers
{
	[ApiController]
	[Route("v1")]
	public class TodoController : ControllerBase
	{



		[HttpGet]
		[Route("todos")]
		public async Task<IActionResult> Get([FromServices] AppDbContext context)
		{
			var todos = await context.Todos.AsNoTracking().ToListAsync();
			return Ok(todos);
		}
		//public async Task<ActionResult<List<Todo>>> Get([FromServices] AppDbContext context)
		//{
		//	var todos = context.Todos.AsNoTracking().ToListAsync();
		//	return Ok(todos);
		//}

		[HttpGet]
		[Route("todos/{id}")]
		public async Task<IActionResult> GetById([FromServices] AppDbContext context, [FromRoute] int id)
		{
			var todos = await context.Todos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
			return todos != null ? Ok(todos) : NotFound();
		}

		[HttpPost("todos")]
		public async Task<IActionResult> PostAsync([FromServices] AppDbContext context, [FromBody] CreateTodoViewModel todo)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var model = new Todo
			{
				Data = DateTime.Now,
				Done = false,
				Title = todo.Title
			};

			try
			{
				await context.Todos.AddAsync(model);
				await context.SaveChangesAsync();
				return Created("v1/todos/{model.Id}", model);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("todos/{id}")]
		public async Task<IActionResult> PutAsync([FromServices] AppDbContext context, [FromBody] CreateTodoViewModel todo, [FromRoute] int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var model = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);

			if (model == null)
			{
				return NotFound();
			}
			try
			{
				model.Title = todo.Title;
				context.Todos.Update(model);
				await context.SaveChangesAsync();
				return Ok(model);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("todos/{id}")]
		public async Task<IActionResult> DeleteAsync([FromServices] AppDbContext context, [FromRoute] int id)
		{
			var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
			if (todo == null)
			{
				return NotFound();
			}
			try
			{
				context.Todos.Remove(todo);
				await context.SaveChangesAsync();
				return Ok();

			}
			catch (Exception ex) { return BadRequest(ex.Message); }
		}
	}
}

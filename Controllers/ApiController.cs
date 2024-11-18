
using LearningRedis.Domain.Entities;
using LearningRedis.Infrastructure.Caching;
using LearningRedis.Infrastructure.Persistence;
using LearningRedis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LearningRedis.Controllers
{
    public static class ApiController
    {
        public static void AddEndpoints(this WebApplication app)
        {
            app.MapGet("hello-world", ()=> new {saudacao = "hello world!"});
            app.MapGet("todo/get-by-id/{id}", async([FromRoute] int id, [FromServices] ICachingService cache, [FromServices] ToDoListDbContext context)=>
            {
                var todoCache = await cache.GetAsync(id.ToString());
                ToDo? todo;

                if (!string.IsNullOrWhiteSpace(todoCache)) {
                    todo = JsonConvert.DeserializeObject<ToDo>(todoCache);

                    Console.WriteLine("Loadded from cache.");
                    todo.LoadedFrom = "CACHE";
                    return Results.Ok(todo);
                }

                todo = await context.ToDos.SingleOrDefaultAsync(t => t.Id == id);
                todo.LoadedFrom = "DB";
                if (todo == null)
                    return Results.NotFound();

                await cache.SetAsync(id.ToString(), JsonConvert.SerializeObject(todo));

                return Results.Ok(todo);
            }).WithName("GetById");;
            app.MapPost("todo/set-item", async ([FromBody] ToDoInputModel model, [FromServices] ToDoListDbContext context)=>{
                var todo = new ToDo(1, model.Title, model.Description);

                await context.ToDos.AddAsync(todo);
                await context.SaveChangesAsync();

                return Results.CreatedAtRoute("GetById", new { id = todo.Id }, model);
            });
        }
        
    }
}
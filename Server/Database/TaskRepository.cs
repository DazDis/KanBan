using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase;

public class TaskRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<TaskDTO>> GetTasksAsync()
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Tasks.ToListAsync();

        return entities.Select(x => new TaskDTO
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
        }).ToList();

    }

    public async Task AddTaskAsync(TaskDTO task)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TaskEntity
        {
            //Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            UserIds = task.UserIds,
            LabelIds = task.LabelIds,
        };

        await context.Tasks.AddAsync(entity);
        await context.SaveChangesAsync();
        task.Id = entity.Id;
    }

    public async Task DeleteTaskAsync(TaskDTO task)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TaskEntity
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
        };

        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    public async Task DeleteTaskAsync(int id)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Tasks.FindAsync(id);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

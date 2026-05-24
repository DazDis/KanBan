using Microsoft.EntityFrameworkCore;
using Server.Database.Entities;
using Server.DataBase;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Repositories;

public class TaskRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<TaskDTO>> GetTasksAsync(CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        // var entities = await context.Tasks.ToListAsync();
        var entities = await context.Tasks.Include(x => x.Users).Include(x => x.Labels).OrderBy(t => t.Position).ToListAsync(token);

        return entities.Select(x => new TaskDTO
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Deadline = x.Deadline,
            ColumnId = x.ColumnId,
            Position = x.Position,
            Color = x.Color,
            // преобразование сущностей в Id
            LabelIds = x.Labels.Select(l => l?.Id).ToList(),
            UserIds = x.Users.Select(l => l?.UserId).ToList(),
            TeamIds = x.Teams.Select(l => l?.Id).ToList(),
        }).ToList();

    }

    public async Task AddTaskAsync(TaskDTO task, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        // загрузка меток из БД
        var labels = await context.Labels
            .Where(l => task.LabelIds.Contains(l.Id))
            .ToListAsync(token);
        var users = await context.Users
            .Where(l => task.UserIds.Contains(l.UserId))
            .ToListAsync(token);
        var teams = await context.Teams
            .Where(l => task.TeamIds.Contains(l.Id))
            .ToListAsync(token);

        var entity = new TaskEntity
        {
            //Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Color = task.Color,
            ColumnId = task.ColumnId,
            Labels = labels,
            Users = users,
            Teams = teams,
            Position = task.Position,
        };

        await context.Tasks.AddAsync(entity, token);
        await context.SaveChangesAsync(token);
        task.Id = entity.Id;
    }
    public async Task UpdateTaskAsync(TaskDTO task, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Tasks
            .Include(t => t.Users)
            .Include(t => t.Labels)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        if (entity == null)
            throw new Exception($"Task with id {task.Id} not found");

        // Обновляем поля
        entity.Title = task.Title;
        entity.Description = task.Description;
        entity.ColumnId = task.ColumnId;
        entity.Position = task.Position;
        // Обновляем связи (многие-ко-многим)
        entity.Users.Clear();
        entity.Users = await context.Users
            .Where(u => task.UserIds.Contains(u.UserId))
            .ToListAsync(token);

        entity.Labels.Clear();
        entity.Labels = await context.Labels
            .Where(l => task.LabelIds.Contains(l.Id))
            .ToListAsync(token);

        entity.Teams.Clear();
        entity.Teams = await context.Teams
            .Where(u => task.UserIds.Contains(u.Id))
            .ToListAsync(token);

        await context.SaveChangesAsync(token);
    }
    public async Task DeleteTaskAsync(TaskDTO task, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TaskEntity
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Deadline= task.Deadline,
            Color = task.Color,
        };

        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(token);
        }
    }
    public async Task DeleteTaskAsync(int id, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Tasks.FindAsync(id, token);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(token);
        }
    }
}

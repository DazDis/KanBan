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

public class LabelRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<LabelDTO>> GetLabelsAsync(CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Labels.ToListAsync(token);

        return entities.Select(x => new LabelDTO
        {
            Id = x.Id,
            Name = x.Name,
            Color = x.Color,
        }).ToList();

    }

    public async Task AddLabelAsync(LabelDTO label, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new LabelEntity
        {
            //Id = x.Id,
            Name = label.Name,
            Color = label.Color,
        };

        await context.Labels.AddAsync(entity, token);
        await context.SaveChangesAsync(token);
        label.Id = entity.Id;
    }
    public async Task UpdateLabelAsync(LabelDTO label, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Labels.FindAsync(label.Id, token);

        if (entity == null)
            throw new Exception($"Column with id {label.Id} not found");

        // Обновляем поля
        entity.Name = label.Name;
        entity.Color = label.Color;
        await context.SaveChangesAsync(token);
    }
    public async Task DeleteLabelAsync(LabelDTO label, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new LabelEntity
        {
            Id = label.Id,
            Name = label.Name,
            Color = label.Color,
        };

        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(token);
        }
    }
    public async Task DeleteLabelAsync(int id, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Labels.FindAsync(id, token);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

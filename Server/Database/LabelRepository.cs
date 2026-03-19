using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase;

public class LabelRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<LabelDTO>> GetLabelsAsync()
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Labels.ToListAsync();

        return entities.Select(x => new LabelDTO
        {
            Id = x.Id,
            Name = x.Name,
            Color = x.Color,
        }).ToList();

    }

    public async Task AddLabelAsync(LabelDTO label)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new LabelEntity
        {
            //Id = x.Id,
            Name = label.Name,
            Color = label.Color,
        };

        await context.Labels.AddAsync(entity);
        await context.SaveChangesAsync();
        label.Id = entity.Id;
    }

    public async Task DeleteLabelAsync(LabelDTO label)
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
            await context.SaveChangesAsync();
        }
    }
    public async Task DeleteLabelAsync(int id)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Labels.FindAsync(id);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

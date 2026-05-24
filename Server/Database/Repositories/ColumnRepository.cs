using Microsoft.EntityFrameworkCore;
using Server.Database.Entities;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase;

public class ColumnRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<ColumnDTO>> GetColumnsAsync(CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Columns.ToListAsync(token);

        return entities.Select(x => new ColumnDTO
        {
            Id = x.Id,
            Title = x.Title,
            Position = x.Position,
        }).ToList();

    }

    public async Task AddColumnAsync(ColumnDTO column, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new ColumnEntity
        {
            //Id = x.Id,
            Title = column.Title,
            Position = column.Position,
        };

        await context.Columns.AddAsync(entity, token);
        await context.SaveChangesAsync(token);
        column.Id = entity.Id;
    }
    public async Task UpdateColumnAsync(ColumnDTO column, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Columns.FindAsync(column.Id, token);


        if (entity == null)
            throw new Exception($"Column with id {column.Id} not found");

        // Обновляем поля
        entity.Title = column.Title;
        entity.Position = column.Position;
        await context.SaveChangesAsync(token);
    }

    public async Task DeleteColumnAsync(int id, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Columns.FindAsync(id, token);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(token);
        }
    }
}

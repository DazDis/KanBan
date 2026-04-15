using Microsoft.EntityFrameworkCore;
using Server.Database.Entities;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase;

public class ColumnRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<ColumnEntity>> GetColumnsAsync()
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Columns.ToListAsync();

        return entities;

    }

    public async Task AddColumnAsync(ColumnDTO column)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new ColumnEntity
        {
            //Id = x.Id,
            Title = column.Title,
            Position = column.Position,
        };

        await context.Columns.AddAsync(entity);
        await context.SaveChangesAsync();
        column.Id = entity.Id;
    }
    public async Task UpdateColumnAsync(ColumnDTO column)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Columns
            .FirstOrDefaultAsync(t => t.Id == column.Id);

        if (entity == null)
            throw new Exception($"Column with id {column.Id} not found");

        // Обновляем поля
        entity.Title = column.Title;
        entity.Position = column.Position;
        await context.SaveChangesAsync();
    }

    public async Task DeleteColumnAsync(int id)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Columns.FindAsync(id);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

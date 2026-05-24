using Microsoft.EntityFrameworkCore;
using Server.Database.Entities;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase;

public class TeamRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<TeamDTO>> GetTeamsAsync(CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Teams.ToListAsync(token);

        return entities.Select(x => new TeamDTO
        {
            Id = x.Id,
            Title = x.Title,
            
        }).ToList();

    }

    public async Task AddTeamAsync(TeamDTO team, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TeamEntity
        {
            //Id = x.Id,
            Title = team.Title,
        };

        await context.Teams.AddAsync(entity, token);
        await context.SaveChangesAsync(token);
        team.Id = entity.Id;
    }
    public async Task<TeamDTO> UpdateTeamAsync(TeamDTO team, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Teams
            .FirstOrDefaultAsync(t => t.Id == team.Id);

        if (entity == null)
            throw new Exception($"Team with id {team.Id} not found");

        // Обновляем поля
        entity.Title = team.Title;

        await context.SaveChangesAsync(token);

        return team;
    }
    public async Task DeleteTeamAsync(TeamDTO team, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TeamEntity
        {
            Id = team.Id,
            Title = team.Title,
        };

        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(token);
        }
    }
    public async Task DeleteTeamAsync(int id, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Teams.FindAsync(id, token);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(token);
        }
    }
}

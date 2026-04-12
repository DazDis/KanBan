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

    public async Task<IReadOnlyList<TeamDTO>> GetTeamsAsync()
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Teams.ToListAsync();

        return entities.Select(x => new TeamDTO
        {
            Id = x.Id,
            Title = x.Title,
        }).ToList();

    }

    public async Task AddTeamAsync(TeamDTO team)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TeamEntity
        {
            //Id = x.Id,
            Title = team.Title,
        };

        await context.Teams.AddAsync(entity);
        await context.SaveChangesAsync();
        team.Id = entity.Id;
    }
    
    public async Task DeleteTeamAsync(TeamDTO team)
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
            await context.SaveChangesAsync();
        }
    }
    public async Task DeleteTeamAsync(int id)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Teams.FindAsync(id);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

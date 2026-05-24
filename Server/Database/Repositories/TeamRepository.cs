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

        var entities = await context.Teams.Include(t => t.Users).ToListAsync(token);

        return entities.Select(x => new TeamDTO
        {
            Id = x.Id,
            Title = x.Title,
            Color = x.Color,
            UserIds = x.Users?.Select(l => (int?)l.UserId).ToList() ?? new List<int?>(),

        }).ToList();

    }

    public async Task AddTeamAsync(TeamDTO team, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new TeamEntity
        {
            //Id = x.Id,
            Title = team.Title,
            Color = team.Color,
        };

        await context.Teams.AddAsync(entity, token);
        await context.SaveChangesAsync(token);
        team.Id = entity.Id;
    }
    public async Task<TeamDTO> UpdateTeamAsync(TeamDTO team, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Teams
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == team.Id, token);

        if (entity == null)
            throw new Exception($"Team with id {team.Id} not found");

        entity.Title = team.Title;
        entity.Color = team.Color;

        entity.Users.Clear();

        if (team.UserIds != null && team.UserIds.Any())
        {
            var users = await context.Users
                .Where(u => team.UserIds.Contains(u.UserId))
                .ToListAsync(token);

            foreach (var user in users)
            {
                entity.Users.Add(user);
            }
        }

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

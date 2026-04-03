using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase;

public class UserRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<UserDTO>> GetUsersAsync()
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Users.ToListAsync();

        return entities.Select(x => new UserDTO
        {
            Id = x.UserId,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
        }).ToList();

    }

    public async Task AddUserAsync(UserDTO user)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new UserEntity
        {
            //Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
        user.Id = entity.UserId;
    }

    public async Task DeleteUserAsync(UserDTO user)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new UserEntity
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    public async Task DeleteUserAsync(int id)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = await context.Users.FindAsync(id);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

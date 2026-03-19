using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesUI.DataBase;

public class UserRepository(ApplicationDbContextFactory contextFactory)
{
    private ApplicationDbContextFactory _contextFactory = contextFactory;

    public async Task<IReadOnlyList<UserDTO>> GetUserAsync()
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entities = await context.Users.ToListAsync();

        return entities.Select(x => new UserDTO
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
        }).ToList();

    }

    public async Task AddUserAsync(UserDTO shape)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new UserEntity
        {
            Id = shape.Id,
            FirstName = shape.FirstName,
            LastName = shape.LastName,
            Email = shape.Email,
        };

        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(UserDTO shape)
    {
        using var context = _contextFactory.CreateApplicationContext();

        var entity = new UserEntity
        {
            Id = shape.Id,
            FirstName = shape.FirstName,
            LastName = shape.LastName,
            Email = shape.Email,
        };

        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}

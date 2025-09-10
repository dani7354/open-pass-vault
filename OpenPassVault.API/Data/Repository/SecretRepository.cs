using Microsoft.EntityFrameworkCore;
using OpenPassVault.API.Data.DataContext;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Data.Exceptions;
using OpenPassVault.API.Data.Interfaces;

namespace OpenPassVault.API.Data.Repository;

public class SecretRepository(ApplicationDatabaseContext dbContext) : ISecretRepository
{
    public async  Task<IList<Secret>> GetAll()
    {
        return await dbContext.Secret.ToListAsync();
    }

    public async Task<Secret?> GetById(string id)
    {
        return await dbContext.Secret.FindAsync(id);
    }

    public async Task<string> Add(Secret entity)
    {
        var newSecret = await dbContext.Secret.AddAsync(entity);
        return newSecret.Entity.Id;
    }

    public async Task Update(Secret entity)
    {
        var secretEntity =  await dbContext.Secret.FindAsync(entity.Id);
        if (secretEntity == null)
            throw new NotFoundException();
        
        secretEntity.Name = entity.Name;
        secretEntity.Username = entity.Username;
        secretEntity.Type = entity.Type;
        secretEntity.Description = entity.Description;
        secretEntity.Updated = DateTime.Now;
        secretEntity.Content = entity.Content;
        
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(string id)
    {
        var secretEntity = await dbContext.Secret.FindAsync(id);
        if (secretEntity == null)
            throw new NotFoundException();
        
        dbContext.Secret.Remove(secretEntity);
    }

    public async Task<IList<Secret>> ListForUser(string userId)
    {
        return await dbContext.Secret.Where(s => s.UserId == userId).ToListAsync();
    }
}
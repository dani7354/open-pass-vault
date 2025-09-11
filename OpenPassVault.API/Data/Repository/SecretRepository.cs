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

    public Task Update(Secret entity)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Secret entity, string userId)
    {
        var secretEntity =  await dbContext.Secret.FirstOrDefaultAsync(
            x => x.Id == entity.Id && x.UserId == userId);
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

    public async Task<Secret?> GetSecret(string id, string userId)
    {
        return await dbContext.Secret.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
    }
}
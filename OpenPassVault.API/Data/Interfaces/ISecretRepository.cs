using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Data.Repository;

namespace OpenPassVault.API.Data.Interfaces;

public interface ISecretRepository : IRepository<Secret>
{
    Task<IList<Secret>> ListForUser(string userId);
}
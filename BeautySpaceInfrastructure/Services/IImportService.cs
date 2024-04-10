using BeautySpaceDomain.Model;

namespace BeautySpaceInfrastructure.Services
{
    public interface IImportService<TEntity>
        where TEntity : Entity
    {
        Task<bool> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}

namespace Blockpit.Repository
{
    using Data;
    using Data.Models;
    using Interfaces;
    using LinqToDB;
    using LinqToDB.Async;

    public class BlockTickRepository(DbContext dbContext) : IRepository<BlockTick>
    {
        public Task<BlockTick?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BlockTick>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(BlockTick entity)
        {
            await dbContext.InsertAsync(entity);
        }

        public void Update(BlockTick entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(BlockTick entity)
        {
            throw new NotImplementedException();
        }

        public async Task<BlockTick?> GetByCompositeKeyAsync(BlockTick model)
        {
            return await dbContext
                .BlockTick
                .FirstOrDefaultAsync(f => f.Name == model.Name
                                          && f.Hash == model.Hash
                                          && f.Height == model.Height
                                          && f.PreviousHash == model.PreviousHash
                                          && f.RollbackAt == null);
        }

        public async Task CommitAsync(Guid guid)
        {
            var existing = dbContext.BlockTick.FirstOrDefault(f => f.Guid == guid);
            if (existing != null)
            {
                existing.CommittedAt = DateTime.Now;
                await dbContext.UpdateAsync(existing);
            }
        }

        public async Task RollbackAsync(Guid guid)
        {
            var existing = dbContext.BlockTick.FirstOrDefault(f => f.Guid == guid);
            if (existing != null)
            {
                existing.RollbackAt = DateTime.Now;
                await dbContext.UpdateAsync(existing);
            }
        }
    }
}

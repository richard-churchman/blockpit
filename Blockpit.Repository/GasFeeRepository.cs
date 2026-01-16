namespace Blockpit.Repository
{
    using Data;
    using Data.Models;
    using Interfaces;
    using LinqToDB;

    public class GasFeeRepository(DbContext dbContext) : IRepository<GasFee>
    {
        public Task<GasFee?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GasFee>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(GasFee entity)
        {
            await dbContext.InsertAsync(entity);
        }

        public void Update(GasFee entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(GasFee entity)
        {
            throw new NotImplementedException();
        }
    }
}

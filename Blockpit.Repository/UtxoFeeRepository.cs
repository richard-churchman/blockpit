namespace Blockpit.Repository
{
    using Data;
    using Data.Models;
    using Interfaces;
    using LinqToDB;

    public class UtxoFeeRepository(DbContext dbContext) : IRepository<UtxoFee>
    {
        public Task<UtxoFee?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UtxoFee>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(UtxoFee entity)
        {
            await dbContext.InsertAsync(entity);
        }

        public void Update(UtxoFee entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(UtxoFee entity)
        {
            throw new NotImplementedException();
        }
    }
}

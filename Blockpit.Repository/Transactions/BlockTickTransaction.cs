namespace Blockpit.Repository.Transactions
{
    using Data;
    using Data.Exceptions;
    using LinqToDB.Data;

    public class BlockTickTransaction(DbContext context) : IDisposable
    {
        private BlockTickRepository? _blockTickRepository;
        private GasFeeRepository? _gasFeeRepository;
        private DataConnectionTransaction? _transaction;
        private UtxoFeeRepository? _utxoFeeRepository;

        public BlockTickRepository BlockTickRepository
        {
            get
            {
                return _blockTickRepository ??= new BlockTickRepository(context);
            }
        }

        public GasFeeRepository GasFeeRepository
        {
            get
            {
                return _gasFeeRepository ??= new GasFeeRepository(context);
            }
        }

        public UtxoFeeRepository UtxoFeeRepository
        {
            get
            {
                return _utxoFeeRepository ??= new UtxoFeeRepository(context);
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new TransactionStartedException();
            }

            _transaction = await context.BeginTransactionAsync(cancellationToken);
        }

        public async Task<bool> CommitAsync(Guid handleGuid, CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new NoActiveTransactionException();
            }

            try
            {
                await BlockTickRepository.CommitAsync(handleGuid);
                await _transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await _transaction.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(Guid handleGuid, CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            await BlockTickRepository.RollbackAsync(handleGuid);
        }
    }
}

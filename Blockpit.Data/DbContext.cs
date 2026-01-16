namespace Blockpit.Data
{
    using LinqToDB;
    using LinqToDB.Data;
    using Models;

    public class DbContext(DataOptions options) : DataConnection(options)
    {
        public ITable<BlockTick> BlockTick
        {
            get
            {
                return this.GetTable<BlockTick>();
            }
        }

        public ITable<GasFee> GasFee
        {
            get
            {
                return this.GetTable<GasFee>();
            }
        }

        public ITable<UtxoFee> UxtoFee
        {
            get
            {
                return this.GetTable<UtxoFee>();
            }
        }
    }
}

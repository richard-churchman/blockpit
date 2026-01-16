namespace Blockpit.Data.Models
{
    using LinqToDB.Mapping;

    [Table]
    public class UtxoFee
    {
        [PrimaryKey] [Identity]
        public long Id { get; set; }
        [Column]
        public Guid BlockTickGuid { get; set; }
        [Column]
        public long HighFeePerKb { get; set; }
        [Column]
        public long MediumFeePerKb { get; set; }
        [Column]
        public long LowFeePerKb { get; set; }
        [Column]
        public DateTime CreatedAt { get; set; }
    }
}

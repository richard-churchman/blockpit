namespace Blockpit.Data.Models
{
    using LinqToDB.Mapping;

    [Table]
    public class GasFee
    {
        [PrimaryKey] [Identity]
        public long Id { get; set; }
        [Column]
        public Guid BlockTickGuid { get; set; }
        [Column]
        public long HighGasPrice { get; set; }
        [Column]
        public long MediumGasPrice { get; set; }
        [Column]
        public long LowGasPrice { get; set; }
        [Column]
        public long HighPriorityFee { get; set; }
        [Column]
        public long MediumPriorityFee { get; set; }
        [Column]
        public long LowPriorityFee { get; set; }
        [Column]
        public long BaseFee { get; set; }
        [Column]
        public DateTime CreatedAt { get; set; }
    }
}

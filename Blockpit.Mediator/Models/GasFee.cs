namespace Blockpit.Mediator.Models
{
    using Interfaces;

    public class GasFee(
        long highGasPrice,
        long mediumGasPrice,
        long lowGasPrice,
        long highPriorityFee,
        long mediumPriorityFee,
        long lowPriorityFee,
        long baseFee)
        : IGasFee
    {
        public long HighGasPrice { get; set; } = highGasPrice;
        public long MediumGasPrice { get; set; } = mediumGasPrice;
        public long LowGasPrice { get; set; } = lowGasPrice;
        public long HighPriorityFee { get; set; } = highPriorityFee;
        public long MediumPriorityFee { get; set; } = mediumPriorityFee;
        public long LowPriorityFee { get; set; } = lowPriorityFee;
        public long BaseFee { get; set; } = baseFee;
    }
}

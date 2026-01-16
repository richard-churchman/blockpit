namespace Blockpit.Mediator.Models.Interfaces
{
    public interface IGasFee
    {
        public long HighGasPrice { get; set; }
        long MediumGasPrice { get; set; }
        long LowGasPrice { get; set; }
        long HighPriorityFee { get; set; }
        long MediumPriorityFee { get; set; }
        long LowPriorityFee { get; set; }
        long BaseFee { get; set; }
    }
}

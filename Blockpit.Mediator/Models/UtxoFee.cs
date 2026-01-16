namespace Blockpit.Mediator.Models
{
    using Interfaces;

    public class UtxoFee(
        long highFeePerKb,
        long mediumFeePerKb,
        long lowFeePerKb) : IUtxoFee
    {
        public long HighFeePerKb { get; set; } = highFeePerKb;
        public long MediumFeePerKb { get; set; } = mediumFeePerKb;
        public long LowFeePerKb { get; set; } = lowFeePerKb;
    }
}

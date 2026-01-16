namespace Blockpit.Listener.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BlockCypherEth
    {
        public string? Name { get; set; }
        public int? Height { get; set; }
        public string? Hash { get; set; }
        public DateTime? Time { get; set; }
        public string? LatestUrl { get; set; }
        public string? PreviousHash { get; set; }
        public string? PreviousUrl { get; set; }
        public int? PeerCount { get; set; }
        public int? UnconfirmedCount { get; set; }
        public long? HighGasPrice { get; set; }
        public long? MediumGasPrice { get; set; }
        public int? LowGasPrice { get; set; }
        public int? HighPriorityFee { get; set; }
        public int? MediumPriorityFee { get; set; }
        public int? LowPriorityFee { get; set; }
        public int? BaseFee { get; set; }
        public int? LastForkHeight { get; set; }
        public string? LastForkHash { get; set; }
    }
}

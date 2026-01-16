namespace Blockpit.Mediator.Models
{
    using Interfaces;

    public class BlockTick(
        string symbol,
        string name,
        long height,
        string hash,
        DateTime time,
        string latestUrl,
        string previousHash,
        string previousUrl,
        int peerCount,
        int unconfirmedCount,
        long lastForkHeight,
        string lastForkHash)
        : IBlockTick
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public UtxoFee? UtxoFees { get; private set; }
        public GasFee? GasFees { get; private set; }
        public string Symbol
        {
            get;
        } = symbol;
        public string Name
        {
            get;
        } = name;
        public long Height
        {
            get;
        } = height;
        public string Hash
        {
            get;
        } = hash;
        public DateTime Time
        {
            get;
        } = time;
        public string LatestUrl
        {
            get;
        } = latestUrl;
        public string PreviousHash
        {
            get;
        } = previousHash;
        public string PreviousUrl
        {
            get;
        } = previousUrl;
        public int PeerCount
        {
            get;
        } = peerCount;
        public int UnconfirmedCount
        {
            get;
        } = unconfirmedCount;
        public long LastForkHeight
        {
            get;
        } = lastForkHeight;
        public string LastForkHash
        {
            get;
        } = lastForkHash;

        public void SetUtxoFees(long high, long medium, long low)
        {
            UtxoFees = new UtxoFee(high, medium, low);
        }

        public void SetGasFees(
            long highGas, long mediumGas, long lowGas,
            long highPriority, long mediumPriority, long lowPriority,
            long baseFee)
        {
            GasFees = new GasFee(
                highGas, mediumGas, lowGas,
                highPriority, mediumPriority, lowPriority,
                baseFee);
        }
    }


}

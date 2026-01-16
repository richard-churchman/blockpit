namespace Blockpit.Mediator.Models.Interfaces
{
    public interface IBlockTick
    {
        string Name { get; }
        long Height { get; }
        string Hash { get; }
        DateTime Time { get; }
        string LatestUrl { get; }
        string PreviousHash { get; }
        string PreviousUrl { get; }
        int PeerCount { get; }
        int UnconfirmedCount { get; }
        long LastForkHeight { get; }
        string LastForkHash { get; }
        string Symbol { get; }
    }
}

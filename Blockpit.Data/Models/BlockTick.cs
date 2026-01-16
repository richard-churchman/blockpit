namespace Blockpit.Data.Models
{
    using LinqToDB.Mapping;

    [Table]
    public class BlockTick
    {
        [PrimaryKey] [Identity]
        public long Id { get; set; }
        [Column]
        public Guid Guid { get; init; }
        [Column]
        public string? Symbol { get; set; }
        [Column]
        public string? Name { get; set; }
        [Column]
        public long? Height { get; set; }
        [Column]
        public string? Hash { get; set; }
        [Column]
        public DateTime? Time { get; set; }
        [Column]
        public string? LatestUrl { get; set; }
        [Column]
        public string? PreviousHash { get; set; }
        [Column]
        public string? PreviousUrl { get; set; }
        [Column]
        public int? PeerCount { get; set; }
        [Column]
        public int? UnconfirmedCount { get; set; }
        [Column]
        public long? LastForkHeight { get; set; }
        [Column]
        public string? LastForkHash { get; set; }
        [Column]
        public DateTime? CreatedAt { get; set; }
        [Column]
        public DateTime? RollbackAt { get; set; }
        [Column]
        public DateTime? CommittedAt { get; set; }
    }
}

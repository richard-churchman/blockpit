namespace Blockpit.Query
{
    using System.Text.Json.Serialization;
    using Data;
    using LinqToDB.Async;

    public class DenormalizedBlockTicksQuery(DbContext context)
    {
        public async Task<List<Dto>> ExecuteAsync(string symbol, DateTime fromDate, int limit)
        {
            return await (from bt in context.BlockTick
                where bt.CreatedAt > fromDate
                      && bt.Symbol != null && bt.Symbol == symbol.ToUpper()
                      && bt.CommittedAt != null
                join uf in context.UxtoFee on bt.Guid equals uf.BlockTickGuid into ufGroup
                from uf in ufGroup.DefaultIfEmpty()
                join gf in context.GasFee on bt.Guid equals gf.BlockTickGuid into gfGroup
                from gf in gfGroup.DefaultIfEmpty()
                orderby bt.CreatedAt descending
                select new Dto
                {
                    Id = bt.Id,
                    Guid = bt.Guid,
                    Symbol = bt.Symbol,
                    Name = bt.Name,
                    Height = bt.Height,
                    Hash = bt.Hash,
                    Time = bt.Time,
                    LatestUrl = bt.LatestUrl,
                    PreviousHash = bt.PreviousHash,
                    PreviousUrl = bt.PreviousUrl,
                    PeerCount = bt.PeerCount,
                    UnconfirmedCount = bt.UnconfirmedCount,
                    LastForkHeight = bt.LastForkHeight,
                    LastForkHash = bt.LastForkHash,
                    CreatedAt = bt.CreatedAt,
                    HighGasPrice = gf.HighGasPrice,
                    MediumGasPrice = gf.MediumGasPrice,
                    LowGasPrice = gf.LowGasPrice,
                    HighPriorityFee = gf.HighPriorityFee,
                    MediumPriorityFee = gf.MediumPriorityFee,
                    LowPriorityFee = gf.LowPriorityFee,
                    BaseFee = gf.BaseFee,
                    HighFeePerKb = uf.HighFeePerKb,
                    MediumFeePerKb = uf.MediumFeePerKb,
                    LowFeePerKb = uf.LowFeePerKb
                }).Take(limit).ToListAsync();
        }
    }

    public class Dto
    {
        public long Id { get; set; }
        public Guid Guid { get; init; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public long? Height { get; set; }
        public string? Hash { get; set; }
        public DateTime? Time { get; set; }
        public string? LatestUrl { get; set; }
        public string? PreviousHash { get; set; }
        public string? PreviousUrl { get; set; }
        public int? PeerCount { get; set; }
        public int? UnconfirmedCount { get; set; }
        public long? LastForkHeight { get; set; }
        public string? LastForkHash { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? HighGasPrice { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? MediumGasPrice { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? LowGasPrice { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? HighPriorityFee { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? MediumPriorityFee { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? LowPriorityFee { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? BaseFee { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? HighFeePerKb { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? MediumFeePerKb { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? LowFeePerKb { get; set; }
    }
}

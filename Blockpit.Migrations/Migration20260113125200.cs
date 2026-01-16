namespace Blockpit.Migrations
{
    using FluentMigrator;

    [Migration(20260113125200)]
    public class Migration20260113125200 : Migration
    {
        public override void Up()
        {
            Create.Table("BlockTick")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Guid").AsGuid()
                .WithColumn("Symbol").AsString()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("Height").AsInt64().Nullable()
                .WithColumn("Hash").AsString().Nullable()
                .WithColumn("Time").AsDateTime2().Nullable()
                .WithColumn("LatestUrl").AsString().Nullable()
                .WithColumn("PreviousHash").AsString().Nullable()
                .WithColumn("PreviousUrl").AsString().Nullable()
                .WithColumn("PeerCount").AsInt32().Nullable()
                .WithColumn("UnconfirmedCount").AsInt32().Nullable()
                .WithColumn("LastForkHeight").AsInt64().Nullable()
                .WithColumn("LastForkHash").AsString().Nullable()
                .WithColumn("CreatedAt").AsDateTime().Nullable()
                .WithColumn("RollbackAt").AsDateTime().Nullable()
                .WithColumn("CommittedAt").AsDateTime().Nullable();

            Create.Table("UtxoFee")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("BlockTickGuid").AsGuid()
                .WithColumn("HighFeePerKb").AsInt64()
                .WithColumn("LowFeePerKb").AsInt64()
                .WithColumn("MediumFeePerKb").AsInt64()
                .WithColumn("CreatedAt").AsDateTime().Nullable();

            Create.Table("GasFee")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("BlockTickGuid").AsGuid()
                .WithColumn("HighGasPrice").AsInt64()
                .WithColumn("MediumGasPrice").AsInt64()
                .WithColumn("LowGasPrice").AsInt64()
                .WithColumn("HighPriorityFee").AsInt64()
                .WithColumn("MediumPriorityFee").AsInt64()
                .WithColumn("LowPriorityFee").AsInt64()
                .WithColumn("BaseFee").AsInt64()
                .WithColumn("CreatedAt").AsDateTime().Nullable();

            Create.Index().OnTable("BlockTick").OnColumn("Guid").Unique();
            Create.Index().OnTable("BlockTick").OnColumn("Symbol").Ascending().OnColumn("CreatedAt").Descending();
            Create.Index().OnTable("BlockTick").OnColumn("Symbol").Ascending().OnColumn("Hash").Ascending().OnColumn("Height").Ascending().OnColumn("PreviousHash").Ascending();
            Create.Index().OnTable("GasFee").OnColumn("BlockTickGuid");
            Create.Index().OnTable("UtxoFee").OnColumn("BlockTickGuid");
        }
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}

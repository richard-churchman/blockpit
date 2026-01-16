namespace Blockpit.Configuration
{
    public class Settings
    {

        public Settings()
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            ConnectionString = connectionString ?? "Data Source=blockpit.db";

            var blockCypherBtcUrl = Environment.GetEnvironmentVariable("BLOCK_CYPHER_BTC_URL");
            BlockCypherBtcUrl = blockCypherBtcUrl ?? "https://api.blockcypher.com/v1/btc/main";

            var blockCypherDashUrl = Environment.GetEnvironmentVariable("BLOCK_CYPHER_DASH_URL");
            BlockCypherDashUrl = blockCypherDashUrl ?? "https://api.blockcypher.com/v1/dash/main";

            var blockCypherEthUrl = Environment.GetEnvironmentVariable("BLOCK_CYPHER_ETH_URL");
            BlockCypherEthUrl = blockCypherEthUrl ?? "https://api.blockcypher.com/v1/eth/main";

            var corsOrigin = Environment.GetEnvironmentVariable("CORS_ORIGIN");
            CorsOrigin = corsOrigin ?? "*";

            var fetchLimit = Environment.GetEnvironmentVariable("FETCH_LIMIT");
            FetchLimit = fetchLimit != null ? Int32.Parse(fetchLimit) : 100;

            var fetchDateOffsetDays = Environment.GetEnvironmentVariable("FETCH_DATE_OFFSET_DAYS");
            FetchDateOffsetDays = fetchDateOffsetDays != null ? Int32.Parse(fetchDateOffsetDays) : 7;

            var log4NetLogLevel = Environment.GetEnvironmentVariable("LOG4NET_LOG_LEVEL");
            Log4NetLogLevel = log4NetLogLevel ?? "INFO";

            var log4NetPatternLayout = Environment.GetEnvironmentVariable("LOG4NET_PATTERN_LAYOUT");
            Log4NetPatternLayout = log4NetPatternLayout ?? "%date:%-5level:[%thread]:[%logger::%method]:%line:%message%newline";

            var log4NetMaximumFileSize = Environment.GetEnvironmentVariable("LOG4NET_MAXIMUM_FILE_SIZE");
            Log4NetMaximumFileSize = log4NetMaximumFileSize ?? "100MB";

            var log4NetMaxSizeRollBackups = Environment.GetEnvironmentVariable("LOG4NET_MAX_SIZE_ROLL_BACKUPS");
            Log4NetMaxSizeRollBackups = log4NetMaxSizeRollBackups != null ? Int32.Parse(log4NetMaxSizeRollBackups) : 1000;

            Log4NetLogPath = Environment.GetEnvironmentVariable("LOG4NET_LOG_PATH");

            var ignoreSsl = Environment.GetEnvironmentVariable("IGNORE_SSL");
            IgnoreSsl = ignoreSsl != null && ignoreSsl.Equals("True", StringComparison.CurrentCultureIgnoreCase);

            var pollRate = Environment.GetEnvironmentVariable("POLL_RATE");
            PollRate = pollRate != null ? Int32.Parse(pollRate) : 30000;
        }

        public string ConnectionString { get; set; }
        public string BlockCypherBtcUrl { get; set; }
        public string BlockCypherDashUrl { get; set; }
        public string BlockCypherEthUrl { get; set; }
        public string CorsOrigin { get; set; }
        public int FetchLimit { get; set; }
        public int FetchDateOffsetDays { get; set; }
        public string Log4NetLogLevel { get; set; }
        public string Log4NetPatternLayout { get; set; }
        public string Log4NetMaximumFileSize { get; set; }
        public int Log4NetMaxSizeRollBackups { get; set; }
        public string? Log4NetLogPath { get; set; }
        public int PollRate { get; set; }
        public bool IgnoreSsl { get; set; }
    }
}

namespace Blockpit.Api.Helpers
{
    using System.Text;
    using Configuration;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    public static class ConfigureLog4Net
    {
        public static ILog FromSettings(Settings settings)
        {
            var patternLayout = PatternLayout(settings);
            var log4NetLevel = Log4NetLogLevel(settings);

            ConfigureConsole(patternLayout, log4NetLevel);
            ConfigureRollingFileAppender(patternLayout, log4NetLevel, settings);

            ((Hierarchy)LogManager.GetRepository()).Root.Level = Log4NetLogLevel(settings);
            return LogManager.GetLogger(typeof(ILog));
        }

        private static void ConfigureRollingFileAppender(PatternLayout patternLayout, Level log4NetLevel, Settings settings)
        {
            var log4NetLogFile = Log4NetLogFile(settings);

            if (log4NetLogFile == null)
            {
                return;
            }

            Console.WriteLine("Configuring software instantiation of log4net RollingFileAppender:");

            var rollingFileAppender = new RollingFileAppender
            {
                File = log4NetLogFile,
                Encoding = Encoding.UTF8,
                Layout = patternLayout,
                MaximumFileSize = settings.Log4NetMaximumFileSize,
                MaxSizeRollBackups = settings.Log4NetMaxSizeRollBackups,
                StaticLogFileName = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                AppendToFile = true,
                Threshold = log4NetLevel
            };

            rollingFileAppender.ActivateOptions();

            BasicConfigurator.Configure(rollingFileAppender);

            Console.WriteLine("Configured software instantiation of log4net RollingFileAppender.");
        }

        private static void ConfigureConsole(PatternLayout patternLayout, Level log4NetLevel)
        {
            var consoleAppender = new ConsoleAppender
            {
                Layout = patternLayout,
                Threshold = log4NetLevel
            };
            consoleAppender.ActivateOptions();
            BasicConfigurator.Configure(consoleAppender);
        }

        private static Level Log4NetLogLevel(Settings settings)
        {
            return settings.Log4NetLogLevel switch
            {
                "ERROR" => Level.Error,
                "WARN" => Level.Warn,
                "INFO" => Level.Info,
                "DEBUG" => Level.Debug,
                _ => Level.Error
            };
        }

        private static PatternLayout PatternLayout(Settings settings)
        {
            var log4NetPatternLayout = settings.Log4NetPatternLayout;
            var layout = new PatternLayout(log4NetPatternLayout);
            layout.ActivateOptions();
            return layout;
        }

        private static string? Log4NetLogFile(Settings settings)
        {
            return settings.Log4NetLogPath == null ? null : Path.Combine(settings.Log4NetLogPath, "Blockpit.log");
        }
    }
}

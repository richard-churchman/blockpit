namespace Blockpit.Api.Extensions
{
    using FluentMigrator.Runner;

    public static class MigrationExtensions
    {
        public static IApplicationBuilder UseFluentMigrator(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();

            return app;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels.Contexts;

namespace ShoeGrabOrderManagement.Extensions;

public static class AppExtension
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using (var context = scope.ServiceProvider.GetRequiredService<OrderContext>())
            {
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database");
                    throw;
                }
            }
        }
    }
}

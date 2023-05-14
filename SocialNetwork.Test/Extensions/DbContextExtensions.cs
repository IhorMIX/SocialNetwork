using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Test.Extensions;

public static class DbContextExtensions
{
    public static void DetachAllEntities(this DbContext context)
    {
        context.ChangeTracker.Clear();
    }
}
using DocumentsStorage.Core.Entities;
using DocumentsStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentsStorage.Web;

public static class SeedData
{
    public static Tag Tag1 = new () { Name = "important" };
    public static Tag Tag2 = new () { Name = ".net" };

    public static readonly DataPayload Data1 = new()
    {
        Author = "Stanislav Khavruk",
        Description = "Some demo project",
        ProjectName = "Demo project"
    };
    //
    // public static readonly Document DemoDoc = new()
    // {
    //     DocumentId = "demoDoc",
    //     Tags = { Tag1, Tag2 },
    //     Data = Data1
    // };

    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var dbContext = new AppDbContext(
                   serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
        {
            // Look for any documents.
            if (dbContext.Documents.Any())
            {
                return; // Database has been seeded
            }
            PopulateTestData(dbContext);
        }
    }
    public static void PopulateTestData(AppDbContext dbContext)
    {
        foreach (var item in dbContext.Documents)
        {
            dbContext.Remove(item);
        }
        foreach (var item in dbContext.Tags)
        {
            dbContext.Remove(item);
        }

        dbContext.SaveChanges();

        var tag1 = dbContext.Tags.Add(Tag1);
        var tag2 = dbContext.Tags.Add(Tag2);
        dbContext.SaveChanges();

        var demoDoc = new Document
        {
            DocumentId = "demoDoc",
            Tags = new List<Tag> { tag1.Entity, tag2.Entity },
            Data = Data1
        };

        dbContext.Documents.Add(demoDoc);

        dbContext.SaveChanges();
    }
}
using Ardalis.Specification.EntityFrameworkCore;
using DocumentsStorage.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentsStorage.Infrastructure.Data;

public sealed class EfRepository<T> : RepositoryBase<T> where T : class
{
    public EfRepository(AppDbContext context) : base(context)
    {
        
    }
}
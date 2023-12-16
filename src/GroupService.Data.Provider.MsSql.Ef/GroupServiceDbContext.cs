using System.Reflection;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace UniversityHelper.GroupService.Data.Provider.MsSql.Ef;

public class GroupServiceDbContext : DbContext, IDataProvider
{
  public DbSet<DbGroup> Groups { get; set; }
  public DbSet<DbTeamCategory> Categories { get; set; }
  public DbSet<DbGroupTeam> GroupsCategories { get; set; }
  public DbSet<DbFile> Files { get; set; }
  public DbSet<DbImage> Images { get; set; }
  public DbSet<DbGroupUser> GroupsUsers { get; set; }
  public DbSet<DbGroupComment> GroupComments { get; set; }
  public DbSet<DbUserBirthday> UsersBirthdays { get; set; }

  public GroupServiceDbContext(DbContextOptions<GroupServiceDbContext> options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("UniversityHelper.GroupService.Models.Db"));
  }

  public void Save()
  {
    SaveChanges();
  }

  public async Task SaveAsync()
  {
    await SaveChangesAsync();
  }

  public object MakeEntityDetached(object obj)
  {
    Entry(obj).State = EntityState.Detached;

    return Entry(obj).State;
  }

  public void EnsureDeleted()
  {
    Database.EnsureDeleted();
  }

  public bool IsInMemory()
  {
    return Database.IsInMemory();
  }
}

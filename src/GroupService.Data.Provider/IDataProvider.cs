using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.EFSupport.Provider;
using UniversityHelper.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace UniversityHelper.GroupService.Data.Provider;

[AutoInject(InjectType.Scoped)]
public interface IDataProvider : IBaseDataProvider
{
  public DbSet<DbGroup> Groups { get; set; }
  public DbSet<DbGroupCategory> Categories { get; set; }
  public DbSet<DbGroupTeam> GroupsCategories { get; set; }
  public DbSet<DbFile> Files { get; set; }
  public DbSet<DbImage> Images { get; set; }
  public DbSet<DbGroupUser> GroupsUsers { get; set; }
  public DbSet<DbGroupComment> GroupComments { get; set; }
  public DbSet<DbUserBirthday> UsersBirthdays { get; set; }
}

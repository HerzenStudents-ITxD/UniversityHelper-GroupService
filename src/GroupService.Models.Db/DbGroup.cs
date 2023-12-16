using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityHelper.GroupService.Models.Db;
public class DbGroup
{
  public const string TableName = "Groups";

  public Guid Id { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public Guid? ModifiedBy { get; set; }
  public DateTime? ModifiedAtUtc { get; set; }
  public bool IsActive { get; set; }

  public string Name { get; set; }

  public DbGroupTeam MainTeam { get; set; }
  public ICollection<DbGroupCategory> GroupCategories { get; set; }

  public DbGroup()
  {
    GroupCategories = new HashSet<DbGroupCategory>();
  }
}

public class DbGroupConfiguration : IEntityTypeConfiguration<DbGroup>
{
  public void Configure(EntityTypeBuilder<DbGroup> builder)
  {
    builder
      .ToTable(DbGroup.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasOne(e => e.MainTeam)
      .WithOne(ec => ec.Group);

    builder
      .HasMany(e => e.GroupCategories)
      .WithOne(ec => ec.Group);
  }
}

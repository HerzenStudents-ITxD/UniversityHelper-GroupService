using System;
using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Dto.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UniversityHelper.GroupService.Models.Db;

public class DbGroupCategory
{
  public const string TableName = "TeamCategories";

  public Guid Id { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public Guid? ModifiedBy { get; set; }
  public DateTime? ModifiedAtUtc { get; set; }
  public bool IsActive { get; set; }

  public string Name { get; set; }
  public DbGroup Group { get; set; }
  public ICollection<DbGroupTeam> GroupsTeams { get; set; }
  public DbCategoryColor Color { get; set; }
}

public class DbCategoryConfiguration : IEntityTypeConfiguration<DbGroupCategory>
{
  public void Configure(EntityTypeBuilder<DbGroupCategory> builder)
  {
    builder
      .ToTable(DbGroupCategory.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasMany(e => e.GroupsTeams)
      .WithOne(ec => ec.Category);

    builder
      .HasOne(e => e.Group)
      .WithMany(ec => ec.GroupCategories);

    builder
      .HasOne(e => e.Color)
      .WithMany(ec => ec.Categories);
  }
}

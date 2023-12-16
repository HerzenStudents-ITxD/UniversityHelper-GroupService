using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Enums;

namespace UniversityHelper.GroupService.Models.Db;
public class DbGroupCategoryColor
{
  public const string TableName = "CategoryColors";

  public Guid Id { get; set; }
  public bool IsActive { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public Guid? ModifiedBy { get; set; }
  public DateTime? ModifiedAtUtc { get; set; }

  public Guid UniversityId { get; set; }
  public string Name { get; set; }
  public byte R { get; set; }
  public byte G { get; set; }
  public byte B { get; set; }
  public float A { get; set; }

  public ICollection<DbGroupCategory> Categories { get; set; }
}

public class DbCategoryColorConfiguration : IEntityTypeConfiguration<DbGroupCategoryColor>
{
  public void Configure(EntityTypeBuilder<DbGroupCategoryColor> builder)
  {
    builder
      .ToTable(DbGroupCategoryColor.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasMany(e => e.Categories)
      .WithOne(ec => ec.Color);
  }
}


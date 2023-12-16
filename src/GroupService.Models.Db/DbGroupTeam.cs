using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityHelper.GroupService.Models.Db;

public class DbGroupTeam
{
  public const string TableName = "GroupTeams";

  public Guid Id { get; set; }

  public Guid CreatedBy { get; set; }
  public DateTime CreatedAtUtc { get; set; }

  public Guid TeamId { get; set; }

  public DbGroup? Group { get; set; }
  public DbGroupCategory Category { get; set; }
}

public class DbGroupTeamConfiguration : IEntityTypeConfiguration<DbGroupTeam>
{
  public void Configure(EntityTypeBuilder<DbGroupTeam> builder)
  {
    builder
      .ToTable(DbGroupTeam.TableName);

    builder
      .HasKey(t => t.Id);

    builder
      .HasOne(e => e.Group)
      .WithOne(ec => ec.MainTeam);

    builder
      .HasOne(e => e.Category)
      .WithMany(ec => ec.GroupsTeams);
  }
}

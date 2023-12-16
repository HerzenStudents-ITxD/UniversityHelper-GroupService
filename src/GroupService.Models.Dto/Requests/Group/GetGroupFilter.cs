using System;
using Microsoft.AspNetCore.Mvc;

namespace UniversityHelper.GroupService.Models.Dto.Requests.Group;

public record GetGroupFilter
{
  [FromQuery(Name = "GroupId")]
  public Guid GroupId { get; set; }

  [FromQuery(Name = "includeUsers")]
  public bool IncludeUsers { get; set; } = false;

  [FromQuery(Name = "includeCategories")]
  public bool IncludeCategories { get; set; } = false;

  [FromQuery(Name = "includeImages")]
  public bool IncludeImages { get; set; } = false;

  [FromQuery(Name = "includeFiles")]
  public bool IncludeFiles { get; set; } = false;

  [FromQuery(Name = "includeComments")]
  public bool IncludeComments { get; set; } = false;
}

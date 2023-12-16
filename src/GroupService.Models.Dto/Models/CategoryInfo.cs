using System;
using UniversityHelper.GroupService.Models.Dto.Enums;

namespace UniversityHelper.GroupService.Models.Dto.Models;

public class CategoryInfo
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public CategoryColor? Color { get; set; }
}


using System;
using System.Collections.Generic;

namespace UniversityHelper.GroupService.Models.Dto.Models;

public class GroupInfo
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public DateTime Date { get; set; }
  public DateTime? EndDate { get; set; }
  public List<CategoryInfo> GroupsCategories { get; set; }
}

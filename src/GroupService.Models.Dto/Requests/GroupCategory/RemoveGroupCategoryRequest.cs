using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;

public class RemoveGroupCategoryRequest
{
  public Guid GroupId { get; set; }
  [Required]
  public List<Guid> CategoriesIds { get; set; }
}

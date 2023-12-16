using System.ComponentModel.DataAnnotations;
using UniversityHelper.GroupService.Models.Dto.Enums;

namespace UniversityHelper.GroupService.Models.Dto.Requests.Category;

public class CreateCategoryRequest
{
  [Required]
  public string Name { get; set; }
  public CategoryColor Color { get; set; }
}


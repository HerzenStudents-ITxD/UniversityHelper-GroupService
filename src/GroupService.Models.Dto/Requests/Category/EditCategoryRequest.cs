using UniversityHelper.GroupService.Models.Dto.Enums;

namespace UniversityHelper.GroupService.Models.Dto.Requests.Category;

public class EditCategoryRequest
{
  public string Name { get; set; }
  public CategoryColor Color { get; set; }
  public bool IsActive { get; set; }
}

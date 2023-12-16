using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;

namespace UniversityHelper.GroupService.Models.Dto.Requests.Group;

public record CreateGroupRequest
{
  [Required]
  public string Name { get; set; }
  public string Address { get; set; }
  public string Description { get; set; }
  public DateTime Date { get; set; }
  public DateTime? EndDate { get; set; }
  public FormatType Format { get; set; }
  public AccessType Access { get; set; }
  [Required]
  public List<UserRequest> Users { get; set; }
  public List<Guid> CategoriesIds { get; set; }
  public List<CreateCategoryRequest> CategoriesRequests { get; set; }
  public List<ImageContent> GroupImages { get; set; }
}

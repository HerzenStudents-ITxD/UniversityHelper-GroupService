using System;
using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Models;

namespace UniversityHelper.GroupService.Models.Dto.Responses.Group;

public class GroupResponse
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Address { get; set; }
  public string Description { get; set; }
  public DateTime Date { get; set; }
  public DateTime? EndDate { get; set; }
  public FormatType Format { get; set; }
  public AccessType Access { get; set; }
  public DateTime CreatedAtUtc { get; set; }
  public List<FileInfo> GroupFiles { get; set; }
  public List<CategoryInfo> GroupCategories { get; set; }
  public List<UserInfo> GroupUsers { get; set; }
  public List<ImageInfo> GroupImages { get; set; }
  public List<CommentInfo> GroupComments { get; set; }
}

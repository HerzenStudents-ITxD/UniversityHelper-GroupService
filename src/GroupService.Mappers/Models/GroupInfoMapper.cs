using System.Linq;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;

namespace UniversityHelper.GroupService.Mappers.Models;

public class GroupInfoMapper : IGroupInfoMapper
{
  private readonly ICategoryInfoMapper _categoryInfoMapper;

  public GroupInfoMapper(ICategoryInfoMapper categoryInfoMapper)
  {
    _categoryInfoMapper = categoryInfoMapper;
  }

  public GroupInfo Map(DbGroup dbGroup)
  {
    return dbGroup is null
      ? null
      : new GroupInfo
      {
        Id = dbGroup.Id,
        Name = dbGroup.Name,
        Description = dbGroup.Description,
        Date = dbGroup.Date,
        GroupsCategories = dbGroup.GroupsCategories.Select(ec => _categoryInfoMapper.Map(ec.Category)).ToList()
      };
  }
}

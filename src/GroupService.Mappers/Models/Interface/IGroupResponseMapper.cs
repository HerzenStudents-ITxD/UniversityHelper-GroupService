using System.Collections.Generic;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Responses.Group;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Models.Broker.Models.File;
using UniversityHelper.Models.Broker.Models.Image;

namespace UniversityHelper.GroupService.Mappers.Models.Interface;

[AutoInject]
public interface IGroupResponseMapper
{
  GroupResponse Map(
    DbGroup dbGroup,
    List<UserData> usersData,
    List<ImageInfo> images,
    List<FileCharacteristicsData> files,
    List<CommentInfo> comments);
}

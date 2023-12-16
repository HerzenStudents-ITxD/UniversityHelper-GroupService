using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Models.Broker.Models.File;

namespace UniversityHelper.GroupService.Broker.Requests.Interfaces;

[AutoInject]
public interface IFileService
{
  Task<List<FileCharacteristicsData>> GetFilesCharacteristicsAsync(List<Guid> filesIds, List<string> errors = null);
}

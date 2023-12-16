using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Enums;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.Core.BrokerSupport.Helpers;
using UniversityHelper.Models.Broker.Models.File;
using UniversityHelper.Models.Broker.Requests.File;
using UniversityHelper.Models.Broker.Responses.File;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace UniversityHelper.GroupService.Broker.Requests;

public class FileService : IFileService
{
  private readonly ILogger<FileService> _logger;
  private readonly IRequestClient<IGetFilesRequest> _rcGetFiles;

  public FileService(
    ILogger<FileService> logger,
    IRequestClient<IGetFilesRequest> rcGetFiles)
  {
    _logger = logger;
    _rcGetFiles = rcGetFiles;
  }

  public async Task<List<FileCharacteristicsData>> GetFilesCharacteristicsAsync(List<Guid> filesIds, List<string> errors = null)
  {
    if (filesIds is null || !filesIds.Any())
    {
      return null;
    }

    return (await RequestHandler.ProcessRequest<IGetFilesRequest, IGetFilesResponse>(
        _rcGetFiles,
        IGetFilesRequest.CreateObj(FileSource.Group, filesIds),
        errors,
        _logger))
      ?.FilesCharacteristicsData;
  }
}

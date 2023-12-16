using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Models.Image;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests;
using UniversityHelper.Core.BrokerSupport.Helpers;
using UniversityHelper.Core.Extensions;
using UniversityHelper.Models.Broker.Enums;
using UniversityHelper.Models.Broker.Models.Image;
using UniversityHelper.Models.Broker.Requests.Image;
using UniversityHelper.Models.Broker.Responses.Image;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UniversityHelper.GroupService.Broker.Requests;

public class ImageService : IImageService
{
  private readonly ILogger<ImageService> _logger;
  private readonly IRequestClient<ICreateImagesRequest> _rcCreateImages;
  private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
  private readonly IImageInfoMapper _mapper;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ImageService(
    ILogger<ImageService> logger,
    IRequestClient<ICreateImagesRequest> rcCreateImages,
    IRequestClient<IGetImagesRequest> rcGetImages,
    IImageInfoMapper mapper,
    IHttpContextAccessor httpContextAccessor)
  {
    _logger = logger;
    _rcCreateImages = rcCreateImages;
    _rcGetImages = rcGetImages;
    _mapper = mapper;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<List<Guid>> CreateImagesAsync(List<ImageContent> images, ResizeParameters resizeParameters, List<string> errors = null)
  {
    return images is null || !images.Any()
      ? null
      : (await RequestHandler
        .ProcessRequest<ICreateImagesRequest, ICreateImagesResponse>(
          _rcCreateImages,
          ICreateImagesRequest.CreateObj(
            images: images.ConvertAll(x => new CreateImageData(x.Name, x.Content, x.Extension, resizeParameters)),
            imageSource: ImageSource.Group,
            createdBy: _httpContextAccessor.HttpContext.GetUserId()),
          errors,
          _logger)).ImagesIds;
  }

  public async Task<List<ImageInfo>> GetImagesAsync(List<Guid> imagesIds, List<string> errors = null)
  {
    return imagesIds is null || !imagesIds.Any()
      ? default
      : (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
          _rcGetImages,
          IGetImagesRequest.CreateObj(imagesIds, ImageSource.Group),
          errors,
          _logger))
        ?.ImagesData
        .ConvertAll(_mapper.Map);
  }
}

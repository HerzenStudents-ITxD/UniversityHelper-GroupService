using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Business.Commands.Category.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.Category;

public class FindCategoriesCommand : IFindCategoriesCommand
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly ICategoryInfoMapper _mapper;

  public FindCategoriesCommand(
    ICategoryRepository categoryRepository,
    ICategoryInfoMapper mapper)
  {
    _categoryRepository = categoryRepository;
    _mapper = mapper;
  }
  
  public async Task<FindResultResponse<CategoryInfo>> ExecuteAsync(
    FindCategoriesFilter filter, 
    CancellationToken cancellationToken = default)
  {
    (List<DbGroupCategory> dbCategories, int totalCount) = await _categoryRepository.FindAsync(filter, cancellationToken);

    return new FindResultResponse<CategoryInfo>(
      body: dbCategories.ConvertAll(_mapper.Map),
      totalCount: totalCount);
  }
}


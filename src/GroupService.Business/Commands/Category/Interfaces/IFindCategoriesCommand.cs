using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.Category.Interfaces;

[AutoInject]
public interface IFindCategoriesCommand
{
  Task<FindResultResponse<CategoryInfo>> ExecuteAsync(
    FindCategoriesFilter filter,
    CancellationToken cancellationToken = default);
}


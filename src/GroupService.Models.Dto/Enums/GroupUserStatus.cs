using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UniversityHelper.GroupService.Models.Dto.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum GroupUserStatus
{
  Invited,
  Refused,
  Participant,
  Discarded
}

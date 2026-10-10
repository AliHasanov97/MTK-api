using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.AddNoticeOfChangeInWorkingConditions;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.GetNoticeOfChangeInWorkingConditionsById;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.SearchNoticesOfChangeInWorkingConditions;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class NoticeOfChangeInWorkingConditionsProfile : Profile
{
    public NoticeOfChangeInWorkingConditionsProfile()
    {
        CreateMap<NoticeOfChangeInWorkingConditions, ResponseObjectWithName>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Index.ToString()));

        CreateMap<NoticeOfChangeInWorkingConditions, AddNoticeOfChangeInWorkingConditionsResponse>();
        CreateMap<NoticeOfChangeInWorkingConditions, GetNoticeOfChangeInWorkingConditionsByIdResponse>();
        CreateMap<NoticeOfChangeInWorkingConditions, SearchNoticesOfChangeInWorkingConditionsResponseItem>();
    }
}
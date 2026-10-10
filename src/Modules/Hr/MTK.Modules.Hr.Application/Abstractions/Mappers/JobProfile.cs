using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.Jobs.AddJob;
using MTK.Modules.Hr.Application.Jobs.GetJobById;
using MTK.Modules.Hr.Application.Jobs.SearchJobs;
using MTK.Modules.Hr.Application.Jobs.UpdateJob;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Abstractions.Mappers;

public class JobProfile : Profile
{
    public JobProfile()
    {
        CreateMap<Job, ResponseObjectWithName>();
        CreateMap<Job, AddJobResponse>();
        CreateMap<Job, UpdateJobResponse>();
        CreateMap<Job, SearchJobsResponseItem>();
        CreateMap<Job, GetJobByIdResponse>();
    }
}

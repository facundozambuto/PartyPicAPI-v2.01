using AutoMapper;
using PartyPic.DTOs.Plans;
using PartyPic.Models.Plans;

namespace PartyPic.Profiles.Plans
{
    public class PlanProfile : Profile
    {
        public PlanProfile()
        {
            CreateMap<Plan, PlanReadDTO>();
            CreateMap<PlanReadDTO, Plan>();
            CreateMap<PlanCreateDTO, Plan>();
            CreateMap<PlanGrid, PlanReadDTOGrid>();
            CreateMap<PlanReadDTOGrid, PlanGrid>();
            CreateMap<PlanUpdateDTO, Plan>();
            CreateMap<Plan, PlanUpdateDTO>();
            CreateMap<AllPlansResponse, GetAllPlansApiResponse>();
            CreateMap<GetAllPlansApiResponse, AllPlansResponse>();
            CreateMap<AllPlansResponse, GetAllPlansApiResponse>();
            CreateMap<GetAllPlansApiResponse, AllPlansResponse>();
            CreateMap<PlanGrid, GridPlanApiResponse>();
            CreateMap<GridPlanApiResponse, PlanGrid>();
            CreateMap<PlanApiResponse, PlanReadDTO>();
            CreateMap<PlanReadDTO, PlanApiResponse>();
        }
    }
}

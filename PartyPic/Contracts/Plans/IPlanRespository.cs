using PartyPic.DTOs.Plans;
using PartyPic.Models.Plans;
using PartyPic.Models.Common;
using System.Collections.Generic;

namespace PartyPic.Contracts.Plans
{
    public interface IPlanRepository
    {
        AllPlansResponse GetAllPlans();
        PlanReadDTO GetPlanById(int id);
        PlanReadDTO CreatePlan(PlanCreateDTO plan);
        bool SaveChanges();
        void DeletePlan(int id);
        PlanReadDTO UpdatePlan(int id, PlanUpdateDTO plan);
        void PartiallyUpdate(int id, PlanUpdateDTO plan);
        PlanGrid GetAllPlansForGrid(GridRequest gridRequest);
        IEnumerable<PlanPriceHistoryDTO> GetPlanPriceHistory(int planId);
    }
}

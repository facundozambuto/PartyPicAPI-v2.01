using AutoMapper;
using PartyPic.DTOs.Plans;
using PartyPic.Helpers;
using PartyPic.Models.Plans;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PartyPic.Contracts.Plans
{
    public class SqlPlanRepository : IPlanRepository
    {
        private readonly PlanContext _planContext;
        private readonly IMapper _mapper;

        public SqlPlanRepository(PlanContext planContext, IMapper mapper)
        {
            _planContext = planContext;
            _mapper = mapper;
        }

        public PlanReadDTO CreatePlan(PlanCreateDTO newPlan)
        {
            this.ThrowExceptionIfArgumentIsNull(newPlan);

            var plan = _mapper.Map<Plan>(newPlan);

            plan.CreatedDatetime = DateTime.Now;

            _planContext.Plans.Add(plan);

            this.SaveChanges();

            var addedPlan = _planContext.Plans.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            _planContext.PriceHistories.Add(new PriceHistory
            {
                PlanId = addedPlan.Id,
                Price = newPlan.InitialPrice,
                CreatedDatetime = DateTime.Now,
                StartDate = DateTime.Now
            });

            this.SaveChanges();

            return _mapper.Map<PlanReadDTO>(addedPlan);
        }

        public void DeletePlan(int id)
        {
            var retrievedPlan = _planContext.Plans.FirstOrDefault(p => p.Id == id);

            if (retrievedPlan == null)
            {
                throw new NotPlanFoundException();
            }

            _planContext.Plans.Remove(retrievedPlan);

            this.SaveChanges();
        }


        public AllPlansResponse GetAllPlans()
        {
            return new AllPlansResponse
            {
                Plans = _planContext.Plans
                    .Select(plan => new PlanReadDTO
                    {
                        Id = plan.Id,
                        Name = plan.Name,
                        Description = plan.Description,
                        CreatedDatetime = plan.CreatedDatetime,
                        LatestPrice = plan.PriceHistories
                                            .Where(ph => !ph.EndDate.HasValue)
                                            .OrderByDescending(ph => ph.StartDate)
                                            .Select(ph => ph.Price)
                                            .FirstOrDefault()
                    })
                    .ToList()
            };
        }

        public PlanGrid GetAllPlansForGrid(GridRequest gridRequest)
        {
            var planRows = new List<PlanReadDTO>();

            planRows = _planContext.Plans
                    .Select(plan => new PlanReadDTO
                    {
                        Id = plan.Id,
                        Name = plan.Name,
                        Description = plan.Description,
                        CreatedDatetime = plan.CreatedDatetime,
                        LatestPrice = plan.PriceHistories
                                            .Where(ph => !ph.EndDate.HasValue)
                                            .OrderByDescending(ph => ph.StartDate)
                                            .Select(ph => ph.Price)
                                            .FirstOrDefault()
                    })
                    .ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                planRows = _planContext.Plans.Where(pl => pl.Description.Contains(gridRequest.SearchPhrase))
                                .Select(pl => new PlanReadDTO
                                {
                                    Id = pl.Id,
                                    Name = pl.Name,
                                    Description = pl.Description,
                                    CreatedDatetime = pl.CreatedDatetime
                                })
                           .ToList();
            }

            if (gridRequest.RowCount != -1 && _planContext.Plans.Count() > gridRequest.RowCount && gridRequest.Current > 0 && planRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((planRows.Count % gridRequest.RowCount) != 0 && (planRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = planRows.Count % gridRequest.RowCount;
                }

                planRows = planRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                planRows = planRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    planRows.Reverse();
                }
            }

            var categoriesGrid = new PlanGrid
            {
                Rows = planRows,
                Total = _planContext.Plans.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return categoriesGrid;
        }

        public PlanReadDTO GetPlanById(int id)
        {
            var plan = _planContext.Plans
                .Include(p => p.PriceHistories)
                .Where(p => p.Id == id)
                .Select(p => new PlanReadDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedDatetime = p.CreatedDatetime,
                    LatestPrice = p.PriceHistories
                        .Where(ph => !ph.EndDate.HasValue)
                        .OrderByDescending(ph => ph.StartDate)
                        .Select(ph => ph.Price)
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            if (plan == null)
            {
                throw new NotPlanFoundException();
            }

            return plan;
        }

        public void PartiallyUpdate(int id, PlanUpdateDTO plan)
        {
            this.UpdatePlan(id, plan);

            this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_planContext.SaveChanges() >= 0);
        }

        public PlanReadDTO UpdatePlan(int id, PlanUpdateDTO planUpdateDto)
        {
            var retrievedPlan = this.GetPlanById(id);

            if (retrievedPlan == null)
            {
                throw new NotPlanFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(planUpdateDto);

            var planEntity = _planContext.Plans.FirstOrDefault(p => p.Id == id);
            if (planEntity == null)
            {
                throw new NotPlanFoundException();
            }

            planEntity.Name = planUpdateDto.Name;
            planEntity.Description = planUpdateDto.Description;

            this.ThrowExceptionIfPropertyIsIncorrect(planEntity, false);

            if (retrievedPlan.LatestPrice != planUpdateDto.Price)
            {
                var lastPrice = _planContext.PriceHistories
                    .Where(ph => ph.PlanId == id && !ph.EndDate.HasValue)
                    .OrderByDescending(ph => ph.StartDate)
                    .FirstOrDefault();

                if (lastPrice != null)
                {
                    lastPrice.EndDate = DateTime.Now;
                    _planContext.PriceHistories.Update(lastPrice);
                }

                _planContext.PriceHistories.Add(new PriceHistory
                {
                    PlanId = id,
                    Price = planUpdateDto.Price,
                    CreatedDatetime = DateTime.Now,
                    StartDate = DateTime.Now
                });
            }

            this.SaveChanges();

            return this.GetPlanById(id);
        }

        public IEnumerable<PlanPriceHistoryDTO> GetPlanPriceHistory(int planId)
        {
            var prices = _planContext.PriceHistories
                .Where(ph => ph.PlanId == planId)
                .OrderBy(ph => ph.StartDate)
                .Select(ph => new PlanPriceHistoryDTO
                {
                    PlanName = ph.Plan.Name,
                    Price = ph.Price,
                    StartDate = ph.StartDate,
                    EndDate = ph.EndDate
                })
                .ToList();

            if (!prices.Any())
            {
                throw new NotPlanFoundException();
            }

            return prices;
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Plan plan, bool isNew)
        {
            if (_planContext.Plans.Any(pl => pl.Description == plan.Description && pl.Id != plan.Id))
            {
                throw new PropertyIncorrectException();
            }

            if (_planContext.Plans.Any(pl => pl.Name == plan.Name && pl.Id != plan.Id))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfArgumentIsNull(PlanCreateDTO plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            if (string.IsNullOrEmpty(plan.Description))
            {
                throw new ArgumentNullException(nameof(plan.Description));
            }
        }

        private void ThrowExceptionIfArgumentIsNull(PlanUpdateDTO plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            if (string.IsNullOrEmpty(plan.Description))
            {
                throw new ArgumentNullException(nameof(plan.Description));
            }
        }
    }
}

using AutoMapper;
using PartyPic.Helpers;
using PartyPic.Models.Subscriptions;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using PartyPic.DTOs.Subscriptions;
using PartyPic.Models.Users;
using Microsoft.AspNetCore.Http;
using PartyPic.ThirdParty;
using PartyPic.Contracts.Plans;
using PartyPic.Contracts.Users;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PartyPic.ThirdParty.Impl;
using PartyPic.Contracts.Subscriptions;

namespace PartyPic.Contracts.Subscriptions
{
    public class SqlSubscriptionRepository : ISubscriptionRepository
    {
        private readonly SubscriptionContext _subscriptionContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrencyConverter _currencyConverterManager;
        private readonly PlanContext _planContext;
        private readonly UserContext _userContext;
        private readonly PaymentGatewayFactory _factory;

        public SqlSubscriptionRepository(SubscriptionContext subscriptionContext,
                                        IMapper mapper,
                                        IHttpContextAccessor httpContextAccessor,
                                        ICurrencyConverter currencyConverterManager,
                                        PlanContext planContext,
                                        UserContext userContext,
                                        PaymentGatewayFactory factory)
        {
            _subscriptionContext = subscriptionContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _planContext = planContext;
            _userContext = userContext;
            _currencyConverterManager = currencyConverterManager;
            _factory = factory;
        }

        public SubscriptionReadDTO CreateSubscription(SubscriptionCreateDTO subscriptionDto)
        {
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            if (_subscriptionContext.Subscriptions.Any(s => s.IsActive && s.MercadoPagoId.ToUpper() != "PENDING" && currentUser.UserId == s.UserId))
            {
                throw new AlreadyExistingElementException();
            }

            this.ThrowExceptionIfArgumentIsNull(subscriptionDto);

            var newSub = new Subscription
            {
                PlanId = subscriptionDto.PlanId,
                IsActive = false,
                IsAutoRenew = subscriptionDto.IsAutoRenew,
                UserId = currentUser.UserId,
                StartDate = DateTime.Now,
                EndDate = DateTime.MinValue,
                CreatedDatetime = DateTime.Now,
                MarketReference = Guid.NewGuid(),
                MercadoPagoId = "Pending"
            };

            _subscriptionContext.Subscriptions.Add(newSub);

            this.SaveChanges();

            var paymentGateway = _factory.GetPaymentGateway("MercadoPago");

            var mpInitPoint = paymentGateway.CreateSubscription(new MPSNewSubscriptionRequest
            {
                MarketReference = newSub.MarketReference,
                IsAutoRenew = newSub.IsAutoRenew,
                PlanType = newSub.PlanId == 1 ? "months" : "years",
                Amount = (double)this.GetSubscriptionAmount(newSub.PlanId),
                UserEmail = _userContext.Users.FirstOrDefault(u => u.UserId == newSub.UserId).Email
            });

            var addedSubscription = _subscriptionContext.Subscriptions.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            var response = _mapper.Map<SubscriptionReadDTO>(addedSubscription);

            response.MPInitPoint = mpInitPoint;

            return response;
        }

        public void DeleteSubscription(int id)
        {
            var subscription = _subscriptionContext.Subscriptions.FirstOrDefault(s => s.SubscriptionId == id);

            if (subscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            _subscriptionContext.Subscriptions.Remove(subscription);

            this.SaveChanges();
        }

        public AllSubscriptionsResponse GetAllSubscriptions()
        {
            var subs = _mapper.Map<List<SubscriptionReadDTO>>(_subscriptionContext.Subscriptions.ToList());

            return new AllSubscriptionsResponse
            {
                Subscriptions = subs
            };
        }

        public async Task<AllSubscriptionsResponse> GetAllMySubscriptionsAsync()
        {
            try
            {
                var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

                var availablePlans = await _planContext.Plans.ToListAsync();

                var subs = await _subscriptionContext.Subscriptions
                    .Where(s => s.UserId == currentUser.UserId)
                    .ToListAsync();

                var subscriptionDtos = subs.Select(s => new SubscriptionReadDTO
                {
                    SubscriptionId = s.SubscriptionId,
                    UserId = s.UserId,
                    PlanType = availablePlans.FirstOrDefault(p => p.Id == s.PlanId)?.Name ?? "N/A",
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive,
                    IsAutoRenew = s.IsAutoRenew,
                    MarketReference = s.MarketReference,
                    MercadoPagoId = s.MercadoPagoId,
                    CreatedDatetime = s.CreatedDatetime,
                    IsCancelled = s.IsCancelled,
                    CancelledDate = s.CancelledDate,
                    RenewalDate = DateTime.MinValue,
                    LatestPrice = this.GetSubscriptionAmount(s.PlanId)
                }).ToList();

                var activeSubscriptions = subscriptionDtos
                    .Where(s => s.IsActive && !string.IsNullOrEmpty(s.MercadoPagoId))
                    .ToList();

                if (activeSubscriptions.Any())
                {
                    var tasks = activeSubscriptions.Select(async activeSub =>
                    {
                        try
                        {
                            var paymentGateway = _factory.GetPaymentGateway("MercadoPago");

                            var mpSub = await paymentGateway.GetSubscriptionAsync(activeSub.MercadoPagoId);

                            if (mpSub.Status.ToUpper() != "AUTHORIZED")
                            {
                                throw new NotActiveSubscription();
                            }
                            activeSub.RenewalDate = (DateTime)(mpSub?.NextPaymentDate ?? null);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    });

                    await Task.WhenAll(tasks);
                }

                var response = new AllSubscriptionsResponse
                {
                    Subscriptions = subscriptionDtos
                };

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public SubscriptionGrid GetAllSubscriptionsForGrid(GridRequest gridRequest)
        {
            var subscriptionRows = new List<Subscription>();

            subscriptionRows = _subscriptionContext.Subscriptions.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                subscriptionRows = _subscriptionContext.Subscriptions.Where(sub => sub.MarketReference.ToString().Contains(gridRequest.SearchPhrase)).ToList();
            }

            if (gridRequest.RowCount != -1 && _subscriptionContext.Subscriptions.Count() > gridRequest.RowCount && gridRequest.Current > 0 && subscriptionRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((subscriptionRows.Count % gridRequest.RowCount) != 0 && (subscriptionRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = subscriptionRows.Count % gridRequest.RowCount;
                }

                subscriptionRows = subscriptionRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                subscriptionRows = subscriptionRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    subscriptionRows.Reverse();
                }
            }

            var susbcriptionsGrid = new SubscriptionGrid
            {
                Rows = subscriptionRows,
                Total = _subscriptionContext.Subscriptions.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return susbcriptionsGrid;
        }

        public async Task<SubscriptionReadDTO> GetSubscriptionByIdAsync(int subId)
        {
            var subscription = _subscriptionContext.Subscriptions.FirstOrDefault(sub => sub.SubscriptionId == subId);

            if (subscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            var response =  _mapper.Map<SubscriptionReadDTO>(subscription);

            var availablePlans = await _planContext.Plans.ToListAsync();

            response.LatestPrice = this.GetSubscriptionAmount(subscription.PlanId);
            response.PlanType = availablePlans.FirstOrDefault(p => p.Id == subscription.PlanId).Name;

            return response;
        }

        public void PartiallyUpdate(int id, SubscriptionUpdateDTO subscription)
        {
            this.UpdateSubscriptionAsync(id, subscription);

            this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_subscriptionContext.SaveChanges() >= 0);
        }

        public async Task<SubscriptionReadDTO> UpdateSubscriptionAsync(int id, SubscriptionUpdateDTO subscriptionUpdateDto)
        {
            var retrievedSubscription = _subscriptionContext.Subscriptions.FirstOrDefault(s => s.SubscriptionId == id);

            if (retrievedSubscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            _mapper.Map(subscriptionUpdateDto, retrievedSubscription);

            _subscriptionContext.Subscriptions.Update(retrievedSubscription);

            this.SaveChanges();

            return  await this.GetSubscriptionByIdAsync(id);
        }

        public SubscriptionReadDTO ConfirmSubscription(string externalReference)
        {
            var externalReferenceGuid = Guid.Parse(externalReference);

            var subscription = _subscriptionContext.Subscriptions
                .FirstOrDefault(s => s.MarketReference == externalReferenceGuid);

            if (subscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            if (subscription.IsActive || subscription.MercadoPagoId.ToUpper() != "PENDING" || currentUser.UserId != subscription.UserId)
            {
                throw new InvalidOperationException();
            }

            var paymentGateway = _factory.GetPaymentGateway("MercadoPago");

            var mercadoPagoSubscription = paymentGateway.GetSubscriptionByExternalReference(externalReference);

            if (mercadoPagoSubscription == null || mercadoPagoSubscription.Status.ToUpper() != "AUTHORIZED")
            {
                throw new NotSubscriptionFoundException();
            }

            subscription.IsActive = true;
            subscription.MercadoPagoId = mercadoPagoSubscription.Id;

            if (!subscription.IsAutoRenew && mercadoPagoSubscription.AutoRecurring.EndDate != null)
            {
                subscription.EndDate = mercadoPagoSubscription.AutoRecurring.EndDate;
            }

            _subscriptionContext.Update(subscription);

            this.SaveChanges();

            var response = _mapper.Map<SubscriptionReadDTO>(subscription);

            response.LatestPrice = this.GetSubscriptionAmount(subscription.PlanId);

            return response;
        }

        public async Task<SubscriptionReadDTO> ToggleAutoRenewalAsync(int subId)
        {
            var subscription = _subscriptionContext.Subscriptions.FirstOrDefault(s => s.SubscriptionId == subId);

            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            if (subscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            if (subscription.UserId != currentUser.UserId)
            {
                throw new InvalidOperationException();
            }

            if (!subscription.IsActive)
            {
                throw new NotActiveSubscription();
            }

            var paymentGateway = _factory.GetPaymentGateway("MercadoPago");

            if (await paymentGateway.ToggleAutoRenewAsync(subscription.MercadoPagoId, !subscription.IsAutoRenew))
            {
                if (subscription.IsAutoRenew)
                {
                    subscription.IsActive = false;
                    subscription.IsCancelled = true;
                    subscription.CancelledDate = DateTime.Now;
                }
                subscription.IsAutoRenew = !subscription.IsAutoRenew;
            }

            _subscriptionContext.Update(subscription);

            this.SaveChanges();

            return _mapper.Map<SubscriptionReadDTO>(subscription);
        }

        private void ThrowExceptionIfArgumentIsNull(SubscriptionCreateDTO subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
        }

        private decimal GetSubscriptionAmount(int? planId)
        {
            var latestPlanPrice = _planContext.Plans
                            .SelectMany(p => p.PriceHistories)
                            .Where(ph => !ph.EndDate.HasValue && ph.PlanId == planId)
                            .OrderByDescending(ph => ph.StartDate)
                            .Select(ph => ph.Price)
                            .FirstOrDefault();

            return _currencyConverterManager.GetAmountOfPesosByUSD(amountOfUSD: latestPlanPrice);
        }
    }
}
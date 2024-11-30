using AutoMapper;
using PartyPic.Helpers;
using PartyPic.Models.Subscriptions;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using PartyPic.DTOs.Subscriptions;

namespace PartyPic.Contracts.Subscriptions
{
    public class SqlSubscriptionRepository : ISubscriptionRespository
    {
        private readonly SubscriptionContext _subscriptionContext;
        private readonly IMapper _mapper;

        public SqlSubscriptionRepository(SubscriptionContext subscriptionContext, IMapper mapper)
        {
            _subscriptionContext = subscriptionContext;
            _mapper = mapper;
        }

        public Subscription CreateSubscription(Subscription subscription)
        {
            this.ThrowExceptionIfArgumentIsNull(subscription);
            this.ThrowExceptionIfPropertyAlreadyExists(subscription, true, 0);

            subscription.CreatedDatetime = DateTime.Now;

            _subscriptionContext.Subscriptions.Add(subscription);

            this.SaveChanges();

            var addedSubscription = _subscriptionContext.Subscriptions.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedSubscription;
        }

        public void DeleteSubscription(int id)
        {
            var subscription = this.GetSubscriptionById(id);

            if (subscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            _subscriptionContext.Subscriptions.Remove(subscription);

            this.SaveChanges();
        }

        public AllSubscriptionsResponse GetAllSubscriptions()
        {
            return new AllSubscriptionsResponse
            {
                Subscriptions = _subscriptionContext.Subscriptions.ToList()
            };
        }

        public SubscriptionGrid GetAllSubscriptionsForGrid(GridRequest gridRequest)
        {
            var subscriptionRows = new List<Subscription>();

            subscriptionRows = _subscriptionContext.Subscriptions.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                subscriptionRows = _subscriptionContext.Subscriptions.Where(sub => sub.PlanType.Contains(gridRequest.SearchPhrase)).ToList();
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

            var categoriesGrid = new SubscriptionGrid
            {
                Rows = subscriptionRows,
                Total = _subscriptionContext.Subscriptions.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return categoriesGrid;
        }

        public Subscription GetSubscriptionById(int id)
        {
            var subscription = _subscriptionContext.Subscriptions.FirstOrDefault(sub => sub.SubscriptionId == id);

            if (subscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            return subscription;
        }

        public void PartiallyUpdate(int id, SubscriptionUpdateDTO subscription)
        {
            this.UpdateSubscription(id, subscription);

            this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_subscriptionContext.SaveChanges() >= 0);
        }

        public Subscription UpdateSubscription(int id, SubscriptionUpdateDTO subscriptionUpdateDto)
        {
            var subscription = _mapper.Map<Subscription>(subscriptionUpdateDto);

            var retrievedSubscription = this.GetSubscriptionById(id);

            if (retrievedSubscription == null)
            {
                throw new NotSubscriptionFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(subscription);
            this.ThrowExceptionIfPropertyIsIncorrect(subscription, false, id);

            _mapper.Map(subscriptionUpdateDto, retrievedSubscription);

            _subscriptionContext.Subscriptions.Update(retrievedSubscription);

            this.SaveChanges();

            return this.GetSubscriptionById(id);
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Subscription subscription, bool isNew, int id)
        {
            if (_subscriptionContext.Subscriptions.ToList().Any(sub => sub.PlanType == subscription.PlanType))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfPropertyAlreadyExists(Subscription subscription, bool isNew, int id)
        {
            if (!isNew)
            {
                if (subscription.CreatedDatetime != _subscriptionContext.Subscriptions.FirstOrDefault(e => e.SubscriptionId == id).CreatedDatetime)
                {
                    throw new PropertyIncorrectException();
                }
            }

            if (_subscriptionContext.Subscriptions.ToList().Any(sub => sub.PlanType == subscription.PlanType))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfArgumentIsNull(Subscription subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            if (string.IsNullOrEmpty(subscription.PlanType))
            {
                throw new ArgumentNullException(nameof(subscription.PlanType));
            }
        }
    }
}

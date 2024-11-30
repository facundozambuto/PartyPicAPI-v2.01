namespace PartyPic.Contracts.Payments
{
    using AutoMapper;
    using PartyPic.DTOs.Payments;
    using PartyPic.Helpers;
    using PartyPic.Models.Common;
    using PartyPic.Models.Exceptions;
    using PartyPic.Models.Payments;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SqlPaymentRepository : IPaymentRespository
    {
        private readonly PaymentContext _paymentContext;
        private readonly IMapper _mapper;

        public SqlPaymentRepository(PaymentContext paymentContext, IMapper mapper)
        {
            _paymentContext = paymentContext;
            _mapper = mapper;
        }

        public Payment CreatePayment(Payment payment)
        {
            this.ThrowExceptionIfArgumentIsNull(payment);
            this.ThrowExceptionIfPropertyAlreadyExists(payment, true, 0);

            payment.CreatedDatetime = DateTime.Now;

            _paymentContext.Payments.Add(payment);

            this.SaveChanges();

            var addedPayment = _paymentContext.Payments.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedPayment;
        }

        public void DeletePayment(int id)
        {
            var payment = this.GetPaymentById(id);

            if (payment == null)
            {
                throw new NotPaymentFoundException();
            }

            _paymentContext.Payments.Remove(payment);

            this.SaveChanges();
        }

        public AllPaymentsResponse GetAllPayments()
        {
            return new AllPaymentsResponse
            {
                Payments = _paymentContext.Payments.ToList()
            };
        }

        public PaymentGrid GetAllPaymentsForGrid(GridRequest gridRequest)
        {
            var paymentRows = new List<Payment>();

            paymentRows = _paymentContext.Payments.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                paymentRows = _paymentContext.Payments.Where(pay => pay.PaymentType.Contains(gridRequest.SearchPhrase)).ToList();
            }

            if (gridRequest.RowCount != -1 && _paymentContext.Payments.Count() > gridRequest.RowCount && gridRequest.Current > 0 && paymentRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((paymentRows.Count % gridRequest.RowCount) != 0 && (paymentRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = paymentRows.Count % gridRequest.RowCount;
                }

                paymentRows = paymentRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                paymentRows = paymentRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    paymentRows.Reverse();
                }
            }

            var categoriesGrid = new PaymentGrid
            {
                Rows = paymentRows,
                Total = _paymentContext.Payments.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return categoriesGrid;
        }

        public Payment GetPaymentById(int id)
        {
            var payment = _paymentContext.Payments.FirstOrDefault(pay => pay.PaymentId == id);

            if (payment == null)
            {
                throw new NotPaymentFoundException();
            }

            return payment;
        }

        public void PartiallyUpdate(int id, PaymentUpdateDTO payment)
        {
            this.UpdatePayment(id, payment);

            this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_paymentContext.SaveChanges() >= 0);
        }

        public Payment UpdatePayment(int id, PaymentUpdateDTO paymentUpdateDto)
        {
            var payment = _mapper.Map<Payment>(paymentUpdateDto);

            var retrievedPayment = this.GetPaymentById(id);

            if (retrievedPayment == null)
            {
                throw new NotPaymentFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(payment);
            this.ThrowExceptionIfPropertyIsIncorrect(payment, false, id);

            _mapper.Map(paymentUpdateDto, retrievedPayment);

            _paymentContext.Payments.Update(retrievedPayment);

            this.SaveChanges();

            return this.GetPaymentById(id);
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Payment payment, bool isNew, int id)
        {
            if (_paymentContext.Payments.ToList().Any(pay => pay.PaymentType == payment.PaymentType))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfPropertyAlreadyExists(Payment payment, bool isNew, int id)
        {
            if (!isNew)
            {
                if (payment.CreatedDatetime != _paymentContext.Payments.FirstOrDefault(e => e.PaymentId == id).CreatedDatetime)
                {
                    throw new PropertyIncorrectException();
                }
            }

            if (_paymentContext.Payments.ToList().Any(pay => pay.PaymentType == payment.PaymentType))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfArgumentIsNull(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            if (string.IsNullOrEmpty(payment.PaymentStatus))
            {
                throw new ArgumentNullException(nameof(payment.PaymentStatus));
            }
        }
    }
}

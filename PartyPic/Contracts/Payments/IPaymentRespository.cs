using PartyPic.DTOs.Payments;
using PartyPic.Models.Payments;
using PartyPic.Models.Common;

namespace PartyPic.Contracts.Payments
{
    public interface IPaymentRepository
    {
        AllPaymentsResponse GetAllPayments();
        Payment GetPaymentById(int id);
        Payment CreatePayment(Payment category);
        bool SaveChanges();
        void DeletePayment(int id);
        Payment UpdatePayment(int id, PaymentUpdateDTO category);
        void PartiallyUpdate(int id, PaymentUpdateDTO category);
        PaymentGrid GetAllPaymentsForGrid(GridRequest gridRequest);
    }
}

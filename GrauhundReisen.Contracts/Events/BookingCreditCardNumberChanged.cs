using System;

namespace GrauhundReisen.Contracts.Events
{
    public class BookingCreditCardNumberChanged
    {
        public string BookingId { get; set; }
        public String CreditCardNumber { get; set; }
    }
}
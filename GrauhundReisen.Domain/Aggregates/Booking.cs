using System;
using System.Collections.Generic;
using System.Linq;
using GrauhundReisen.Contracts.Events;

namespace GrauhundReisen.Domain.Aggregates
{
    internal class Booking
    {
        private readonly IEnumerable<object> _history;
        private readonly IList<object> _changes;

        public static Booking FromEvents(IEnumerable<object> events)
        {
            return new Booking(events);
        }

        public Booking()
        {
            _changes = new List<object>();
        }

        protected Booking(IEnumerable<object> history) : this()
        {
            _history = history;
        }

        public IEnumerable<object> Changes
        {
            get { return _changes; }
        }

        public string Id
        {
            get { return _history.OfType<BookingOrdered>().First().BookingId; }
        }

        public void Order(string bookingId, string destination, 
            string creditCardNumber, string creditCardType, 
            string email, string firstName, string lastName)
        {
            var bookingOrdered = new BookingOrdered()
            {
                BookingId = bookingId,
                Destination = destination,
                CreditCardNumber = creditCardNumber,
                CreditCardType = creditCardType,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            _changes.Add(bookingOrdered);
        }

        public void UpdateEmail(String email)
        {
            var emailChanged = new BookingEmailChanged()
            {
                BookingId = this.Id,
                Email = email
            };

            _changes.Add(emailChanged);
        }

        public void UpdateCreditCardNumber(string creditCardNumber)
        {
            var creditCardNumberChanged = new BookingCreditCardNumberChanged()
            {
                BookingId = this.Id,
                CreditCardNumber = creditCardNumber
            };

            _changes.Add(creditCardNumberChanged);
        }
    }
}
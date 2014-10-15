using System;
using System.Linq;
using System.Threading.Tasks;
using GrauhundReisen.Domain.Aggregates;
using Grauhundreisen.Infrastructure;

namespace GrauhundReisen.Domain.Services
{
    public class BookingService
    {
        private readonly EventStoreClient _eventStoreClient;
        private Func<object, Task> _eventHandling;

        public BookingService()
        {
            _eventStoreClient = EventStoreClient.InitWith(
                DomainSettings.DefaultEventStoreCleintConfiguration);
        }

        public void WhenStatusChanged(Func<object, Task> handle)
        {
            _eventHandling = handle;
        }

        public async Task OrderBooking(String bookingId, 
            string destination, 
            string creditCardNumber, string creditCardType, 
            string email, string firstName, string lastName)
        {
            // Das aller erste Mal, wenn eine Buchung beauftragt wird, 
            // hat sie noch keine Events und wird daher initial erzeugt
            var booking = new Booking();

            booking.Order(bookingId, 
                destination, 
                creditCardNumber, creditCardType, 
                email, firstName, lastName);

            await Task.Factory.StartNew(() => booking.Changes.ToList().ForEach(
                change =>
                {
                    _eventStoreClient.Store(bookingId, change);
                    _eventHandling(change);
                }));
        }

        public async Task UpdateBookingDetails(String bookingId, String email, String creditCardNumber)
        {
            var aggregateEvents = _eventStoreClient.RetrieveFor(bookingId);
            var booking = Booking.FromEvents(aggregateEvents);

            booking.UpdateEmail(email);
            booking.UpdateCreditCardNumber(creditCardNumber);

            await Task.Factory.StartNew(() => booking.Changes.ToList().ForEach(
                change =>
                {
                    _eventStoreClient.Store(booking.Id, change);
                    _eventHandling(change);
                }));
        }
    }
}

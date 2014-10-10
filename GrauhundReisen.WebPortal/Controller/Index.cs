using System;
using DatabaseLayer;
using Nancy;
using Nancy.ModelBinding;
using DatabaseLayer.DataObjects;

namespace GrauhundReisen.WebPortal
{
	public class Index : NancyModule
	{
		DatabaseServer _db;

		public Index (DatabaseServer db)
		{
			_db = db;

			Get [""] = _ => {

				var emptyOrderForm = new OrderFormViewModel {
					CreditCardTypes = db.GetAlCreditCardTypes (),
					Destinations = db.GetAllDestinations ()
				};

				return View ["index", emptyOrderForm];
			};

			Post [""] = _ => ProceedBooking ();
		}

		object ProceedBooking ()
		{
			var orderVM = this.Bind<OrderViewModel> ();
			var order = new Order ();

			order.Traveller.FirstName = orderVM.TravellerFirstName;
			order.Traveller.LastName = orderVM.TravellerLastName;
			order.Traveller.EMail = orderVM.TravellerEmail;
			order.Payment.CreditCardType = orderVM.PaymentCreditCardType;
			order.Payment.CreditCardNumber = orderVM.PaymentCreditCardNumber;
			order.Destination = orderVM.Destination;

			var bookingId = _db.SaveOrder (order);

			return View ["confirmation", new {BookingId = bookingId}];
		}
	}
}
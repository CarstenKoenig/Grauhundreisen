using System;

namespace GrauhundReisen.Contracts.Events
{
	public class BookingUpdated
	{
		public String BookingId {
			get;
			set;
		}

		public String Email {
			get;
			set;
		}

		public String CreditCardNumber {
			get;
			set;
		}
	}
}


using System;

namespace GrauhundReisen.ReadModel.Models
{
	public class Booking
	{
		public String Id {
			get;
			set;
		}

        public String FirstName
        {
            get;
            set;
        }

        public String LastName
        {
            get;
            set;
        }

        public String EMail
        {
            get;
            set;
        }
        public String CreditCardType
        {
            get;
            set;
        }

        public String CreditCardNumber
        {
            get;
            set;
        }

		public String Destination {
			get;
			set;
		}
	}
}
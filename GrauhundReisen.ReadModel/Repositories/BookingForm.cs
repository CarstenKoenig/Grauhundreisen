using System.Collections.Generic;
using GrauhundReisen.Contracts.ViewModels;
using GrauhundReisen.ReadModel.Repositories;

namespace GrauhundReisen.ReadModel.Repositories
{
	public class BookingForm
	{
		public IEnumerable<CreditCardType> CreditCardTypes
		{
			get {
				return new CreditCardTypes ().GetAll (); 
			}
		}

		public IEnumerable<Destination> Destinations
		{
			get{
				return new Destinations ().GetAll ();
			}
		}
	}
}


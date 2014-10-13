using System.Collections.Generic;
using System.Linq;
using GrauhundReisen.Contracts.ViewModels;

namespace GrauhundReisen.ReadModel.Repositories
{
	public class Destinations
	{
		public IEnumerable<Destination> GetAll(){

			var destinations =  new List<Destination>{ 
				new Destination{Name="Berlin"},
				new Destination{Name="Hamburg"},
				new Destination{Name="München"},
				new Destination{Name="Köln"},
				new Destination{Name="Leipzig"},
				new Destination{Name="Dresden"},
				new Destination{Name="Rostock"},
			};

			return destinations.OrderBy(destination => destination.Name);
		}
	}

}


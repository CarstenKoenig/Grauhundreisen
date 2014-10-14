using Nancy;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;
using EyKeule;
using System;

namespace GrauhundReisen.WebPortal
{
	public class Boostrapper : DefaultNancyBootstrapper
	{
		const string ConnectionString = "Content/DataStore/Bookings/";

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;

			var esClientConfig = new EventStoreClientConfiguration {
				AccountId = "MyTestAccount",
				InitActionName = "init",
				RemoveActionName= "remove",
				RetrieveActionName="events",
				StoreActionName = "store",
				ServerUri = new Uri("http://openspace2014.azurewebsites.net")
			};

			var eventStoreClient = EventStoreClient.InitWith (esClientConfig);
			var bookingHandler = new BookingHandler (eventStoreClient, ConnectionString);
			var bookingForm = new BookingForm ();
			var bookings = new Bookings (ConnectionString);

			container.Register (eventStoreClient);
			container.Register (bookingHandler);
			container.Register (bookingForm);
			container.Register (bookings);

			base.ApplicationStartup (container, pipelines);
		}
	}
}
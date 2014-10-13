using Nancy;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;

namespace GrauhundReisen.WebPortal
{
	public class Boostrapper : DefaultNancyBootstrapper
	{
		const string ConnectionString = "Content/DataStore/Bookings/";

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;

			container.Register<BookingForm> (new BookingForm ());
			container.Register<Bookings> (new Bookings(ConnectionString));
			container.Register<BookingHandler> (new BookingHandler (ConnectionString));

			base.ApplicationStartup (container, pipelines);
		}
	}
}
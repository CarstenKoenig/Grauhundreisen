using Nancy;
using DatabaseLayer;
using GrauhundReisen.EventHandler;

namespace GrauhundReisen.WebPortal
{
	public class Boostrapper : DefaultNancyBootstrapper
	{
		const string ConnectionString = "Content/DataStore/Bookings/";

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;

			container.Register<ViewModels> (new ViewModels(ConnectionString));
			container.Register<Bookings> (new Bookings (ConnectionString));

			base.ApplicationStartup (container, pipelines);
		}
	}
}
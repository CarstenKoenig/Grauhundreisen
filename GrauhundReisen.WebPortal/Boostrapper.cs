using Nancy;
using DatabaseLayer;

namespace GrauhundReisen.WebPortal
{
	public class Boostrapper : DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;
			base.ApplicationStartup (container, pipelines);
		}

		protected override void ConfigureApplicationContainer (Nancy.TinyIoc.TinyIoCContainer container)
		{
			container.Register<DatabaseServer> ().AsSingleton();
			base.ConfigureApplicationContainer (container);
		}
	}
}


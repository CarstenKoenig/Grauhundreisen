using Nancy;
using DatabaseLayer;

namespace GrauhundReisen.WebPortal
{
	public class Boostrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureApplicationContainer (Nancy.TinyIoc.TinyIoCContainer container)
		{
			container.Register<DatabaseServer> ().AsSingleton();
			base.ConfigureApplicationContainer (container);
		}
	}
}


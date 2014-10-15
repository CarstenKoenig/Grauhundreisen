using System;
using GrauhundReisen.Domain.Services;
using GrauhundReisen.ReadModel.EventHandler;
using GrauhundReisen.ReadModel.Repositories;
using Nancy;
using Nancy.TinyIoc;

namespace GrauhundReisen.WebPortal
{
    public class Boostrapper : DefaultNancyBootstrapper
    {
		  // Auskommentieren und anpassen für die eigene Umgebung
		  // const string ConnectionString = @"C:\[Path to your Development Folder]\GrauhundReisen\GrauhundReisen.WebPortal\Content\DataStore\Bookings\";
		  const String ConnectionString = "Content/DataStore/Bookings/";

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;

            SetupIoC(container);

            base.ApplicationStartup(container, pipelines);
        }

        private static void SetupIoC(TinyIoCContainer container)
        {
            var bookingEventHandler = new BookingHandler(ConnectionString);
            var bookingService = new BookingService();

            bookingService.WhenStatusChanged(bookingEventHandler.Handle);

            container.Register(bookingService);
            container.Register<BookingForm>();
            container.Register(new Bookings(ConnectionString));
        }
    }
}

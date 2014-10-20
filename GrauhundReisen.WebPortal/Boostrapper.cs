using System;
using GrauhundReisen.ReadModel.Repositories;
using Nancy;
using Nancy.TinyIoc;

namespace GrauhundReisen.WebPortal
{
    public class Boostrapper : DefaultNancyBootstrapper
    {
		  // Auskommentieren und anpassen für die eigene Umgebung
		  // const string ConnectionString = @"C:\[Path to your Development Folder]\GrauhundReisen\GrauhundReisen.WebPortal\Content\DataStore\Bookings\";
		  const String ConnectionString = @"c:\Temp\Events";

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;

            SetupIoC(container);

            base.ApplicationStartup(container, pipelines);
        }

        private static void SetupIoC(TinyIoCContainer container)
        {
            var repostiory = EventSourcing.Repositories.InMemory.create(false);
            var bookingService = DomainFunktional.Booking.Service.fromRepository(repostiory);

            var readModel = ReadModelFunktional.Booking.createReadModel(ConnectionString);
            DomainFunktional.Booking.Service.registerReadmodel(ReadModelFunktional.Booking.createFileIO(ConnectionString), bookingService);

            container.Register(bookingService);
            container.Register<BookingForm>();
            container.Register(readModel);
        }
    }
}

namespace GrauhundReisen.ReadModelFunktional

open GrauhundReisen.ReadModel

module Booking =

    let createRm id 
                 (name : GrauhundReisen.DomainFunktional.Booking.Name)
                 (GrauhundReisen.DomainFunktional.Booking.Email email) 
                 creditCard 
                 destination =
        let booking = Models.Booking()
        let (ct, cn) = GrauhundReisen.DomainFunktional.Booking.Convert.fromCreditCard creditCard
        booking.CreditCardNumber <- cn
        booking.CreditCardType   <- ct
        booking.Destination <- destination
        booking.EMail       <- email
        booking.FirstName   <- name.Givenname
        booking.LastName    <- name.Surname
        booking.Id          <- id.ToString()
        booking
    
    module Projections =
        open EventSourcing.Projection
        open GrauhundReisen.DomainFunktional.Booking
        open GrauhundReisen.DomainFunktional.Booking.Projections

        let booking = 
            createRm $ bookingId 
            <*> name <*> email 
            <*> creditCard <*> destination

    let empty =
        let rm = GrauhundReisen.ReadModel.Models.Booking()
        rm.CreditCardNumber <- ""
        rm.CreditCardType <- ""
        rm.Destination <- ""
        rm.EMail <- ""
        rm.FirstName <- ""
        rm.Id <- System.Guid.Empty.ToString()
        rm.LastName <- ""
        rm

    let eventHandler (repo : GrauhundReisen.ReadModel.Repositories.Bookings) 
                     (id : EventSourcing.EntityId, event : GrauhundReisen.DomainFunktional.Booking.Events) =
        let rm = repo.GetBookingBy (id.ToString())
        let rm = if rm = null then empty else rm
        let init = ((((GrauhundReisen.DomainFunktional.Booking.BookingId.Parse rm.Id, 
                       { GrauhundReisen.DomainFunktional.Booking.Givenname = rm.FirstName; GrauhundReisen.DomainFunktional.Booking.Surname = rm.LastName }),
                       GrauhundReisen.DomainFunktional.Booking.Email rm.EMail), 
                       GrauhundReisen.DomainFunktional.Booking.Convert.toCreditCard (rm.CreditCardType, rm.CreditCardNumber)),
                       rm.Destination)
        let rm' = EventSourcing.Projection.foldFrom Projections.booking init (Seq.singleton event)
        repo.DeleteBooking (id.ToString())
        repo.SaveBookingAsFile rm'

    let RegisterAt(repo : GrauhundReisen.ReadModel.Repositories.Bookings, service : GrauhundReisen.DomainFunktional.Booking.Service.T) =
        service |> GrauhundReisen.DomainFunktional.Booking.Service.registerEventHandler (eventHandler repo)
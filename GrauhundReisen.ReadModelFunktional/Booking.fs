namespace GrauhundReisen.ReadModelFunktional

module Booking =
  
    // Readmodel - für Janeks/C# "angepasst" (alle Felder Strings)
    type Id = string
    type Events = GrauhundReisen.DomainFunktional.Booking.Events
    type T = 
        { CreditCardNumber : string
          CreditCardType   : string
          Destination      : string
          EMail            : string
          FirstName        : string
          LastName         : string
          Id               : string
        }

    let empty = 
        { CreditCardNumber = ""
          CreditCardType   = ""
          Destination      = ""
          EMail            = ""
          FirstName        = ""
          LastName         = ""
          Id               = string GrauhundReisen.DomainFunktional.Booking.BookingId.Empty
        }

    let create (id, first, last, email, credType, credNr, dest) = 
        { CreditCardNumber = credNr
          CreditCardType   = credType
          Destination      = dest
          EMail            = email
          FirstName        = first
          LastName         = last
          Id               = id
        }

    let none : T option = None

    let some t : T option = Some t
    

    type IRepository =
        abstract GetBy  : Id -> T option
        abstract Delete : Id -> unit
        abstract Save   : T  -> unit
        

    let fromDomain 
                (id : GrauhundReisen.DomainFunktional.Booking.BookingId)
                (name : GrauhundReisen.DomainFunktional.Booking.Name)
                (GrauhundReisen.DomainFunktional.Booking.Email email) 
                (creditCard : GrauhundReisen.DomainFunktional.Booking.CreditCard)
                (destination : GrauhundReisen.DomainFunktional.Booking.Destination) =
        let (ct, cn) = GrauhundReisen.DomainFunktional.Booking.Convert.fromCreditCard creditCard
        create (id.ToString(), name.Givenname, name.Surname, email, ct, cn, destination)
    
    module Projections =
        open EventSourcing.Projection
        open GrauhundReisen.DomainFunktional.Booking
        open GrauhundReisen.DomainFunktional.Booking.Projections

        let booking = 
            fromDomain $ bookingId 
            <*> name <*> email 
            <*> creditCard <*> destination

    let eventHandler (repo : IRepository) 
                     (id : GrauhundReisen.DomainFunktional.Booking.BookingId, event : Events) =
        let readModelFrom = 
            match repo.GetBy (id.ToString()) with
            | Some rm -> rm
            | None    -> empty
        // das gefällt mir so noch nicht - das ist die Schwäche des "Zwischenschritts/Typs"
        let init = ((((GrauhundReisen.DomainFunktional.Booking.BookingId.Parse readModelFrom.Id,
                       { GrauhundReisen.DomainFunktional.Booking.Givenname = readModelFrom.FirstName; GrauhundReisen.DomainFunktional.Booking.Surname = readModelFrom.LastName }),
                       GrauhundReisen.DomainFunktional.Booking.Email readModelFrom.EMail), 
                       GrauhundReisen.DomainFunktional.Booking.Convert.toCreditCard (readModelFrom.CreditCardType, readModelFrom.CreditCardNumber)),
                       readModelFrom.Destination)
        let readModelTo = EventSourcing.Projection.foldFrom Projections.booking init (Seq.singleton event)
        repo.Delete (id.ToString())
        repo.Save readModelTo

    let RegisterAt(repo : IRepository, service : GrauhundReisen.DomainFunktional.Booking.Service.T) =
        service |> GrauhundReisen.DomainFunktional.Booking.Service.registerEventHandler (eventHandler repo)
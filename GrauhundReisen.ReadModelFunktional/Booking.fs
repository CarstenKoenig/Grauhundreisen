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
        
    module Projections =
        open EventSourcing.Projection
        open GrauhundReisen.DomainFunktional.Booking
        open GrauhundReisen.DomainFunktional.Booking.Projections

        let booking = 
            EventSourcing.Projection.create
                empty
                (fun b ev ->
                    match ev with
                    | Events.Ordered (GrauhundReisen.DomainFunktional.Booking.Booking (id, order)) -> 
                        let (ct,cn) = GrauhundReisen.DomainFunktional.Booking.Convert.fromCreditCard order.CreditCard
                        let (GrauhundReisen.DomainFunktional.Booking.Email email) = order.Email
                        { b with Id               = string id
                                 CreditCardType   = ct
                                 CreditCardNumber = cn
                                 Destination      = order.Destination
                                 EMail            = email
                                 FirstName        = order.Name.Givenname
                                 LastName         = order.Name.Surname
                        }
                    | Events.EmailChanged (GrauhundReisen.DomainFunktional.Booking.Email email) ->
                        { b with EMail = email }
                    | Events.CreditCardChanged creditCard ->
                        let (ct,cn) = GrauhundReisen.DomainFunktional.Booking.Convert.fromCreditCard creditCard
                        { b with CreditCardType   = ct
                                 CreditCardNumber = cn
                        })

    let eventHandler (repo : IRepository) 
                     (id : GrauhundReisen.DomainFunktional.Booking.BookingId, event : Events) =
        let readModelFrom = 
            match repo.GetBy (id.ToString()) with
            | Some rm -> rm
            | None    -> empty
        // das gefällt mir so noch nicht - das ist die Schwäche des "Zwischenschritts/Typs"
        let readModelTo = EventSourcing.Projection.foldFrom Projections.booking readModelFrom (Seq.singleton event)
        repo.Delete (id.ToString())
        repo.Save readModelTo

    let RegisterAt(repo : IRepository, service : GrauhundReisen.DomainFunktional.Booking.Service.T) =
        service |> GrauhundReisen.DomainFunktional.Booking.Service.registerEventHandler (eventHandler repo)
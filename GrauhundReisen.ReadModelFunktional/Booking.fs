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

    let internal create (id, first, last, email, credType, credNr, dest) = 
        { CreditCardNumber = credNr
          CreditCardType   = credType
          Destination      = dest
          EMail            = email
          FirstName        = first
          LastName         = last
          Id               = id
        }

    type ReadModel internal (load) =
        member __.Load(key) = load key

    let createFileIO path =
        let proj =
            GrauhundReisen.DomainFunktional.Booking.Projections.booking
            |> EventSourcing.Projection.map 
                (fun (GrauhundReisen.DomainFunktional.Booking.Booking (id,order)) -> 
                    let (GrauhundReisen.DomainFunktional.Booking.Email email) = order.Email
                    let (ct, cn) = GrauhundReisen.DomainFunktional.Booking.Convert.fromCreditCard order.CreditCard
                    create (string id, order.Name.Givenname, order.Name.Surname, email, ct, cn, order.Destination))
        EventSourcing.ReadModel.create
            (FileKeyValueStore.createFileStore path)
            proj
            (fun id _ -> id)

    let createReadModel path =
        let rm = createFileIO path
        ReadModel (fun key -> 
            EventSourcing.ReadModel.load rm (EventSourcing.EntityId.Parse key))
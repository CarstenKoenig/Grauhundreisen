namespace GrauhundReisen.DomainFunktional

open EventSourcing

module Booking =

    type BookingId = EntityId

    type Name = 
        { Givenname : string
          Surname   : string
        }

    type Email = Email of string // sollte später mit smart Const. versehen werden

    type CreditCard =
        | MasterCard      of string
        | Visa            of string
        | AmericanExpress of string

    type Destination = string

    type T = 
        { BookingId   : BookingId
          Name        : Name
          Email       : Email
          CreditCard  : CreditCard
          Destination : Destination
        }

    type Events =
        | Ordered           of T
        | EmailChanged      of Email
        | CreditCardChanged of CreditCard

    module ReadModel =
        // normalerweise gehört das ins Readmodel,
        // ich möchte aber erstmal nur eine F# Lib zum Demonstrieren

        type Booking = GrauhundReisen.ReadModel.Models.Booking

        let empty = GrauhundReisen.ReadModel.Models.Booking()

        let internal fromCreditCard (c : CreditCard) =
            match c with
            | AmericanExpress n -> ("American Express", n)
            | Visa n            -> ("Visa", n)
            | MasterCard n      -> ("MasterCard", n)

        let internal toCreditCard (t : string, n : string) =
                match t with
                | "American Express" -> AmericanExpress n
                | "Visa"             -> Visa n
                | "Master Card"      -> MasterCard n
                | _                  -> failwith ("Unbekannter Kreditkartentyp: " + t)
            

        let fromModel (model : T) =
            let (ct, cn) = fromCreditCard model.CreditCard
            let rm = Booking()
            rm.Id <- model.BookingId.ToString()
            rm.FirstName <- model.Name.Givenname
            rm.LastName <- model.Name.Surname
            rm.CreditCardNumber <- cn
            rm.CreditCardType <- ct
            rm.Destination <- model.Destination
            rm

        let toModel (rm : Booking) =
            { BookingId   = EntityId.Parse rm.Id
              Name        = { Givenname = rm.FirstName; Surname = rm.LastName }
              Email       = Email rm.EMail
              CreditCard  = toCreditCard (rm.CreditCardType, rm.CreditCardNumber)
              Destination = rm.Destination
            }

        let copy (rm : GrauhundReisen.ReadModel.Models.Booking) = 
            let rm' = Booking()
            rm'.Id <- rm.Id
            rm'.FirstName <- rm.FirstName
            rm'.LastName <- rm.LastName
            rm'.CreditCardNumber <- rm.CreditCardNumber
            rm'.CreditCardType <- rm.CreditCardType
            rm'.Destination <- rm.Destination
            rm'
            

    module Projections =
        let creditCard = Projection.latest (function 
                            | CreditCardChanged c -> Some c
                            | Ordered t           -> Some t.CreditCard
                            | _                   -> None)

        let bookingReadmodel =
            Projection.create
                ReadModel.empty
                (fun rm event ->
                    match event with
                    | Ordered m  -> 
                        ReadModel.fromModel m
                    | EmailChanged (Email e) ->  
                        let rm' = ReadModel.copy rm
                        rm'.EMail <- e
                        rm'
                    | CreditCardChanged c -> 
                        let (ct, cn) = ReadModel.fromCreditCard c
                        let rm' = ReadModel.copy rm
                        rm'.CreditCardNumber <- cn
                        rm'.CreditCardType <- ct
                        rm')


        let booking =
            Projection.map ReadModel.toModel bookingReadmodel

    module Service = 

        type T = private Service of IEventStore

        let fromRepository (rep : IEventRepository) =
            EventStore.fromRepository rep
            |> Service

        let order (bookingId, destination, creditCard, email, name) (Service service) =
            let event = 
                Ordered 
                    { BookingId   = bookingId
                      Name        = name
                      Email       = email
                      CreditCard  = creditCard 
                      Destination = destination
                    }
            service |> EventStore.add bookingId event

        let update (bookingId, email, creditCardNr) (Service service) =
            Computation.Do {
                let! creditCard = Computation.restore Projections.creditCard bookingId
                let creditCard = 
                    match creditCard with 
                    | MasterCard c      -> MasterCard creditCardNr
                    | Visa c            -> Visa creditCardNr
                    | AmericanExpress c -> AmericanExpress creditCardNr
                do! Computation.add bookingId <| CreditCardChanged creditCard
                do! Computation.add bookingId <| EmailChanged email
            } |> EventStore.execute service

        let private asTask aktion =
            System.Threading.Tasks.Task.Factory.StartNew(fun () -> aktion())

        let OrderBooking (service : T,
                          bookingId : string, destination : string,
                          creditCardNumber : string, creditCardType : string,
                          email : string, firstName : string, lastName : string) =
            // nicht sehr glücklich darüber, aber ich möchte Janeks Access so weit
            // wie möglich erhalten
            let creditCard = ReadModel.toCreditCard (creditCardType, creditCardNumber)
            let name = { Givenname = firstName; Surname = lastName }
            let bookingId' = EntityId.Parse bookingId
            (fun () ->
                service
                |> order (bookingId', destination, 
                          creditCard,
                          Email email, name)
            ) |> asTask :> System.Threading.Tasks.Task

        let UpdateBookingDetail (service : T,
                                 bookingId : string,
                                 email : string,
                                 creditCardNumber : string) =
            // nicht sehr glücklich darüber, aber ich möchte Janeks Access so weit
            // wie möglich erhalten
            let bookingId' = EntityId.Parse bookingId
            (fun () ->
                service
                |> update (bookingId', Email email, creditCardNumber)
            ) |> asTask :> System.Threading.Tasks.Task
        
        let readModelEventHandler (rmRepo : GrauhundReisen.ReadModel.Repositories.Bookings)
                         (bookingId : EntityId, event) =
            let bookingId' = string bookingId
            let rm = rmRepo.GetBookingBy bookingId'
            let rm = if rm = null then ReadModel.empty else rm
            let rm' = Projection.foldFrom Projections.bookingReadmodel rm (Seq.singleton event)
            rmRepo.DeleteBooking bookingId'
            rmRepo.SaveBookingAsFile rm'

        let SetupReadmodelHandler(rmRepo, Service service) =
            service.subscribe (readModelEventHandler rmRepo)


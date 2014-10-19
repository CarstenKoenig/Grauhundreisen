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
        | NoCreditCardSelected

    type Destination = string

    type Order = 
        { Name        : Name
          Email       : Email
          CreditCard  : CreditCard
          Destination : Destination
        }

    let createOrder name email creditCard destination =
        { Name        = name
          Email       = email
          CreditCard  = creditCard
          Destination = destination
        }

    type T = Booking of BookingId * Order

    let create id name email creditCard destination =
        Booking (id, createOrder name email creditCard destination)


    type Events =
        | Ordered           of T
        | EmailChanged      of Email
        | CreditCardChanged of CreditCard

    [<AutoOpen>]
    module Convert = 

        let fromCreditCard (c : CreditCard) =
            match c with
            | AmericanExpress n -> ("American Express", n)
            | Visa n            -> ("Visa", n)
            | MasterCard n      -> ("MasterCard", n)
            | NoCreditCardSelected -> ("", "")

        let toCreditCard (t : string, n : string) =
                match t with
                | "American Express" -> AmericanExpress n
                | "Visa"             -> Visa n
                | "Master Card"      -> MasterCard n
                | ""                 -> NoCreditCardSelected
                | _                  -> failwith (sprintf "unbekannter Kreditkartentyp [%s]" t)

    module Projections =
        let bookingId  = Projection.latest (function
                            | Ordered (Booking (id,_)) -> Some id
                            | _                        -> None)

        let name = Projection.latest (function
                            | Ordered (Booking (_,t)) -> Some t.Name
                            | _                       -> None)

        let destination = Projection.latest (function
                            | Ordered (Booking (_,t)) -> Some t.Destination
                            | _                       -> None)

        let email = Projection.latest (function
                            | Ordered (Booking (_,t)) -> Some t.Email
                            | EmailChanged  e         -> Some e
                            | _                       -> None)

        let creditCard = Projection.latest (function 
                            | Ordered (Booking (_,t)) -> Some t.CreditCard
                            | CreditCardChanged c     -> Some c
                            | _                       -> None)

        let booking =
            create $ bookingId 
            <*> name <*> email 
            <*> creditCard <*> destination

        let allEvents : Projection.T<Events, _, Events seq> =
            Projection.events ()
            |> Projection.map List.toSeq

    module Service = 

        type T = private Service of IEventStore

        let fromRepository (rep : IEventRepository) =
            EventStore.fromRepository rep
            |> Service

        let registerEventHandler handler (Service service) =
            service |> EventStore.subscribe handler

        let order (bookingId, destination, creditCard, email, name) (Service service) =
            let event = 
                create bookingId name email creditCard destination
                |> Ordered
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
            let creditCard = toCreditCard (creditCardType, creditCardNumber)
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

        let GetEventAsString (Service store, id) =
            let serialize ev = Newtonsoft.Json.JsonConvert.SerializeObject ev
            store
            |> EventStore.restore Projections.allEvents id
            |> Seq.map serialize
            |> Seq.toArray

        /// kann ich momentan noch nicht implementieren, da ich die Abfrage aller
        /// Keys aus dem EventStore/Repository im Moment nicht unterstüzte
        /// Ich denke darüber nach
        let GetAllEventsAsString (Service store as service) =
            let ids = EventStore.execute store Computation.allIds
            ids |> Seq.collect (fun id -> GetEventAsString (service, id))

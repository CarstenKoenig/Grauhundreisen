namespace GrauhundReisen.DomainFunktional

open EventSourcing

module Booking =

    // mein EventStore läuft momentan nur über Guids
    // das ist ein Alias dafür:
    type BookingId = EntityId

    // sammle die Typen für eine "Order"

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

    // das ist sozusagen ein Konstruktor (mach mir das leben leichter, wenn ich Records initialisiere)
    let createOrder name email creditCard destination =
        { Name        = name
          Email       = email
          CreditCard  = creditCard
          Destination = destination
        }

    // der "Booking"-Datentyp, da ich das Booking.Booking so hässlich finde benutze ich hier eine
    // ML Konvention, wo der *Hauptyp* eines Modul häufig mit T bezeichnet wird
    // damit kann ich wenigstens Booking.T schreiben - kann man ändern, wenn das nicht gefällt
    type T = Booking of BookingId * Order

    let create id name email creditCard destination =
        Booking (id, createOrder name email creditCard destination)

    // Die Events, die zur Domände "Booking" gehörtn
    // anders als bei Dir gefällt mir der Gedanke diese bei in der Domäne zu haben
    // können wir gerne Diskutieren (PS: das heißt hier, dass ich für verschiedene
    // Domänen verschiedene Event-Typen habe, mein sollte das können)
    type Events =
        | Ordered           of T
        | EmailChanged      of Email
        | CreditCardChanged of CreditCard

    // einige Hilfsfunktionen die ich immer wieder brauche,
    // weil ich mich entschieden hatte die Kreditkarten
    // explizit zu modelieren und nicht über zwei Strings
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

    // einige Projektionen um aus Events - Eigenschaften
    // und schließlich die ganze Buchung zu machen
    // im Readmodel kann man sich einen direkten Weg ansehen
    // das hier ist etwas der Gag an meinem Store, deshalb steht
    // das so hier, auch wenn es in diesem Beispiel nicht direkt
    // gebraucht wird
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

        // hier werden mit Kombinatoren die Buchung aus ihren
        // Einzelteilen gebaut
        let booking =
            create $ bookingId 
            <*> name <*> email 
            <*> creditCard <*> destination

        let allEvents : Projection.T<Events, _, Events seq> =
            Projection.events ()
            |> Projection.map List.toSeq

    // das ist die Service-Schnittstelle nach außen
    // ich habe die hier extra nicht als Klasse gemacht
    // als Diskussionsgrundlage (ein Modul ist sowas wie eine statische Klasse)
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

        // die groß geschriebenen Methoden wird C# nutzen
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

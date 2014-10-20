namespace GrauhundReisen.ReadModelFunktional

module Repositories =

    type CreditCardType = { Name : string }
    type Destionation = { Name : string }

    [<AutoOpen>]
    module internal Enumerations =

        let creditCardTypes =
            let ct n : CreditCardType = { Name = n }
            [ ct "Master Card"; ct "Visa"; ct "Americal Express" ]
            |> Seq.ofList

        let destinations =
            let d n : Destionation = { Name = n }
            [ d "Berlin"; d "Hamburg"; d "München"; d "Köln"
              d "Leipzig"; d "Dresden"; d "Rostock"
            ] |> List.sortBy (fun x -> x.Name) |> Seq.ofList

    type CreditCardTypes() = 
        member __.GetAll() = creditCardTypes

    type Destinations() =
        member __.GetAll() = destinations
           

    type BookingForm() =
        member __.CreditCardTypes = creditCardTypes
        member __.Destinations    = destinations
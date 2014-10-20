namespace GrauhundReisen.ReadModelFunktional

open EventSourcing
open System.IO
open Newtonsoft.Json

module FileKeyValueStore =

    let createFileStore<'key,'value> (path : string) : IKeyValueStore<'key,'value> =
        let readFromFile key = 
            try
                let path = Path.Combine (path, key.ToString())
                if not <| File.Exists path then None else
                let content = File.ReadAllText path
                JsonConvert.DeserializeObject<'value> content
                |> Some
            with
            | _ -> None
        let writeToFile (key, value) =
            let path = Path.Combine(path, key.ToString())
            let json = JsonConvert.SerializeObject value
            if File.Exists path then File.Delete path else
            File.WriteAllText(path, json)
        { new IKeyValueStore<'key,'value> with
            member __.Read key = readFromFile key 
            member __.Save key value = writeToFile (key, value) 
        }
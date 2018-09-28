module MiniBlazor.Test.Client.Main

open MiniBlazor
open MiniBlazor.Html

type Item =
    {
        K: int
        V: string
    }

type Model =
    { 
        input: string
        submitted: option<string>
        addKey: int
        revOrder: bool
        items: Map<int, string>
    }

type Message =
    | SetInput of text: string
    | Submit
    | RemoveItem of key: int
    | SetAddKey of key: int
    | SetKeyOf of key: int
    | AddKey
    | ToggleRevOrder

let InitModel =
    {
        input = ""
        submitted = None
        addKey = 4
        revOrder = false
        items = Map [
            0, "it's 0"
            1, "it's 1"
            2, "it's 2"
            3, "it's 3"
        ]
    }

let Update message model =
    match message with
    | SetInput text -> { model with input = text }
    | Submit -> { model with submitted = Some model.input }
    | RemoveItem k -> { model with items = Map.filter (fun k' _ -> k' <> k) model.items }
    | SetAddKey i -> { model with addKey = i }
    | AddKey -> { model with items = Map.add model.addKey (sprintf "it's %i" model.addKey) model.items }
    | SetKeyOf k ->
        match Map.tryFind k model.items with
        | None -> model
        | Some item ->
            let items = model.items |> Map.remove k |> Map.add model.addKey item
            { model with items = items }
    | ToggleRevOrder -> { model with revOrder = not model.revOrder }

let ViewInput model =
    p [] [
        input [value model.input; on.input SetInput]
        input [type_ "submit"; on.click Submit]
        div [] [text (defaultArg model.submitted "")]
        (match model.submitted with
        | Some s ->
            concat [
                if s.Contains "secret" then
                    yield div [] [text "You typed the secret password!"]
                if s.Contains "super" then
                    yield div [] [text "You typed the super secret password!"]
            ]
        | None -> empty)
    ]

let ViewItem k v =
    concat [
        li [] [text v]
        li [] [
            input []
            button [on.click (SetKeyOf k)] [text "Set key from Add field"]
            button [on.click (RemoveItem k)] [text "Remove"]
        ]
    ]

let ViewList model =
    let items =
        if model.revOrder then
            Seq.rev model.items
        else
            model.items :> _
    p [] [
        input [value (string model.addKey); on.input (int >> SetAddKey)]
        button [on.click AddKey] [text "Add"]
        br []
        button [on.click ToggleRevOrder] [text "Toggle order"]
        ul [] [
            keyed [for KeyValue(k, v) in items -> string k, ViewItem k v]
        ]
    ]

let View model =
    concat [
        ViewInput model
        ViewList model
    ]

[<EntryPoint>]
let Main args =
    App.Create InitModel Update View
    |> App.Run "#main"
    0

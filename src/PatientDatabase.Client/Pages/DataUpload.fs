module PatientDatabase.Client.Pages.DataUpload

open Browser.Types
open Fable.Remoting.Client
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open PatientDatabase.Client.Server
open PatientDatabase.Shared.API
open System

type private State = { InputFile: File option }

type private Msg =
    | FileSaved of unit
    | FileChosen of File
    | FileUploaded

let private init () =
    {
        InputFile = None
    },
    Cmd.none

let upload (file: File) = async {
    let! fileBytes = file.ReadAsByteArray()
    let! output = service.UploadData fileBytes
    return output
}

let private update (msg: Msg) (model: State) : State * Cmd<Msg> =
    match msg with
    | FileSaved _ -> model, Cmd.none
    | FileChosen file -> { model with InputFile = Some file }, Cmd.none
    | FileUploaded ->
         match model.InputFile with
         | None -> model, Cmd.none
         | Some file -> model, Cmd.OfAsync.perform upload file FileSaved

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish (init, update, [||])


    Html.div [
        prop.className "flex flex-col items-center"
        prop.children [
            Daisy.card [
                prop.className "p-6 bg-base-200 shadow-xl m-2"
                prop.children [
                    Html.form [
                        prop.onSubmit (fun e ->
                            e.preventDefault ()
                            FileUploaded |> dispatch)
                        prop.className "flex flex-col items-center"
                        prop.children [
                            Daisy.formControl [
                                Daisy.label [Daisy.labelText "Upload data from file"]
                                Daisy.file [
                                    file.bordered
                                    prop.onChange (fun value -> value |> FileChosen |> dispatch)
                                ]
                            ]
                            Daisy.button.button [
                                prop.className "btn-block m-2"
                                button.outline
                                prop.text "Upload Data"
                                prop.type'.submit
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
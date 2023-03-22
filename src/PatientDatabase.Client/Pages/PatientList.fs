module PatientDatabase.Client.Pages.PatientList

open System

open Browser.Types
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open PatientDatabase.Client.Pages.Index
open PatientDatabase.Client.Server
open PatientDatabase.Shared.API
open Fable.Remoting.Client

type private State = { Patients: PatientListItem list; SearchPhrase: string; SearchField: string; FileName: string }

type private Msg =
    | ShowList
    | SearchFieldChanged of String
    | SearchPhraseChanged of string
    | SearchPhraseSubmitted
    | ListShown of PatientListItem list
    | ExportList
    | ListDownloaded of unit

let private init () =
    { Patients = List.empty
      SearchPhrase = ""
      SearchField = "Name"
      FileName = "export.csv" }, Cmd.ofMsg ShowList

let download (fileName: string, searchPhrase: string, searchField: string) = async {
    let! downloadedFile = service.DownloadData (searchPhrase, searchField)
    downloadedFile.SaveFileAs(fileName)
}

let private update (msg: Msg) (model: State) : State * Cmd<Msg> =
    match msg with
    | ShowList -> model, Cmd.OfAsync.perform service.ShowList (model.SearchPhrase, model.SearchField) ListShown
    | SearchFieldChanged field -> { model with SearchField = field }, Cmd.none
    | SearchPhraseChanged phrase -> { model with SearchPhrase = phrase }, Cmd.none
    | SearchPhraseSubmitted -> model, Cmd.ofMsg ShowList
    | ListShown items -> { model with Patients = items }, Cmd.none
    | ExportList -> model, Cmd.OfAsync.perform download (model.FileName, model.SearchPhrase, model.SearchField) ListDownloaded
    | ListDownloaded _ -> model, Cmd.none

let PatientRow patient =
    Html.tr [
        table.hover
        prop.children [
            Html.td patient.Name
            Html.td patient.Age
            Html.td patient.Sex
            Html.td $"{patient.Symptom1} {patient.Symptom2} {patient.Symptom3}"
            Html.td patient.Tests
            Html.td $"{patient.Diagnosis1} {patient.Diagnosis2} {patient.Diagnosis3}"
            Html.td patient.Treatment
            Html.td (patient.Date.ToLongDateString())
            Html.td patient.Place
            Html.td [
                Daisy.button.button [
                    button.sm
                    button.outline
                    button.primary
                    prop.text "Edit"
                ]
            ]
        ]
    ]

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish (init, update, [||])
    let rows = state.Patients |> List.map PatientRow

    Html.div [
        Html.div [
            prop.className "flex flex-col items-center"
            prop.children [
                Html.div [
                    prop.className "text-xl"
                    prop.children [
                        Daisy.label [ prop.text "Patient Information summary" ]
                    ]
                ]
                Html.div [
                    prop.className " flex flex-row"
                    prop.children [
                        Html.form [
                            prop.onSubmit (fun e ->
                                e.preventDefault ()
                                SearchPhraseSubmitted |> dispatch)
                            prop.className "bg-6 m-1"
                            prop.children [
                                Daisy.card [
                                    prop.className "flex-row p-2 shadow-lg bg-base-200 text-neutral-content"
                                    prop.children [
                                        Html.div [
                                            Daisy.label [
                                                prop.className "m-1 mt-2"
                                                color.textPrimary
                                                prop.text "Find patient by"
                                            ]
                                        ]
                                        Daisy.select [
                                            select.bordered
                                            color.textPrimary
                                            prop.onChange (fun value ->
                                                value |> SearchFieldChanged |> dispatch)
                                            prop.value state.SearchField
                                            prop.className "max-w-xs m-1"
                                            prop.children [
                                                Html.option "Name"
                                                Html.option "Age"
                                                Html.option "Sex"
                                                Html.option "Symptoms"
                                                Html.option "Tests"
                                                Html.option "Results"
                                                Html.option "Diagnosis"
                                                Html.option "Treatment"
                                                Html.option "Date"
                                                Html.option "Place"
                                            ]
                                        ]
                                        Html.div [
                                            prop.className "flex-none m-1"
                                            prop.children [
                                                Daisy.formControl [
                                                    Daisy.input [
                                                        input.bordered
                                                        color.textPrimary
                                                        prop.onChange (fun value ->
                                                            value |> SearchPhraseChanged |> dispatch)
                                                        prop.value state.SearchPhrase
                                                    ]
                                                ]
                                            ]
                                        ]
                                        Html.div [
                                            prop.className "flex-none m-1"
                                            prop.children [
                                                Daisy.button.button [
                                                    button.outline
                                                    prop.text "Search"
                                                    prop.type'.submit
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.form [
                            prop.onSubmit (fun e ->
                                e.preventDefault ()
                                ExportList |> dispatch)
                            prop.className "bg-6 m-1"
                            prop.children [
                                Daisy.card [
                                    prop.className "bg-base-200 p-2 shadow-lg"
                                    prop.children [
                                        Daisy.button.button [
                                            prop.className "m-1"
                                            prop.text "Export current view"
                                            prop.type'.submit
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]

                Daisy.table [
                    // table.compact
                    prop.className "max-w-xs m-2 shadow-xl"
                    prop.children [
                        Html.thead [
                            Html.tr [
                                Html.th "Name"
                                Html.th "Age"
                                Html.th "Sex"
                                Html.th "Symptoms"
                                Html.th "Tests"
                                Html.th "Diagnosis"
                                Html.th "Treatment"
                                Html.th "Date"
                                Html.th "Place"
                                Html.th ""
                            ]
                        ]
                        Html.tbody rows
                    ]
                ]
            ]
        ]
    ]

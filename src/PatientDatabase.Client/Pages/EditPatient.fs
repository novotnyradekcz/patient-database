module PatientDatabase.Client.Pages.EditPatient

open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open PatientDatabase.Client.Server
open PatientDatabase.Shared.API
open System

type private State = { Message: string; Form: PatientForm; PatientId: Guid; }

type private Msg =
    | FetchForm of Guid
    | ShowForm of PatientForm
    | FormChanged of PatientForm
    | FormEdited
    | FormSaved of unit
    | DeleteEntry
    | EntryDeleted of int

let private init patientId =
    {
        Message = "Ready to edit"
        PatientId = patientId
        Form = {
            Place = ""
            Date = DateTime.Today.ToShortDateString()
            Name = ""
            Age = "0"
            Sex = "Unknown"
            Symptom1 = ""
            Symptom2 = ""
            Symptom3 = ""
            Tests = ""
            Test1 = ""
            Test2 = ""
            Test3 = ""
            Result1 = ""
            Result2 = ""
            Result3 = ""
            Diagnosis1 = ""
            Diagnosis2 = ""
            Diagnosis3 = ""
            Treatment = ""
        }
    },
    Cmd.OfAsync.perform service.FetchForm patientId ShowForm

let private update (msg: Msg) (model: State) : State * Cmd<Msg> =
    match msg with
    | FetchForm id -> model, Cmd.OfAsync.perform service.FetchForm id ShowForm
    | ShowForm data -> { model with Form = data }, Cmd.none
    | FormChanged patientForm -> { model with Form = patientForm }, Cmd.none
    | FormEdited -> model, Cmd.OfAsync.perform service.EditForm (model.Form, model.PatientId) FormSaved
    | FormSaved _ -> { model with Message = "Patient entry successfully edited" }, Cmd.none
    | DeleteEntry -> model, Cmd.OfAsync.perform service.DeleteEntry model.PatientId EntryDeleted
    | EntryDeleted _ -> { model with Message = "Patient entry deleted" }, Cmd.none

[<ReactComponent>]
let IndexView patientId =
    let state, dispatch = React.useElmish (init patientId, update, [||])


    Html.div [
        Html.form [
            prop.onSubmit (fun e ->
                e.preventDefault ()
                FormEdited |> dispatch)
            prop.children [
                Html.div [
                    prop.className "flex flex-col items-center"
                    prop.children [
                        Daisy.card [
                            prop.className "p-6 bg-base-200 max-w-xs shadow-xl m-2 text-xl"
                            prop.children [
                                Daisy.label [ prop.text "Editing patient information" ]
                            ]
                        ]
                        Html.div [
                            match state.Message with
                                | "Patient entry deleted" ->
                                    Daisy.alert [
                                        prop.className "m-2 w-auto"
                                        alert.success
                                        prop.text state.Message
                                    ]
                                | _ ->
                                    Daisy.button.label [
                                        prop.htmlFor "delete"
                                        button.primary
                                        prop.text "Delete patient entry"
                                    ]
                                    Daisy.modalToggle [prop.id "delete"]
                                    Daisy.modal [
                                        prop.className "m-2"
                                        prop.children [
                                            Daisy.modalBox [
                                                Html.p $"Are you sure you want to delete patient {state.Form.Name}, {state.Form.Age}?"
                                                Daisy.modalAction [
                                                    Daisy.button.label [
                                                        prop.onClick (fun e ->
                                                            e.preventDefault ()
                                                            DeleteEntry |> dispatch)
                                                        button.warning
                                                        prop.text "Yes"
                                                    ]
                                                    Daisy.button.label [
                                                        prop.htmlFor "delete"
                                                        button.info
                                                        prop.text "No"
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                        ]
                        Html.div [
                            prop.className "text-l"
                            prop.children [
                                Daisy.label [ prop.text "Outreach information" ]
                            ]
                        ]
                        Daisy.card [
                            prop.className "p-6 bg-base-200 shadow-xl m-2"
                            prop.children [
                                Daisy.formControl [
                                    prop.className "flex flex-row items-center"
                                    prop.children [
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Place" ]
                                            Daisy.input [
                                                input.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.className "w-40"
                                                prop.onChange (fun value ->
                                                    { state.Form with Place = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Place
                                            ]
                                        ]
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Date" ]
                                            Daisy.input [
                                                input.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.className "w-32"
                                                prop.onChange (fun value ->
                                                    { state.Form with Date = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Date
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "text-l"
                            prop.children [
                                Daisy.label [ prop.text "Patient information" ]
                            ]
                        ]
                        Daisy.card [
                            prop.className "p-6 bg-base-200 shadow-xl m-2"
                            prop.children [
                                Daisy.formControl [
                                    prop.className "flex flex-row items-center"
                                    prop.children [
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Patient Name" ]
                                            Daisy.input [
                                                input.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.className "w-56"
                                                prop.onChange (fun value ->
                                                    { state.Form with Name = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Name
                                            ]
                                        ]
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Patient Age" ]
                                            Daisy.input [
                                                input.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.className "w-24"
                                                prop.onChange (fun value ->
                                                    { state.Form with Age = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Age
                                            ]
                                        ]
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Patient Sex" ]
                                            Daisy.select [
                                                select.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.onChange (fun value ->
                                                    { state.Form with Sex = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Sex
                                                prop.className "w-full max-w-xs"
                                                prop.children [
                                                    Html.option "Unknown"
                                                    Html.option "Female"
                                                    Html.option "Male"
                                                    Html.option "Other"
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Daisy.card [
                            prop.className "p-6 bg-base-200 max-w-xl shadow-xl m-2"
                            prop.children [
                                Daisy.formControl [
                                    prop.className "flex-row"
                                    prop.children [
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Symptoms" ]
                                            Html.div [
                                                prop.className "flex flex-row w-full"
                                                prop.children [
                                                    Daisy.select [
                                                        select.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.onChange (fun value ->
                                                            { state.Form with Symptom1 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Symptom1
                                                        prop.className "w-1/2"
                                                        prop.children [
                                                            Html.option ""
                                                            Html.option "fever"
                                                            Html.option "cough"
                                                            Html.option "pain"
                                                        ]
                                                    ]
                                                    Daisy.input [
                                                        input.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.className "w-1/2"
                                                        prop.onChange (fun value ->
                                                            { state.Form with Symptom1 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Symptom1
                                                    ]
                                                ]
                                            ]
                                            Html.div [
                                                prop.className "flex flex-row w-full"
                                                prop.children [
                                                    Daisy.select [
                                                        select.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.onChange (fun value ->
                                                            { state.Form with Symptom2 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Symptom2
                                                        prop.className "w-1/2"
                                                        prop.children [
                                                            Html.option ""
                                                            Html.option "fever"
                                                            Html.option "cough"
                                                            Html.option "pain"
                                                        ]
                                                    ]
                                                    Daisy.input [
                                                        input.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.className "w-1/2"
                                                        prop.onChange (fun value ->
                                                            { state.Form with Symptom2 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Symptom2
                                                    ]
                                                ]
                                            ]
                                            Html.div [
                                                prop.className "flex flex-row w-full"
                                                prop.children [
                                                    Daisy.select [
                                                        select.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.onChange (fun value ->
                                                            { state.Form with Symptom3 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Symptom3
                                                        prop.className "w-1/2"
                                                        prop.children [
                                                            Html.option ""
                                                            Html.option "fever"
                                                            Html.option "cough"
                                                            Html.option "pain"
                                                        ]
                                                    ]
                                                    Daisy.input [
                                                        input.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.className "w-1/2"
                                                        prop.onChange (fun value ->
                                                            { state.Form with Symptom3 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Symptom3
                                                    ]
                                                ]
                                            ]
                                            Daisy.label [ Daisy.labelText "Tests" ]
                                            Daisy.input [
                                                input.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.className "w-full"
                                                prop.onChange (fun value ->
                                                    { state.Form with Tests = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Tests
                                            ]
                                        ]
                                        Html.div [
                                            Daisy.label [ Daisy.labelText "Diagnoses" ]
                                            Html.div [
                                                prop.className "flex flex-row w-full"
                                                prop.children [
                                                    Daisy.select [
                                                        select.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.onChange (fun value ->
                                                            { state.Form with Diagnosis1 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Diagnosis1
                                                        prop.className "w-1/2"
                                                        prop.children [
                                                            Html.option ""
                                                            Html.option "malaria"
                                                            Html.option "cold"
                                                            Html.option "influenza"
                                                            Html.option "UTI"
                                                        ]
                                                    ]
                                                    Daisy.input [
                                                        input.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.className "w-1/2"
                                                        prop.onChange (fun value ->
                                                            { state.Form with Diagnosis1 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Diagnosis1
                                                    ]
                                                ]
                                            ]
                                            Html.div [
                                                prop.className "flex flex-row w-full"
                                                prop.children [
                                                    Daisy.select [
                                                        select.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.onChange (fun value ->
                                                            { state.Form with Diagnosis2 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Diagnosis2
                                                        prop.className "w-1/2"
                                                        prop.children [
                                                            Html.option ""
                                                            Html.option "malaria"
                                                            Html.option "cold"
                                                            Html.option "influenza"
                                                            Html.option "UTI"
                                                        ]
                                                    ]
                                                    Daisy.input [
                                                        input.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.className "w-1/2"
                                                        prop.onChange (fun value ->
                                                            { state.Form with Diagnosis2 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Diagnosis2
                                                    ]
                                                ]
                                            ]
                                            Html.div [
                                                prop.className "flex flex-row w-full"
                                                prop.children [
                                                    Daisy.select [
                                                        select.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.onChange (fun value ->
                                                            { state.Form with Diagnosis3 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Diagnosis3
                                                        prop.className "w-1/2"
                                                        prop.children [
                                                            Html.option ""
                                                            Html.option "malaria"
                                                            Html.option "cold"
                                                            Html.option "influenza"
                                                            Html.option "UTI"
                                                        ]
                                                    ]
                                                    Daisy.input [
                                                        input.bordered
                                                        if state.Message = "Patient entry deleted" then prop.disabled true
                                                        prop.className "w-1/2"
                                                        prop.onChange (fun value ->
                                                            { state.Form with Diagnosis3 = value } |> FormChanged |> dispatch)
                                                        prop.value state.Form.Diagnosis3
                                                    ]
                                                ]
                                            ]
                                            Daisy.label [ Daisy.labelText "Treatment" ]
                                            Daisy.input [
                                                input.bordered
                                                if state.Message = "Patient entry deleted" then prop.disabled true
                                                prop.className "w-full"
                                                prop.onChange (fun value ->
                                                    { state.Form with Treatment = value } |> FormChanged |> dispatch)
                                                prop.value state.Form.Treatment
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        match state.Message with
                            | "Patient entry successfully edited" ->
                                Daisy.alert [
                                    prop.className "m-2 w-auto"
                                    alert.success
                                    prop.text state.Message
                                ]
                            | _ -> Html.div []
                        Daisy.button.button [
                            prop.className "btn-wide shadow-2xl m-2"
                            match state.Message with
                            | "Patient entry deleted" -> button.disabled
                            | _ -> button.outline
                            prop.text "Save Edited Data"
                            prop.type'.submit
                        ]
                    ]
                ]
            ]
        ]
    ]


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

type private State = { Form: PatientForm; InputFile: File option }

type private Msg =
    | FormChanged of PatientForm
    | FormSubmitted
    | FormSaved of unit
    | FileChosen of File
    | FileUploaded

let private init () =
    {
        Form = {
            Place = "Nebbi"
            Date = DateTime.Today.ToShortDateString()
            Name = "John Doe"
            Age = "13"
            Sex = "Unknown"
            Symptom1 = ""
            Symptom2 = ""
            Symptom3 = ""
            Tests = "HIV"
            Test1 = ""
            Test2 = ""
            Test3 = ""
            Result1 = "negative"
            Result2 = "negative"
            Result3 = "negative"
            Diagnosis1 = ""
            Diagnosis2 = ""
            Diagnosis3 = ""
            Treatment = "ibuprofen"
        }
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
    | FormChanged patientForm -> { model with Form = patientForm }, Cmd.none
    | FormSubmitted -> model, Cmd.OfAsync.perform service.SaveForm model.Form FormSaved
    | FormSaved _ -> model, Cmd.none
    | FileChosen file -> { model with InputFile = Some file }, Cmd.none
    | FileUploaded ->
         match model.InputFile with
         | None -> model, Cmd.none
         | Some file -> model, Cmd.OfAsync.perform upload file FormChanged
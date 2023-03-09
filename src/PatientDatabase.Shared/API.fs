module PatientDatabase.Shared.API

open System

type PatientForm =
    { Place: string
      Date: string
      Name: string
      Age: string
      Sex: string
      Symptom1: string
      Symptom2: string
      Symptom3: string
      Tests: string
      Test1: string
      Test2: string
      Test3: string
      Result1: string
      Result2: string
      Result3: string
      Diagnosis1: string
      Diagnosis2: string
      Diagnosis3: string
      Treatment: string }

type PatientListItem = {
    Id: Guid
    Place: string
    Date: string
    Name: string
    Age: int
    Sex: string
    Symptoms: string
    Tests: string
    Diagnoses: string
    Treatment: string
}

type Service =
    { GetMessage: bool -> Async<string>
      SaveForm: PatientForm -> Async<unit>
      ShowList: string * string -> Async<PatientListItem list>
      UploadData: Byte[] -> Async<PatientForm> }

    static member RouteBuilder _ m = sprintf "/api/service/%s" m

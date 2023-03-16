module PatientDatabase.Shared.API

open System
open CsvHelper.Configuration.Attributes

// attributes don't seem to help, solved by downgrading CsvHelper
type PatientForm =
    { [<Name("Place")>] Place: string
      [<Name("Date")>] Date: string
      [<Name("Name")>] Name: string
      [<Name("Age")>] Age: string
      [<Name("Sex")>] Sex: string
      [<Name("Symptom1")>] Symptom1: string
      [<Name("Symptom2")>] Symptom2: string
      [<Name("Symptom3")>] Symptom3: string
      [<Name("Tests")>] Tests: string
      [<Name("Test1")>] Test1: string
      [<Name("Test2")>] Test2: string
      [<Name("Test3")>] Test3: string
      [<Name("Result1")>] Result1: string
      [<Name("Result2")>] Result2: string
      [<Name("Result3")>] Result3: string
      [<Name("Diagnosis1")>] Diagnosis1: string
      [<Name("Diagnosis2")>] Diagnosis2: string
      [<Name("Diagnosis3")>] Diagnosis3: string
      [<Name("Treatment")>] Treatment: string }

type PatientListItem = {
    Id: Guid
    Place: string
    Date: DateTime
    Name: string
    Age: int
    Sex: string
    Symptom1: string
    Symptom2: string
    Symptom3: string
    Tests: string
    Diagnosis1: string
    Diagnosis2: string
    Diagnosis3: string
    Treatment: string
}

type Service =
    { GetMessage: bool -> Async<string>
      SaveForm: PatientForm -> Async<unit>
      ShowList: string * string -> Async<PatientListItem list>
      UploadData: byte[] -> Async<int>
      DownloadData: string * string -> Async<byte[]> }

    static member RouteBuilder _ m = sprintf "/api/service/%s" m

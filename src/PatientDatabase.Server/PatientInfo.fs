module PatientDatabase.Server.PatientInfo


open System
open System.IO
open System.Globalization
open System.Data
open System.Data.Common
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Features
open PatientDatabase.Shared.API
open Microsoft.Data.SqlClient
open FsToolkit.ErrorHandling
open FSharp.Data
open CsvHelper

module DataAccess =

    open Dapper.FSharp.MSSQL

    type PatientInfoRow = {
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
        Test1: string
        Test2: string
        Test3: string
        Result1: string
        Result2: string
        Result3: string
        Diagnosis1: string
        Diagnosis2: string
        Diagnosis3: string
        Treatment: string
    }

    let mapRow (id: Guid option) (info: PatientForm) =
        let res, date = DateTime.TryParse(info.Date)
        let resDate =
            match res with
            | false -> DateTime.Today
            | true -> date
        let age =
            try
                int info.Age
            with
                | _ -> 0
        {
            Id =
                match id with
                | Some x -> x
                | None -> Guid.NewGuid()
            Place = info.Place
            Date = resDate
            Name = info.Name
            Age = age
            Sex = info.Sex
            Symptom1 = info.Symptom1
            Symptom2 = info.Symptom2
            Symptom3 = info.Symptom3
            Tests = info.Tests
            Test1 = info.Test1
            Test2 = info.Test2
            Test3 = info.Test3
            Result1 = info.Result1
            Result2 = info.Result2
            Result3 = info.Result3
            Diagnosis1 = info.Diagnosis1
            Diagnosis2 = info.Diagnosis2
            Diagnosis3 = info.Diagnosis3
            Treatment = info.Treatment
        }

    let rowMap (data: PatientListItem) =
        {
            Place = data.Place
            Date = data.Date.ToShortDateString()
            Name = data.Name
            Age = string data.Age
            Sex = data.Sex
            Symptom1 = data.Symptom1
            Symptom2 = data.Symptom2
            Symptom3 = data.Symptom3
            Tests = data.Tests
            Test1 = ""
            Test2 = ""
            Test3 = ""
            Result1 = ""
            Result2 = ""
            Result3 = ""
            Diagnosis1 = data.Diagnosis1
            Diagnosis2 = data.Diagnosis2
            Diagnosis3 = data.Diagnosis3
            Treatment = data.Treatment
        }

    let formMap (data: PatientInfoRow) =
        {
            Place = data.Place
            Date = data.Date.ToShortDateString()
            Name = data.Name
            Age = string data.Age
            Sex = data.Sex
            Symptom1 = data.Symptom1
            Symptom2 = data.Symptom2
            Symptom3 = data.Symptom3
            Tests = data.Tests
            Test1 = data.Test1
            Test2 = data.Test2
            Test3 = data.Test3
            Result1 = data.Result1
            Result2 = data.Result2
            Result3 = data.Result3
            Diagnosis1 = data.Diagnosis1
            Diagnosis2 = data.Diagnosis2
            Diagnosis3 = data.Diagnosis3
            Treatment = data.Treatment
        }

    let patientInfoTable = table'<PatientInfoRow> "patient_info" |> inSchema "dbo"

    let createPatientInfo (conn: IDbConnection) (info: PatientForm) =
        let infoRow = mapRow None info
        insert {
            into patientInfoTable
            value infoRow
        }
        |> conn.InsertAsync

    let fetchPatientForm (conn: IDbConnection) (patientId: Guid) =
        select {
            for p in patientInfoTable do
            where (p.Id = patientId)
        }
        |> conn.SelectAsync<PatientInfoRow>
        |> Task.map (fun items -> items |> Seq.exactlyOne |> formMap)

    let editPatientInfo (conn: IDbConnection) (info: PatientForm) (patientId: Guid) =
        let infoRow = mapRow (Some patientId) info
        update {
            for p in patientInfoTable do
            set infoRow
            where (p.Id = patientId)
        }
        |> conn.UpdateAsync

    let showPatientInfo (conn: IDbConnection) (phrase: string) (field: string) =
        let search = "%" + phrase + "%"
        let query =
            match field with
                | "Name" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Name search)
                        orderBy p.Name
                        thenBy p.Age
                    }
                | "Age" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Age search)
                        orderBy p.Age
                        thenBy p.Name
                    }
                | "Sex" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Sex search)
                        orderBy p.Sex
                        thenBy p.Name
                    }
                | "Symptoms" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Symptom1 search || like p.Symptom2 search || like p.Symptom3 search)
                        orderBy p.Symptom1
                        thenBy p.Name
                    }
                | "Tests" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Tests search || like p.Test1 search || like p.Test2 search || like p.Test3 search)
                    }
                | "Results" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Result1 search || like p.Result2 search || like p.Result3 search)
                    }
                | "Diagnosis" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Diagnosis1 search || like p.Diagnosis2 search || like p.Diagnosis3 search)
                    }
                | "Treatment" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Treatment search)
                    }
                | "Place" ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Place search)
                        orderBy p.Place
                        thenBy p.Date
                    }
                | _ ->
                    select {
                        for p in patientInfoTable do
                        where (like p.Date search)
                        orderBy p.Date
                        thenBy p.Place
                    }
        query
        |> conn.SelectAsync<PatientInfoRow>
        |> Task.map (fun items ->
            items
            |> Seq.map (fun item ->
                (
                    {
                        Id = item.Id
                        Place = item.Place
                        Date = item.Date
                        Name = item.Name
                        Age = item.Age
                        Sex = item.Sex
                        Symptom1 = item.Symptom1
                        Symptom2 = item.Symptom2
                        Symptom3 = item.Symptom3
                        Tests = item.Tests
                        Diagnosis1 = item.Diagnosis1
                        Diagnosis2 = item.Diagnosis2
                        Diagnosis3 = item.Diagnosis3
                        Treatment = item.Treatment
                    }: PatientListItem
                )
            ) |> Seq.toList
        )

    let uploadPatientInfo (conn: IDbConnection) (data: byte[]) =
        let stringData: string = System.Text.Encoding.ASCII.GetString(data)
        use reader = new StringReader(stringData)
        use csv = new CsvReader(reader, CultureInfo.InvariantCulture)
        csv.Configuration.PrepareHeaderForMatch <- ( fun header _ -> CultureInfo.CurrentCulture.TextInfo.ToTitleCase( header )) // only works in CsvHelper 17.0.0 and older it seems
        let patientFormData = List.ofSeq (csv.GetRecords<PatientForm>())
        let infoTable =
            patientFormData |> List.map (mapRow None)
        insert {
            into patientInfoTable
            values infoTable
        }
        |> conn.InsertAsync

    let downloadPatientInfo (conn: IDbConnection) (phrase: string) (field: string) =
        task {
            let! data = showPatientInfo conn phrase field
            let outputData =
                data |> List.map rowMap
            use writer = new StringWriter()
            use csv = new CsvWriter(writer, CultureInfo.InvariantCulture)
            csv.WriteRecords(outputData)
            let downloadedFile = writer.ToString()
            return System.Text.Encoding.ASCII.GetBytes(downloadedFile)
        }

    let deletePatientInfo (conn: IDbConnection) (id: Guid) =
        delete {
            for p in patientInfoTable do
            where (p.Id = id)
        } |> conn.DeleteAsync


module HttpHandlers =
    let createPatientInfo (ctx: HttpContext) (form: PatientForm) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! _ = DataAccess.createPatientInfo conn form
            return()
        }

    let fetchPatientForm (ctx: HttpContext) (id: Guid) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! form = DataAccess.fetchPatientForm conn id
            return form
        }

    let editPatientInfo (ctx: HttpContext) (form: PatientForm, id: Guid) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! _ = DataAccess.editPatientInfo conn form id
            return()
        }

    let showPatientInfo (ctx: HttpContext) (phrase: string, field: string) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! items = DataAccess.showPatientInfo conn phrase field
            return items |> List.ofSeq
        }

    let uploadPatientInfo (ctx: HttpContext) (data: byte[]) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! rows = DataAccess.uploadPatientInfo conn data
            return rows
        }

    let downloadPatientInfo (ctx: HttpContext) (file: string, phrase: string) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! data = DataAccess.downloadPatientInfo conn file phrase
            return data
        }

    let deletePatientInfo (ctx: HttpContext) (id: Guid) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! row = DataAccess.deletePatientInfo conn id
            return row
        }
module PatientDatabase.Server.PatientInfo


open System
open System.Data
open System.Data.Common
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Features
open PatientDatabase.Shared.API
open Microsoft.Data.SqlClient
open FsToolkit.ErrorHandling
open FSharp.Data

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

    let mapRow (info: PatientForm) = {
            Id = Guid.NewGuid()
            Place = info.Place
            Date = DateTime.Parse(info.Date)
            Name = info.Name
            Age = int info.Age
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

    let patientInfoTable = table'<PatientInfoRow> "patient_info" |> inSchema "dbo"

    let createPatientInfo (conn: IDbConnection) (info: PatientForm) =
        let infoRow = mapRow info
        insert {
            into patientInfoTable
            value infoRow
        }
        |> conn.InsertAsync

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
                        Date = item.Date.ToLongDateString()
                        Name = item.Name
                        Age = item.Age
                        Sex = item.Sex
                        Symptoms = item.Symptom1 + " " + item.Symptom2 + " " + item.Symptom3
                        Tests = item.Tests
                        Diagnoses = item.Diagnosis1 + " " + item.Diagnosis2 + " " + item.Diagnosis3
                        Treatment = item.Treatment
                    }: PatientListItem
                )
            )
        )

    let uploadPatientInfo (conn: IDbConnection) (data: byte[]) =
        let stringData: string = System.Text.Encoding.ASCII.GetString(data)
        let stringRows: string[][] = stringData.Split('\n') |> Array.map (fun (row: string) -> row.Split(','))
        let patientFormData: PatientForm list = [
            for row in stringRows[1..stringRows.Length - 2] ->
                {
                Place = row[0];
                Date = row[1];
                Name = row[2];
                Age = row[3];
                Sex = row[4];
                Symptom1 = row[5];
                Symptom2 = row[6];
                Symptom3 = row[7];
                Tests = row[8];
                Test1 = row[9];
                Test2 = row[10];
                Test3 = row[11];
                Result1 = row[12];
                Result2 = row[13];
                Result3 = row[14];
                Diagnosis1 = row[15];
                Diagnosis2 = row[16];
                Diagnosis3 = row[17];
                Treatment = row[18];
                }
        ]

        let infoTable =
            patientFormData |> List.map mapRow
        insert {
            into patientInfoTable
            values infoTable
        }
        |> conn.InsertAsync


module HttpHandlers =
    let createPatientInfo (ctx: HttpContext) (form: PatientForm) =
        task {
            let conn = ctx.GetService<SqlConnection>()
            let! _ = DataAccess.createPatientInfo conn form
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
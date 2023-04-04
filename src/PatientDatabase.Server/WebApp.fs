module PatientDatabase.Server.WebApp

open Giraffe
open Giraffe.GoodRead
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.Extensions.Logging
open PatientDatabase.Shared.API
open PatientDatabase.Shared.Errors
open PatientDatabase.Server.PatientInfo

let service ctx =
    { GetMessage =
        fun success ->
            task {
                if success then
                    return "Hi from Server!"
                else
                    return ServerError.failwith (ServerError.Exception "OMG, something terrible happened")
            }
            |> Async.AwaitTask
      SaveForm = HttpHandlers.createPatientInfo ctx >> Async.AwaitTask
      FetchForm = HttpHandlers.fetchPatientForm ctx >> Async.AwaitTask
      EditForm = HttpHandlers.editPatientInfo ctx >> Async.AwaitTask
      ShowList = HttpHandlers.showPatientInfo ctx >> Async.AwaitTask
      UploadData = HttpHandlers.uploadPatientInfo ctx >> Async.AwaitTask
      DownloadData = HttpHandlers.downloadPatientInfo ctx >> Async.AwaitTask
      DeleteEntry = HttpHandlers.deletePatientInfo ctx >> Async.AwaitTask
    }

let webApp: HttpHandler =
    let remoting logger =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Service.RouteBuilder
        |> Remoting.fromContext service
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.buildHttpHandler

    choose [ Require.services<ILogger<_>> remoting; htmlFile "public/index.html" ]

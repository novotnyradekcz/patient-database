module PatientDatabase.Client.Router

open Browser.Types
open Feliz.Router
open Fable.Core.JsInterop

type Page =
    | Index
    | About
    | PatientList
    | DataUpload

[<RequireQualifiedAccess>]
module Page =
    let defaultPage = Page.Index

    let parseFromUrlSegments =
        function
        | [ "about" ] -> Page.About
        | [ "patientlist" ] -> Page.PatientList
        | [ "dataupload" ] -> Page.DataUpload
        | [] -> Page.Index
        | _ -> defaultPage

    let noQueryString segments : string list * (string * string) list = segments, []

    let toUrlSegments =
        function
        | Page.Index -> [] |> noQueryString
        | Page.About -> [ "about" ] |> noQueryString
        | Page.PatientList -> [ "patientlist" ] |> noQueryString
        | Page.DataUpload -> [ "dataupload" ] |> noQueryString

[<RequireQualifiedAccess>]
module Router =
    let goToUrl (e: MouseEvent) =
        e.preventDefault ()
        let href: string = !!e.currentTarget?attributes?href?value
        Router.navigatePath href

    let navigatePage (p: Page) =
        p |> Page.toUrlSegments |> Router.navigatePath

[<RequireQualifiedAccess>]
module Cmd =
    let navigatePage (p: Page) =
        p |> Page.toUrlSegments |> Cmd.navigatePath

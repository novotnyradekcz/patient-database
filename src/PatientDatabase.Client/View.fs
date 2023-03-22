module PatientDatabase.Client.View

open Feliz
open Router
open Elmish
open SharedView
open Feliz.UseElmish
open Feliz.DaisyUI

type private Msg = UrlChanged of Page

type private State = { Page: Page }

let private init () =
    let nextPage = Router.currentPath () |> Page.parseFromUrlSegments
    { Page = nextPage }, Cmd.navigatePage nextPage

let private update (msg: Msg) (state: State) : State * Cmd<Msg> =
    match msg with
    | UrlChanged page -> { state with Page = page }, Cmd.none

[<ReactComponent>]
let AppView () =
    let state, dispatch = React.useElmish (init, update)

    let navigation =
        Daisy.artboard [
            prop.children [
                Daisy.menu [
                    menu.horizontal
                    prop.className "items-stretch shadow-lg bg-base-100 rounded-box m-2"
                    prop.children [
                        Html.li [Html.a ("Data Entry", Page.Index)]
                        Html.li [Html.a ("Data Upload", Page.DataUpload)]
                        Html.li [Html.a ("Patient List", Page.PatientList)]
                        Html.li [Html.a ("About", Page.About)]
                    ]
                ]
            ]
        ]

    let render =
        match state.Page with
        | Page.Index -> Pages.Index.IndexView()
        | Page.DataUpload -> Pages.DataUpload.IndexView()
        | Page.PatientList -> Pages.PatientList.IndexView()
        | Page.EditPatient -> Pages.EditPatient.IndexView()
        | Page.About -> Html.text "Based on SAFEr Template by Dzoukr"

    React.router
        [ router.pathMode
          router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
          router.children [ navigation; render ] ]

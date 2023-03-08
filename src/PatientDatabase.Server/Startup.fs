module PatientDatabase.Server.Startup

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Microsoft.Data.SqlClient

type Startup(cfg:IConfiguration, env:IWebHostEnvironment) =
    member _.ConfigureServices (services:IServiceCollection) =
        let dbconnstring = cfg["database:connectionString"]
        services.AddTransient<SqlConnection>(fun _ ->
            let conn = new SqlConnection(dbconnstring)
            conn.Open()
            conn
            ) |> ignore
        services
            .AddApplicationInsightsTelemetry(cfg.["APPINSIGHTS_INSTRUMENTATIONKEY"])
            .AddGiraffe() |> ignore
    member _.Configure(app:IApplicationBuilder) =
        app
            .UseStaticFiles()
            .UseGiraffe WebApp.webApp
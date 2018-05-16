open DotLiquid
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.DotLiquid
open FSharp.Data

setTemplatesDir "./templates"

[<Literal>]
let sample = "http://api.openweathermap.org/data/2.5/weather?APPID=96916a5177869f140f4f2b85d6df30ca&q=London"

let apiUrl = "http://api.openweathermap.org/data/2.5/weather?APPID=96916a5177869f140f4f2b85d6df30ca&q="

type Weather = JsonProvider<sample>

let city = Weather.Load(apiUrl + "Tokyo")

type Model = {
    name : string
    windspeed : decimal
    maintemp : decimal
    humidity: int
}

let o = { 
    name = city.Name
    windspeed = city.Wind.Speed
    maintemp = city.Main.Temp
    humidity = city.Main.Humidity
    }

let app =
  choose
    [ GET >=> choose
        [ path "/" >=> page "my_page.liquid" o ]]

startWebServer defaultConfig app

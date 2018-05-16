open FSharp.Data

[<Literal>]
let sample = "http://api.openweathermap.org/data/2.5/weather?APPID=96916a5177869f140f4f2b85d6df30ca&q=London"
let apiUrl = "http://api.openweathermap.org/data/2.5/weather?APPID=96916a5177869f140f4f2b85d6df30ca&q="

type Weather = JsonProvider<sample>

let city = Weather.Load(apiUrl + "Tokyo")
printfn "city.Sys.Country: %s" city.Sys.Country
printfn "city.Wind.Speed: %f" city.Wind.Speed
printfn "city.Main.Temp: %f" city.Main.Temp
printfn "city.Clouds: %f" city.Wind.Deg
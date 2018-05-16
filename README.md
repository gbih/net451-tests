# Various tests for .net 451 on OSX

1. Target .net 451 build on OSX
2. Import and use a .net 451 dll built on Windows
3. Create Suave server on .net 451, using the Windows .net 451 dll

Misc Notes:
1. To test on OS X, we need to install an F# Compiler that runs on .net Framework or Mono.
2. We should be able to use only the command line.
3. For organization (and ease of opening via Visual Studio if desired), we will create a single solution file.


------------


## Quick Install and Test:

Step 1. Clone this entire repo.

Step 2. Build from the project root (containing the solution file)

```shell
$ dotnet restore
$ dotnet add fsharpdata451 package FSharp.Data --version 2.4.6 --package-directory fsharpdata451/packages
$ dotnet add suavedotliquid451 package FSharp.Data --version 2.4.6 --package-directory suavedotliquid451/packages

$ msbuild /t:restore
$ msbuild
```

Step 3. Run
```shell
$ mono helloworld451/bin/Debug/net451/helloworld451.exe
$ mono fsharpdata451/bin/Debug/net451/fsharpdata451.exe
```

For the last example, we are using dot-liquid templates and have to run from within that project directory:
```shell
$ cd suavedotliquid451 && mono bin/Debug/net451/suavedotliquid451.exe
```


-----------------------

## Manual Creation and Test:

#### First, create a single solution file:
```shell
$ dotnet new sln --name net451test
```



## CASE 1: Target NET 4.5.1 build on OS X

Step 1: Setup
We can use the .NET Core CLI here:

```shell
$ dotnet new console -lang F# --target-framework-override net451 -n helloworld451 --output helloworld451
$ dotnet sln add ./helloworld451/helloworld451.fsproj
```


Step 2: Build
To target NET 4.5.1, we use the older .NET Framework via msbuild

```shell
$ cd helloworld451
$ msbuild /t:restore
$ msbuild
$ mono bin/Debug/net451/helloworld451.exe
```

This should output:
Hello World from F#!

-------------------------


## CASE 2: Read a Windows NET 4.5.1 dll, then build to NET 4.5.1 on OS X


Step 1: Search for an appropiate 4.5.1 dll built on Windows.
A popular package is FSharp.Data
We will use version 2.4.6:
https://www.nuget.org/packages/FSharp.Data/2.4.6

Use Telerik's JustDecompile to confirm the Target Framework was built on .NET Framework 4.5:

```shell
[assembly: AssemblyDescription("Library of F# type providers and data access tools")]
[assembly: AssemblyFileVersion("2.4.6.0")]
[assembly: AssemblyProduct("FSharp.Data")]
[assembly: AssemblyTitle("FSharp.Data")]
[assembly: AssemblyVersion("2.4.6.0")]
[assembly: AssemblyVersion("2.4.6.0")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.None)]
[assembly: FSharpInterfaceDataVersion(2, 0, 0)]
[assembly: InternalsVisibleTo("FSharp.Data.Tests")]
[assembly: TargetFramework(".NETFramework,Version=v4.5", FrameworkDisplayName=".NET Framework 4.5")]
[assembly: TypeProviderAssembly("FSharp.Data.DesignTime")]
```


Step 2: Setup Project

In the project root directory:

```shell
$ dotnet new console -lang F# --target-framework-override net451 -n fsharpdata451 --output fsharpdata451
$ dotnet sln add ./fsharpdata451/fsharpdata451.fsproj
$ cd fsharpdata451
$ dotnet add package FSharp.Data --version 2.4.6 --package-directory packages
```


Step 3. Setup .fsproj file

Edit app451.fsproj to hard-code the link to FSharp.Data.dll (for testing purposes):

```shell
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net451</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Program.fs" />
  </ItemGroup>

  <ItemGroup>
    <Reference Include="FSharp.Data, Version=2.4.6">
      <HintPath>packages/fsharp.data/2.4.6/lib/net45/FSharp.Data.dll</HintPath>
    </Reference>
  </ItemGroup>

</Project>
```


Step 4. Setup Program.fs

```
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
```


Step 5. Build and Run

```shell
$ msbuild /t:restore
$ msbuild
```


To run:

```shell
$ mono bin/Debug/net451/fsharpdata451.exe
```

This should output something similar to:

city.Sys.Country: JP
city.Wind.Speed: 4.600000
city.Main.Temp: 299.730000
city.Clouds: 180.000000


---------------------------


## CASE 3: Read a Windows NET 4.5.1 dll, then build to NET 4.5.1 on OS X, using Suave.DotLiquid templates with  FSharp.Data


Step 1: Setup Project

In the project root directory:

```shell
$ dotnet new console -lang F# --target-framework-override net451 -n suavedotliquid451 --output suavedotliquid451
$ dotnet sln add ./suavedotliquid451/suavedotliquid451.fsproj
$ cd suavedotliquid451
$ dotnet add package FSharp.Data --version 2.4.6 --package-directory packages
$ dotnet add package DotLiquid --version 2.0.145
$ dotnet add package Suave --version 2.2.1
$ dotnet add package Suave.DotLiquid --version 2.2.1
```

The above combination of DotLiquid, Suave.DotLiquid, and Suave appears compatible with .net 4.5.1. 


Step 2: Edit the .fsproj file

```shell
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net451</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Program.fs" />
    <None Include="templates/**/*" Link="views/%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />
    <None Include="assets/**/*" Link="assets/%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>

  <ItemGroup>
    <Reference Condition="'$(IsWindows)' != 'true'" Include="FSharp.Data, Version=2.4.6">
      <HintPath>packages/fsharp.data/2.4.6/lib/net45/FSharp.Data.dll</HintPath>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="DotLiquid" Version="2.0.145" />
    <PackageReference Include="Suave" Version="2.2.1" />
    <PackageReference Include="Suave.DotLiquid" Version="2.2.1" />
    <PackageReference Condition="'$(IsWindows)' == 'true'" Include="FSharp.Data" Version="2.4.6" /> 
  </ItemGroup>

</Project>
```


Step 3: Setup Program.fs

```shell
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
```



Step 4: Setup  /templates/my_page.liquid

```shell
$ mkdir templates
$ touch my_page.liquid
```

Edit my_page.liquid as:

```shell
<!doctype html>
<html lang="en">
<head>
    <title>FSharp + DotLiquid Test</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css">
</head>
<body>
    <div class="jumbotron">
        <h1 class="display-5">FSharp.Data + Suave.DotLiquid</h1>
        <table class="table mt-5">
            <thead>
                <tr>
                    <th scope="col">City</th>
                    <th scope="col">Windspeed</th>
                    <th scope="col">Temperature</th>
                    <th scope="col">Humidity</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>{{model.name}}</td>
                    <td>{{model.windspeed}}</td>
                    <td>{{model.maintemp}}</td>
                    <td>{{model.humidity}}</td>
                </tr>
            </tbody>
        </table>
    </div>
</body>
</html>
```


Step 5: Create /assets directory
```shell
$ mkdir assets
```


Step 6: Build
```shell
$ msbuild /t:restore
$ msbuild
$ mono bin/Debug/net451/suavedotliquid451.exe
```

Check output via a browser:
http://127.0.0.1:8080/

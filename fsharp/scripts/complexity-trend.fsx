#r @"C:\NugetLocal\FSharp.Charting.0.91.1\lib\net45\FSharp.Charting.dll"

open FSharp.Charting
open System
open System.Globalization

//Folder containing all the versions
let folder = @"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\in\AuctioningFacade.cs\"

let calculateComplexity file = 
    //https://github.com/adamtornhill/indent-complexity-proxy
    let raw = 
         let p = new System.Diagnostics.Process();
         p.StartInfo.UseShellExecute <- false
         p.StartInfo.RedirectStandardOutput <- true
         p.StartInfo.FileName <- @"c:/Program Files (x86)/Common Files/Oracle/Java/javapath/java"
         p.StartInfo.Arguments <- sprintf @"-jar ""C:\Users\Jo.VanEyck\Desktop\indent-complexity-proxy\target\indent-complexity-proxy-0.2.0-standalone.jar"" -c ""%s""" file
         p.Start() |> ignore

         let output = p.StandardOutput.ReadToEnd()
         p.WaitForExit()

         output
    raw.Split([|"\r\n"|], System.StringSplitOptions.RemoveEmptyEntries)
    |> Seq.head
    |> (fun r -> System.Decimal.Parse(r, CultureInfo.InvariantCulture))

let enumerateFiles directory =
    System.IO.Directory.EnumerateFiles(directory)

type ComplexityPoint = { File : string; Changeset : int; Date : DateTime; Complexity : decimal }
let processFile file =
    printfn "Processing %s..." file
    let name = System.IO.FileInfo(file).Name
    let parts = name.Split([|'.'|])
    let cs = parts.[0] |> System.Int32.Parse
    let date = System.DateTime.ParseExact(parts.[1], "ddMMyyyy", CultureInfo.InvariantCulture)
    let complexity = calculateComplexity file
    {File = file; Changeset = cs; Date = date; Complexity = complexity}

let complexities = 
    enumerateFiles folder
    |> Seq.map processFile
    |> Seq.toList

let sorted = complexities |> Seq.sortBy (fun c -> c.Changeset)
//Heavy operation, best save intermediate results
sorted
|> Seq.map (fun l -> sprintf "%s,%d,%A,%A" l.File l.Changeset l.Date l.Complexity)
|> fun l -> System.IO.File.WriteAllLines(@"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\out\auctionfacade-changeset-complexity.csv", l |> Seq.toArray)

let chart =
    Chart.Point(
        data=(sorted |> Seq.map (fun p -> (p.Date, p.Complexity))), 
        //Labels = (sorted |> Seq.map (fun p -> p.Changeset.ToString())),
        XTitle = "Date",
        YTitle = "Complexity (indent)",
        Title = "mithra-auctionfacade-complexitytrend",
        MarkerSize = 3)

chart |> Chart.Show
//chart |> Chart.Save (sprintf @"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\out\%s.png" "mithra-auctionfacade-complexitytrend")
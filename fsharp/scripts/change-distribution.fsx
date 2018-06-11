#r @"C:\NugetLocal\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"
#r @"C:\NugetLocal\FSharp.Charting.0.91.1\lib\net45\FSharp.Charting.dll"

open FSharp.Data
open FSharp.Charting

let excludePatterns = [
    "/Setup/MithraServer"
    "/packages/"
    "/bin/"
    "/Database.BEL/"
    ".csproj"
    ".json"
    ".config"
]
let exclusionFilter (data : string) = 
    excludePatterns 
    |> List.exists (fun pattern -> data.Contains(pattern)) |> not

//docker run -v C:/Users/Jo.VanEyck/Desktop/tech-debt/mithra/:/data -it code-maat-app -c tfs -l /data/tfslog.log > code-maat-result.csv
let [<Literal>] codemaatresults = @"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\in\code-maat-result.csv"
type CodeMaatResultsCsv = CsvProvider<codemaatresults>
type CodeMaatResult = { Entity : string; NumberCommits : int}
let commitInformation = 
    CodeMaatResultsCsv.GetSample().Rows
    |> Seq.map (fun r -> { Entity = r.Entity.Replace("/MITHRA/030 Development/Dev/", "./"); NumberCommits = r.``N-revs``})
    |> Seq.filter (fun row -> exclusionFilter row.Entity)
    |> Seq.sortByDescending (fun r -> r.NumberCommits)

let c =
    Chart.Point(
        data=(commitInformation |> Seq.map (fun p -> (p.NumberCommits))),
        YTitle = "Number of commits",
        Title = "Commit distribution",
        MarkerSize = 5)
//c |> Chart.Show
c |> Chart.Save (sprintf @"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\out\%s.png" "mithra-commit-distribution")
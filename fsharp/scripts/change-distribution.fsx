#r @"..\packages\FSharp.Data\lib\net45\FSharp.Data.dll"
#r @"..\packages\FSharp.Charting\lib\net45\FSharp.Charting.dll"
open FSharp.Data
open FSharp.Charting

let excludePatterns = [
    ".idea"
    ".iml"
]
let exclusionFilter (data : string) = 
    excludePatterns 
    |> List.exists (fun pattern -> data.Contains(pattern)) |> not

//docker run -v C:/Users/Jo.VanEyck/Desktop/tech-debt/mithra/:/data -it code-maat-app -c tfs -l /data/tfslog.log > code-maat-result.csv
let [<Literal>] codemaatresultsSample = @"..\samples\code-maat-result.csv"
type CodeMaatResultsCsv = CsvProvider<codemaatresultsSample>
type CodeMaatResult = { Entity : string; NumberCommits : int}
let commitInformation =
    CodeMaatResultsCsv.Load(@"..\..\case-studies\cims\in\cims-declaration-api-maat.log").Rows
    |> Seq.map (fun r -> { Entity = r.Entity; NumberCommits = r.``N-revs``})
    |> Seq.filter (fun row -> exclusionFilter row.Entity)
    |> Seq.sortByDescending (fun r -> r.NumberCommits)

let c =
    Chart.Point(
        data=(commitInformation |> Seq.map (fun p -> (p.NumberCommits))),
        YTitle = "Number of commits",
        Title = "Commit distribution",
        MarkerSize = 5)
//c |> Chart.Show
c |> Chart.Save (sprintf @"..\..\case-studies\cims\out\%s.png" "cims-declaration-api-commit-distribution")
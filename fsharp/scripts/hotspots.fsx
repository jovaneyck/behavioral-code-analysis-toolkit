#r @"..\packages\FSharp.Data\lib\net45\FSharp.Data.dll"
#r @"..\packages\FSharp.Charting\lib\net45\FSharp.Charting.dll"
open FSharp.Data
open FSharp.Charting


let excludePatterns = [
    ".idea"
    ".iml"
    "pom.xml"
    "/target/"
    "test/import inbound asn containers/asn-jms.xml"
]
let exclusionFilter (data : string) = 
    excludePatterns 
    |> List.exists (fun pattern -> data.Contains(pattern)) |> not

type ClocCsv = CsvProvider< @"..\samples\cloc.csv">
let cloc = ClocCsv.Load(@"..\..\case-studies\cims\in\cims-cloc.log")
type LocResult = { FileName : string; LinesOfCode : int }

let locInformation =
    cloc.Rows 
    |> Seq.toList
    |> Seq.filter (fun row -> exclusionFilter row.Filename)
    |> Seq.sortByDescending (fun r -> r.Code)
    |> Seq.map (fun r -> { LinesOfCode = r.Code; FileName = r.Filename})

//docker run -v C:/Users/Jo.VanEyck/Desktop/tech-debt/mithra/:/data -it code-maat-app -c tfs -l /data/tfslog.log > code-maat-result.csv
let [<Literal>] codemaatresults = @"..\samples\code-maat-result.csv"
type CodeMaatResultsCsv = CsvProvider<codemaatresults>
type CodeMaatResult = { Entity : string; NumberCommits : int}
let commitInformation = 
    CodeMaatResultsCsv.Load(@"..\..\case-studies\cims\in\cims-maat.log").Rows
    |> Seq.map (fun r -> { Entity = "./" + r.Entity; NumberCommits = r.``N-revs``})
    |> Seq.filter (fun row -> exclusionFilter row.Entity)
    |> Seq.sortByDescending (fun r -> r.NumberCommits)

let extremeCases =
    Seq.append
        (locInformation
        |> Seq.take 50
        |> Seq.map (fun i -> i.FileName))

        (commitInformation
        |> Seq.take 50
        |> Seq.map (fun c -> c.Entity))
    |> Seq.distinct
// extremeCases |> Seq.length
type PlotPoint = { Entity : string; LinesOfCode : LocResult; Commits : CodeMaatResult }
let toPlotpoint cases =
    let locLookup = locInformation |> Seq.map (fun l -> (l.FileName, l)) |> Map.ofSeq
    let commitLookup = commitInformation |> Seq.map (fun c -> (c.Entity, c)) |> Map.ofSeq
    let caseToPlotPoint case = 
        let loc = locLookup |> Map.tryFind case

        let commits = commitLookup |> Map.tryFind case
        match loc, commits with
        | Some l, Some c ->
            Some { Entity = l.FileName; LinesOfCode = l; Commits = c }
        | Some l, _ ->
            printfn "WARNING: no commit information found for %s" case
            None
        | _, Some c ->
            printfn "WARNING: no cloc information found for %s" case
            None
        | _, _ ->
            printfn "WARNING: no commit NOR cloc information found for %s" case
            None
    cases 
    |> Seq.map caseToPlotPoint
    |> Seq.choose id

let points = toPlotpoint extremeCases |> Seq.toList

let c =
    Chart.Point(
        data=(points |> Seq.map (fun p -> (p.LinesOfCode.LinesOfCode,p.Commits.NumberCommits))), 
        Labels = (points |> Seq.map (fun p -> p.Entity)),
        XTitle = "Lines of code",
        YTitle = "Number of commits",
        Title = "Hotspots",
        MarkerSize = 3)   
//c |> Chart.Show
c |> Chart.Save (sprintf @"..\..\case-studies\cims\out\%s.png" "cims-declaration-api-hotspots")
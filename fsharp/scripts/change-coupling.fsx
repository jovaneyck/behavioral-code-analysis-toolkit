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


// docker run -v C:/Users/Jo.VanEyck/Desktop/tech-debt/mithra/:/data -it code-maat-app -c tfs -a coupling -l /data/tfslog.log > code-maat-coupling-result.csv
let [<Literal>] codemaatcouplingresults = @"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\in\code-maat-coupling-result.csv"
type CodeMaatCouplingResultsCsv = CsvProvider<codemaatcouplingresults>
type CodeMaatCouplingResult = { Entity : string; Coupled: string; AverageRevisions : int; Degree : int}

CodeMaatCouplingResultsCsv.GetSample().Rows
|> Seq.map (fun r -> { Entity = r.Entity; Coupled = r.Coupled; AverageRevisions = r.``Average-revs``; Degree = r.Degree })
|> Seq.filter (fun row -> exclusionFilter row.Entity)
//|> Seq.filter (fun c -> c.Entity.Contains("Given") && c.Coupled.Contains("When") || c.Entity.Contains("When") && c.Coupled.Contains("Given"))
|> Seq.filter (fun c -> c.Degree >= 50 && c.AverageRevisions >= 15)
|> Seq.sortByDescending (fun c -> c.Degree, c.AverageRevisions)
|> Seq.map (fun c -> sprintf "%s;%d;%d;\n%s" c.Entity c.Degree c.AverageRevisions c.Coupled)
|> (fun lines -> System.IO.File.WriteAllLines(@"C:\Users\Jo.VanEyck\Desktop\tech-debt\mithra\out\coupling-to-investigate.csv", lines))

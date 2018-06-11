let tfexec arguments = 
     let p = new System.Diagnostics.Process();
     p.StartInfo.UseShellExecute <- false
     p.StartInfo.RedirectStandardOutput <- true
     p.StartInfo.FileName <- @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe"
     p.StartInfo.Arguments <- arguments
     p.Start() |> ignore

     let output = p.StandardOutput.ReadToEnd()
     p.WaitForExit()

     output

let fullFileName = @"C:\Users\Jo.VanEyck\source\Workspaces\MITHRA\030 Development\Dev\Server\Application\Auctions\AuctioningFacade.cs"

let tfhist = tfexec <| sprintf @"hist ""%s"" /noprompt"  fullFileName

let split (sep : string) (s : string) = s.Split([|sep|], System.StringSplitOptions.RemoveEmptyEntries)

//fugly textual output makes it really hard to parse from. Brute-forcing the shit out of it
type VersionInfo = { Changeset : int; Date : System.DateTime }
let rec parseDate (split : string []) =
    match split.[0] |> System.DateTime.TryParse with
    | true, date -> date
    | false, _ -> parseDate (split |> Array.skip 1)

let changeSets = 
    tfhist 
    |> split "\r\n"
    |> Seq.skip 2
    |> Seq.map (fun line -> line |> split " ")
    |> Seq.map (fun splitted -> { Changeset = splitted.[0] |> System.Int32.Parse; Date = parseDate splitted})
    |> Seq.toList

let getVersion file version =
    let getVersionCommand file v =
        sprintf @"get ""%s"";%d /noprompt" file v

    tfexec <| getVersionCommand file version.Changeset |> ignore
    let f = System.IO.FileInfo(file)
    let dir = sprintf "%s\%s" __SOURCE_DIRECTORY__ f.Name
    let destination = sprintf "%s\%d.%s%s" dir version.Changeset (version.Date.ToString("ddMMyyyy")) f.Extension
    System.IO.Directory.CreateDirectory(dir) |> ignore
    System.IO.File.Copy(file, destination)

changeSets
|> Seq.iter (fun version -> 
    printfn "%A" version
    getVersion fullFileName version)
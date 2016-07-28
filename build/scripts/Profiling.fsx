#I @"../../packages/build/FAKE/tools"
#r @"FakeLib.dll"
#r @"System.Xml.Linq.dll"

#load @"Paths.fsx"

open System
open System.IO
open System.Diagnostics
open System.Xml
open System.Xml.Serialization
open System.Xml.Linq

open Fake

open Paths

module Profiler =
    let private project = "Profiling"
    let private profiledApp = sprintf "%s/bin/Release/net45/%s.exe" (Paths.Source(project)) project
    let private snapShotOutput = Paths.Output("ProfilingSnapshot.dtp")
    let private snapShotStatsOutput = Paths.Output("ProfilingSnapshotStats.html")
    let private profileOutput = Paths.Output("ProfilingReport.xml")
    let private patternInput = Paths.Build("profiling/pattern.xml")

    let private element name children =
        XElement(XName.Get name, (children:XObject seq)) :> XObject

    let private attribute name value =
        XAttribute(XName.Get name, value) :> XObject

    let Run() = 
        Tooling.MsBuild.Exec "Rebuild" Tooling.DotNetFramework.Net45.Identifier [Paths.CsProj(project)]
        Tooling.execProcessWithTimeout profiledApp [] (TimeSpan.FromMinutes 30.) |> ignore

        let performanceOutput = Paths.Output("profiling/performance") |> directoryInfo
        let memoryOutput = Paths.Output("profiling/memory") |> directoryInfo

        for snapshot in Directory.EnumerateFiles(memoryOutput.FullName, "*.dmrs.zip", SearchOption.AllDirectories) do
            Rename (snapshot.Replace(".zip", "")) snapshot

        for snapshot in Directory.EnumerateFiles(performanceOutput.FullName, "*.dtp", SearchOption.AllDirectories) do
            let snapshotPath = snapshot |> fileInfo
            let snapShotName = snapshotPath.Directory.FullName
                                    .Replace(performanceOutput.FullName, "")
                                    .Replace(@"\", ".")
                                    .TrimStart('.')
            let snapShotFileName = snapShotName + ".xml"                     
            let snapshotReport = combinePaths snapshotPath.Directory.FullName snapShotFileName

            Tooling.DotTraceSnapshotStats.Exec [snapshotPath.FullName; (combinePaths Paths.BuildOutput (snapShotName + ".html")); @"/full"]
            Tooling.DotTraceReporter.Exec [@"/reporting"; snapshotPath.FullName; patternInput; snapshotReport]

            let report = XDocument.Load snapshotReport
            let reportElem = report.Element(XName.Get "Report")
            reportElem.Add (attribute "Name" snapShotName)
            report.Save snapshotReport

            if fileExists profileOutput = false then
                let reports = element "Reports" [ reportElem ]
                let doc = new XDocument([| reports |])
                doc.Save profileOutput
            else
                let reports = XDocument.Load profileOutput
                let reportsElem = reports.Element(XName.Get "Reports")
                reportsElem.Add reportElem
                reports.Save profileOutput
    
module Benchmarker =
   let private benchmarkingApp = sprintf "%s/%s" (Paths.BinFolder("Benchmarking")) "Benchmarking.exe" 

   let private failure errors =
        raise (BuildException("The project benchmarking failed.", errors |> List.ofSeq))

   let Run() =
        !! Paths.Source("Benchmarking/project.json") 
        |> Seq.map DirectoryName
        |> Seq.iter(fun project -> 
                Tooling.DotNet.Exec ["--project"; project; "run"; "-i false"; "-t 5"])
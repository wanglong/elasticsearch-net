#I @"../../packages/build/FAKE/tools"
#r @"FakeLib.dll"
open Fake

#load @"Paths.fsx"

open System
open System.IO
open System.Linq
open System.Diagnostics
open Paths

module Benchmarker =
   let private testsProjectDirectory = Path.GetFullPath(Paths.Source("Tests"))
   let private failure errors =
        raise (BuildException("benchmarking failed.", errors |> List.ofSeq))
   let private clrOutput = Path.GetFullPath(Paths.Output("benchmarks/clr")) |> directoryInfo
   let private coreClrOutput = Path.GetFullPath(Paths.Output("benchmarks/coreclr")) |> directoryInfo
   let private benchmarkOutput = combinePaths testsProjectDirectory "BenchmarkDotNet.Artifacts"
   
   let private copyToOutput runtime file =
        match runtime with
        | Tooling.DotNetRuntime.Desktop -> 
            CopyFile clrOutput.FullName file
        | Tooling.DotNetRuntime.Core -> 
            CopyFile coreClrOutput.FullName file
        | _ -> ()

   let Run() =
        ensureDirExists clrOutput
        ensureDirExists coreClrOutput

        let sourceDirs = Directory.EnumerateDirectories Paths.SourceFolder 
                         |> Seq.toList

        // running benchmarks can timeout so clean up any generated benchmark files
        try
            for runtime in [Tooling.DotNetRuntime.Desktop; Tooling.DotNetRuntime.Core] do
                try
                    // The way that BenchmarkDotNet references the project that contains the benchmark from within
                    // the generated isolated benchmark code, is by relying on the dnx project referencing, which means
                    // that the generated code must live in ~/src/{Generated Test Dir}. It makes an assumption that dnx/dotnet
                    // is being run from the project directory that contains the benchmarks.
                    Tooling.Dnx.Exec runtime failure testsProjectDirectory ["--project"; Paths.Quote(testsProjectDirectory); "benchmark"]
                finally
                    // Clr (dnx451) and CoreClr (dnxcore50) output files in different locations.
                    // Might be because former uses 0.9.5.0 and latter, 0.9.4.0
                    let benchmarkOutputFiles = 
                        match runtime with
                        | Tooling.DotNetRuntime.Desktop -> 
                            Directory.EnumerateFiles(benchmarkOutput, "*.*", SearchOption.AllDirectories) 
                            |> Seq.toList
                        | Tooling.DotNetRuntime.Core -> 
                            Directory.EnumerateFiles(testsProjectDirectory, "*-report*.*", SearchOption.TopDirectoryOnly)
                                .Union(Directory.EnumerateFiles(testsProjectDirectory, "*.log", SearchOption.TopDirectoryOnly))
                                |> Seq.toList
                        | _ -> list.Empty
            
                    for file in benchmarkOutputFiles do copyToOutput runtime file
                    DeleteFiles benchmarkOutputFiles
        finally
            DeleteDir benchmarkOutput
            Directory.EnumerateDirectories Paths.SourceFolder 
            |> Seq.toList
            |> Seq.except sourceDirs
            |> Seq.iter DeleteDir

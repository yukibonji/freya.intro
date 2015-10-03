(**
- title : Relax and Let the HTTP Machine do the Work
- description : A functional-first web stack in F#
- author : Andrew Cherry (@kolektiv) and Ryan Riley (@panesofglass)
- theme : night
- transition : default

***

*)

(*** hide ***)
#I "../packages"
#r "Aether/lib/net40/Aether.dll"
#r "Hekate/lib/net40/Hekate.dll"
#r "FParsec/lib/net40-client/FParsecCS.dll"
#r "FParsec/lib/net40-client/FParsec.dll"
#r "Chiron/lib/net45/Chiron.dll"
#r "Owin/lib/net40/owin.dll"
#r "Arachne.Core/lib/net45/Arachne.Core.dll"
#r "Arachne.Language/lib/net45/Arachne.Language.dll"
#r "Arachne.Uri/lib/net45/Arachne.Uri.dll"
#r "Arachne.Uri.Template/lib/net45/Arachne.Uri.Template.dll"
#r "Arachne.Http/lib/net45/Arachne.Http.dll"
#r "Arachne.Http.Cors/lib/net45/Arachne.Http.Cors.dll"
#r "Freya.Core/lib/net45/Freya.Core.dll"
#r "Freya.Recorder/lib/net45/Freya.Recorder.dll"
#r "Freya.Lenses.Http/lib/net45/Freya.Lenses.Http.dll"
#r "Freya.Lenses.Http.Cors/lib/net45/Freya.Lenses.Http.Cors.dll"
#r "Freya.Machine/lib/net45/Freya.Machine.dll"
#r "Freya.Machine.Extensions.Http/lib/net45/Freya.Machine.Extensions.Http.dll"
#r "Freya.Machine.Extensions.Http.Cors/lib/net45/Freya.Machine.Extensions.Http.Cors.dll"
#r "Freya.Router/lib/net45/Freya.Router.dll"
#r "Freya.Machine.Router/lib/net45/Freya.Machine.Router.dll"
#r "Unquote/lib/net40/Unquote.dll"

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Owin
open Freya.Core
open Freya.Core.Operators
open Freya.Core.Integration
open Freya.Lenses.Http
open Freya.Machine
open Freya.Router
open Freya.Machine.Router
open Swensen.Unquote

(**

# Relax and let the HTTP Machine do the Work

## [Andrew Cherry](https://twitter.com/kolektiv) and [Ryan Riley](https://twitter.com/panesofglass)

***

<img alt="Tachyus logo" src="images/tachyus.png" style="background-color:#fff;" />

***

# [@panesofglass](https://twitter.com/panesofglass)

***

## https://github.com/panesofglass

***

# [OWIN](http://owin.org/)

***

![F# logo](images/fssf.png)

***

# Contents

* Problems
* Solution
* Freya
* Examples
* Questions

***

# HTTP is Hard

***

<img src="images/http-state-diagram.png" alt="HTTP State Machine" />

***

## What's wrong with existing solutions?

***

## Weak support for most of HTTP

***

## Little to no enforcement of the HTTP RFCs

***

## Manual protocol manipulation

***

## Team disagreements ...

***

## ... and bloodshed

![There Will Be Blood](images/there-will-be-blood.png)

***

## Prior Attempts (in F#)

* [Frank](https://github.com/frank-fs/frank)
* [Dyfrig](https://github.com/panesofglass/dyfrig)
* [Taliesin](https://github.com/frank-fs/taliesin)
* [Frost](https://github.com/kolektiv/frost)

***

# A Better Solution

***

## Machine-style Frameworks

***

## Modeled as a graph

***

![Freya visual debugging](images/graph.png)

***

## Declarative config via overrides

***

## Resource = defaults + overrides

***

> After all, if you're working against an enigma, you don't stand a chance without a machine.

- [Andrei Neculau](http://hyperrest.github.io/2015-03-15-API-andropause/#content)

***

## [Webmachine](https://github.com/basho/webmachine/wiki)

***

## [Liberator](http://clojure-liberator.github.io/liberator/)

***

# Freya

## A functional web stack in F#

<img src="images/freya.svg" alt="Freya logo" height="300px" width="300px" style="background-color:#fff;" />

***

# Architectural Principles

***

## Stack rather than monolithic framework

***

![Freya stack](images/freya-stack.svg)

***

## Building blocks for higher-level abstractions

***

## Compat with external libraries

***

# "Ethical" Principles

***

## Work with existing abstractions

(where possible)

***

## Enforce the pit of success

***

## Leverage F#; eliminate the wrong thing

***

# Static File Server

***

# [Todo Backend](http://todobackend.com/)

***

## Next for Freya

* Hypermedia
* Inspectors
* Documentation - http://docs.freya.io/

***

# Questions?

***

# Freya Stack

A Tour

***

## `Freya.Core`

* Basic abstractions over OWIN
* `freya` computation expression (the wrapping abstraction for our mutable state -- we will never speak of this in polite conversation)
* Set of operators to use `freya` computation expressions in a more concise way
* Some basic functions and **lenses** to give access to the state in a controllable way

***
*)

(*** include: def-freya ***)

(**

Roughly equivalent to Erlang's webmachine signature:

```
f(ReqData, State) ->
    {RetV, ReqData, State}.
```

***
*)

(*** include: def-freyastate ***)

(**
***

## Lenses?

*)

(*** include: lenses ***)


(**
***

## OWIN Integration

*)

(*** include: def-integration ***)

(**
***
## Use OWIN in Freya
*)

(*** include: integration ***)

(**
***
## Convert Freya to OWIN
*)

(*** include: freya-to-owin ***)

(**
***

## `Freya.Pipeline`

* Very small and simple -- all about composing `freya` computations in a way that represents a continue/halt processing pipeline
* A pipeline is simply a `freya` computation that returns `Next` or `Halt` (`FreyaPipelineChoice` cases)
* Single, simple operator: `>?=`

***
*)

(*** include: pipeline ***)

(**
***

## `Freya.Recorder`

* Build introspection into the framework at a low level
* Provide some infrastructure for recording metadata about processing that more specific implemenations can use
* For example, `Freya.Machine` records the execution process so it can be examined later

***

## `Freya.Types.*`

* Set of libraries providing F# types which map (very closely) to various specifications, such as HTTP, URI, LanguageTag, etc.
* These are used throughout the higher level stack projects
* Always favor strongly-typed representations of data over strings
* Provides parsers, formatters (statically on the types) and lenses from state to that type (either total or partial)

***

## Really?

* Why not use `System.Net.Whatever`?
* Well ...

***

![Ask the UriKind. One. More. Time.](images/ask-the-urikind.png)

***

## `Freya.Types.*`

* Types and parsers for when you don't already know everything about the string you've been given
* Types which map closely to HTTP specifications
* Types which can distinguish between different kinds of URIs being valid in different places
* Types which can actually express languages that aren't "en-US"
* ("hy-Latn-IT-arevela"? Of course we support Aremenian with a Latin script as spoken in Northern Italy why do you ask?)

***

## Integration with existing standards

## [OWIN](http://owin.org/)

```
Func<IDictionary<string, obj>, Task>
```

* Standard contract between servers and apps/frameworks
* Several server implementations, including IIS
* Reasonably well followed standard

***

## `Freya.Router`

* A simple, trie-based router, does pretty much what you'd expect
* Doesn't try and do anything but route requests to pipelines
* (and is itself a pipeline -- everything's composable / nestable!)

***

## `Freya.Machine`

* A "machine" style resource definition / processing library
* Inspired by projects like webmachine (Erlang) and Liberator (Clojure)
* Adds types

***

## `Freya.Inspector`

* Built-in introspection
* Has an extensibility model (WIP)
* Right now provides an API; UI in-progress

***

## `Freya.*.Inspector`

* Component-specific extensions to the inspector, currently providing component-specific JSON for the inspection API
* Will provide UI extensions, too, but haven't decided on the best approach to that (suggestions welcome, of course)

*)

(*** hide ***)
let env () =
    let e = Dictionary<string, obj> () :> IDictionary<string, obj>
    e.["o1"] <- false
    e.["o2"] <- false
    e

let appendString str = function
    | Some s -> sprintf "%s,%s" s str
    | None   -> str

let freyaState () =
    { Environment = env()
      Meta =
        { Memos = Map.empty } }

let invoke (composed: OwinAppFunc) =
    let e = env()
    composed.Invoke(e).ContinueWith<unit>(fun _ -> ())
    |> Async.AwaitTask
    |> Async.RunSynchronously
    e

let run m =
    Async.RunSynchronously (m (freyaState ()))

let answer_ =
    Environment.Required_ "Answer"

(*** define: lenses ***)
let ``getLM, setLM, modLM behave correctly`` () =
    let m =
        freya {
            do! Freya.setLens answer_ 42
            let! v1 = Freya.getLens answer_

            do! Freya.mapLens answer_ ((*) 2)
            let! v2 = Freya.getLens answer_

            return v1, v2 }

    let result = run m

    fst result =! (42, 84)

(*** define: integration ***)
let ``freya computation can compose with an OwinAppFunc`` () =
    let app =
        OwinAppFunc(fun (env: OwinEnvironment) ->
            env.["Answer"] <- 42
            Task.FromResult<obj>(null) :> Task)

    let converted = OwinAppFunc.toFreya app

    let m =
        freya {
            do! converted
            let! v1 = Freya.getLens answer_
            return v1 }
    
    let result = run m
    fst result =! 42

(*** define: freya-to-owin ***)
let ``freya computation can roundtrip to and from OwinAppFunc`` () =
    let app = Freya.setLens answer_ 42

    let converted =
        app
        |> OwinAppFunc.ofFreya
        |> OwinAppFunc.toFreya

    let m =
        freya {
            do! converted
            let! v1 = Freya.getLens answer_
            return v1 }
    
    let result = run m
    fst result =! 42

(*** define: pipeline ***)
let ``pipeline executes both monads if first returns next`` () =
    let o1 = Freya.mapState (fun x -> x.Environment.["o1"] <- true; x) *> Freya.next
    let o2 = Freya.mapState (fun x -> x.Environment.["o2"] <- true; x) *> Freya.next

    let choice, env = run (o1 >?= o2)

    choice =! Next
    unbox env.Environment.["o1"] =! true
    unbox env.Environment.["o2"] =! true

let ``pipeline executes only the first monad if first halts`` () =
    let o1 = Freya.mapState (fun x -> x.Environment.["o1"] <- true; x) *> Freya.halt
    let o2 = Freya.mapState (fun x -> x.Environment.["o2"] <- true; x) *> Freya.next

    let choice, env = run (o1 >?= o2)

    choice =! Halt
    unbox env.Environment.["o1"] =! true
    unbox env.Environment.["o2"] =! false

(*** define: def-freya ***)
type Freya<'T> =
    FreyaState -> Async<'T * FreyaState>

(*** define: def-freyastate ***)
type FreyaState =
    { Environment: FreyaEnvironment
      Meta: FreyaMetaState }

    static member internal EnvironmentLens =
        (fun x -> x.Environment), 
        (fun e x -> { x with Environment = e })

    static member internal MetaLens =
        (fun x -> x.Meta), 
        (fun m x -> { x with Meta = m })

and FreyaMetaState =
    { Memos: Map<Guid, obj> }

    static member internal MemosLens =
        (fun x -> x.Memos),
        (fun m x -> { x with Memos = m })

(*** define: def-integration ***)
/// Type alias of <see cref="FreyaEnvironment" /> in terms of OWIN.
type OwinEnvironment =
    FreyaEnvironment

/// Type alias for the F# equivalent of the OWIN AppFunc signature.
type OwinApp = 
    OwinEnvironment -> Async<unit>

/// Type alias for the OWIN AppFunc signature.
type OwinAppFunc = 
    Func<OwinEnvironment, Task>

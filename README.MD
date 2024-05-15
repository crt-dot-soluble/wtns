# WTNS

Pronounced like 'witness'

## Overview

`wtns` at its core is a framework for creating, and interfacing with, both public and private networkse with emphasis on abstracting complex topics into a more accessible and unified codebase.

The `wtns` solution houses three projects, a single core library and two implementatations: a graphical UI and a command line interface. These projects are found in the directories of the same name at the root of the repository.

## Projects

- **lib** : WTNS core library containing all meaningful code.
  - `WTNS.Me.Lib`

- **cli** : The command line interface - a tool for accessing WTNS functions via the command line.
  - `WTNS.Me.Cli`

- **gui** : The graphical UI - a graphical user interface providing the same features as the `WTNS.Me.Cli` and then some.
  - `WTNS.Me.Gui`

The namespaces these projects contain are expanded upon below.

## Legend


🟢 : The current feature set has been completed.

🟡 : In progress

🔴 : Not started / Not functional

---

### lib

Contains the core functionality of its implementations. All executable projects depend on `WTNS.Me.Lib`. The current existing namespaces can be found in the table below. 



| **Namspace**             | X Platform  | Windows  | Linux    | Mac      |
|------------------------- |:-----------:|:--------:|:--------:|:--------:|
| WTNS.Me                  | 🟡          | ➖       | ➖       | ➖       |
| WTNS.Me.Net              | 🟡          | ➖       | ➖       | ➖       |
| WTNS.Me.Commands         | 🟡          | ➖       | ➖       | ➖       |
| WTNS.Me.Telemetry        | ➖          | 🟡       | 🔴       | 🔴       |

### cli

| **Namspace** | X Platform    | Windows       | Linux         | Mac           |
|--------------|:-------------:|:-------------:|:-------------:|:-------------:|
| WTNS.Me.CLI         | 🟡            | ➖            | ➖            | ➖           |
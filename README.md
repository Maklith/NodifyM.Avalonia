# NodifyM.Avalonia
[![NuGet](https://img.shields.io/nuget/v/NodifyM.Avalonia?style=for-the-badge&logo=nuget&label=release)](https://www.nuget.org/packages/NodifyM.Avalonia/)
[![NuGet](https://img.shields.io/nuget/dt/NodifyM.Avalonia?label=downloads&style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/NodifyM.Avalonia)
[![License](https://img.shields.io/github/license/miroiu/nodify?style=for-the-badge)](https://github.com/miroiu/nodify/blob/master/LICENSE)

A collection of controls for node based editors designed for MVVM.
## About
This project is a refactoring of [Nodify](https://github.com/miroiu/nodify) on the Avalonia platform and is not a 1:1 replica of Nodify, but they have many similarities.
![image](https://raw.githubusercontent.com/MakesYT/NodifyM.Avalonia/master/assets/Kitopia1706877412160.png)
![image](https://raw.githubusercontent.com/MakesYT/NodifyM.Avalonia/master/assets/Kitopia1706877401219.png)

## Features
 - Designed from the start to work with **MVVM**
 - Built-in dark and light **themes**
 - **Selecting**， **zooming**， **panning**
 - Select， move, **_auto align_**, **auto panning** when close to edge and connect nodes
### What are the differences compared to Nodify
 - **Supports** 
   - auto align Node
   - display text on Connection
   - Select multiple nodes
 - **Nonsupport**
     - Minimap
 - **Will be supported in the future**
   - 
## Usage
### NodifyEditor
 - `Press` and `Hold` -> Move the all show items
 -  Mouse wheel -> Zoom all show items
- Hold `Ctrl` and `Press Move` -> Box select Nodes
### Node

- `Press Move` and `Hold` -> Move the Node
 - `Press Move` and `Hold Shift` -> Move the Node(without automatically align)
 - `Press` the Node -> Select the Node
- Hold `Ctrl` and `Click` -> Select or unselect Node
- Hold `Ctrl` and `Press Move` on the selected Node -> Move all selected Nodes
### Connection
 - `Press` and `Hold` the Connector and move to another Connector -> Create a new connection
 - Hold `Alt` and `Click` Connection -> Remove Connection
 - `DoubleClick` Connection -> Split the connection in the double-click position
### PendingConnection
 - `Press` and `Hold` the Connector -> Show connection preview
### Connector
 - Hold `Alt` and `Click` Connector -> Remove all the Connections on the Connector

## Notice
1. ViewModelBase is for reference only, you don't need to use it, there are binding methods in Example's AXAML code.

### Some known problems
1. Do not use `Mode=OneWayToSource` when you have multiple Nodify bound to the same ViewModel, this is a bug from [Avalonia](https://github.com/AvaloniaUI/Avalonia/issues/4438)

## Example
#### please see the [NodifyM.Avalonia.Example](https://github.com/MakesYT/NodifyM.Avalonia/tree/master/NodifyM.Avalonia.Example)
#### You can git clone the project and run `NodifyM.Avalonia.Example`

## Changelog

### 1.1.0

- Support box selection and multiple selection
- Fixed memory leaks
### 1.0.18

- Allow node to be used outside the `NodifyEditor` (Just for display)

### 1.0.17

- Fixed event handling to avoid duplicate processing of handled pointer events
### 1.0.16
- Optimization No longer forces connector type
- Fix If the connector is a Combobox can't click to expand it

### 1.0.15

- Fix OnPointerPressed event handler

### 1.0.14

- Remove unnecessary packages
### 1.0.13
- Fixed SelectedNode Property
- Added the ability to select and drag the Node corresponding to the Connector
### 1.0.12
- Fixed the adaptive node layout calculation error
- Added node centerline alignment
### 1.0.11
- Added automatic adaptation to display all Nodes when NodifyEditor is initialized
- Optimize child node search
### 1.0.10
- Fixed Node Header/Input/OutputTemplate allow use IDataTemplate
- Added Avalonia.Diagnostics Condition
### 1.0.9
- Allows not to use the built-in ViewModelBase
- Fixed ViewTranslateTransform and AlignmentLine exceptions when multiple NodifyEditor
### 1.0.8
- Added Light and Dark themes follow the Avalonia toggle
- Fixed invalid Connection Text Brush modifications
- Optimize the Dark theme color
- Added GridLine color definition
### 1.0.7
- Added Node **auto panning** when close to edge
### 1.0.6
- Fixed Node IsSelected property
- Fixed Node BorderBrush Style
- Added Node Alignment hint
### 1.0.5
 - Added the ability to temporarily without automatically align Node while holding Shift
 - Added the ability to display text on Connection
### 1.0.4
 - Add align Node configuration properties
 - Add Node automatic alignment
### 1.0.3
 - Added the connection SplitConnection and DisconnectConnection commands
 - Add CircuitConnection
 - Fixed default control color to dictionary color
 - Support to override the Connect and Disconnect from NodifyEditorViewModelBase method
 - Fix KnotNode Show
 - Remove some useless attributes
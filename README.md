# NodifyM.Avalonia
[![NuGet](https://img.shields.io/nuget/v/NodifyM.Avalonia?style=for-the-badge&logo=nuget&label=release)](https://www.nuget.org/packages/NodifyM.Avalonia/)
[![NuGet](https://img.shields.io/nuget/dt/NodifyM.Avalonia?label=downloads&style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/NodifyM.Avalonia)
[![License](https://img.shields.io/github/license/Maklith/NodifyM.Avalonia?style=for-the-badge)](https://github.com/Maklith/NodifyM.Avalonia/blob/master/LICENSE)

A collection of controls for node based editors designed for MVVM.
## About
This project is a refactoring of [Nodify](https://github.com/miroiu/nodify) on the Avalonia platform and is not a 1:1 replica of Nodify, but they have many similarities.
<img width="2859" height="1520" alt="image" src="https://github.com/user-attachments/assets/eb8f0cf3-d93c-4adc-a2ef-75657a8deb81" />

<img width="2859" height="1525" alt="image" src="https://github.com/user-attachments/assets/2ab8810a-01a5-47c8-bacb-a8d0f5f27f03" />


## Features
 - Designed from the start to work with **MVVM**
 - Built-in dark and light **themes**
 - **Selecting**， **zooming**， **panning**
 - Select， move, **_auto align_**, **auto panning** when close to edge and connect nodes
 - AotCompatible
### What are the differences compared to Nodify
 - **Supports** 
   - auto align Node
   - display text on Connection
   - Select multiple nodes
   - Minimap
 - **Nonsupport**
 - 
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
1. `BaseNodeViewModel` is provided as a reference only. You do not need to use it.
2. For binding examples, see the AXAML files in `NodifyM.Avalonia.Example` or [Another Example which without `BaseNodeViewModel`](https://github.com/Maklith/Kitopia/blob/c7a1be3ee147befa9059e0a6d9e4e6915d1af9fd/KitopiaAvalonia/Windows/TaskEditors/TaskEditor.axaml#L351).
3. To support node positioning, implement the `INodePosition` interface in your view model.


| Avalonia version | NodifyM.Avalonia version | Note |
|---|--------------------------|---|
| Avalonia 11 | <= 1.1.9                 | Use 1.1.9 or earlier |
| Avalonia 12 | \>= 12.0.0               | Recommended: latest stable |


### Some known problems
1. Do not use `Mode=OneWayToSource` when multiple Nodify controls are bound to the same view model. This is caused by an Avalonia issue: [#4438](https://github.com/AvaloniaUI/Avalonia/issues/4438).

## Example
#### please see the [NodifyM.Avalonia.Example](https://github.com/MakesYT/NodifyM.Avalonia/tree/master/NodifyM.Avalonia.Example)
#### You can git clone the project and run `NodifyM.Avalonia.Example`

## Changelog

### 12.0.0
- decouple editor node positioning style from BaseNodeViewModel for AOT-safe bindings

### 1.3.0
- Now is AotCompatible
- In 1.2.1 due to an issue with Avalonia, the connector could not be hit correctly, and has now been upgraded to 12.0.2
- Fix all warning messages

### 1.2.1
- Add Minimap control for `NodifyEditor`

### 1.2.0
- Support for Avalonia 12

### 1.1.9
- Allow to modify node header and footer ContentPresenter style 

### 1.1.8
- Fixed unselect nodes not working when clicking with `Ctrl` key pressed

### 1.1.7
- Prevent auto-panning when not dragging a node

### 1.1.6
- Implement dragging the corner of a single node to zoom

### 1.1.5
- Fixed an incorrect zoom center compensation calculation

### 1.1.4
- Add binding support for NodifyEditor.SelectedItems
- Change Property from DataTemplate to IDataTemplate interface.

### 1.1.3
- Fixed NuGet contains a reference to Avalonia.Diagnostics

### 1.1.2
- Refactor NodifyEditor to improve transform handling (#13)

### 1.1.1

- Fix Zoom property only binds one-way
- Add ZoomCenter property
### 1.1.0

- Support box selection and multiple selection
- Fixed memory leaks

### ....

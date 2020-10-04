# SimpleSelectionExport
[![Github All Releases](https://img.shields.io/github/downloads/markjszy/SimpleSelectionExport/total)](https://github.com/markjszy/SimpleSelectionExport/releases)
[![License](https://img.shields.io/github/license/markjszy/SimpleSelectionExport)](https://github.com/markjszy/SimpleSelectionExport/blob/master/LICENSE)

SimpleSelectionExport is a [KeePass2](https://keepass.info) plugin which allows you to export basic entry 
information to flat text files. It's useful when you want to send a quick overview of an arbitrary set of 
entries -- selected from the main window -- to parties who are not using KeePass.

![Screenshot](screenshot.png)

## Installation Instructions

Note: Builds target .NET Framework 4.5+.

Unzip the release and move both DLLs to your KeePass plugins directory, as you would with any other KeePass plugin.

There are two DLLs since SimpleSelectionExport depends upon the [CsvHelper](https://github.com/JoshClose/CsvHelper) library.
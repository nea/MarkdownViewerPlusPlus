# MarkdownViewerPlusPlus
A Notepad++ Plugin to view a Markdown file rendered on-the-fly

## Features
* Dockable panel (toggle) with a rendered HTML of the currently selected file/tab
* CommonMark compliant ([0.27][4])
* Links open in a separate Browser
* Basic HTML Export
* Basic PDF Export
* Unicode Notepad++ 32-bit plugin

### Planned
* Highlight the current cursor row
* Export as PDF with properties/templates
* Synchronizing scroll between Markdown file and rendered view
* ILMerge support
* x64 support
* Improved performance with large documents
* BugFixes ^^'

## Latest Versions
* 0.2.2
  * Changed *GetText* method to support growing text lengths (lead to crashes)
* 0.2.1
  * Updated CommonMark to version 0.15
* 0.2.0
  * Changed the rendered view from a WinForms.WebBrowser to the [HTMLRenderer][6].HtmlPanel
  * Fixed an issue of the toolbar icon not toggling correctly via 'x' and multiple docked panels
  
Download the latest [release here][9].

## Compatibility
This plugin requires 
* Notepad++ 32-bit (won't work with 64-bit)
* Windows

It has been tested under the following conditions
* Notepad++ 7.2.2 (32-bit)
* Windows 10 Professional (64-bit)

## Installation
Download a release version and copy the included **MarkdownViewerPlusPlus.dll** to the *plugins* sub-folder at your Notepad++ installation directory. The plugin adds a small Markdown icon ![Markdown icon](https://github.com/nea/MarkdownViewerPlusPlus/raw/master/MarkdownViewerPlusPlus/Resources/markdown-16x16-solid.png) to the toolbar to toggle the viewer as dockable panel.

## License and Credits
The MarkdownViewerPlusPlus is released under the MIT license.

This Notepad++ plugin integrates the sources of multiple other libraries, because of issues with the library merging process. Credits and thanks to all the developers working on these great projects:
* The plugin is based on the [Notepad++ PluginPack.net][2] by kbilsted provided under the Apache-2.0 license.
* The renderer uses the [CommonMark.NET][3] library by Knagis provided under the BSD-3-Clause license.
* The PDF Exporter uses 
  * [PDFSharp][5] by empira Software GmbH and provided under the MIT license
  * [HTMLRenderer][6] by ArthurHub provided under the BSD-3-Clause license
* The menu icons are by [FontAwesome][7] provided under the SIL OFL 1.1 license
* The Markdown icon is by [dcurtis][8] provided under the CC0-1.0 license

## Disclaimer
This source and the whole package comes without warranty. It may or may not harm your computer or cell phone. Please use with care. Any damage cannot be related back to the author. The source has been tested on a virtual environment and scanned for viruses and has passed all tests.

## Personal Note
*I don't know if this is very useful for a lot of people but I wanted something in private to quickly write and see some formatted Markdown documents. As I was not able to find something similar very quickly I created this project. I hope this proves useful to you... with all its Bugs and Issues ;) If you like it you can give me a shout at [INsanityDesign][1] or let me know via this repository.*


  [1]: http://www.insanitydesign.com/wp/
  [2]: https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
  [3]: https://github.com/Knagis/CommonMark.NET
  [4]: http://spec.commonmark.org/
  [5]: http://www.pdfsharp.net/
  [6]: https://htmlrenderer.codeplex.com/
  [7]: http://fontawesome.io/
  [8]: https://github.com/dcurtis/markdown-mark
  [9]: https://github.com/nea/MarkdownViewerPlusPlus/releases
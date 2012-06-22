BreakAll for VisualStudio
=========================

Overview
--------

A Visual Studio 2010 add-in to set a class breakpoint: a breakpoint in every access to the class.

Known issues and limitations
----------------------------

* Visual Studio must be opened before any solution. When double clicking an sln file the command is not loaded.
* Fails to set a breakpoint when the first line on a method is not allowed to have a breakpoint.
* Doesn't work on earlier versions of Visual Studio and it wasn't checked with versions in languages other than English.

Download
--------

- [Version 0.1 (453.5 KB)](https://github.com/alfonsocora/vs-breakall/raw/master/Installer.msi)

Screenshot
----------

![Alt desc](https://github.com/alfonsocora/files/raw/master/images/vs-breakall.png)

License
-------

(The MIT License)

Copyright (c) 2012 Alfonso Cora

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Thanks
------

Thanks to kosher for his contribution on error handling.

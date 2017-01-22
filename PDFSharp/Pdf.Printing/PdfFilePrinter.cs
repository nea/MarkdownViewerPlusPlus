#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

// Removed

#if true_
namespace PdfSharp.Pdf.Printing
{
    // Some googled inforamtion about command line switches:
    // 
    // AcroRd32.exe filename                                   Executes the reader and displays a file.
    // AcroRd32.exe /p filename                                Executes the reader and prints a file.
    // AcroRd32.exe /t path printername drivername portname    Executes the reader and prints a file
    //                                                         while suppressing the Acrobat print
    //                                                         dialog box, then terminating the Reader.
    //
    // The four parameters of the /t option evaluate to strings.
    // printername     The name of the Printer.
    // drivername      Your printer drivers name i.e. whatever apperars in the Driver Used box when viewing printer properties.
    // portname        The printers port. portname cannot contain any "/" characters; if it does, output is routed to
    //                 the default port for that printer.
    //
    //                                
    // Acrobat.exe /n    Launch a separate instance of the Acrobat application
    // Acrobat.exe /s    Open Acrobat suppressing the splash screen
    // Acrobat.exe /o    Open Acrobat suppressing the openfile dialog
    // Acrobat.exe /h    Open Acrobat in hidden mode


    /// <summary>
    /// A wrapper around Adobe Reader or Adobe Acrobat that helps to print PDF files.
    /// The property AdobeReaderPath must be set before the class can be used for printing.
    /// The class was tested with Adobe Reader 7.0.7.
    /// If this stuff does not work, <c>please</c> don't write me mails!
    /// If you enhance this class, please let me know.
    /// </summary>
    public class PdfFilePrinter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFilePrinter"/> class.
        /// </summary>
        public PdfFilePrinter()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFilePrinter"/> class.
        /// </summary>
        /// <param name="pdfFileName">Name of the PDF file.</param>
        public PdfFilePrinter(string pdfFileName)
        {
            PdfFileName = pdfFileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFilePrinter"/> class.
        /// </summary>
        /// <param name="pdfFileName">Name of the PDF file.</param>
        /// <param name="printerName">Name of the printer.</param>
        public PdfFilePrinter(string pdfFileName, string printerName)
        {
            _pdfFileName = pdfFileName;
            _printerName = printerName;
        }

        /// <summary>
        /// Gets or sets the name of the PDF file to print.
        /// </summary>
        public string PdfFileName
        {
            get { return _pdfFileName; }
            set { _pdfFileName = value; }
        }
        string _pdfFileName;

        /// <summary>
        /// Gets or sets the name of the printer. A typical name looks like '\\myserver\HP LaserJet PCL5'.
        /// </summary>
        /// <value>The name of the printer.</value>
        public string PrinterName
        {
            get { return _printerName; }
            set { _printerName = value; }
        }
        string _printerName;

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }
        string _workingDirectory;

        /// <summary>
        /// Prints the PDF file.
        /// </summary>
        public void Print()
        {
            Print(-1);
        }

        /// <summary>
        /// Prints the PDF file.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to wait for completing the print job.</param>
        public void Print(int milliseconds)
        {
            if (String.IsNullOrEmpty(_printerName))
                _printerName = _defaultPrinterName;

            if (String.IsNullOrEmpty(_adobeReaderPath))
                throw new InvalidOperationException("No full qualified path to AcroRd32.exe or Acrobat.exe is set.");

            if (String.IsNullOrEmpty(_printerName))
                throw new InvalidOperationException("No printer name set.");

            // Check whether file exists.
            string fqName;
            if (!string.IsNullOrEmpty(_workingDirectory))
                fqName = Path.Combine(_workingDirectory, _pdfFileName);
            else
                fqName = Path.Combine(Directory.GetCurrentDirectory(), _pdfFileName);
            if (!File.Exists(fqName))
                throw new InvalidOperationException(String.Format("The file {0} does not exist.", fqName));

            // TODO: Check whether printer exists.

            try
            {
                DoSomeVeryDirtyHacksToMakeItWork();

                //acrord32.exe /t          <- seems to work best
                //acrord32.exe /h /p       <- some swear by this version
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = _adobeReaderPath;
                string args = String.Format("/t \"{0}\" \"{1}\"", _pdfFileName, _printerName);
                //Debug.WriteLine(args);
                startInfo.Arguments = args;
                startInfo.CreateNoWindow = true;
                startInfo.ErrorDialog = false;
                startInfo.UseShellExecute = false;
                if (!String.IsNullOrEmpty(_workingDirectory))
                    startInfo.WorkingDirectory = _workingDirectory;

                Process process = Process.Start(startInfo);
                if (!process.WaitForExit(milliseconds))
                {
                    // Kill Adobe Reader/Acrobat
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                // ReSharper disable PossibleIntendedRethrow
                throw ex;
                // ReSharper restore PossibleIntendedRethrow
            }
        }

        /// <summary>
        /// For reasons only Adobe knows the Reader seams to open and shows the document instead of printing it
        /// when it was not already running.
        /// If you use PDFsharp and have any suggestions to circumvent this function, please let us know.
        /// </summary>
        void DoSomeVeryDirtyHacksToMakeItWork()
        {
            if (_runningAcro != null)
            {
                if (!_runningAcro.HasExited)
                    return;
                _runningAcro.Dispose();
                _runningAcro = null;
            }
            // Is any Adobe Reader/Acrobat running?
            Process[] processes = Process.GetProcesses();
            int count = processes.Length;
            for (int idx = 0; idx < count; idx++)
            {
                try
                {
                    Process process = processes[idx];
                    ProcessModule module = process.MainModule;

                    if (String.Compare(Path.GetFileName(module.FileName), Path.GetFileName(_adobeReaderPath), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Yes: Fine, we can print.
                        _runningAcro = process;
                        break;
                    }
                }
                catch { }
            }
            if (_runningAcro == null)
            {
                // No: Start an Adobe Reader/Acrobat.
                // If you are within ASP.NET, good luck...
                _runningAcro = Process.Start(_adobeReaderPath);
                if (_runningAcro != null)
                    _runningAcro.WaitForInputIdle();
            }
        }
        static Process _runningAcro;

        /// <summary>
        /// Gets or sets the Adobe Reader or Adobe Acrobat path.
        /// A typical name looks like 'C:\Program Files\Adobe\Adobe Reader 7.0\AcroRd32.exe'.
        /// </summary>
        static public string AdobeReaderPath
        {
            get { return _adobeReaderPath; }
            set { _adobeReaderPath = value; }
        }
        static string _adobeReaderPath;

        /// <summary>
        /// Gets or sets the name of the default printer. A typical name looks like '\\myserver\HP LaserJet PCL5'.
        /// </summary>
        static public string DefaultPrinterName
        {
            get { return _defaultPrinterName; }
            set { _defaultPrinterName = value; }
        }
        static string _defaultPrinterName;
    }
}
#endif
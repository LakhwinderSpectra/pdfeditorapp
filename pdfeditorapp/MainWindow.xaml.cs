using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using PdfiumViewer;
using System.Windows.Forms.Integration;
using iText.Kernel.Pdf;

namespace PdfEditorApp
{
    public partial class MainWindow : Window
    {
        private string _currentPdfPath;
        private PdfViewer _pdfViewer;
        private PdfiumViewer.PdfDocument _pdfDocument;

        public MainWindow()
        {
            InitializeComponent();
            InitializePdfViewer();
        }

        private void InitializePdfViewer()
        {
            _pdfViewer = new PdfViewer();
            pdfHost.Child = _pdfViewer;
        }
       //Upload Button integration:
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                _currentPdfPath = openFileDialog.FileName;
                LoadPdf(_currentPdfPath);
            }
        }

        //Rotate Button integration:

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPdfPath != null)
            {
                RotatePdf(_currentPdfPath);
                LoadPdf(_currentPdfPath);
            }
        }
        //Save Button integration:

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPdfPath != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(_currentPdfPath, saveFileDialog.FileName, true);
                    MessageBox.Show("PDF saved successfully.");
                }
            }
        }

        private void LoadPdf(string path)
        {
            _pdfDocument = PdfiumViewer.PdfDocument.Load(path);
            _pdfViewer.Document = _pdfDocument;
        }

        private void RotatePdf(string path)
        {
           
            _pdfViewer.Document?.Dispose();
            _pdfViewer.Document = null;

            string tempPath = Path.Combine(Path.GetDirectoryName(path), "temp.pdf");

            using (iText.Kernel.Pdf.PdfReader reader = new iText.Kernel.Pdf.PdfReader(path))
            using (iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(tempPath))
            using (iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader, writer))
            {
                int n = pdfDoc.GetNumberOfPages();
                for (int i = 1; i <= n; i++)
                {
                    iText.Kernel.Pdf.PdfPage page = pdfDoc.GetPage(i);
                    int rotation = (page.GetRotation() + 90) % 360;
                    page.SetRotation(rotation);
                }
            }

            File.Delete(path);
            File.Move(tempPath, path);
        }

    }
}

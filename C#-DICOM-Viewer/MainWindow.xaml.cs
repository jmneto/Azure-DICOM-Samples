// DICOM Viewer

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

using System.IO;
using Dicom;
using Dicom.Imaging;

namespace DICOMViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select a DICOM file
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "DICOM files (*.dcm)|*.dcm";
            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            // Load the DICOM file using fo-dicom
            var file = DicomFile.Open(openFileDialog.FileName);
            var image = new DicomImage(file.Dataset);

            // Create a bitmap from the DICOM image and display it in the Image control
            var bitmap = image.RenderImage().AsClonedBitmap();
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // Set the bitmap as the source for the Image Control
            ImageControl.Source = source;

        }
    }
}

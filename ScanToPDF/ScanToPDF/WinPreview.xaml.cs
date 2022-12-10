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
using System.Windows.Shapes;

namespace ScanToPDF
{
    /// <summary>
    /// Logica di interazione per WinPreview.xaml
    /// </summary>
    public partial class WinPreview : Window
    {
        public WinPreview(string path)
        {
            InitializeComponent();
            BitmapImage img = new BitmapImage(new Uri(path));
            double width = img.Width * 0.75;
            double height = img.Height * 0.75;

            imgPreview.Width = width;
            imgPreview.Height = height;
            imgPreview.Source = img;

            this.SizeToContent = SizeToContent.Width;
            this.Height = height + 90;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
using WIA;

namespace ScanToPDF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Scanner> scannerList = new List<Scanner>();
        private DeviceInfo currentDeviceInfo = null;

        public MainWindow()
        {
            InitializeComponent();

            DeviceManager deviceManager = new DeviceManager();

            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                    continue;

                Scanner scanner = new Scanner(deviceManager.DeviceInfos[i]);
                scannerList.Add(scanner);
                cmbScanners.Items.Add(scanner);

            }
        }

        private void cmbScanners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Scanner s = (Scanner)cmbScanners.SelectedItem;
            currentDeviceInfo = s.GetDeviceInfo();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            Device device = currentDeviceInfo.Connect();
            Item i = device.Items[1];

            ImageFile imgFile = (ImageFile)i.Transfer(FormatID.wiaFormatJPEG);

            string path = @"";

            imgFile.SaveFile(path);
        }
    }
}

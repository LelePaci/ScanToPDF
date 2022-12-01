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
        private DeviceManager deviceManager = new DeviceManager();
        private List<Scanner> scannerList = new List<Scanner>();
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine();

            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                    continue;

                scannerList.Add(new Scanner((string)deviceManager.DeviceInfos[i].Properties["Name"].get_Value(),
                    (string)deviceManager.DeviceInfos[i].Properties["Description"].get_Value(),
                    (string)deviceManager.DeviceInfos[i].Properties["Port"].get_Value()));
            }
            cmbScanners.ItemsSource = scannerList;
        }

        private void cmbScanners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = cmbScanners.SelectedIndex;
            String info = "Name: " + scannerList.ElementAt(index).getName() + "\n" +
                "Description: " + scannerList.ElementAt(index).getDescription() + "\n" +
                "Port: " + scannerList.ElementAt(index).getPort();
            Console.WriteLine(info);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private DeviceInfo currentDeviceInfo = null;

        private List<Scanner> scannerList = new List<Scanner>();

        private string scansDirectory = null;
        private string resultDirectory = null;
        private string format = "yyyyMMdd-HHmmss";

        public MainWindow()
        {
            InitializeComponent();
            LoadScanners();
            CheckFolders();
        }

        private void LoadScanners()
        {
            foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
            {
                if (deviceInfo.Type != WiaDeviceType.ScannerDeviceType) continue;

                Scanner scanner = new Scanner(deviceInfo);
                scannerList.Add(scanner);
                cmbScanners.Items.Add(scanner);
            }
        }

        private void CheckFolders()
        {
            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            resultDirectory = docsFolder + @"\ScanToPDF";
            scansDirectory = resultDirectory + @"\scans";
            Directory.CreateDirectory(scansDirectory);
        }

        private void cmbScanners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Scanner s = (Scanner)cmbScanners.SelectedItem;
            currentDeviceInfo = s.GetDeviceInfo();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Device device = currentDeviceInfo.Connect();
                Item i = device.Items[1];

                ImageFile imgFile = (ImageFile)i.Transfer(FormatID.wiaFormatJPEG);
                string path = scansDirectory + @"\scan" + DateTime.Now.ToString(format) + ".jpg";
                imgFile.SaveFile(path);
            }
            catch (COMException ex)
            {
                MessageBox.Show(getExceptionMessage((uint)ex.ErrorCode));
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Nessuno scanner selezionato");
            }
        }

        private String getExceptionMessage(uint errorCode)
        {
            string message = "";
            switch (errorCode)
            {
                case 0x80210006:
                    message = "Il dispositivo è occupato. Chiudere tutte le app che usano questo dispositivo o attendere il completamento e quindi riprovare.";
                    break;
                case 0x80210016:
                    message = "Una o più cover del dispositivo sono aperte.";
                    break;
                case 0x8021000A:
                    message = "La comunicazione con il dispositivo WIA non è riuscita. Assicurarsi che il dispositivo sia acceso e connesso al PC. Se il problema persiste, disconnettersi e riconnettere il dispositivo.";
                    break;
                case 0x8021000D:
                    message = "Il dispositivo è bloccato. Chiudere tutte le app che usano questo dispositivo o attendere il completamento e quindi riprovare.";
                    break;
                case 0x8021000E:
                    message = "Il driver di dispositivo ha generato un'eccezione.";
                    break;
                case 0x80210001:
                    message = "Si è verificato un errore sconosciuto con il dispositivo WIA";
                    break;
                case 0x8021000C:
                    message = "Nel dispositivo WIA è presente un'impostazione non corretta";
                    break;
                case 0x8021000B:
                    message = "Il dispositivo non supporta questo comando";
                    break;
                case 0x8021000F:
                    message = "La risposa del driver non è valida";
                    break;
                case 0x80210009:
                    message = "Il dispositivo WIA è stato eliminato. Non è più disponibile";
                    break;
                case 0x80210017:
                    message = "Si è verificato un errore di analisi a causa di una condizione di feed di pagine multipla.";
                    break;
                case 0x80210005:
                    message = "Il dispositivò è offline. Assicurarsi che il dispositivo sia acceso e connesso al PC";
                    break;
            }
            return message;
        }
    }
}

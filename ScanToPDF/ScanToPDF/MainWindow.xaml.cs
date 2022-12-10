using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
        // scanner
        private DeviceManager deviceManager = new DeviceManager();
        private DeviceInfo currentDeviceInfo = null;

        private List<Scanner> scannerList = new List<Scanner>();

        // informazioni per salvataggio delle immagini
        private string scansDirectory = null;
        private string resultDirectory = null;
        private string format = "yyyyMMdd-HHmmss";

        private PhotoElement currentPhotoElement = null;

        public MainWindow()
        {
            InitializeComponent();
            CheckEnabling();
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

        private void CheckEnabling()
        {
            btnScan.IsEnabled = false;
            listDocuments.IsEnabled = false;
            btnUp.IsEnabled = false;
            btnDown.IsEnabled = false;
            btnPreview.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnCreatePDF.IsEnabled = false;
        }
        private void cmbScanners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Scanner s = (Scanner)cmbScanners.SelectedItem;
            currentDeviceInfo = s.GetDeviceInfo();

            if (cmbScanners.SelectedIndex != -1)
            {
                btnScan.IsEnabled = true;
            }
            else
            {
                btnScan.IsEnabled = false;
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Connette il device scelto per eseguire la scansione
                Device device = currentDeviceInfo.Connect();
                Item item = device.Items[1];

                // Sistemo le proprietà dello scanner selezionato
                SettingsWia(item);

                // Ottiene un immagine in formato "jpg" dalla scansione
                ImageFile imgFile = (ImageFile)item.Transfer(FormatID.wiaFormatJPEG);

                // Salva l'immagine ottenuta 
                string name = "scan" + DateTime.Now.ToString(format) + ".jpg";
                string path = scansDirectory + @"\" +  name;
                imgFile.SaveFile(path);

                // Aggiunge le informazioni dell'immagine ad una ListBox
                PhotoElement pe = new PhotoElement(path, name);
                listDocuments.Items.Add(pe);

                new WinPreview(pe).Show();

                // Controllo che siano presenti degli item e in caso positivio abilito la ListBox e i pulsanti per rimuovere elementi
                // e per visualizzare le anteprime
                // Abilito anche il pulsante per creare il documento in PDF
                if (listDocuments.Items.Count > 0)
                {
                    listDocuments.IsEnabled = true;
                    btnCreatePDF.IsEnabled = true;
                }
                else
                {
                    listDocuments.IsEnabled = false;
                    btnCreatePDF.IsEnabled = false;
                }

                // Controllo che sia presente più di un elemento nella ListView e in caso positivo abilito i pulsanti per spostare gli oggetti
                if (listDocuments.Items.Count > 1)
                {
                    btnUp.IsEnabled = true;
                    btnDown.IsEnabled = true;
                }
                else
                {
                    btnUp.IsEnabled = false;
                    btnDown.IsEnabled = false;
                }

                //// Mostra l'immagine ottenuta all'interno di una Image
                //imgScan.Source = new BitmapImage(new Uri(path));

                //imgScan.Stretch = Stretch.Uniform;
            }
            catch (COMException ex)
            {
                MessageBox.Show(getExceptionMessage((uint)ex.ErrorCode));
                Console.WriteLine(ex.Message);
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
                default:
                    message = errorCode.ToString();
                    break;
            }
            return message;
        }

        private void SettingsWia(Item item)
        {
            item.Properties["4104"].set_Value(24);
            item.Properties["6146"].set_Value(1); // Modalià colore 1 -> colori , 2 -> grayscale
            item.Properties["6147"].set_Value(150); // DPI verticale
            item.Properties["6148"].set_Value(150); // DPI orizzontale
            item.Properties["6151"].set_Value(1243); // Larghezza della pagina
        }

        private void btnCreatePDF_Click(object sender, RoutedEventArgs e)
        {
            PdfDocument doc = new PdfDocument();
            foreach (PhotoElement pe in listDocuments.Items)
            {
                PdfPage page = doc.AddPage();

                XImage image = XImage.FromFile(pe.getPath());
                XGraphics graphics = XGraphics.FromPdfPage(page);
                graphics.DrawImage(image, 0, 0, page.Width, page.Height);
            }
            if (doc.PageCount > 0)
            {
                string name = "pdf" + DateTime.Now.ToString(format) + ".pdf";
                string path = resultDirectory + @"\" + name;
                doc.Save(path);
            }
        }

        private void listDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listDocuments.SelectedIndex != -1)
            {
                currentPhotoElement = (PhotoElement)listDocuments.SelectedItem;
                btnPreview.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            new WinPreview(currentPhotoElement).Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            listDocuments.Items.Remove(currentPhotoElement);
            listDocuments.SelectedItem = -1;

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            File.Delete(currentPhotoElement.getPath());

            btnPreview.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void toBlackWhite()
        {

        }

        private void cmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

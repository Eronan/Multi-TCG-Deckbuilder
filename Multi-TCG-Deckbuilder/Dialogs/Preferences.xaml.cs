using Multi_TCG_Deckbuilder.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Multi_TCG_Deckbuilder.Dialogs
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window, INotifyPropertyChanged
    {
        // Accessors
        public bool DownloadImages { get; set; }

        public bool HideWarning { get; set; }

        public bool TempDownloadDisabled { get; set; }

        public Preferences()
        {
            InitializeComponent();

            DownloadImages = Properties.Settings.Default.DownloadImages;
            HideWarning = Properties.Settings.Default.HideWarningDialogs;
            TempDownloadDisabled = MTCGHttpClientFactory.disableDownloading;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DownloadImages = DownloadImages;
            Properties.Settings.Default.HideWarningDialogs = HideWarning;
            MTCGHttpClientFactory.disableDownloading = TempDownloadDisabled;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}

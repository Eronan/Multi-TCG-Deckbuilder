using System.ComponentModel;
using System.Windows;

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

        public bool AutoDelete { get; set; }

        public Preferences()
        {
            InitializeComponent();

            DownloadImages = Properties.Settings.Default.DownloadImages;
            HideWarning = Properties.Settings.Default.HideWarningDialogs;
            AutoDelete = Properties.Settings.Default.AutoDeleteCorrupted;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DownloadImages = DownloadImages;
            Properties.Settings.Default.HideWarningDialogs = HideWarning;
            Properties.Settings.Default.AutoDeleteCorrupted = AutoDelete;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}

using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Contexts;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Models
{
    public class CardModel : DeckBuilderCardArt, INotifyPropertyChanged
    {
        private BitmapImage? _image;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CardModel(string cardID, string artID, string name, string fileLocation, string downloadUrl, CardArtOrientation orientation, string viewDetails = "") : base(cardID, artID, name, fileLocation, downloadUrl, orientation, viewDetails)
        {
        }

        public BitmapImage Image
        {
            get
            {
                if (FileCorrupted && !Properties.Settings.Default.AutoDeleteCorrupted) { return (_image = _image ?? new BitmapImage()); }

                if (File.Exists(FullPath) && !FileCorrupted)
                {
                    if (_image == null || !Loaded)
                    {
                        try
                        {
                            _image = new BitmapImage(new Uri(FullPath));
                        }
                        catch (NotSupportedException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("{0} Image is corrupted. Please delete image.", FullPath);
                            FileCorrupted = true;

                            Loaded = false;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Loaded"));
                            _image = new BitmapImage();
                        }
                    }
                    return _image;
                }

                if (Properties.Settings.Default.DownloadImages && !MTCGHttpClientFactory.FileNames.Contains(FullPath))
                {
                    MTCGHttpClientFactory.FileNames.Add(FullPath);
                    if (_image != null && Loaded) { SaveImage(_image, new EventArgs()); }
                    else
                    {
                        _image = new BitmapImage(new Uri(DownloadLocation));
                        _image.DownloadFailed += FailedImage;
                        _image.DownloadCompleted += SaveImage;
                        _image.DownloadCompleted += SuccessImage;
                    }

                    return _image;
                }

                if (_image == null || !Loaded)
                {
                    _image = new BitmapImage(new Uri(DownloadLocation));
                    _image.DownloadCompleted += SuccessImage;
                    _image.DownloadFailed += FailedImage;
                }

                return _image;
            }
        }

        private void SaveImage(object? sender, EventArgs e)
        {
            var image = sender as BitmapImage;
            if (image == null) { return; }

            var ImageEncoder = GetEncoderFromExtension(FullPath);
            ImageEncoder.Frames.Add(BitmapFrame.Create(image));

            string? directoryPath = Path.GetDirectoryName(FullPath);
            if (directoryPath != null && !Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }

            try
            {
                using (var fileStream = new FileStream(FullPath, FileMode.Create))
                {
                    ImageEncoder.Save(fileStream);
                    FileCorrupted = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileCorrupted"));
                }

                MTCGHttpClientFactory.FileNames.Remove(FullPath);
                if (image.CanFreeze) { image.Freeze(); }
            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private void SuccessImage(object? sender, EventArgs e)
        {
            Loaded = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Loaded"));
        }

        private void FailedImage(object? sender, EventArgs e)
        {
            Loaded = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Loaded"));
        }

        private BitmapEncoder GetEncoderFromExtension(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath);

            switch (fileExtension.ToLower())
            {
                case ".jpeg":
                case ".jpg":
                    return new JpegBitmapEncoder();
                case ".png":
                    return new PngBitmapEncoder();
                case ".gif":
                    return new GifBitmapEncoder();
                case ".tiff":
                    return new TiffBitmapEncoder();
                case ".wmp":
                    return new WmpBitmapEncoder();
                case ".bmp":
                    return new BmpBitmapEncoder();
                default:
                    throw new FileFormatException("File Extension is not a valid Image Format.");
            }
        }

        public string FullPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + FileLocation;
            }
        }

        public bool Loaded { get; set; } = true;

        public bool FileCorrupted { get; set; } = false;
    }
}

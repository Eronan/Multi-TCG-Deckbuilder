using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Contexts;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Multi_TCG_Deckbuilder
{
    /// <summary>
    /// Interaction logic for LoadPlugin.xaml
    /// </summary>
    public partial class LoadPlugin : Window
    {
        GitHubClient? client;
        Version? currentVersion;

        public LoadPlugin()
        {
            InitializeComponent();

            // Find Installed Plug-Ins
            string[] pluginPaths = Directory.GetFiles(@".\plug-ins\", "*.dll", SearchOption.AllDirectories);

            // Load Plug-Ins
            List<IGamePlugIn> gamePlugIns = new List<IGamePlugIn>();

            if (true || pluginPaths.Length > 0 &&
                    MessageBox.Show(
                        "DLL files can be harmful to your computer, and cause irrepairable damage. Please only download Plug-Ins trusted sources.\nDo you trust all of the following Plug-Ins?\n\n\t" +
                            string.Join("\n\t-", pluginPaths.Select(file => Path.GetFileName(file))),
                        "Warning!",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        MessageBoxResult.Yes
                    ) == MessageBoxResult.Yes)
            {
                foreach (string pluginPath in pluginPaths)
                {
                    try
                    {
                        Assembly pluginAssembly = LoadPlugins(pluginPath);
                        gamePlugIns.AddRange(GetGamePlugIns(pluginAssembly));
                    }
                    catch (ApplicationException e)
                    {
                        Console.WriteLine(pluginPath + " is not a valid Plug-In\n" + e.Message);
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        Console.WriteLine(pluginPath + " is outdated.\n" + e.Message);
                        MessageBox.Show(pluginPath + " is outdated.\n" + e.Message, "Plug-In Outdated", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                this.Close();
                return;
            }

            listBox_GameList.ItemsSource = gamePlugIns;

            // Get Arguments
            string[] args = Environment.GetCommandLineArgs();
            foreach (string path in args)
            {
                if (!path.EndsWith(".mtdk")) { continue; }
                LoadFromFile(path);
            }
        }

        private void LoadFromFile(string filePath)
        {
            string? jsonText = Contexts.FileLoadContext.ReadFromFile(filePath);
            DeckBuilderDeckFile? deckFile = Contexts.FileLoadContext.ConvertFromJson(jsonText != null ? jsonText : "");

            if (deckFile == null)
            {
                MessageBox.Show("There was a problem reading the mtdk File.", "Can't Open File!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IGamePlugIn? game = listBox_GameList.Items.Cast<IGamePlugIn>().FirstOrDefault(item => item.Name == deckFile.Game);
            IFormat? format = game != null ? game.Formats.FirstOrDefault(item => item.Name == deckFile.Format) : null;

            if (game == null || format == null)
            {
                MessageBox.Show("The Game Plug-In or Format for this Deck File is not installed.\nPlease Check the Plug-In is installed and Updated.", "Missing Plug-In!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DeckBuilder deckBuilderWindow = new DeckBuilder(game, format, deckFile, filePath);
            deckBuilderWindow.Show();
        }

        static Assembly LoadPlugins(string relativePath)
        {
            // Navigate up to the solution root
            string? root = System.IO.Path.GetDirectoryName(typeof(LoadPlugin).Assembly.Location);

            if (root == null) throw new NullReferenceException("root is Null.");

            string pluginLocation = System.IO.Path.GetFullPath(System.IO.Path.Combine(root, relativePath.Replace('\\', System.IO.Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            GameLoadContext loadContext = new GameLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(System.IO.Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<IGamePlugIn> GetGamePlugIns(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IGamePlugIn).IsAssignableFrom(type))
                {
                    IGamePlugIn? result = Activator.CreateInstance(type) as IGamePlugIn;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        private void listBox_GameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IGamePlugIn? selectedGame = listBox_GameList.SelectedItem as IGamePlugIn;
            if (selectedGame != null)
            {
                listBox_FormatList.ItemsSource = selectedGame.Formats;
            }
        }

        private void CommandBinding_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            IGamePlugIn? game = listBox_GameList.SelectedItem as IGamePlugIn;
            IFormat? format = listBox_FormatList.SelectedItem as IFormat;
            e.CanExecute = game != null && format != null;
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IGamePlugIn? game = listBox_GameList.SelectedItem as IGamePlugIn;
            IFormat? format = listBox_FormatList.SelectedItem as IFormat;
            if (game != null && format != null)
            {
                // Initialize Plug-In Information, if not previously initialized
                if (!game.CardListInitialized)
                {
                    try
                    {
                        game.InitializePlugIn();

                        DeckBuilder deckbuilderWindow = new DeckBuilder(game, format);
                        deckbuilderWindow.Show();
                    }
                    catch (PlugInFilesMissingException error)
                    {
                        Console.WriteLine(error.Message);
                        MessageBox.Show("Download the Latest Version of the Plug-In before continuing.");

                        MenuItem_PlugInFiles_Click(sender, e);
                    }
                    catch (BadImageFormatException error)
                    {
                        MessageBox.Show(error.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a Game and a Format!", "Please Select!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.DefaultExt = ".mtdk";
            openDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

            if (openDialog.ShowDialog() == true)
            {
                LoadFromFile(openDialog.FileName);
            }
        }

        private void CommandBinding_Preferences_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        // Run Update Method
        private void MenuItem_UpdatePlugIn_Click(object sender, RoutedEventArgs e)
        {
            IGamePlugIn? game = listBox_GameList.SelectedItem as IGamePlugIn;
            if (game == null)
            {
                return;
            }

            var process = new System.Diagnostics.ProcessStartInfo(game.DownloadLink)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            System.Diagnostics.Process.Start(process);
        }

        // Allow Plug-In to Download Files
        private void MenuItem_PlugInFiles_Click(object sender, RoutedEventArgs e)
        {
            IGamePlugIn? game = listBox_GameList.SelectedItem as IGamePlugIn;
            if (game == null || 
                MessageBox.Show("Make sure that you have already previously downloaded files manually! If downloading takes too long, the program will time-out and corrupt the downloaded files.\nAre you sure you want to download Files, this can take a while?", "Confirm Download", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
            {
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                var download = game.DownloadFiles();
                download.Wait(new TimeSpan(0, 10, 0));

                MessageBox.Show("Download Successful!", "Success!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception error)
            {
                MessageBox.Show("Download Unsuccessful. Attempt to download the necessary files manually.\n" + error.Message, "Unsuccesful!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Mouse.OverrideCursor = null;
        }

        // Open Update Link
        private async void MenuItem_UpdateApp_Click(object sender, RoutedEventArgs e)
        {
            // Initialize Values
            this.client = this.client ?? new GitHubClient(new ProductHeaderValue("tcg-deck-builder"));
            this.currentVersion = this.currentVersion ?? Assembly.GetExecutingAssembly().GetName().Version;

            var releases = await this.client.Repository.Release.GetAll("Eronan", "Multi-TCG-Deckbuilder");
            var latest = releases.FirstOrDefault(release => !release.Prerelease);

            if (latest == null)
            {
                MessageBox.Show("Program is up to date!", "Up to Date!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Console.WriteLine(
                "The latest release is tagged at {0} and is named {1}",
                latest.TagName,
                latest.Name);

            var latestVersion = new Version(latest.TagName);

            if (latestVersion.CompareTo(this.currentVersion) > 0)
            {
                var result = MessageBox.Show("There seems to be a new Version available, do you want to download it?", "New Version Available", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    var process = new System.Diagnostics.ProcessStartInfo(latest.HtmlUrl)
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    System.Diagnostics.Process.Start(process);
                }
            }
            else
            {
                MessageBox.Show("Program is up to date!", "Up to Date!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Open About Window for Program
        private void MenuItem_ProgramAbout_Click(object sender, RoutedEventArgs e)
        {
            new Dialogs.About().ShowDialog();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

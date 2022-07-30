using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Multi_TCG_Deckbuilder
{
    /// <summary>
    /// Interaction logic for LoadPlugin.xaml
    /// </summary>
    public partial class LoadPlugin : Window
    {
        public LoadPlugin()
        {
            InitializeComponent();

            // Find Installed Plug-Ins
            XDocument pluginPathDoc = XDocument.Load(@".\PlugInLocations.xml");
            var root = pluginPathDoc.Root;
            if (root == null) throw new NullReferenceException();
            var descendants = root.Descendants("Plugin");

            // Load Installed Plug-Ins
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var pluginPaths = descendants.Select(e => e.Attribute("Path") != null ? e.Attribute("Path").Value : "").ToArray();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            IEnumerable<IGamePlugIn> gamePlugIns = pluginPaths.SelectMany(pluginPath => 
            {
                Assembly pluginAssembly = LoadPlugins(pluginPath);
                return GetGamePlugIns(pluginAssembly);
            });

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

            IGamePlugIn? game = listBox_GameList.Items.Cast<IGamePlugIn>().Where(item => item.Name == deckFile.Game).FirstOrDefault();
            IFormat? format = game != null ? game.Formats.Where(item => item.Name == deckFile.Format).FirstOrDefault() : null;

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
                DeckBuilder deckbuilderWindow = new DeckBuilder(game, format);
                deckbuilderWindow.Show();
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

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

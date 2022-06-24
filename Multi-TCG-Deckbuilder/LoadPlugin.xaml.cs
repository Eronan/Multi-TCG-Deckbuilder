using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using System.Text.Json;
using Multi_TCG_Deckbuilder.Models;

namespace Multi_TCG_Deckbuilder
{
    /// <summary>
    /// Interaction logic for LoadPlugin.xaml
    /// </summary>
    public partial class LoadPlugin : Window
    {
        private List<Game> gameList;

        public LoadPlugin()
        {
            InitializeComponent();

            gameList = new List<Game>();

            string pluginDirectory = Directory.GetCurrentDirectory() + @"\plug-ins\";
            string[] pluginFolders = Directory.GetDirectories(pluginDirectory);
            foreach (string folder in pluginFolders)
            {
                string gameFile = folder + @"\game.json";
                if (!File.Exists(gameFile)) { continue; }

                string jsonText = File.ReadAllText(gameFile);
                try
                {
                    Game tcg = JsonSerializer.Deserialize<Game>(jsonText);
                    tcg.logo = folder + @"\" + tcg.logo;
                    gameList.Add(tcg);
                }
                catch (JsonException error)
                {
                    Console.WriteLine(error.Message);
                    //Output Error File
                }
            }

            listBox_GameList.ItemsSource = gameList;
        }
    }
}

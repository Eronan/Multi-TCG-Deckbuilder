using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
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

            XDocument pluginPathDoc = XDocument.Load(@".\PlugInLocations.xml");
            var root = pluginPathDoc.Root;
            var descendants = root.Descendants("Plugin");
            
            var pluginPaths = descendants.Select(e => e.Attribute("Path").Value).ToArray();

            IEnumerable<IGamePlugIn> gamePlugIns = pluginPaths.SelectMany(pluginPath => 
            {
                Assembly pluginAssembly = LoadPlugins(pluginPath);
                return GetGamePlugIns(pluginAssembly);
            });

            listBox_GameList.ItemsSource = gamePlugIns;
        }

        static Assembly LoadPlugins(string relativePath)
        {
            // Navigate up to the solution root
            string root = System.IO.Path.GetDirectoryName(typeof(LoadPlugin).Assembly.Location);

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
                    IGamePlugIn result = Activator.CreateInstance(type) as IGamePlugIn;
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
    }
}

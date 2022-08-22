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
using System.Windows.Shapes;

namespace Multi_TCG_Deckbuilder.Dialogs
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {

        public About(string? text = null)
        {
            InitializeComponent();

            // Program About
            if (text == null)
            {
                Version? version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                text = "Multi-TCG Deck Builder\n" +
                    (version != null ? string.Format("Version {0}\n", version.ToString()) : "") +
                    "Developed by Eronan\n" +
                    "-\n" +
                    "The Program is free and released under the \"GNU General Public License v3.0\". Any iterations on the program must be open-sourced.\n" +
                    "https://github.com/Eronan/Multi-TCG-Deckbuilder/blob/master/LICENSE.md\n" +
                    "-\n" +
                    "Check for New Releases on GitHub:\n" +
                    "https://github.com/Eronan/Multi-TCG-Deckbuilder/releases\n" +
                    "-\n" +
                    "To find Verified Plug-Ins that work with the latest versions of the Application, please visit: \n" +
                    "https://github.com/Eronan/Multi-TCG-Deckbuilder/releases";
            }
            
            // Create About Text
            foreach(string line in text.Split('\n'))
            {
                // If text is only lines, replace with a separator and go to next line
                if (line.All(character => character == '-' || character == '=' || character == '_'))
                {
                    Separator separator = new Separator();
                    separator.Foreground = SystemColors.ControlLightBrush;
                    stack_Text.Children.Add(separator);
                    continue;
                }

                // Create TextBlock
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Margin = new Thickness(10);

                if (line.Contains("https://"))
                { 
                    string nonLink = "";
                    foreach (string word in line.Split(' '))
                    {
                        if (word.StartsWith("http"))
                        {
                            textBlock.Inlines.Add(nonLink);
                            Hyperlink hyperlink = new Hyperlink();
                            hyperlink.NavigateUri = new Uri(word);
                            hyperlink.Inlines.Add(word);
                            hyperlink.RequestNavigate += (sender, e) => {
                                var process = new System.Diagnostics.ProcessStartInfo(e.Uri.ToString())
                                {
                                    UseShellExecute = true,
                                    Verb = "open"
                                };
                                System.Diagnostics.Process.Start(process);
                            };
                            textBlock.Inlines.Add(hyperlink);
                            nonLink = " ";
                        }
                        else
                        {
                            nonLink += word + " ";
                        }
                    }
                    textBlock.Inlines.Add(nonLink.TrimEnd());
                }
                else
                {
                    textBlock.Text = line;
                }
                stack_Text.Children.Add(textBlock);
                
            }
        }
    }
}

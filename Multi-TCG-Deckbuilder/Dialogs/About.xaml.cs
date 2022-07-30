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
        public About(string text)
        {
            InitializeComponent();
            
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

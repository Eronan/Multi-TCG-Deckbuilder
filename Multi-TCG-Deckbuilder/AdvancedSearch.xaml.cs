using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IGamePlugInBase;

namespace Multi_TCG_Deckbuilder
{
    /// <summary>
    /// Interaction logic for AdvancedSearch.xaml
    /// </summary>
    public partial class AdvancedSearch : Window
    {
        private SearchField[] searchFields;
        private dynamic[] inputBoxes;
        public AdvancedSearch(SearchField[] fields)
        {
            InitializeComponent();

            this.searchFields = fields;
            this.inputBoxes = new dynamic[searchFields.Length];
            this.CreateFields();
        }

        // Create all Fields from searchFields
        private void CreateFields()
        {
            for (int i = 0; i < this.searchFields.Length; i++)
            {
                CreateField(i);
            }
        }

        // Create Field from SearchField Class
        private void CreateField(int index)
        {
            SearchField field = this.searchFields[index];

            // Grid Column 1
            ColumnDefinition firstColumn = new ColumnDefinition();
            firstColumn.Width = new GridLength(75, GridUnitType.Star);

            // Grid Column 2
            ColumnDefinition secondColumn = new ColumnDefinition();
            secondColumn.Width = new GridLength(75, GridUnitType.Star);

            // Grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(firstColumn);
            grid.ColumnDefinitions.Add(secondColumn);
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Height = double.NaN;
            grid.Width = double.NaN;
            panel_Fields.Children.Add(grid);

            // Label
            Label label = new Label();
            label.Content = field.Label;
            label.HorizontalAlignment = HorizontalAlignment.Stretch;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.VerticalContentAlignment = VerticalAlignment.Top;
            label.HorizontalContentAlignment = HorizontalAlignment.Right;
            label.Width = double.NaN;
            label.Height = double.NaN;
            grid.Children.Add(label);

            // Value Input
            if (field.FieldType == SearchFieldType.Text)
            {
                // Text Box
                TextBox textBox = new TextBox();
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.Height = double.NaN;
                textBox.Width = double.NaN;
                textBox.MaxLength = field.Maximum.HasValue ? field.Maximum.Value: 255;
                textBox.SetValue(Grid.ColumnProperty, 1);
                textBox.Text = field.Value;
                textBox.DataContext = field;
                this.inputBoxes[index] = textBox;
                grid.Children.Add(textBox);
            }
            else if (field.FieldType == SearchFieldType.Number)
            {
                TextBox textBox = new TextBox();
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.Height = double.NaN;
                textBox.Width = double.NaN;
                textBox.MaxLength = field.Maximum.HasValue ? field.Maximum.Value % 10 : 255;
                textBox.SetValue(Grid.ColumnProperty, 1);
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                textBox.Text = field.Value;
                textBox.DataContext = field;
                this.inputBoxes[index] = textBox;
                grid.Children.Add(textBox);
            }
            else if (field.FieldType == SearchFieldType.Selection)
            {
                ComboBox comboBox = new ComboBox();
                comboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                comboBox.VerticalAlignment = VerticalAlignment.Center;
                comboBox.Height = double.NaN;
                comboBox.Width = double.NaN;
                comboBox.ItemsSource = field.Options;
                comboBox.SetValue(Grid.ColumnProperty, 1);
                comboBox.Margin = new Thickness(1);
                comboBox.SelectedValue = field.Value;
                comboBox.DataContext = field;
                this.inputBoxes[index] = comboBox;
                grid.Children.Add(comboBox);
            }
        }

        // Limit Input to Numbers and dependent on Maximum/Minimum
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((new Regex("[^0-9-]+")).IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            TextBox? numberbox = sender as TextBox;
            SearchField? field = numberbox != null ? numberbox.DataContext as SearchField : null;
            if (numberbox != null && field != null)
            {
                e.Handled = int.Parse(numberbox.Text + e.Text) < field.Minimum || int.Parse(numberbox.Text + e.Text) > field.Maximum;
            }
        }

        // Set Window Height
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinHeight = stack_Search.ActualHeight + SystemParameters.WindowCaptionHeight * 2;
            this.MaxHeight = stack_Search.ActualHeight + SystemParameters.WindowCaptionHeight * 2;
        }

        // Stop Window from Closing, and Hide it instead
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;
            //this.Visibility = Visibility.Collapsed;
        }

        private void UpdateValues()
        {
            for (int i = 0; i < this.inputBoxes.Length; i++)
            {
                SearchField searchField = this.searchFields[i];
                dynamic control = this.inputBoxes[i];
                
                switch(searchField.FieldType)
                {
                    case SearchFieldType.Text:
                        searchField.Value = control.Text;
                        break;
                    case SearchFieldType.Number:
                        searchField.Value = control.Text;
                        break;
                    case SearchFieldType.Selection:
                        searchField.Value = control.SelectedValue;
                        break;
                }
            }
        }

        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateValues();
            //this.Visibility = Visibility.Collapsed;
            this.DialogResult = true;
            this.Close();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            this.DialogResult = false;
        }

        private void button_Clear_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

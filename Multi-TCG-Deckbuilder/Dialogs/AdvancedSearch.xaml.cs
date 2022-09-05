using IGamePlugInBase;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Multi_TCG_Deckbuilder.Dialogs
{
    /// <summary>
    /// Interaction logic for AdvancedSearch.xaml
    /// </summary>
    public partial class AdvancedSearch : Window
    {
        private IList<SearchField> searchFields;
        private ComboBox[] searchComparisons;
        private dynamic[] inputBoxes;
        
        /// <summary>
        /// Constructs an Advanced Search Window
        /// </summary>
        /// <param name="fields">Fields that can be searched.</param>
        public AdvancedSearch(IEnumerable<SearchField> fields)
        {
            InitializeComponent();

            this.searchFields = fields.ToList();
            this.searchComparisons = new ComboBox[searchFields.Count];
            this.inputBoxes = new dynamic[searchFields.Count];
            this.CreateFields();
        }

        // Create all Fields from searchFields
        private void CreateFields()
        {
            for (int i = 0; i < this.searchFields.Count; i++)
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
            secondColumn.Width = new GridLength(27, GridUnitType.Star);

            // Grid Column 3
            ColumnDefinition thirdColumn = new ColumnDefinition();
            thirdColumn.Width = new GridLength(75, GridUnitType.Star);

            // Grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(firstColumn);
            grid.ColumnDefinitions.Add(secondColumn);
            grid.ColumnDefinitions.Add(thirdColumn);
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

            // Comparison
            ComboBox comparison = new ComboBox();
            comparison.HorizontalAlignment = HorizontalAlignment.Stretch;
            comparison.VerticalAlignment = VerticalAlignment.Center;
            comparison.Height = double.NaN;
            comparison.Width = double.NaN;
            comparison.Items.Add('=');
            comparison.Items.Add('≠');
            if (field.FieldType == SearchFieldType.Number)
            {
                comparison.Items.Add('≤');
                comparison.Items.Add('≥');
            }
            comparison.SetValue(Grid.ColumnProperty, 1);
            comparison.SelectedValue = ((char)field.Comparison);
            searchComparisons[index] = comparison;
            grid.Children.Add(comparison);

            // Value Input
            if (field.FieldType == SearchFieldType.Text)
            {
                // Text Box
                TextBox textBox = new TextBox();
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.Height = double.NaN;
                textBox.Width = double.NaN;
                textBox.MaxLength = field.Maximum.HasValue ? field.Maximum.Value : 255;
                textBox.SetValue(Grid.ColumnProperty, 2);
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
                textBox.MaxLength = field.Maximum.HasValue && field.Minimum.HasValue ? field.Maximum.Value / 10 + (field.Minimum.Value < 0 ? 2 : 1) : 255;
                textBox.SetValue(Grid.ColumnProperty, 2);
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
                comboBox.SetValue(Grid.ColumnProperty, 2);
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
                string text = "";
                if (numberbox.SelectedText.Length > 0)
                {
                    text = numberbox.Text.Replace(numberbox.SelectedText, e.Text);
                }
                else
                {
                    text = numberbox.Text;
                    text = text.Insert(numberbox.CaretIndex, e.Text);
                }

                int value;
                e.Handled = !int.TryParse(text, out value) || value < field.Minimum || value > field.Maximum;
            }
        }

        // Set Window Height
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinHeight = stack_Search.ActualHeight + SystemParameters.WindowCaptionHeight * 2;
            this.MaxHeight = stack_Search.ActualHeight + SystemParameters.WindowCaptionHeight * 2;
        }

        /// <summary>
        /// Sets the Values after Confirming an Advanced Search Filter
        /// </summary>
        /// <returns>Confirms that none of the Fields are currently their Default Value.</returns>
        private bool UpdateValues()
        {
            bool existsNonDefault = false;
            for (int i = 0; i < this.inputBoxes.Length; i++)
            {
                SearchField searchField = this.searchFields[i];
                string? comparisonValue = this.searchComparisons[i].SelectedValue.ToString();
                searchField.Comparison = comparisonValue != null ? (SearchFieldComparison)comparisonValue[0] : SearchFieldComparison.Equals;
                var control = this.inputBoxes[i];

                switch (searchField.FieldType)
                {
                    case SearchFieldType.Text:
                        if (control.Text.Length > 0) { existsNonDefault = true; }
                        searchField.Value = control.Text;
                        break;
                    case SearchFieldType.Number:
                        if (control.Text.Length > 0) { existsNonDefault = true; }
                        searchField.Value = control.Text;
                        break;
                    case SearchFieldType.Selection:
                        if (control.SelectedValue != searchField.DefaultValue) { existsNonDefault = true; }
                        searchField.Value = control.SelectedValue;
                        break;
                }
            }

            return existsNonDefault;
        }

        // Confirm Button to Search
        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = this.UpdateValues();
            this.Close();
        }

        // Cancels Searching
        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Clears and Resets everything to their Defaults
        private void button_Clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.inputBoxes.Length; i++)
            {
                SearchField searchField = this.searchFields[i];
                this.searchComparisons[i].SelectedValue = '=';

                switch (searchField.FieldType)
                {
                    case SearchFieldType.Text:
                        this.inputBoxes[i].Text = "";
                        break;
                    case SearchFieldType.Number:
                        this.inputBoxes[i].Text = "";
                        break;
                    case SearchFieldType.Selection:
                        this.inputBoxes[i].SelectedValue = searchField.DefaultValue;
                        break;
                }
            }
        }

        public IEnumerable<SearchField> SearchFields
        {
            get { return this.searchFields; }
        }
    }
}

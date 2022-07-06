﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGamePlugInBase
{
    public enum SearchFieldType
    { 
        Text = 0,
        Number = 1,
        Selection = 2,
    }

    /// <summary>
    /// Used for Grabbing the Fields in the Advanced Search for the Deck Builder
    /// </summary>
    public class SearchField
    {
        string id;
        string label;
        SearchFieldType fieldType;
        int? min;
        int? max;
        string[]? options;
        dynamic? value;

        /// <summary>
        /// Constructor that creates a Text Field
        /// </summary>
        /// <param name="id">ID the Field used for Retrieving the Value</param>
        /// <param name="label">Label of the Field that is shown on the Application</param>
        /// <param name="max">Maximum Length of the Text Value</param>
        public SearchField(string id, string label, int? max = null)
        {
            this.id = id;
            this.label = label;
            this.fieldType = SearchFieldType.Text;


            this.max = max != null ? max : 255;
        }

        /// <summary>
        /// Constructor that creates a Drop Down Field
        /// </summary>
        /// <param name="id">ID the Field used for Retrieving the Value</param>
        /// <param name="label">Label of the Field that is shown on the Application</param>
        /// <param name="options">An Array of Selectable Options</param>
        /// <param name="defaultValue">The Option that appears on Load.</param>
        public SearchField(string id, string label, string[] options, string? defaultValue = null)
        {
            this.id = id;
            this.label = label;
            this.fieldType = SearchFieldType.Selection;
            this.options = options;
            this.value = defaultValue;
        }

        /// <summary>
        /// Constructor that creates a Number Field
        /// </summary>
        /// <param name="id">ID the Field used for Retrieving the Value</param>
        /// <param name="label">Label of the Field that is shown on the Application</param>
        /// <param name="min">Minimum Number Value allowed</param>
        /// <param name="max">Maximum Number Value allowed</param>
        public SearchField(string id, string label, int min, int max)
        {
            this.id = id;
            this.label = label;
            this.fieldType = SearchFieldType.Number;
            this.min = min;
            this.max = max;

        }

        /// <summary>
        /// The ID used for retrieving the Field and Value
        /// </summary>
        public string Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// The Label that shows up on the Application
        /// </summary>
        public string Label
        {
            get { return this.label; }
        }

        /// <summary>
        /// The Type the Field will Search
        /// </summary>
        public SearchFieldType FieldType
        {
            get { return this.fieldType; }
        }

        /// <summary>
        /// Minimum Value of the Number
        /// </summary>
        public int? Minimum
        {
            get { return this.min; }
        }

        /// <summary>
        /// Maximum Value of the Number or Maximum Length of the Text
        /// </summary>
        public int? Maximum
        {
            get { return this.max; }
        }

        /// <summary>
        /// Options that are Selectable by the Drop Down
        /// </summary>
        public string[]? Options
        {
            get { return this.options; }
        }

        /// <summary>
        /// The Return Value retrieved from the Advanced Search
        /// </summary>
        public dynamic? Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
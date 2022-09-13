namespace IGamePlugInBase.IO
{
    /// <summary>
    /// Represents an Error caused by Necessary Files being Missing from the Plug-In Folder
    /// </summary>
    public class PlugInFilesMissingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of PlugInFilesMissingException class
        /// </summary>
        public PlugInFilesMissingException() :
            base("An update is required for this Plug-In.")
        {

        }

        /// <summary>
        /// Initializes a new instance of PlugInFilesMissingException class with a specified Error Message
        /// </summary>
        public PlugInFilesMissingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of PlugInFilesMissingException class with a specified Error Message and Inner Exception
        /// </summary>
        public PlugInFilesMissingException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}

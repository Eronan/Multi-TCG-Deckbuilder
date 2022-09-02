namespace IGamePlugInBase.IO
{
    public class PlugInFilesMissingException : Exception
    {
        public PlugInFilesMissingException() :
            base("An update is required for this Plug-In.")
        {

        }

        public PlugInFilesMissingException(string message)
            : base(message)
        {
        }

        public PlugInFilesMissingException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}

using System.Drawing.Imaging;

namespace IGamePlugInBase
{
    public class Format
    {
        string name;
        string longName;
        byte[] icon;
        string description;

        public Format(string name, string longName, byte[] iconImage, string description)
        {
            this.name = name;
            this.longName = longName;
            this.icon = iconImage;
            this.description = description;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string LongName
        {
            get { return this.longName; }
        }

        public byte[] Icon
        {
            get { return this.icon; }
        }

        public string Description
        {
            get { return this.description; }
        }
    }
}

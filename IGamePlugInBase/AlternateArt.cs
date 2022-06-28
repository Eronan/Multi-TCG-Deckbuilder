﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGamePlugInBase
{
    public interface AlternateArt
    {
        /// <summary>
        /// ID of the Alternate Art to be saved into the Deck File.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Location of the Image to be shown on the Deck Builder
        /// Can be loaded from a File Location externally or internally.
        /// </summary>
        public byte[] ImageLocation { get; }
    }
}
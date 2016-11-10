using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using System.IO;

namespace BinModel.src
{
    public class Model
    {
        public byte Version;
        public string Name;

        public TexBlock Textures;

        /// <summary>
        /// Loads a .bin model from file.
        /// </summary>
        /// <param name="fileName">File to load data from</param>
        public Model(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                EndianBinaryReader reader = new EndianBinaryReader(stream, Endian.Big);
                OpenModel(reader);
            }
        }

        /// <summary>
        /// Loads a .bin model from a stream.
        /// </summary>
        /// <param name="reader">Stream to load data from</param>
        public Model(EndianBinaryReader reader)
        {
            OpenModel(reader);
        }

        /// <summary>
        /// Loads data from a .bin file via a stream.
        /// </summary>
        /// <param name="reader">Stream to load data from</param>
        private void OpenModel(EndianBinaryReader reader)
        {
            Version = reader.ReadByte();
            if (Version != 2)
                throw new FormatException("Model version wasn't 2!");

            Name = new string(reader.ReadChars(11));
            Name = Name.Trim('\0');

            Textures = new TexBlock(reader);
            Textures.GetTextureSettings(reader);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

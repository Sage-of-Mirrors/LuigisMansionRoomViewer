using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using System.IO;

namespace BinModel.src.Model
{
    public class Model : IDisposable
    {
        public byte Version;
        public string Name;

        public TexBlock TextureData;
        public VertexAttribBlock VertexData;
        public MaterialBlock MaterialData;

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

            TextureData = new TexBlock(reader);
            TextureData.GetTextureSettings(reader);
            VertexData = new VertexAttribBlock(reader);
            MaterialData = new MaterialBlock(reader);
            MaterialData.GetMaterialTextures(TextureData);
        }

        public override string ToString()
        {
            return Name;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (Texture tex in TextureData.Textures)
                        tex.Dispose();
                }

                disposedValue = true;
            }
        }

         ~Model() {
           // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
           Dispose(false);
         }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

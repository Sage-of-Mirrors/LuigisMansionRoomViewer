using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using OpenTK.Graphics.OpenGL;
using JStudio.OpenGL;
using WindEditor;

namespace BinModel.src
{
    public class Texture : IDisposable
    {
        public BinaryTextureImage ImageData;

        private int m_glTextureIndex;
        // To detect redundant calls
        private bool m_hasBeenDisposed = false;

        public Texture(BinaryTextureImage imageData)
        {
            ImageData = imageData;

            m_glTextureIndex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, m_glTextureIndex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GXToOpenGL.GetWrapMode(imageData.WrapS));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GXToOpenGL.GetWrapMode(imageData.WrapT));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GXToOpenGL.GetMinFilter(imageData.MinFilter));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GXToOpenGL.GetMagFilter(imageData.MagFilter));

            // Border Color
            WLinearColor borderColor = imageData.BorderColor;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new[] { borderColor.R, borderColor.G, borderColor.B, borderColor.A });

            // ToDo: Min/Mag LOD & Biases

            // Upload Image Data
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, imageData.Width, imageData.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, imageData.GetData());

            // Generate Mip Maps
            if (imageData.MipMapCount > 0)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Bind(int toIndex)
        {
            int glIndex = toIndex + (int)TextureUnit.Texture0;
            GL.ActiveTexture((TextureUnit)glIndex);
            GL.BindTexture(TextureTarget.Texture2D, m_glTextureIndex);
        }

        #region IDisposable Support
        ~Texture()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        protected virtual void Dispose(bool manualDispose)
        {
            if (!m_hasBeenDisposed)
            {
                if (manualDispose)
                {
                    // TODO: dispose managed state (managed objects).
                }

                GL.DeleteTexture(m_glTextureIndex);

                // Set large fields to null.
                ImageData = null;
                m_hasBeenDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class TexBlock
    {
        public List<BinaryTextureImage> Images;
        public List<Texture> Textures;

        public TexBlock(EndianBinaryReader reader)
        {
            Images = new List<BinaryTextureImage>();
            Textures = new List<Texture>();

            // Get the offset of the TexBlock, save the reader's current offset, then go to the TexBlock
            int offset = reader.ReadInt32();
            long curOffset = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            // Get the offset of the first texture's image data
            int firstTexOffset = offset + reader.ReadInt32At(offset + 8);

            // We'll read texture data as long as the next image's width isn't 0 and 
            // the stream's position isn't at the place where the first texture's image data starts.
            while (reader.PeekReadInt16() != 0 && reader.BaseStream.Position != firstTexOffset)
            {
                BinaryTextureImage tex = new BinaryTextureImage();
                tex.LoadBinTex(reader, offset);
                Images.Add(tex);
            }

            //DumpTextures(@"D:\SZS Tools\TextureTest");

            reader.BaseStream.Seek(curOffset, System.IO.SeekOrigin.Begin);
        }

        /// <summary>
        /// Reads in the clamp settings and mipmap count for each texture.
        /// Textures are often duplicated to have one image with multiple pairs of clamp settings.
        /// </summary>
        /// <param name="reader"></param>
        public void GetTextureSettings(EndianBinaryReader reader)
        {
            // Get the offset of the TexBlock, get the offset of the next section, save the reader's current offset,
            // then go to the TextureSettings section
            int offset = reader.ReadInt32();
            int nextSectionOffset = reader.PeekReadInt32();
            long curOffset = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            // New list to store the images with proper clamp settings
            List<BinaryTextureImage> newImageList = new List<BinaryTextureImage>();

            while ((reader.PeekReadInt32() & 0xFFFF) == 0xFFFF && reader.BaseStream.Position != nextSectionOffset)
            {
                // Create a new texture from the texture at the index provided
                BinaryTextureImage modifiedTex = new BinaryTextureImage(Images[reader.ReadInt16()]);
                reader.SkipInt16(); // Padding, 0xFFFF
                modifiedTex.SetTexSettingsFromBin(reader);
                newImageList.Add(modifiedTex);
                Textures.Add(new Texture(modifiedTex));
            }

            // Replace original list of textures
            Images = newImageList;
            DumpTextures(@"D:\SZS Tools\TextureTest");

            reader.BaseStream.Seek(curOffset, System.IO.SeekOrigin.Begin);
        }

        private void DumpTextures(string location)
        {
            for (int i = 0; i < Images.Count; i++)
            {
                Images[i].SaveImageToDisk(string.Format("{0}\\tex_{1}.png", location, i));
            }
        }
    }
}

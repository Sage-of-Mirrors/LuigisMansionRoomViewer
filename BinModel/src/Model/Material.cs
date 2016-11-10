using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using WindEditor;

namespace BinModel.src.Model
{
    public class Material
    {
        List<Texture> Textures;
        byte Unknown1;
        byte Unknown2;
        byte Unknown3;
        WLinearColor AmbientColor;
        byte Unknown4;
        List<short> textureIndexes;

        public Material(EndianBinaryReader reader)
        {
            Textures = new List<Texture>();
            textureIndexes = new List<short>();

            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            Unknown3 = reader.ReadByte();
            AmbientColor = WLinearColor.FromHexString(string.Format("{0:X}{1:X}{2:X}{3:X}", reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte()));
            Unknown4 = reader.ReadByte();

            for (int i = 0; i < 8; i++)
            {
                textureIndexes.Add(reader.ReadInt16());
            }

            // I don't know what the rest of the data does
            reader.Skip(16);
        }

        public void GetTextures(TexBlock texBlock)
        {
            for (int i = 0; i < textureIndexes.Count; i++)
            {
                if (textureIndexes[i] == -1)
                    continue;
                Textures.Add(texBlock.Textures[textureIndexes[i]]);
            }
        }
    }
}

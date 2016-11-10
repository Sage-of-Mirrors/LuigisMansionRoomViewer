using System;
using System.Collections.Generic;
using OpenTK;
using WindEditor;
using GameFormatReader.Common;

namespace BinModel.src
{
    public sealed class MeshVertexHolder
    {
        public List<Vector3> Position = new List<Vector3>();
        public List<Vector3> Normal = new List<Vector3>();
        public List<Vector3> Binormal = new List<Vector3>();
        public List<WLinearColor> Color0 = new List<WLinearColor>();
        public List<WLinearColor> Color1 = new List<WLinearColor>();
        public List<Vector2> Tex0 = new List<Vector2>();
        public List<Vector2> Tex1 = new List<Vector2>();
        public List<Vector2> Tex2 = new List<Vector2>();
        public List<Vector2> Tex3 = new List<Vector2>();
        public List<Vector2> Tex4 = new List<Vector2>();
        public List<Vector2> Tex5 = new List<Vector2>();
        public List<Vector2> Tex6 = new List<Vector2>();
        public List<Vector2> Tex7 = new List<Vector2>();
        public List<int> PositionMatrixIndexes = new List<int>();
    }

    public class VertexAttribBlock
    {
        public MeshVertexHolder VertexData { get; private set; }

        public VertexAttribBlock(EndianBinaryReader reader)
        {
            // Read in offsets to different blocks of data
            int[] DataOffsets = new int[8];

            for (int i = 0; i < DataOffsets.Length; i++)
            {
                DataOffsets[i] = reader.ReadInt32();
            }

            // Store offset of next data offset
            long curOffset = reader.BaseStream.Position;

            // We'll use this to determine 
            int nextIndex = reader.PeekReadInt32();

            for (int i = 0; i < DataOffsets.Length; i++)
            {
                if (DataOffsets[i] == 0)
                    continue;

                switch (i)
                {
                    // Vertex position data
                    case 0:
                        break;
                    // Vertex normal data
                    case 1:
                        break;
                    // 2 and 3 are unknown. Vertex color 0 and 1 data?
                    case 2:
                    case 3:
                        throw new FormatException(string.Format("Data offset at {0:X} wasn't 0!", (i * 4) + 0x14));
                    // Tex coord 0 data
                    case 4:
                        break;
                    // 5, 6, and 7 are unknown. Tex coord 1-3 data?
                    case 5:
                    case 6:
                    case 7:
                        throw new FormatException(string.Format("Data offset at {0:X} wasn't 0!", (i * 4) + 0x14));
                }
            }

            reader.BaseStream.Seek(curOffset, System.IO.SeekOrigin.Begin);
        }
    }
}

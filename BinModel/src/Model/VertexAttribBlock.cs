using System;
using System.Collections.Generic;
using OpenTK;
using WindEditor;
using GameFormatReader.Common;

namespace BinModel.src.Model
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
            VertexData = new MeshVertexHolder();

            // Read in offsets to different blocks of data
            int[] DataOffsets = new int[8];

            for (int i = 0; i < DataOffsets.Length; i++)
            {
                DataOffsets[i] = reader.ReadInt32();
            }

            // Store offset of next data offset
            long curOffset = reader.BaseStream.Position;

            // We'll use this to determine the last data bank's size.
            int matDataOffset = reader.PeekReadInt32();

            for (int i = 0; i < DataOffsets.Length; i++)
            {
                // Skip this loop if the offset is 0
                if (DataOffsets[i] == 0)
                    continue;

                int dataLength = 0;

                // We need to calculate the length of the data bank.
                // We'll do that by finding the next valid offset and subtracting
                // the current offset from it.
                if (i != DataOffsets.Length - 1)
                {
                    int index = i + 1;

                    while (index != DataOffsets.Length - 1)
                    {
                        if (DataOffsets[index] != 0)
                        {
                            dataLength = DataOffsets[index] - DataOffsets[i];
                            break;
                        }
                        else
                            index++;
                    }

                    // We got through the while loop without setting dataLength, which means
                    // the offsets after offset i are 0
                    if (dataLength == 0)
                        dataLength = matDataOffset - DataOffsets[i];
                }
                // If the last offset isn't 0, which might not be often, we'll use the material data's offset to calculate
                // the dataLength
                else
                {
                    dataLength = matDataOffset - DataOffsets[i];
                }

                switch (i)
                {
                    // Vertex position data
                    case 0:
                        VertexData.Position = ReadPositionBank(reader, DataOffsets[i], dataLength);
                        break;
                    // Vertex normal data
                    case 1:
                        VertexData.Normal = ReadNormalBank(reader, DataOffsets[i], dataLength);
                        break;
                    // 2 and 3 are unknown. Vertex color 0 and 1 data?
                    case 2:
                    case 3:
                        throw new FormatException(string.Format("Data offset at {0:X} wasn't 0!", (i * 4) + 0x14));
                    // Tex coord 0 data
                    case 4:
                        VertexData.Tex0 = ReadUVBank(reader, DataOffsets[i], dataLength);
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

        private List<Vector3> ReadPositionBank(EndianBinaryReader reader, int offset, int size)
        {
            List<Vector3> posList = new List<Vector3>();
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < size; i += 6)
            {
                Vector3 vec = new Vector3((float)reader.ReadInt16(), (float)reader.ReadInt16(), (float)reader.ReadInt16());
                posList.Add(vec);
            }

            return posList;
        }

        private List<Vector3> ReadNormalBank(EndianBinaryReader reader, int offset, int size)
        {
            List<Vector3> normalList = new List<Vector3>();
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < size; i += 12)
            {
                Vector3 vec = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                normalList.Add(vec);
            }

            return normalList;
        }

        private List<Vector2> ReadUVBank(EndianBinaryReader reader, int offset, int size)
        {
            List<Vector2> uvList = new List<Vector2>();
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < size; i += 8)
            {
                Vector2 vec = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                uvList.Add(vec);
            }

            return uvList;
        }
    }
}

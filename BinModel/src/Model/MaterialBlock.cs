using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace BinModel.src.Model
{
    public class MaterialBlock
    {
        public List<Material> MaterialList;

        public MaterialBlock(EndianBinaryReader reader)
        {
            MaterialList = new List<Material>();

            // Read material data offset, peak the offset to the batch data, then store the current offset
            int offset = reader.ReadInt32();
            int batchDataOffset = reader.PeekReadInt32();
            long curOffset = reader.BaseStream.Position;

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            while(reader.BaseStream.Position + 40 < batchDataOffset)
            {
                Material mat = new Material(reader);
                MaterialList.Add(mat);
            }

            reader.BaseStream.Seek(curOffset, System.IO.SeekOrigin.Begin);
        }
    }
}

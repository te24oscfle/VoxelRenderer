using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelRenderer.Classes
{
    public class Block
    {
        public int BlockId = 0;
        public uint[] FacesToRender;
        public uint FaceCount = 0;

        public Block() : this(0) { }
        
        public Block(int blockId)
        {
            BlockId = blockId;
            FacesToRender = new uint[6];
        }

        public void AddFaceToRender(uint direction)
        {
            FacesToRender[FaceCount] = direction;
            FaceCount++;
        }
    }
}

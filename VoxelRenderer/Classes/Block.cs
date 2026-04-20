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
        public Vector3 Position;

        public Block() : this(new Vector3(0, 0, 0)) { }
        
        public Block(Vector3 position)
        {
            Position = position;
        }
    }
}

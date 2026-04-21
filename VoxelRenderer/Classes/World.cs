using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelRenderer.Classes
{
    public class World
    {
        private int chunkSizeX = 16;
        private int chunkSizeY = 32;
        private int chunkSizeZ = 16;

        public required Block[] blocks;

        public void InitilizeWorld()
        {
            blocks = new Block[chunkSizeX * chunkSizeY * chunkSizeZ];

            for (int x = 0; x < chunkSizeX; x++) {
                for (int y = 0; y < chunkSizeY; y++) {
                    for (int z = 0; z < chunkSizeZ; z++) 
                    {
                        int index = (x + x + chunkSizeX * (y + chunkSizeY * z));
                        Block block = new Block();
                        blocks[index] = block;
                    }
                }
            }

        }
    }
}

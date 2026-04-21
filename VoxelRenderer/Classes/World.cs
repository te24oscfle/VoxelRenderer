using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelRenderer.Classes
{
    public static class World
    {
        public static int chunkSizeX = 3;
        public static int chunkSizeY = 3;
        public static int chunkSizeZ = 3;

        public static Block[] blocks = new Block[chunkSizeX * chunkSizeY * chunkSizeZ];

        public static int GetIndexFromCoordinates(int x, int y, int z)
        {
            return x + (y * chunkSizeX) + (z * chunkSizeX * chunkSizeY);
        }

        // Not yet implemented
        public static (int, int, int) GetCoordinatesFromIndex(int index)
        {
            return (0, 0, 0);
        }

        public static void IterateBlocks(Action<int, int, int> iterator)
        {
            for (int x = 0; x < chunkSizeX; x++)
            {
                for (int y = 0; y < chunkSizeY; y++)
                {
                    for (int z = 0; z < chunkSizeZ; z++)
                    {
                        iterator(x, y, z);
                    }
                }
            }
        }

        public static void InitilizeWorld()
        {
            IterateBlocks((x, y, z) =>
            {
                int index = GetIndexFromCoordinates(x, y, z);
                Block block = new Block();
                blocks[index] = block;
            });
        }
    }
}

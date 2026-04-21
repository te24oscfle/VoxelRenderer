using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelRenderer.Classes
{
    public enum Direction
    {
        NULL,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FORWARD,
        BACKWARD
    }
    
    public static class World
    {
        public static int chunkSizeX = 1;
        public static int chunkSizeY = 2;
        public static int chunkSizeZ = 1;

        public static Block[] blocks = new Block[chunkSizeX * chunkSizeY * chunkSizeZ];

        private static int GetNeighbourIndexFromDirection(uint direction)
        {
            switch(direction)
            {
                case (uint)Direction.UP:
                    return GetIndexFromCoordinates(0, 1, 0);

                case (uint)Direction.DOWN:
                    return GetIndexFromCoordinates(0, -1, 0);

                case (uint)Direction.LEFT:
                    return GetIndexFromCoordinates(-1, 0, 0);

                case (uint)Direction.RIGHT:
                    return GetIndexFromCoordinates(1, 0, 0);

                case (uint)Direction.FORWARD:
                    return GetIndexFromCoordinates(0, 1, -1);

                case (uint)Direction.BACKWARD:
                    return GetIndexFromCoordinates(0, 1, 1);
            }

            return 0;
        }


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

        public static Block? GetNeighbourFromDirection(int originX, int originY, int originZ, uint direction)
        {
            int originIndex = GetIndexFromCoordinates(originX, originY, originZ);
            int neighbourIndex = originIndex + GetNeighbourIndexFromDirection(direction);

            if (blocks.Length > neighbourIndex && neighbourIndex >= 0)
            {
                return blocks[neighbourIndex];
            }

            return null;
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

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
        public static int chunkSizeY = 1;
        public static int chunkSizeZ = 1;

        public static Block[] blocks = new Block[chunkSizeX * chunkSizeY * chunkSizeZ];

        private static (int, int, int) GetNeighbourIndexFromDirection(uint direction)
        {
            
            switch(direction)
            {
                case (uint)Direction.UP:
                    return (0, 1, 0);

                case (uint)Direction.DOWN:
                    return (0, -1, 0);

                case (uint)Direction.LEFT:
                    return (-1, 0, 0);

                case (uint)Direction.RIGHT:
                    return (1, 0, 0);

                case (uint)Direction.FORWARD:
                    return (0, 0, 1);

                case (uint)Direction.BACKWARD:
                    return (0, 0, -1);
            }

            return (0, 0, 0);
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
            (int dirX, int dirY, int dirZ) = GetNeighbourIndexFromDirection(direction);

            int x = originX + dirX;
            int y = originY + dirY;
            int z = originZ + dirZ;

            // Check if index is invalid
            if((uint)x >= chunkSizeX || (uint)y >= chunkSizeY || (uint)z >= chunkSizeZ) {
                return null;
            }

            int index = GetIndexFromCoordinates(x, y, z);
            if((uint)index >= blocks.Length)
            {
                return null;
            }

            return blocks[index];
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

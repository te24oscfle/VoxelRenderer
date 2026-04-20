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
        private Vector3 CHUNK_SIZE = new Vector3(16, 32, 16);

        Block[] blocks;

        public void InitilizeWorld()
        {
            blocks = new Block[(int)(CHUNK_SIZE.X * CHUNK_SIZE.Y * CHUNK_SIZE.Z)];

            for (int x = 0; x < (int)CHUNK_SIZE.X; x++) {
                for (int y = 0; y < (int)CHUNK_SIZE.Y; y++) {
                    for (int z = 0; z < (int)CHUNK_SIZE.Z; z++) {
                        int index = (int)(x + x + CHUNK_SIZE.X * (y + CHUNK_SIZE.Y * z));

                        Block block = new Block();

                        blocks[index] = block;
            }
        }
    }
}

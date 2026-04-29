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

        private static float[] GetFaceVerticesFromDirection(Direction direction)
        {
            switch(direction)
            {
                case (Direction.UP):
                    return [
                        -0.5f, 0.5f, 0.5f,
                        0.5f, 0.5f, 0.5f,
                        0.5f, 0.5f, -0.5f,

                        0.5f, 0.5f, -0.5f,
                        -0.5f, 0.5f, -0.5f,
                        -0.5f, 0.5f, 0.5f,
                    ];

                case (Direction.DOWN):
                    return [
                        -0.5f, -0.5f, 0.5f,
                        -0.5f, -0.5f, -0.5f,
                        0.5f, -0.5f, -0.5f,

                        0.5f, -0.5f, -0.5f,
                        0.5f, -0.5f, 0.5f,
                        -0.5f, -0.5f, 0.5f,
                    ];

                case (Direction.LEFT):
                    return [
                        -0.5f, 0.5f, -0.5f,
                        -0.5f, -0.5f, -0.5f,
                        -0.5f, -0.5f, 0.5f,

                        -0.5f, -0.5f, 0.5f,
                        -0.5f, 0.5f, 0.5f,
                        -0.5f, 0.5f, -0.5f,
                    ];

                case (Direction.RIGHT):
                    return [
                        0.5f, 0.5f, -0.5f,
                        0.5f, 0.5f, 0.5f,
                        0.5f, -0.5f, 0.5f,

                        0.5f, -0.5f, 0.5f,
                        0.5f, -0.5f, -0.5f,
                        0.5f, 0.5f, -0.5f,
                    ];

                case (Direction.FORWARD):
                    return [
                        -0.5f, 0.5f, -0.5f,
                        0.5f, 0.5f, -0.5f,
                        0.5f, -0.5f, -0.5f,

                        0.5f, -0.5f, -0.5f,
                        -0.5f, -0.5f, -0.5f,
                        -0.5f, 0.5f, -0.5f,
                    ];

                case (Direction.BACKWARD):
                    return [
                        -0.5f, 0.5f, 0.5f,
                        -0.5f, -0.5f, 0.5f,
                        0.5f, -0.5f, 0.5f,

                        0.5f, -0.5f, 0.5f,
                        0.5f, 0.5f, 0.5f,
                        -0.5f, 0.5f, 0.5f,
                    ];

                default:
                    return new float[18];
            }
        }

        public float[] GetVertices()
        {
            float[] blockVertices = new float[18 * FaceCount];

            for (uint i = 0; i < FaceCount; i++)
            {
                Direction direction = (Direction)FacesToRender[i];
                
                // Get the vertices of the face and copy them into the blockVertices
                float[] faceVertices = GetFaceVerticesFromDirection(direction);
                Array.Copy(faceVertices, 0, blockVertices, faceVertices.Length * i, faceVertices.Length);
            }

            return blockVertices;
        }

        public void AddFaceToRender(uint direction)
        {
            FacesToRender[FaceCount] = direction;
            FaceCount++;
        }
    }
}

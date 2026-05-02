namespace VoxelRenderer.Classes
{
    public enum BlockID
    {
        GRASS,
        DIRT,
        STONE
    }

    public class Block
    {
        public int BlockId = 0;
        public int[] FacesToRender;
        public int FaceCount = 0;

        public float[] Vertices;
        public int VertexCount;

        public Block() : this(0) { }
        
        public Block(int blockId)
        {
            BlockId = blockId;
            FacesToRender = new int[6];
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
            // 1 face * 2 triangles * 3 verticies * 3 floats = 18 floats
            float[] blockVertices = new float[18 * FaceCount];

            for (int i = 0; i < FaceCount; i++)
            {
                Direction direction = (Direction)FacesToRender[i];
                
                float[] faceVertices = GetFaceVerticesFromDirection(direction);
                Array.Copy(faceVertices, 0, blockVertices, faceVertices.Length * i, faceVertices.Length);
            }

            return blockVertices;
        }

        public void AddFaceToRender(int direction)
        {
            FacesToRender[FaceCount] = direction;
            FaceCount++;
        }
    }
}

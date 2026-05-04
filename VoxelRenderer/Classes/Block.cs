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
        public BlockID BlockId = 0;
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
                        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, // A
                        0.5f, 0.5f, 0.5f, 0.5f, 0.0f, // D
                        0.5f, 0.5f, -0.5f, 0.5f, 0.5f, // C

                        0.5f, 0.5f, -0.5f, 0.5f, 0.5f, // C
                        -0.5f, 0.5f, -0.5f, 0.0f, 0.5f, // B
                        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f // A
                    ];

                case (Direction.DOWN):
                    return [
                        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // A
                        -0.5f, -0.5f, -0.5f, 0.5f, 0.0f, // D
                        0.5f, -0.5f, -0.5f, 0.5f, 0.5f, // C

                        0.5f, -0.5f, -0.5f, 0.5f, 0.5f, // C
                        0.5f, -0.5f, 0.5f, 0.0f, 0.5f, // B
                        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // A
                    ];

                case (Direction.LEFT):
                    return [
                        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, // A
                        -0.5f, -0.5f, -0.5f, 0.5f, 0.0f, // D
                        -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, // C

                        -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, // C
                        -0.5f, 0.5f, 0.5f, 0.0f, 0.5f, // B
                        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, // A
                    ];

                case (Direction.RIGHT):
                    return [
                        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, // A
                        0.5f, 0.5f, 0.5f, 0.5f, 0.0f, // D
                        0.5f, -0.5f, 0.5f, 0.5f, 0.5f, // C

                        0.5f, -0.5f, 0.5f, 0.5f, 0.5f, // C
                        0.5f, -0.5f, -0.5f, 0.0f, 0.5f, // B
                        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, // A
                    ];

                case (Direction.FORWARD):
                    return [
                        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, // A
                        0.5f, 0.5f, -0.5f, 0.5f, 0.0f, // D
                        0.5f, -0.5f, -0.5f, 0.5f, 0.5f, // C

                        0.5f, -0.5f, -0.5f, 0.5f, 0.5f, // C
                        -0.5f, -0.5f, -0.5f, 0.0f, 0.5f, // B
                        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, // A
                    ];

                case (Direction.BACKWARD):
                    return [
                        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, // A
                        -0.5f, -0.5f, 0.5f, 0.5f, 0.0f, // D
                        0.5f, -0.5f, 0.5f, 0.5f, 0.5f, // C

                        0.5f, -0.5f, 0.5f, 0.5f, 0.5f, // C
                        0.5f, 0.5f, 0.5f, 0.0f, 0.5f, // B
                        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, // A
                    ];

                default:
                    return new float[30];
            }
        }

        private float[] GetUVOffsetFromId(BlockID id)
        {
            // Not yet implemented
            return [
                0.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 0.0f,

                0.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 0.0f
            ];
        }

        public float[] GetVertices()
        {
            // 1 face * 2 triangles * (3 verticies + 2 uv coordinates) * 3 vertex positions = 30 floats 
            float[] blockVertices = new float[30 * FaceCount];

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

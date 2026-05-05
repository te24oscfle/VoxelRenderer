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
        public static int chunkSizeX = 16;
        public static int chunkSizeY = 24;
        public static int chunkSizeZ = 16;

        public static Block[] blocks = new Block[chunkSizeX * chunkSizeY * chunkSizeZ];

        private static (int, int, int) GetNeighbourIndexFromDirection(int direction)
        {
            switch(direction)
            {
                case (int)Direction.UP:
                    return (0, 1, 0);

                case (int)Direction.DOWN:
                    return (0, -1, 0);

                case (int)Direction.LEFT:
                    return (-1, 0, 0);

                case (int)Direction.RIGHT:
                    return (1, 0, 0);

                case (int)Direction.FORWARD:
                    return (0, 0, -1);

                case (int)Direction.BACKWARD:
                    return (0, 0, 1);
            }

            return (0, 0, 0);
        }

        public static int GetIndexFromCoordinates(int x, int y, int z)
        {
            return x + (y * chunkSizeX) + (z * chunkSizeX * chunkSizeY);
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

        public static Block? GetNeighbourFromDirection(int originX, int originY, int originZ, int direction)
        {
            (int dirX, int dirY, int dirZ) = GetNeighbourIndexFromDirection(direction);

            int x = originX + dirX;
            int y = originY + dirY;
            int z = originZ + dirZ;

            // Null checks
            if (x < 0 || y < 0 || z < 0) 
                return null;
            
            if(x >= chunkSizeX || y >= chunkSizeY || z >= chunkSizeZ)
                return null;

            int index = GetIndexFromCoordinates(x, y, z);
            if(index >= blocks.Length)
                return null;

            return blocks[index];
        }

        public static void InitilizeWorld()
        {
            // Create blocks
            IterateBlocks((x, y, z) =>
            {
                int index = GetIndexFromCoordinates(x, y, z);
                Block block = new Block();

                if(y == chunkSizeY-1)
                    block.BlockId = BlockID.GRASS;
                else if (y > chunkSizeY-5)
                    block.BlockId = BlockID.DIRT;
                else
                    block.BlockId = BlockID.STONE;

                blocks[index] = block;
            });

            // Calculate culling
            IterateBlocks((x, y, z) =>
            {
                int index = GetIndexFromCoordinates(x, y, z);
                Block block = blocks[index];

                // Direction = 0 is reserved for null, directions are defined for 1 through 6. See Direction enum in World.cs
                for (int direction = 1; direction < 7; direction++)
                {
                    Block? neighbour = GetNeighbourFromDirection(x, y, z, direction);
                    // If a neighbour doesnt exist in this direction, the face will be visible, so add it to the render list
                    if (neighbour == null)
                    {
                        block.AddFaceToRender(direction);
                    }

                    // If the neighbour does exist, the face wont be visible and we wont need to render it
                }
            });

            // Initilize vertices
            IterateBlocks((x, y, z) =>
            {
                int index = GetIndexFromCoordinates(x, y, z);
                Block block = blocks[index];

                float[] vertices = block.GetVertices();
                block.Vertices = vertices;
                block.VertexCount = block.Vertices.Length / 5;
            });
        }
    }
}

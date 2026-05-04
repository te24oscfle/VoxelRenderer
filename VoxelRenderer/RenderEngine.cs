namespace VoxelRenderer
{
    public class RenderEngine : GameWindow
    {
        public RenderEngine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        bool WIREFRAME_MODE = false;

        int VBO, VAO, EBO;
        int shaderProgram;
        int texture;

        Matrix4 model;
        Matrix4 view;
        Matrix4 projection;

        const float NEAR_PLANE = 0.1f;
        const float FAR_PLANE = 100.0f;

        readonly string texturePath = Path.Combine(
            AppContext.BaseDirectory,
            "textureAtlas.jpg"
        );

        float[] textureCoords = [
            0.0f, 0.0f,
            0.5f, 0.0f,
            0.5f, 0.5f,
            0.0f, 0.5f
        ];

        protected string GetShaderSource(string shaderPath)
        {
            string fullPath = Path.Combine(
                AppContext.BaseDirectory,
                "Shaders",
                shaderPath
            );
            string source = File.ReadAllText(fullPath);
            return source;
        }
        
        protected Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(CameraManager.FOV), 
                ClientSize.X / (float)ClientSize.Y, 
                NEAR_PLANE, 
                FAR_PLANE
            );
        }

        protected void RenderBlock(Block block, int x, int y, int z)
        {
            // No faces to render
            if (block.FaceCount == 0)
                return;
            
            // Send vertices data to the GPU
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, block.Vertices.Length * sizeof(float), block.Vertices, BufferUsageHint.DynamicDraw);

            // Position the block
            model = Matrix4.CreateTranslation(x, y, z);
            ShaderManager.SetMatrix4(shaderProgram, "model", model);

            // Render the block
            GL.DrawArrays(PrimitiveType.Triangles, 0, block.VertexCount);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            float deltaTime = (float)e.Time;
            var keyboardState = KeyboardState;

            if(!IsFocused)
            {
                return;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyPressed(Keys.F))
            {
                WIREFRAME_MODE = !WIREFRAME_MODE;
            }

            CameraManager.UpdatePosition(keyboardState, deltaTime);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // GL Setup
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);

            CursorState = CursorState.Grabbed;

            // Background Color
            GL.ClearColor(0.2f, 0.4f, 0.6f, 0.1f);

            // VBO, VAO and EBO Setup
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            // Bind VAO & VBO
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            // Configure vertex attributes
            // position data
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // UV data
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Textures
            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            // Min & Mag filters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Wrapping
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            // stb_image loads images from the top-left pixel, OpenGL loads images from bottom-left, causing images to appear flipped
            // Flipping the image vertically upon load will fix this.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image
            ImageResult textureAtlas = ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);

            // Upload image to the GPU
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                textureAtlas.Width,
                textureAtlas.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                textureAtlas.Data
            );

            // Unbind VBO & VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Shader Setup
            string vertexShaderSource = GetShaderSource("vertexShader.glsl");
            string fragmentShaderSource = GetShaderSource("fragmentShader.glsl");

            int vertexShader = ShaderManager.CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = ShaderManager.CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

            shaderProgram = ShaderManager.LinkShaders(vertexShader, fragmentShader);

            // World Initilizing
            World.InitilizeWorld();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            view = CameraManager.GetViewMatrix();
            projection = GetProjectionMatrix();

            ShaderManager.SetMatrix4(shaderProgram, "view", view);
            ShaderManager.SetMatrix4(shaderProgram, "projection", projection);

            GL.BindTexture(TextureTarget.Texture2D, texture);
            

            GL.UseProgram(shaderProgram);
            GL.BindVertexArray(VAO);

            // Render Blocks
            World.IterateBlocks((x, y, z) =>
            {
                int index = World.GetIndexFromCoordinates(x, y, z);
                Block block = World.blocks[index];

                RenderBlock(block, x, y, z);
            });

            // WIREFRAME MODE
            if (WIREFRAME_MODE)
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
            }

            SwapBuffers();
        }

        protected override void OnMouseMove(MouseMoveEventArgs mouse)
        {
            base.OnMouseMove(mouse);

            CameraManager.OnMouseMove(mouse);
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Cleanup
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
            GL.DeleteProgram(shaderProgram);
        }
    }
}
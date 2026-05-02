namespace VoxelRenderer
{
    public class RenderEngine : GameWindow
    {
        public RenderEngine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        bool WIREFRAME_MODE = false;

        int VBO, VAO, EBO;
        int shaderProgram;

        Matrix4 model;
        Matrix4 view;
        Matrix4 projection;

        const float NEAR_PLANE = 0.1f;
        const float FAR_PLANE = 100.0f;

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

            // Shader Setup
            string vertexShaderSource = GetShaderSource("vertexShader.glsl");
            string fragmentShaderSource = GetShaderSource("fragmentShader.glsl");

            int vertexShader = ShaderManager.CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = ShaderManager.CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

            shaderProgram = ShaderManager.LinkShaders(vertexShader, fragmentShader);

            // VBO, VAO and EBO Setup
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            // Bind VAO & VBO
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            // Configure vertex attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Textures

            
            // Unbind VBO & VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

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
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using VoxelRenderer.Classes;

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

        Vector3 CHUNK_SIZE = new Vector3(4, 1, 4);

        Vector3 cameraPosition = new Vector3(1.0f, 0.0f, 4.0f);
        
        float yaw = -90.0f;
        float pitch = 0.0f;
        Vector3 lookVector = new Vector3(0.0f, 1.0f, 0.0f);

        float movementSpeed = 6.5f;
        float sensitivity = 0.1f;
        float FOV = 65.0f;

        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        Vector2 lastMousePos;

        float[] vertices = {
            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,

            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,

             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,

            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f
        };

        //float[] vertices = {
        //    -0.5f, 0.5f, 0.0f,  // Top-left
        //    0.5f, 0.5f, 0.0f,   // Top-right
        //    -0.5f, -0.5f, 0.0f, // Bottom-left
        //    0.5f, -0.5f, 0.0f,  // Bottom right
        //};

        //uint[] indicies = {
        //    0, 2, 3, // Left triangle
        //    0, 1, 3, // Right triangle
        //};

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            float deltaTime = (float)e.Time;

            if(!IsFocused)
            {
                return;
            }
            
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // Camera Movement
            if (KeyboardState.IsKeyDown(Keys.W)) // Forward
            {
                cameraPosition += lookVector * movementSpeed * deltaTime;
            }

            if (KeyboardState.IsKeyDown(Keys.S)) // Backward
            {
                cameraPosition -= lookVector * movementSpeed * deltaTime;
            }

            if (KeyboardState.IsKeyDown(Keys.D)) // Right
            {
                cameraPosition += Vector3.Normalize(Vector3.Cross(lookVector, up)) * movementSpeed * deltaTime;
            }

            if (KeyboardState.IsKeyDown(Keys.A)) // Left
            {
                cameraPosition -= Vector3.Normalize(Vector3.Cross(lookVector, up)) * movementSpeed * deltaTime;
            }
            // Technically you can move faster by moving diagonally, but since movement is not the core of this project, it doesn't matter.

            if (KeyboardState.IsKeyDown(Keys.Space)) // Up
            {
                cameraPosition += up * movementSpeed * deltaTime;
            }

            if (KeyboardState.IsKeyDown(Keys.LeftShift)) // Down
            {
                cameraPosition -= up * movementSpeed * deltaTime;
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            CursorState = CursorState.Grabbed;

            // Background Color
            GL.ClearColor(0.2f, 0.4f, 0.6f, 0.1f);
            
            // Shader Setup
            string vertexShaderSource = File.ReadAllText(Path.Combine(
                AppContext.BaseDirectory,
                "Shaders",
                "vertexShader.glsl"
            ));

            string fragmentShaderSource = File.ReadAllText(Path.Combine(
                AppContext.BaseDirectory,
                "Shaders",
                "fragmentShader.glsl"
            ));

            int vertexShader = ShaderManager.CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = ShaderManager.CompileShader(ShaderType.FragmentShader, fragmentShaderSource);
            shaderProgram = ShaderManager.LinkShaders(vertexShader, fragmentShader);

            // VBO, VAO and EBO Setup
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            EBO = GL.GenBuffer();

            // Bind VAO
            GL.BindVertexArray(VAO);

            // Bind VBO and bind vertices
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Bind EBO
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indicies.Length * sizeof(uint), indicies, BufferUsageHint.StaticDraw);

            // Configure vertex attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Unbind VBO & VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // WIREFRAME MODE
            if (WIREFRAME_MODE)
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VAO);

            lookVector.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            lookVector.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            lookVector.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));

            view = Matrix4.LookAt(cameraPosition, cameraPosition + lookVector, up);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), ClientSize.X / (float)ClientSize.Y, 0.1f, 100.0f);

            GL.UseProgram(shaderProgram);

            ShaderManager.SetMatrix4(shaderProgram, "view", view);
            ShaderManager.SetMatrix4(shaderProgram, "projection", projection);

            // Chunk Rendering (will use later)
            
            /*for (uint x = 0; x < CHUNK_SIZE.X; x++)
            {
                for (uint z = 0; z < CHUNK_SIZE.Z; z++)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    model = Matrix4.CreateTranslation(pos);
                    ShaderManager.SetMatrix4(shaderProgram, "model", model);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }*/


            model = Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
            ShaderManager.SetMatrix4(shaderProgram, "model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            model = Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
            ShaderManager.SetMatrix4(shaderProgram, "model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            SwapBuffers();
        }

        protected override void OnMouseMove(MouseMoveEventArgs mouse)
        {
            base.OnMouseMove(mouse);

            Vector2 deltaPos = new Vector2(mouse.X - lastMousePos.X, mouse.Y - lastMousePos.Y);
            yaw += deltaPos.X * sensitivity;
            pitch -= deltaPos.Y * sensitivity;

            lastMousePos = new Vector2(mouse.X, mouse.Y);
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
            GL.DeleteProgram(shaderProgram);
        }
    }
}
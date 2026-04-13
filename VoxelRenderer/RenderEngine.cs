using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            // Background Color
            GL.ClearColor(0.2f, 0.4f, 0.6f, 0.1f);

            // Coordinate Matricies
            model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-45.0f)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(30.0f));
            view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), ClientSize.X / (float)ClientSize.Y, 0.1f, 100.0f);
            
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

            GL.UseProgram(shaderProgram);

            Console.WriteLine(GL.GetUniformLocation(shaderProgram, "model"));

            ShaderManager.SetMatrix4(shaderProgram, "model", model);
            ShaderManager.SetMatrix4(shaderProgram, "view", view);
            ShaderManager.SetMatrix4(shaderProgram, "projection", projection);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //GL.DrawElements(PrimitiveType.Triangles, indicies.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            // Update the projection matrix, otherwise the cube will appear stretched.
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), e.Width / (float)e.Height, 0.1f, 100.0f);
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
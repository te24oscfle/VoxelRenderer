global using OpenTK.Graphics.OpenGL4;

using VoxelRenderer;

using (RenderEngine renderEngine = new RenderEngine(800, 600, "VoxelRenderer"))
{
    renderEngine.Run();
}
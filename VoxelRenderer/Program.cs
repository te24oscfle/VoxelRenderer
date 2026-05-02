global using OpenTK.Graphics.OpenGL4;
global using OpenTK.Mathematics;
global using OpenTK.Windowing.Common;
global using OpenTK.Windowing.Desktop;
global using OpenTK.Windowing.GraphicsLibraryFramework;
global using StbImageSharp;

global using VoxelRenderer.Classes;

using VoxelRenderer;

using (RenderEngine renderEngine = new RenderEngine(800, 600, "VoxelRenderer"))
{
    renderEngine.Run();
}
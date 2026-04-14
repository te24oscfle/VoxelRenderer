using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VoxelRenderer.Classes
{
    static class CameraManager
    {
        // Camera Config
        public static float Speed = 6.5f;
        public static float Sensitivity = 0.1f;
        public static float FOV = 65.0f;

        // Public Camera fields
        public static Vector3 Position = new Vector3(0.0f, 1.0f, -2.0f);
        public static Vector3 LookVector = new Vector3();
        
        // Private Camera fields
        private static float yaw = -90.0f;
        private static float pitch = 0.0f;

        private static Vector2 lastMousePosition;

        private static void UpdateYawAndPitch(Vector2 deltaPosition)
        {
            yaw += deltaPosition.X * Sensitivity;
            pitch -= deltaPosition.Y * Sensitivity;
        }

        private static void UpdateLookVector()
        {
            LookVector.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            LookVector.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            LookVector.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
        }
        public static void OnMouseMove(MouseMoveEventArgs mouse)
        {
            Vector2 deltaPos = new Vector2(mouse.X - lastMousePosition.X, mouse.Y - lastMousePosition.Y);
            UpdateYawAndPitch(deltaPos);
            UpdateLookVector();
            lastMousePosition = new Vector2(mouse.X, mouse.Y);
        }
    }
}

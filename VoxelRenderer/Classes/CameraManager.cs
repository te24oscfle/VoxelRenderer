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
        public static Vector3 Position = new Vector3(8.0f, 26.0f, 8.0f);
        public static Vector3 LookVector = new Vector3(0.0f, 0.0f, -1.0f);
        
        // Private Camera fields
        private static float yaw = -90.0f;
        private static float pitch = 0.0f;

        private static Vector2 lastMousePosition;

        private static void UpdateYawAndPitch(Vector2 deltaPosition)
        {
            float deltaYaw = deltaPosition.X * Sensitivity;
            float deltaPitch = deltaPosition.Y * Sensitivity;
            
            yaw += deltaYaw;

            if (pitch - deltaPitch < -89.0f) {
                pitch = -89.0f;
            } 
            else if (pitch - deltaPitch > 89.0f)
            {
                pitch = 89.0f;
            }
            else
            {
                pitch -= deltaPitch;
            }
        }
        
        private static void UpdateLookVector()
        {
            LookVector.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            LookVector.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            LookVector.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
        }

        public static void UpdatePosition(KeyboardState keyboardState, float deltaTime)
        {
            // Camera Movement
            if (keyboardState.IsKeyDown(Keys.W)) // Forward
            {
                Position += LookVector * Speed * deltaTime;
            }

            if (keyboardState.IsKeyDown(Keys.S)) // Backward
            {
                Position -= LookVector * Speed * deltaTime;
            }

            if (keyboardState.IsKeyDown(Keys.D)) // Right
            {
                Position += Vector3.Normalize(Vector3.Cross(LookVector, Vector3.UnitY)) * Speed * deltaTime;
            }

            if (keyboardState.IsKeyDown(Keys.A)) // Left
            {
                Position -= Vector3.Normalize(Vector3.Cross(LookVector, Vector3.UnitY)) * Speed * deltaTime;
            }
            // Technically you can move faster by moving diagonally, but since movement is not the core of this project, it doesn't matter.

            if (keyboardState.IsKeyDown(Keys.Space)) // Up
            {
                Position += Vector3.UnitY * Speed * deltaTime;
            }

            if (keyboardState.IsKeyDown(Keys.LeftShift)) // Down
            {
                Position -= Vector3.UnitY * Speed * deltaTime;
            }
        }

        public static Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + LookVector, Vector3.UnitY);
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

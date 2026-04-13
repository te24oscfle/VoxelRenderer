using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxelRenderer.Classes
{
    internal class ShaderManager
    {
        public static int CompileShader(ShaderType shaderType, string source)
        {
            // Build and compile shader
            int shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            // Check if anything went wrong
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"Something went wrong when compiling shader {shader}");
                Console.WriteLine(infoLog);
            }

            return shader;
        }

        public static int LinkShaders(int vertexShader, int fragmentShader)
        {
            // Attach shaders and link program
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            // Check if anything went wrong
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"Something went wrong when linking program {program} with shaders {vertexShader} & {fragmentShader}");
                Console.WriteLine(infoLog);
            }

            // Cleanup
            // Once shaders have been linked to the program, they are essentially useless.
            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        public static void SetMatrix4(int shaderProgram, string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(shaderProgram, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }
    }
}

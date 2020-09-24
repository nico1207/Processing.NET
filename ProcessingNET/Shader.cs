using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ProcessingNET
{
    public class Shader
    {
        public int ProgramId;

        private Dictionary<string, int> uniformDictionary;

        public static Shader FromFiles(string vertexShaderPath, string fragmentShaderPath)
        {
            if (!File.Exists(vertexShaderPath))
                throw new FileNotFoundException("Vertex Shader not found at specified path.", vertexShaderPath);
            if (!File.Exists(fragmentShaderPath))
                throw new FileNotFoundException("Fragment Shader not found at specified path.", fragmentShaderPath);

            string vertexSource = File.ReadAllText(vertexShaderPath);
            string fragmentSource = File.ReadAllText(fragmentShaderPath);

            return new Shader(vertexSource, fragmentSource);
        }

        public static Shader FromSource(string vertexSource, string fragmentSource)
        {
            return new Shader(vertexSource, fragmentSource);
        }

        private Shader(string vertexSource, string fragmentSource)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexCompileStatus);
            if (vertexCompileStatus != 1)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new ShaderCompilationException(infoLog);
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragmentCompileStatus);
            if (fragmentCompileStatus != 1)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new ShaderCompilationException(infoLog);
            }

            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertexShader);
            GL.AttachShader(ProgramId, fragmentShader);
            GL.LinkProgram(ProgramId);
            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out int programLinkStatus);
            if (programLinkStatus != 1)
            {
                string infoLog = GL.GetProgramInfoLog(ProgramId);
                throw new ShaderCompilationException(infoLog);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            uniformDictionary = new Dictionary<string, int>();
        }

        /// <summary>
        /// Use this shader for rendering.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        /// <summary>
        /// Set uniform bool value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetBool(string uniformName, bool value)
        {
            if (!uniformDictionary.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    uniformDictionary.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform1(uniformDictionary[uniformName], value ? 1 : 0);
        }

        /// <summary>
        /// Set uniform float value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetFloat(string uniformName, float value)
        {
            if (!uniformDictionary.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    uniformDictionary.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }
            
            GL.Uniform1(uniformDictionary[uniformName], value);
        }

        /// <summary>
        /// Set uniform int value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetInteger(string uniformName, int value)
        {
            if (!uniformDictionary.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    uniformDictionary.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }
                
            GL.Uniform1(uniformDictionary[uniformName], value);
        }

        /// <summary>
        /// Set uniform Vector3 value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetVector3(string uniformName, Vector3 value)
        {
            if (!uniformDictionary.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    uniformDictionary.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform3(uniformDictionary[uniformName], value);
        }

        /// <summary>
        /// Set uniform Vector2 value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetVector2(string uniformName, Vector2 value)
        {
            if (!uniformDictionary.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    uniformDictionary.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform2(uniformDictionary[uniformName], value);
        }

        /// <summary>
        /// Set uniform Matrix4 value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetMatrix4(string uniformName, Matrix4 value, bool transpose = false)
        {
            if (!uniformDictionary.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    uniformDictionary.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.UniformMatrix4(uniformDictionary[uniformName], transpose, ref value);
        }
    }

    public class ShaderCompilationException : Exception
    {
        public ShaderCompilationException() : base() { }
        public ShaderCompilationException(string message) : base(message) { }
        public ShaderCompilationException(string message, Exception inner) : base(message, inner) { }
    }
}

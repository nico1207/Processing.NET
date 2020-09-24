using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ProcessingNET
{
    public class ComputeShader
    {
        private int programId;
        private Dictionary<string, int> uniformDictionary;

        public ComputeShader(string source)
        {
            int computeShader = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(computeShader, source);
            GL.CompileShader(computeShader);
            GL.GetShader(computeShader, ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus != 1)
            {
                string infoLog = GL.GetShaderInfoLog(computeShader);
                throw new ShaderCompilationException(infoLog);
            }

            programId = GL.CreateProgram();
            GL.AttachShader(programId, computeShader);
            GL.LinkProgram(programId);
            GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus != 1)
            {
                string infoLog = GL.GetProgramInfoLog(programId);
                throw new ShaderCompilationException(infoLog);
            }

            GL.DeleteShader(computeShader);

            uniformDictionary = new Dictionary<string, int>();
        }

        /// <summary>
        /// Use this shader for computing.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(programId);
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
                int location = GL.GetUniformLocation(programId, uniformName);
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
                int location = GL.GetUniformLocation(programId, uniformName);
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
                int location = GL.GetUniformLocation(programId, uniformName);
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
                int location = GL.GetUniformLocation(programId, uniformName);
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
                int location = GL.GetUniformLocation(programId, uniformName);
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
                int location = GL.GetUniformLocation(programId, uniformName);
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
}

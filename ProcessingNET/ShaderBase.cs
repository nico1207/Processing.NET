using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ProcessingNET
{
    public abstract class ShaderBase
    {
        public static ShaderBase CurrentShader;

        public int ProgramId { get; set; }

        protected Dictionary<string, int> UniformLocations = new Dictionary<string, int>();
        protected App App;

        /// <summary>
        /// Use this shader for rendering.
        /// </summary>
        public void Use(bool setDefaultUniforms = true)
        {
            CurrentShader = this;
            GL.UseProgram(ProgramId);
            if (setDefaultUniforms)
                SetDefaultUniforms();
        }

        private void SetDefaultUniforms()
        {
            SetMatrix4("projectionMatrix", App.ProjectionMatrix);
        }

        /// <summary>
        /// Set uniform bool value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetBool(string uniformName, bool value)
        {
            if (CurrentShader != this)
                Use();

            if (!UniformLocations.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    UniformLocations.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform1(UniformLocations[uniformName], value ? 1 : 0);
        }

        /// <summary>
        /// Set uniform float value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetFloat(string uniformName, float value)
        {
            if (CurrentShader != this)
                Use();

            if (!UniformLocations.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    UniformLocations.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform1(UniformLocations[uniformName], value);
        }

        /// <summary>
        /// Set uniform int value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetInteger(string uniformName, int value)
        {
            if (CurrentShader != this)
                Use();

            if (!UniformLocations.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    UniformLocations.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform1(UniformLocations[uniformName], value);
        }

        /// <summary>
        /// Set uniform Vector3 value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetVector3(string uniformName, Vector3 value)
        {
            if (CurrentShader != this)
                Use();

            if (!UniformLocations.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    UniformLocations.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform3(UniformLocations[uniformName], value);
        }

        /// <summary>
        /// Set uniform Vector2 value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetVector2(string uniformName, Vector2 value)
        {
            if (CurrentShader != this)
                Use();

            if (!UniformLocations.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    UniformLocations.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.Uniform2(UniformLocations[uniformName], value);
        }

        /// <summary>
        /// Set uniform Matrix4 value.
        /// </summary>
        /// <param name="uniformName">Name of the uniform</param>
        /// <param name="value">Value that the uniform should be set to</param>
        public void SetMatrix4(string uniformName, Matrix4 value, bool transpose = false)
        {
            if (CurrentShader != this)
                Use();

            if (!UniformLocations.ContainsKey(uniformName))
            {
                int location = GL.GetUniformLocation(ProgramId, uniformName);
                if (location != -1)
                {
                    UniformLocations.Add(uniformName, location);
                }
                else
                {
                    return;
                }
            }

            GL.UniformMatrix4(UniformLocations[uniformName], transpose, ref value);
        }
    }

    public class ShaderCompilationException : Exception
    {
        public ShaderCompilationException() : base() { }
        public ShaderCompilationException(string message) : base(message) { }
        public ShaderCompilationException(string message, Exception inner) : base(message, inner) { }
    }
}

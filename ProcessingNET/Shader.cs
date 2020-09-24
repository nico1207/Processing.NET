using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ProcessingNET.Properties;

namespace ProcessingNET
{
    public class Shader : ShaderBase
    {
        public static Shader FromFiles(App app, string vertexShaderPath, string fragmentShaderPath)
        {
            if (!File.Exists(vertexShaderPath))
                throw new FileNotFoundException("Vertex Shader not found at specified path.", vertexShaderPath);
            if (!File.Exists(fragmentShaderPath))
                throw new FileNotFoundException("Fragment Shader not found at specified path.", fragmentShaderPath);

            string vertexSource = File.ReadAllText(vertexShaderPath);
            string fragmentSource = File.ReadAllText(fragmentShaderPath);

            return FromSource(app, vertexSource, fragmentSource);
        }

        public static Shader FromFiles(App app, string fragmentShaderPath)
        {
            if (!File.Exists(fragmentShaderPath))
                throw new FileNotFoundException("Fragment Shader not found at specified path.", fragmentShaderPath);

            string fragmentSource = File.ReadAllText(fragmentShaderPath);

            return FromSource(app, fragmentSource);
        }

        public static Shader FromSource(App app, string vertexSource, string fragmentSource)
        {
            return new Shader(app, vertexSource, fragmentSource);
        }

        public static Shader FromSource(App app, string fragmentSource)
        {
            return new Shader(app, System.Text.Encoding.Default.GetString(Resources.DefaultShader), fragmentSource);
        }

        private Shader(App app, string vertexSource, string fragmentSource)
        {
            App = app;

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
        }
    }
}

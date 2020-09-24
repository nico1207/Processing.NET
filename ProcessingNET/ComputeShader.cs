using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ProcessingNET
{
    public class ComputeShader : ShaderBase
    {
        public static ComputeShader FromFile(App app, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Compute Shader not found at specified path.", path);

            return new ComputeShader(app, File.ReadAllText(path));
        }

        public static ComputeShader FromSource(App app, string source)
        {
            return new ComputeShader(app, source);
        }

        private ComputeShader(App app, string source)
        {
            App = app;

            int computeShader = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(computeShader, source);
            GL.CompileShader(computeShader);
            GL.GetShader(computeShader, ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus != 1)
            {
                string infoLog = GL.GetShaderInfoLog(computeShader);
                throw new ShaderCompilationException(infoLog);
            }

            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, computeShader);
            GL.LinkProgram(ProgramId);
            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus != 1)
            {
                string infoLog = GL.GetProgramInfoLog(ProgramId);
                throw new ShaderCompilationException(infoLog);
            }

            GL.DeleteShader(computeShader);
        }

        public static void Dispatch(int workGroupAmountX, int workGroupAmountY, int workGroupAmountZ)
        {
            GL.DispatchCompute(workGroupAmountX, workGroupAmountY, workGroupAmountZ);
        }
    }
}

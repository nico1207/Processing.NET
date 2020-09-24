using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ProcessingNET.Properties;
using RectpackSharp;
using SixLabors.Fonts;
using Font = System.Drawing.Font;
using FontFamily = System.Drawing.FontFamily;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace ProcessingNET
{
    public partial class App
    {
        private Dictionary<char, Rectangle> characterPositions;
        private Dictionary<char, Vector4> characterUvs;
        private Shader sdfShader;
        private int fontAtlasTexture;
        private float fontHeight;

        private unsafe void GenerateFontAtlas()
        {
            Log("Generating font atlas...");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int fontSize = 256;
            int glyphStart = 32;
            int glyphEnd = 127;
            int border = 32;

            PrivateFontCollection coll = new PrivateFontCollection();
            fixed (byte* fontData = Resources.Helvetica)
            {
                coll.AddMemoryFont(new IntPtr(fontData), Resources.Helvetica.Length);
            }

            FontFamily family = new FontFamily("Helvetica", coll);
            Font ffont = new Font(family, fontSize);
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));

            PackingRectangle[] rectangles = new PackingRectangle[glyphEnd - glyphStart + 1];
            for (int i = glyphStart; i <= glyphEnd; i++)
            {
                var size = g.MeasureString(((char)i).ToString(), ffont, PointF.Empty, StringFormat.GenericTypographic);
                rectangles[i - glyphStart] = new PackingRectangle(0, 0, (uint)(size.Width + 2 * border), (uint)(size.Height + 2 * border), i);
            }

            RectanglePacker.Pack(rectangles, out PackingRectangle bounds);

            Bitmap bitmap = new Bitmap((int)bounds.Width, (int)bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);

            characterPositions = new Dictionary<char, Rectangle>();
            characterUvs = new Dictionary<char, Vector4>();
            foreach (PackingRectangle rectangle in rectangles)
            {
                fontHeight = rectangle.Height;
                characterPositions.Add((char)rectangle.Id, new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height));
                characterUvs.Add((char)rectangle.Id, new Vector4((float)rectangle.X / bounds.Width, (float)rectangle.Y / bounds.Height, (float)(rectangle.X + rectangle.Width) / bounds.Width, (float)(rectangle.Y + rectangle.Height) / bounds.Height));
                graphics.DrawString(((char)rectangle.Id).ToString(), ffont, Brushes.White, new RectangleF(rectangle.X + border, rectangle.Y + border, rectangle.Width, rectangle.Height), StringFormat.GenericTypographic);
            }

            ComputeShader setupBufferShader = new ComputeShader(Encoding.Default.GetString(Resources.SDFSetupBuffer));

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int pixels = bitmapData.Width * bitmapData.Height;

            int inputSSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, inputSSBO);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, pixels * 4, bitmapData.Scan0, BufferUsageHint.StreamDraw);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, inputSSBO);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            bitmap.UnlockBits(bitmapData);

            Vector4[] data = new Vector4[pixels];
            int dataSSBO = 0;
            dataSSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, dataSSBO);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, pixels * 16, data, BufferUsageHint.StaticRead);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, dataSSBO);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            setupBufferShader.Use();
            setupBufferShader.SetInteger("imageWidth", bitmap.Width);
            GL.DispatchCompute(pixels, 1, 1);

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            ComputeShader sdfComputeShader = new ComputeShader(Encoding.Default.GetString(Resources.SDFCompute));
            int steps = (int)MathF.Floor(MathF.Log2(Math.Min(bitmap.Width, bitmap.Height)));
            int sampleOffset = (int)MathF.Pow(2, steps - 1);

            for (int step = 0; step < steps; step++)
            {
                sdfComputeShader.Use();
                sdfComputeShader.SetInteger("imageWidth", bitmap.Width);
                sdfComputeShader.SetInteger("sampleOffset", sampleOffset);
                GL.DispatchCompute(pixels, 1, 1);
                GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
                sampleOffset /= 2;
            }

            ComputeShader distanceOutputShader = new ComputeShader(Encoding.Default.GetString(Resources.SDFDistance));

            fontAtlasTexture = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, fontAtlasTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, bitmap.Width, bitmap.Height, 0, PixelFormat.Red, PixelType.Byte, new IntPtr(0));
            GL.BindImageTexture(0, fontAtlasTexture, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.R8);

            distanceOutputShader.Use();
            distanceOutputShader.SetInteger("imageWidth", bitmap.Width);
            distanceOutputShader.SetInteger("maxDistanceOutside", border);
            distanceOutputShader.SetInteger("maxDistanceInside", border);
            GL.DispatchCompute(pixels, 1, 1);

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            GL.DeleteBuffer(inputSSBO);
            GL.DeleteBuffer(dataSSBO);

            watch.Stop();
            Log($"Font Atlas with size {bitmap.Width}x{bitmap.Height} finished! Took {watch.Elapsed.TotalMilliseconds}ms!");

            sdfShader = Shader.FromSource(Encoding.Default.GetString(Resources.SDFVertShader), Encoding.Default.GetString(Resources.SDFFragShader));
        }

        /// <summary>
        /// Draw a string.
        /// </summary>
        /// <param name="text">Text that will be drawn</param>
        /// <param name="x">X-Position of the text</param>
        /// <param name="y">Y-Position of the text</param>
        protected void DrawString(string text, float x, float y, float size = 32f)
        {
            float scale = size / fontHeight;

            sdfShader.Use();
            GL.GetFloat(GetPName.ProjectionMatrix, out Matrix4 projMatrix);
            sdfShader.SetMatrix4("projection", projMatrix);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, fontAtlasTexture);
            sdfShader.SetInteger("fontAtlas", 0);
            sdfShader.SetFloat("smoothing", 8f / size);

            int texcoordAttribLocation = GL.GetAttribLocation(sdfShader.ProgramId, "texcoord");

            float currentX = x;
            GL.Begin(PrimitiveType.Quads);
            foreach (char textChar in text)
            {
                if (!characterPositions.ContainsKey(textChar))
                    continue;

                Rectangle rect = characterPositions[textChar];
                Vector4 uvs = characterUvs[textChar];
                //DrawRectangle(currentX, y, rect.Width * size, rect.Height * size);
                GL.VertexAttrib2(texcoordAttribLocation, uvs.X, uvs.W);
                GL.Vertex2(currentX, y);
                GL.VertexAttrib2(texcoordAttribLocation, uvs.Z, uvs.W);
                GL.Vertex2(currentX + rect.Width * scale, y);
                GL.VertexAttrib2(texcoordAttribLocation, uvs.Z, uvs.Y);
                GL.Vertex2(currentX + rect.Width * scale, y + rect.Height * scale);
                GL.VertexAttrib2(texcoordAttribLocation, uvs.X, uvs.Y);
                GL.Vertex2(currentX, y + rect.Height * scale);
                currentX += rect.Width * scale;
            }
            GL.End();
        }

        /// <summary>
        /// Draw a string.
        /// </summary>
        /// <param name="text">Text that will be drawn</param>
        /// <param name="position">Position of the text</param>
        protected void DrawString(string text, Vector2 position)
        {
            DrawString(text, position.X, position.Y);
        }
    }
}

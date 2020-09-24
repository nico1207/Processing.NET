using OpenTK.Mathematics;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL;
using ProcessingNET.Properties;
using SixLabors.Fonts;

namespace ProcessingNET
{
    public partial class App
    {
        public FontAtlas DefaultFontAtlas { get; set; }

        private FontAtlas currentFontAtlas;
        private Shader textShader;

        private void GenerateDefaultFontAtlas()
        {
            FontCollection fontCollection = new FontCollection();
            using MemoryStream stream = new MemoryStream(Resources.Helvetica);
            FontFamily builtinFont = fontCollection.Install(stream);

            DefaultFontAtlas = new FontAtlas(builtinFont, 128);
            currentFontAtlas = DefaultFontAtlas;
            textShader = Shader.FromSource(this, System.Text.Encoding.Default.GetString(Resources.TextShader));
        }

        /// <summary>
        /// Draw a string.
        /// </summary>
        /// <param name="text">Text that will be drawn</param>
        /// <param name="x">X-Position of the text</param>
        /// <param name="y">Y-Position of the text</param>
        protected void DrawString(string text, float x, float y, float size = 32f)
        {
            float scale = size / currentFontAtlas.Size;

            ShaderBase previousShader = ShaderBase.CurrentShader;

            textShader.Use();
            textShader.SetVector3("textColor", new Vector3(Color.R, Color.G, Color.B));

            int texcoordAttribLocation = GL.GetAttribLocation(textShader.ProgramId, "aTexcoord");

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, currentFontAtlas.TextureId);
            GL.Begin(PrimitiveType.Quads);
            float currentX = x;
            foreach (char character in text)
            {
                if (!currentFontAtlas.Characters.ContainsKey(character))
                    continue;

                CharacterData data = currentFontAtlas.Characters[character];

                GL.VertexAttrib2(texcoordAttribLocation, data.uvX1, data.uvY2);
                GL.Vertex2(currentX, y);
                GL.VertexAttrib2(texcoordAttribLocation, data.uvX2, data.uvY2);
                GL.Vertex2(currentX + data.xAdvance * scale, y);
                GL.VertexAttrib2(texcoordAttribLocation, data.uvX2, data.uvY1);
                GL.Vertex2(currentX + data.xAdvance * scale, y + data.height * scale);
                GL.VertexAttrib2(texcoordAttribLocation, data.uvX1, data.uvY1);
                GL.Vertex2(currentX, y + data.height * scale);

                currentX += data.xAdvance * scale;
            }
            
            GL.End();

            previousShader?.Use();
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

        protected void SetFont(FontAtlas fontAtlas)
        {
            currentFontAtlas = fontAtlas;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using RectpackSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ProcessingNET
{
    public class FontAtlas
    {
        public int TextureId { get; set; }
        public Dictionary<char, CharacterData> Characters { get; set; }
        public int Size { get; set; }

        public FontAtlas(FontFamily fontFamily, int fontSize) : this(fontFamily, fontSize,
            Enumerable.Range(32, 127 - 32))
        {

        }

        public unsafe FontAtlas(FontFamily fontFamily, int fontSize, IEnumerable<int> characters)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Size = fontSize;
            Font font = new Font(fontFamily, fontSize);
            float factor = (float)fontSize / font.EmSize;
            List<PackingRectangle> glyphRectangles = new List<PackingRectangle>();

            foreach (int character in characters)
            {
                GlyphInstance glyph = font.GetGlyph(character).Instance;
                glyphRectangles.Add(new PackingRectangle(0, 0, (uint)((glyph.LeftSideBearing + glyph.AdvanceWidth) * factor), (uint)(glyph.Height * factor), character));
            }

            PackingRectangle[] rectArray = glyphRectangles.ToArray();
            RectanglePacker.Pack(rectArray, out PackingRectangle bounds);

            Image<A8> image = new Image<A8>((int)bounds.Width, (int)bounds.Height);

            Characters = new Dictionary<char, CharacterData>();
            image.Mutate(x =>
            {
                foreach (PackingRectangle glyphRectangle in rectArray)
                {
                    GlyphInstance glyph = font.GetGlyph(glyphRectangle.Id).Instance;
                    Characters.Add((char)glyphRectangle.Id, new CharacterData()
                    {
                        uvX1 = glyphRectangle.X / (float)image.Width,
                        uvX2 = (glyphRectangle.X + glyphRectangle.Width) / (float)image.Width,
                        uvY1 = glyphRectangle.Y / (float)image.Height,
                        uvY2 = (glyphRectangle.Y + glyphRectangle.Height) / (float)image.Height,
                        xAdvance = glyph.AdvanceWidth * factor,
                        height = glyph.Height * factor
                    });
                    x.DrawText(((char) glyphRectangle.Id).ToString(), font, Color.White,
                        new PointF(glyphRectangle.X, glyphRectangle.Y));
                }
            });

            image.Save("atlas.png", new PngEncoder());

            image.TryGetSinglePixelSpan(out Span<A8> pixels);

            TextureId = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            fixed (A8* pixelPointer = pixels)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, image.Width, image.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, new IntPtr(pixelPointer));
            }
            GL.GenerateTextureMipmap(TextureId);

            watch.Stop();
            Console.WriteLine($"Font atlas with size {image.Width}x{image.Height} was created in {watch.ElapsedMilliseconds}ms!");
        }
    }

    public struct CharacterData
    {
        public float uvX1;
        public float uvX2;
        public float uvY1;
        public float uvY2;
        public float xAdvance;
        public float height;
    }
}

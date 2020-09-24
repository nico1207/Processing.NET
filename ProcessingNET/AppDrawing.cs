using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ProcessingNET
{
    public abstract partial class App
    {
        public Matrix4 ProjectionMatrix { get; set; }
        public Color4 Color { get; set; }

        /// <summary>
        /// Clear screen with background color.
        /// </summary>
        /// <param name="color"></param>
        protected void ClearBackground(Color4 color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        #region Shapes

        /// <summary>
        /// Draw a rectangle.
        /// </summary>
        /// <param name="x">X-Position of the rectangle's upper left corner</param>
        /// <param name="y">Y-Position of the rectangle's upper left corner</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        protected void DrawRectangle(float x, float y, float width, float height)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + width, y);
            GL.Vertex2(x + width, y + height);
            GL.Vertex2(x, y + height);
            GL.End();
        }

        /// <summary>
        /// Draw a rectangle.
        /// </summary>
        /// <param name="position">Position of the rectangle's upper left corner</param>
        /// <param name="size">Size of the rectangle</param>
        protected void DrawRectangle(Vector2 position, Vector2 size)
        {
            DrawRectangle(position.X, position.Y, size.X, size.Y);
        }

        /// <summary>
        /// Draw a circle.
        /// </summary>
        /// <param name="x">X-Position of the circle's center</param>
        /// <param name="y">Y-Position of the circle's center</param>
        /// <param name="radius">Radius of the circle</param>
        protected void DrawCircle(float x, float y, float radius)
        {
            int steps = (int) Math.Clamp(MathHelper.TwoPi * radius / 10f, 20f, 200f);
            float angleStep = MathHelper.TwoPi / steps;

            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(x, y);
            for (int i = 0; i <= steps; ++i)
            {
                float angle = angleStep * i;
                GL.Vertex2(x + MathF.Cos(angle) * radius, y + MathF.Sin(angle) * radius);
            }
            GL.End();
        }

        /// <summary>
        /// Draw a circle.
        /// </summary>
        /// <param name="center">Position of the circle's center</param>
        /// <param name="radius">Radius of the circle</param>
        protected void DrawCircle(Vector2 center, float radius)
        {
            DrawCircle(center.X, center.Y, radius);
        }

        #endregion
    }
}

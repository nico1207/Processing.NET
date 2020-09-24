using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProcessingNET;

namespace TestApp
{
    public class TestApp : App
    {

        private Vector2 pos;
        private int size = 200;
        private float speed = 200f;
        private Shader testShader;
        private int currentFps;
        private float time;

        protected override void Configure()
        {
            Settings.FramesPerSecond = 144;
            Settings.Window.Title = "Test App";
            Settings.DebugLog = true;
            Settings.MSAASamples = 4;
        }

        protected override void Start()
        {
            testShader = Shader.FromFiles(this, "Shaders/testShader.vert", "Shaders/testShader.frag");
        }

        protected override void Update(float deltaTime)
        {
            currentFps = (int) (1.0f / deltaTime);
            time += deltaTime;


            //pos += spd * deltaTime;

            if (Window.KeyboardState[Key.W])
            {
                pos.Y += speed * deltaTime;
            }

            if (Window.KeyboardState[Key.A])
            {
                pos.X -= speed * deltaTime;
            }

            if (Window.KeyboardState[Key.S])
            {
                pos.Y -= speed * deltaTime;
            }

            if (Window.KeyboardState[Key.D])
            {
                pos.X += speed * deltaTime;
            }

            if (pos.X < size)
            {
                pos.X = size;
            }

            if (pos.X > Settings.Window.Width - size)
            {
                pos.X = Settings.Window.Width - size;
            }

            if (pos.Y < size)
            {
                pos.Y = size;
            }

            if (pos.Y > Settings.Window.Height - size)
            {
                pos.Y = Settings.Window.Height - size;
            }
        }

        protected override void Render()
        {
            ClearBackground(Color4.Black);

            //testShader.Use();
            //DrawCircle(pos, size);

            DrawString("FPS: " + currentFps, 0, Settings.Window.Height - 16, 16);
        }
    }
}

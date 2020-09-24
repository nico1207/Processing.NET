using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProcessingNET.Properties;
using RectpackSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace ProcessingNET
{
    public abstract partial class App
    {
        public AppSettings Settings { get; set; } = new AppSettings();

        protected GameWindow Window;

        public void Run()
        {
            Configure();

            InitializeWindow();
        }

        protected virtual void Configure() { }
        protected virtual void Start() { }
        protected virtual void Update(float deltaTime) { }
        protected virtual void Render() { }

        private void InitializeWindow()
        {
            Log("Initializing Window...");

            //GLFW.Init();
            //GLFW.WindowHint(WindowHintInt.Samples, Settings.MSAASamples);

            Window = new GameWindow(new GameWindowSettings()
            {
                IsMultiThreaded = true, 
                RenderFrequency = Settings.FramesPerSecond, 
                UpdateFrequency = Settings.FramesPerSecond
            }, new NativeWindowSettings()
            {
                APIVersion = new Version(4, 6),
                API = ContextAPI.OpenGL,
                AutoLoadBindings = true,
                IsFullscreen = Settings.Window.Fullscreen,
                Size = new Vector2i(Settings.Window.Width, Settings.Window.Height),
                Profile = ContextProfile.Compatability,
                Title = Settings.Window.Title,
                WindowBorder = WindowBorder.Fixed,
                WindowState = Settings.Window.Fullscreen ? WindowState.Fullscreen : WindowState.Normal,
                NumberOfSamples = Settings.MSAASamples
            });

            Window.Load += OnLoad;
            Window.UpdateFrame += OnUpdateFrame;
            Window.RenderFrame += OnRenderFrame;
            Window.KeyDown += OnKeyDown;
            Window.KeyUp += OnKeyUp;
            Window.MouseDown += OnMouseDown;
            Window.MouseUp += OnMouseUp;
            Window.MouseMove += OnMouseMove;

            Window.Run();
        }

        private void OnUpdateFrame(FrameEventArgs e)
        {
            Update((float)e.Time);
        }

        private void OnRenderFrame(FrameEventArgs e)
        {
            Matrix4 projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Settings.Window.Width, 0, Settings.Window.Height, 0f, 1f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);
            Render();
            Window.SwapBuffers();
        }

        private void OnLoad()
        {
            GenerateFontAtlas();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Start();
        }

        protected void Log(string message)
        {
            if (Settings.DebugLog)
            {
                Console.WriteLine(message);
            }
        }
    }
}

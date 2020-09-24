using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingNET
{
    public class AppSettings
    {
        public WindowSettings Window { get; set; } = new WindowSettings();

        public int MSAASamples { get; set; } = 4;
        public float FramesPerSecond { get; set; } = 60;
        public bool DebugLog { get; set; } = false;
    }

    public class WindowSettings
    {
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;
        public string Title { get; set; } = "Processing.NET App";
        public bool Fullscreen { get; set; } = false;
    }
}

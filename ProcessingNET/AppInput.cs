using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ProcessingNET
{
    public partial class App
    {
        protected virtual void OnKeyDown(KeyboardKeyEventArgs e) { }
        protected virtual void OnKeyUp(KeyboardKeyEventArgs e) { }
        protected virtual void OnMouseMove(MouseMoveEventArgs obj) { }
        protected virtual void OnMouseUp(MouseButtonEventArgs obj) { }
        protected virtual void OnMouseDown(MouseButtonEventArgs obj) { }

        /// <summary>
        /// Returns whether the specified key is currently pressed.
        /// </summary>
        /// <param name="key">The key that should be checked</param>
        /// <returns></returns>
        protected bool IsKeyDown(Keys key) => Window.IsKeyDown(key);

        /// <summary>
        /// Returns whether the specified mouse button is currently pressed.
        /// </summary>
        /// <param name="button">The mouse button that should be checked</param>
        /// <returns></returns>
        protected bool IsMouseButtonDown(MouseButton button) => Window.IsMouseButtonDown(button);
    }
}

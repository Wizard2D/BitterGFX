using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using System.Runtime.InteropServices;
using SharpDX.Windows;
using SharpDX.Mathematics;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using System.IO;
using SharpDX.DXGI;
using BitterGFX;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace BitterGFX.Samples
{
    class Program2
    {
        [STAThread]
        static void Main(string[] args)
        {
            var notFull = BitterGFX.BeginWindow("Example : Rectangle", 500, 600); //Create Window.
            var form = notFull.actualWindow; //It's renderform
            //Set variables for some resources.
            SwapChain swapChain = notFull.swapChain;
            Device device = notFull.device;
            var d2dFactory = notFull.factory;
            int width = form.ClientSize.Width;
            int height = form.ClientSize.Height;
            Factory factory = notFull.normalFactory; //(DXGI factory.)
            //new RenderTargetView from backBuffer.
            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);
            Surface surface = backBuffer.QueryInterface<Surface>();
            var d2dRenderTarget = new RenderTarget(d2dFactory, surface,
            new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
            var solidColorBrush = new SolidColorBrush(d2dRenderTarget, Color.White);
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            // Main loop.
            RenderLoop.Run(form, () =>
            {
                d2dRenderTarget.BeginDraw();
                d2dRenderTarget.Clear(Color.Lerp(Color.Turquoise, Color.Teal, 0.1f));
                new Rectangle().UpdateRectangle(d2dFactory, d2dRenderTarget, 0, 0, 0, 0, SharpDX.Color.AntiqueWhite, 25, 25);
                d2dRenderTarget.EndDraw();
                //Present.
                swapChain.Present(0, PresentFlags.None);
            });
            //Dispose Resources.
            renderView.Dispose();
            backBuffer.Dispose();
            device.ImmediateContext.ClearState();
            device.ImmediateContext.Flush();
            device.Dispose();
            device.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }
    }
}
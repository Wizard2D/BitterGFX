using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Windows;
using SharpDX.Mathematics;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using System.IO;
using SharpDX.DXGI;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;

namespace BitterGFX
{
    public class BGFXWindow
    {
       public int xSize;
       public int ySize;
       public string name;
       public RenderForm actualWindow;
       public Device device;
       public SwapChain swapChain;
       public Factory normalFactory;
       public SharpDX.Direct2D1.Factory factory;
        public void CloseWindow()
        {
            actualWindow.Close();
        }
        public BGFXWindow(string sname, int x, int y)
        {
            name = sname;
            actualWindow = new RenderForm(name);
            
            var form = actualWindow;
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                                   new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                                       new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 }, desc, out device, out swapChain);
            var d2dFactory = new SharpDX.Direct2D1.Factory();
            factory = d2dFactory;
            Factory normfactory = swapChain.GetParent<Factory>();
            normalFactory = normfactory;
            normfactory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);
            actualWindow.Size = new System.Drawing.Size(new System.Drawing.Point(x, y));
        }
    }
}

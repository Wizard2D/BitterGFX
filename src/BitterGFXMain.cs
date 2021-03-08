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

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using Bitmap = SharpDX.Direct2D1.Bitmap;
namespace BitterGFX
{
    public static class HelperFunctions
    {
        public static SharpDX.Direct2D1.Bitmap LoadFromFile(RenderTarget renderTarget, string file)
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file))
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;

                    return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
                }
            }
        }
    }
    public abstract class BitterGFX
    {
        public static BGFXWindow BeginWindow(string name, int x, int y)
        {
            return new BGFXWindow(name, x, y);
        }
    }

    public class Drawable
    {
       public int xPos;
        public int yPos;
        public int xSize;
        public int ySize;
        public Rect rect;
    }
    public class Rect
    {
      public float width;
      public float height;
      public float top;
      public float left;
      public Rect(float l, float t, float w, float h)
      {
         width = w;
         height = h;
         left = l;
         top = t;
      }
    }
    public abstract class Image : Drawable
    {
        public static string filePath;
       public static Bitmap usingBitmap;
        /*
            <summary>
                No need to place DrawBMP inside of a BeginDraw, since it already does it for you... nvm.
            </summary>
        */
        public static void DrawBMP(RenderTarget rndTarget, string fileePath)
        {
            filePath = fileePath;
            usingBitmap = HelperFunctions.LoadFromFile(rndTarget, filePath);
            rndTarget.DrawBitmap(usingBitmap, 1.0f, BitmapInterpolationMode.NearestNeighbor);
        }
    }
    public class Rectangle : Drawable
    {
        SharpDX.Color rectColor;
       
        public void UpdateRectangle(SharpDX.Direct2D1.Factory d2dFactory, RenderTarget d2dRenderTarget, int xPos, int yPos, float top, float left, SharpDX.Color color, float r, float b)
        {
            var rectangleGeometry = new RectangleGeometry(d2dFactory, new RawRectangleF(left, top, r, b));
            this.rect = new Rect(left, top, r, b);
            var solidColorBrush = new SolidColorBrush(d2dRenderTarget, Color.White);
            solidColorBrush.Color = color;
            d2dRenderTarget.FillGeometry(rectangleGeometry, solidColorBrush, null);
        }
    }
    public class bVec3
    {
       public int x;
       public int y;
       public int z;
       public bVec3(int xs, int ys, int zs)
       {
            x = xs;
            y = ys;
            z = zs;
       }
    }
    /*public class Camera
    {
        public bVec3 rotation = new bVec3(0,0,-1);
        public bVec3 position = new bVec3(605,0,-1);
        public bVec3 offset = new bVec3(0, 0, -1);
        public int fov = 65;

        public void SetCamera(RenderForm form) //Place in Loop.
        {
            var camMatrix = Matrix.LookAtLH(new Vector3(position.x, position.y,position.z), new Vector3(offset.x, offset.y,offset.z),Vector3.Right);
            Matrix proj = Matrix.Identity;
            proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
            var viewProj = Matrix.Multiply(camMatrix, proj);
            var worldViewProj = 1 * viewProj;
            worldViewProj.Transpose();
        } 
    }*/ //Lmao making a camera is a mistake, don't do it.
    public abstract class BGFXKeyboard
    {
        public static string key = "";
        public static void hookKeyEvent(RenderForm form)
        {
            form.KeyDown += (s, e) =>
             {
                 key = e.KeyCode.ToString();
             };
        }
    }
}

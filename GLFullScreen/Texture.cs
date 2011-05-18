using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.ApplicationServices;
using Monobjc.CoreFoundation;
using Monobjc.Foundation;
using Monobjc.OpenGL;
using System.Runtime.InteropServices;

namespace GLFullScreen
{
	[ObjectiveCClass]
	public partial class Texture : NSObject
	{
		public static readonly Class TextureClass = Class.Get (typeof(Texture));

		private const int TEXTURE_WIDTH = 1024;
		private const int TEXTURE_HEIGHT = 512;

		private uint texId;
		private uint pboId;
		private IntPtr data;

		public Texture ()
		{
		}

		public Texture (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public Texture (NSString path) : base()
		{
			if (this.GetImageDataFromPath (path)) {
				this.LoadTexture ();
			}
		}

		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			GL.glDeleteTextures (1, ref this.texId);
			GL.glDeleteBuffers (1, ref this.pboId);
			this.SendMessageSuper (TextureClass, "dealloc");
		}

		public uint TextureName {
			get { return this.texId; }
		}

		private bool GetImageDataFromPath (NSString path)
		{
			this.data = Marshal.AllocHGlobal (TEXTURE_WIDTH * TEXTURE_HEIGHT * 4);
			
			NSURL url = NSURL.FileURLWithPath (path);
			IntPtr src = CGImageSource.CreateWithURL (url, null);
			
			if (src == IntPtr.Zero) {
				Marshal.FreeHGlobal (this.data);
				return false;
			}
			
			IntPtr image = CGImageSource.CreateImageAtIndex (src, 0, null);
			CFType.CFRelease (image);
			
			NSUInteger width = CGImage.GetWidth (image);
			NSUInteger height = CGImage.GetHeight (image);
			
			IntPtr colorSpace = CGColorSpace.CreateDeviceRGB ();
			IntPtr context = CGBitmapContext.Create (data, width, height, 8, 4 * width, colorSpace, (CGBitmapInfo)CGImageAlphaInfo.kCGImageAlphaPremultipliedFirst | CGBitmapInfo.kCGBitmapByteOrder32Little);
			CGColorSpace.Release (colorSpace);
			
			// Core Graphics referential is upside-down compared to OpenGL referential
			// Flip the Core Graphics context here
			// An alternative is to use flipped OpenGL texture coordinates when drawing textures
			CGContext.TranslateCTM (context, 0, height);
			CGContext.ScaleCTM (context, 1, -1);
			
			// Set the blend mode to copy before drawing since the previous contents of memory aren't used. This avoids unnecessary blending.
			CGContext.SetBlendMode (context, CGBlendMode.kCGBlendModeCopy);
			
			CGContext.DrawImage (context, new CGRect (0, 0, width, height), image);
			
			CGContext.Release (context);
			CGImage.Release (image);
			
			return true;
		}

		private void LoadTexture ()
		{
			GL.glGenTextures (1, out this.texId);
			GL.glGenBuffers (1, out this.pboId);
			
			// Bind the texture
			GL.glBindTexture (GL.GL_TEXTURE_2D, this.texId);
			
			// Bind the PBO
			GL.glBindBuffer (GL.GL_PIXEL_UNPACK_BUFFER, this.pboId);
			
			// Upload the texture data to the PBO
			GL.glBufferData (GL.GL_PIXEL_UNPACK_BUFFER, new IntPtr (TEXTURE_WIDTH * TEXTURE_HEIGHT * 4), data, GL.GL_STATIC_DRAW);
			
			// Setup texture parameters
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_NEAREST);
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_NEAREST);
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP_TO_EDGE);
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP_TO_EDGE);
			
			GL.glPixelStorei (GL.GL_UNPACK_ROW_LENGTH, 0);
			
			// OpenGL likes the GL_BGRA + GL_UNSIGNED_INT_8_8_8_8_REV combination
			// Use offset instead of pointer to indictate that we want to use data copied from a PBO		
			GL.glTexImage2D (GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, TEXTURE_WIDTH, TEXTURE_HEIGHT, 0, GL.GL_BGRA, GL.GL_UNSIGNED_INT_8_8_8_8_REV, IntPtr.Zero);
			
			// We can delete the application copy of the texture data now
			Marshal.FreeHGlobal (this.data);
			
			GL.glBindTexture (GL.GL_TEXTURE_2D, 0);
			GL.glBindBuffer (GL.GL_PIXEL_UNPACK_BUFFER, 0);
		}
	}
}


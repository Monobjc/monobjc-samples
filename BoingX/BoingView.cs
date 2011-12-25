using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.OpenGL;

namespace Monobjc.Samples.BoingX
{
	[ObjectiveCClass]
	public partial class BoingView : NSOpenGLView
	{
		public static readonly Class BoingViewClass = Class.Get (typeof(BoingView));

		Vertex[] boingData = new Vertex[8 * 16 * 4];
		private float angle;
		private float angleDelta;
		private float r;
		private float bgFade;
		private float xPos, yPos;
		private float xVelocity, yVelocity;
		private float lightFactor;
		private float scaleFactor;

		private bool doScale;
		private bool doLight;
		private bool bounceInsideWindow;
		private bool didInit;
		private bool doMultiSampleStuff;
		private bool enableMultiSample;

		int doTransparency;

		NSRect screenBounds;

		public BoingView ()
		{
		}

		public BoingView (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public BoingView (NSRect frameRect) : base(frameRect)
		{
			doMultiSampleStuff = true;
			
			this.r = 48.0f;
			
			xVelocity = 1.5f;
			yVelocity = 0.0f;
			xPos = r * 2.0f;
			yPos = r * 3.0f;
		}

		[ObjectiveCMessage("initWithFrame:")]
		public override Id InitWithFrame (NSRect frameRect)
		{
			//uint[] attribsNice = new uint[] { (uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFAAccelerated, (uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFADoubleBuffer, (uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFADepthSize, 24, (uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFAAlphaSize, 8, (uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFAColorSize, 32, (uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFANoRecovery, (uint)CGLPixelFormatAttribute.kCGLPFASampleBuffers, 1, (uint)NSOpenGLPixelFormatAttribute.kCGLPFASamples, 2, 0 };
			
			uint[] attribsJaggy = new uint[] {
				(uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFAAccelerated, 
				(uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFADoubleBuffer, 
				(uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFADepthSize, 24, 
				(uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFAAlphaSize, 8, 
				(uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFAColorSize, 32,
				(uint)NSOpenGLPixelFormatAttribute.NSOpenGLPFANoRecovery,
				0 };
			
			NSOpenGLPixelFormat fmt = new NSOpenGLPixelFormat (attribsJaggy);
			
			this.SendMessage<IntPtr>("initWithFrame:pixelFormat:", frameRect, fmt);
			
			fmt.Release ();
			
			return this;
		}

		public override bool IsOpaque {
			[ObjectiveCMessage("isOpaque")]
			get { return true; }
		}

		public void generateBoingData ()
		{
			int x;
			int index = 0;
			
			float v1x, v1y, v1z;
			float v2x, v2y, v2z;
			float d;
			
			int theta, phi;
			
			float theta0, theta1;
			float phi0, phi1;
			
			Vertex[] vtx = new Vertex[4];
			
			float delta = (float)(Math.PI / 8.0f);
			
			// 8 vertical segments
			for (theta = 0; theta < 8; theta++) {
				theta0 = theta * delta;
				theta1 = (theta + 1) * delta;
				
				// 16 horizontal segments
				for (phi = 0; phi < 16; phi++) {
					phi0 = phi * delta;
					phi1 = (phi + 1) * delta;
					
					// For now, generate 4 full points.			
					vtx[0].x = (float)(r * Math.Sin (theta0) * Math.Cos (phi0));
					vtx[0].y = (float)(r * Math.Cos (theta0));
					vtx[0].z = (float)(r * Math.Sin (theta0) * Math.Sin (phi0));
					
					vtx[1].x = (float)(r * Math.Sin (theta0) * Math.Cos (phi1));
					vtx[1].y = (float)(r * Math.Cos (theta0));
					vtx[1].z = (float)(r * Math.Sin (theta0) * Math.Sin (phi1));
					
					vtx[2].x = (float)(r * Math.Sin (theta1) * Math.Cos (phi1));
					vtx[2].y = (float)(r * Math.Cos (theta1));
					vtx[2].z = (float)(r * Math.Sin (theta1) * Math.Sin (phi1));
					
					vtx[3].x = (float)(r * Math.Sin (theta1) * Math.Cos (phi0));
					vtx[3].y = (float)(r * Math.Cos (theta1));
					vtx[3].z = (float)(r * Math.Sin (theta1) * Math.Sin (phi0));
					
					// Generate normal
					if (theta >= 4) {
						v1x = vtx[1].x - vtx[0].x;
						v1y = vtx[1].y - vtx[0].y;
						v1z = vtx[1].z - vtx[0].z;
						
						v2x = vtx[3].x - vtx[0].x;
						v2y = vtx[3].y - vtx[0].y;
						v2z = vtx[3].z - vtx[0].z;
					} else {
						v1x = vtx[0].x - vtx[3].x;
						v1y = vtx[0].y - vtx[3].y;
						v1z = vtx[0].z - vtx[3].z;
						
						v2x = vtx[2].x - vtx[3].x;
						v2y = vtx[2].y - vtx[3].y;
						v2z = vtx[2].z - vtx[3].z;
					}
					
					vtx[0].nx = (v1y * v2z) - (v2y * v1z);
					vtx[0].ny = (v1z * v2x) - (v2z * v1x);
					vtx[0].nz = (v1x * v2y) - (v2x * v1y);
					
					d = (float)(1.0f / Math.Sqrt (vtx[0].nx * vtx[0].nx + vtx[0].ny * vtx[0].ny + vtx[0].nz * vtx[0].nz));
					
					vtx[0].nx *= d;
					vtx[0].ny *= d;
					vtx[0].nz *= d;
					
					// Generate color			
					if (((theta ^ phi) & 1) == 1) {
						vtx[0].r = 1.0f;
						vtx[0].g = 1.0f;
						vtx[0].b = 1.0f;
						vtx[0].a = 1.0f;
					} else {
						vtx[0].r = 1.0f;
						vtx[0].g = 0.0f;
						vtx[0].b = 0.0f;
						vtx[0].a = 1.0f;
					}
					
					// Replicate vertex info
					for (x = 0; x < 4; x++) {
						vtx[x].nx = vtx[0].nx;
						vtx[x].ny = vtx[0].ny;
						vtx[x].nz = vtx[0].nz;
						vtx[x].r = vtx[0].r;
						vtx[x].g = vtx[0].g;
						vtx[x].b = vtx[0].b;
						vtx[x].a = vtx[0].a;
					}
					
					// Store vertices
					boingData[index++] = vtx[0];
					boingData[index++] = vtx[1];
					boingData[index++] = vtx[2];
					boingData[index++] = vtx[3];
				}
			}
		}

		public static float[] light0Position = new float[] { -2.0f, 2.0f, 1.0f, 0.0f };

		public static float[] light0Ambient = new float[] { 0.2f, 0.2f, 0.2f, 0.2f };
		public static float[] light0Diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
		public static float[] light0Specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

		public static float[] materialShininess = new float[4] { 10.0f, 0.0f, 0.0f, 0.0f };

		public static float[] materialAmbient = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
		public static float[] materialDiffuse = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
		public static float[] materialSpecular = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };

		public static float[] materialEmission = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect (NSRect aRect)
		{
			int i;
			float[] lightModelAmbient = new float[4];
			float[] lightSpecular = new float[4];
			
			if (!this.didInit) {
				NSColor.ClearColor.Set ();
				AppKitFramework.NSRectFill (this.Bounds);
				this.OpenGLContext.SetValuesForParameter (new int[] { 0 }, NSOpenGLContextParameter.NSOpenGLCPSurfaceOpacity);
				this.Window.IsOpaque = false;
				this.Window.AlphaValue = 0.999f;
				this.Window.IsMovableByWindowBackground = true;
				
				this.generateBoingData ();
				this.didInit = true;
				screenBounds = NSScreen.MainScreen.Frame;
				angleDelta = -2.5f;
				GL.glClearColor (0.0f, 0.0f, 0.0f, 0.0f);
				GL.glClear (GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
				this.OpenGLContext.FlushBuffer ();
				bgFade = 1.0f;
				lightFactor = 0.0f;
				scaleFactor = 1.0f;
				bounceInsideWindow = true;
				if (doMultiSampleStuff) {
					GL.glDisable (GL.GL_MULTISAMPLE_ARB);
					GL.glHint (GL.GL_MULTISAMPLE_FILTER_HINT_NV, GL.GL_NICEST);
				}
			}
			
			if (enableMultiSample)
				GL.glEnable (GL.GL_MULTISAMPLE_ARB);
			
			if (doTransparency == 2) {
				bgFade -= 0.05f;
				if (bgFade < 0.0f)
					bgFade = 0.0f;
			}
			
			if (doLight) {
				lightFactor += 0.005f;
				if (lightFactor > 1.0f) {
					lightFactor = 1.0f;
					doLight = false;
				}
			}
			
			if (doScale) {
				float oldr = r;
				scaleFactor += 0.025f;
				r = scaleFactor * 48.0f;
				yPos += r - oldr;
				
				if (scaleFactor > 2.0f) {
					scaleFactor = 2.0f;
					doScale = false;
				}
				this.generateBoingData ();
			}
			
			GL.glViewport (0, 0, (int)this.Bounds.size.width, (int)this.Bounds.size.height);
			
			GL.glScissor (0, 0, (int) (320 * scaleFactor), (int) (200 * scaleFactor));
			GL.glEnable (GL.GL_SCISSOR_TEST);
			GL.glClearColor (0.675f * bgFade, 0.675f * bgFade, 0.675f * bgFade, bgFade);
			GL.glClear (GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
			
			GL.glMatrixMode (GL.GL_PROJECTION);
			GL.glLoadIdentity ();
			GL.glOrtho (0, this.Bounds.size.width, 0, this.Bounds.size.height, 0.0f, 2000.0);
			
			GL.glDisable (GL.GL_LIGHTING);
			GL.glEnable (GL.GL_LIGHT0);
			
			GL.glLightModeli (GL.GL_LIGHT_MODEL_LOCAL_VIEWER, (int)GL.GL_FALSE);
			GL.glLightModeli (GL.GL_LIGHT_MODEL_TWO_SIDE, (int)GL.GL_FALSE);
			lightModelAmbient[0] = 1.0f - lightFactor;
			lightModelAmbient[1] = 1.0f - lightFactor;
			lightModelAmbient[2] = 1.0f - lightFactor;
			lightModelAmbient[3] = 1.0f - lightFactor;
			GL.glLightModelfv (GL.GL_LIGHT_MODEL_AMBIENT, lightModelAmbient);
			
			// Directional white light
			GL.glLightfv (GL.GL_LIGHT0, GL.GL_AMBIENT, light0Ambient);
			GL.glLightfv (GL.GL_LIGHT0, GL.GL_DIFFUSE, light0Diffuse);
			lightSpecular[0] = lightFactor;
			lightSpecular[1] = lightFactor;
			lightSpecular[2] = lightFactor;
			lightSpecular[3] = lightFactor;
			GL.glLightfv (GL.GL_LIGHT0, GL.GL_SPECULAR, lightSpecular);
			
			// Material properties
			GL.glEnable (GL.GL_COLOR_MATERIAL);
			GL.glEnable (GL.GL_NORMALIZE);
			GL.glColorMaterial (GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE);
			GL.glMaterialfv (GL.GL_FRONT_AND_BACK, GL.GL_SHININESS, materialShininess);
			GL.glMaterialfv (GL.GL_FRONT_AND_BACK, GL.GL_SPECULAR, materialSpecular);
			GL.glMaterialfv (GL.GL_FRONT_AND_BACK, GL.GL_EMISSION, materialEmission);
			
			
			// Reset transformation matrix and set light position.
			GL.glMatrixMode (GL.GL_MODELVIEW);
			GL.glLoadIdentity ();
			GL.glLightfv (GL.GL_LIGHT0, GL.GL_POSITION, light0Position);
			
			// I'm doing the scaling myself (rather than using OpenGL) just because
			// I want to get exact positioning of the lines.  There is probably a better
			// way to do this if I'd try harder with the math.  Hey, it's a demo. ;)
			if (bgFade > 0.0f) {
				GL.glDepthMask ((byte)GL.GL_FALSE);
				GL.glDisable (GL.GL_DEPTH_TEST);
				
				GL.glBegin (GL.GL_LINES);
				GL.glColor4f (0.6275f * bgFade, 0.0f, 0.6275f * bgFade, bgFade);
				for (i = 40; i <= 280; i += 16) {
					GL.glVertex3f ((float)Math.Floor (i * scaleFactor) + 0.5f,(float) Math.Floor (7 * scaleFactor) + 0.5f, -500.0f);
					GL.glVertex3f ((float)Math.Floor (i * scaleFactor) + 0.5f, (200 * scaleFactor) + 0.5f, -500.0f);
					
					// Do stragglers along the bottom. Not exactly the same as
					// the original, but close enough.
					GL.glVertex3f ((float)Math.Floor (i * scaleFactor) + 0.5f, (float)Math.Floor (7 * scaleFactor) + 0.5f, -500.0f);
					GL.glVertex3f ((i - 160) * scaleFactor * 1.1f + 160.0f * scaleFactor, -0.5f, -500.0f);
				}
				for (i = 8; i <= 200; i += 16) {
					GL.glVertex3f (40.0f * scaleFactor, (float)Math.Floor (i * scaleFactor) - 0.5f, -500.0f);
					GL.glVertex3f (280.0f * scaleFactor, (float)Math.Floor (i * scaleFactor) - 0.5f, -500.0f);
				}
				// Do final two horizontal lines
				GL.glVertex3f ((float)Math.Floor ((40.0f - 3.0f) * scaleFactor) + 0.5f, (float)Math.Floor ((7.0f - 2.0f) * scaleFactor) + 0.5f, -500.0f);
				GL.glVertex3f ((float)Math.Floor ((280.0f + 3.0f) * scaleFactor) + 0.5f, (float)Math.Floor ((7.0 - 2.0f) * scaleFactor) + 0.5f, -500.0f);
				GL.glVertex3f ((float)Math.Floor ((40.0f - 8.0f) * scaleFactor) + 0.5f, (float)Math.Floor ((7.0f - 5.0f) * scaleFactor) + 0.5f, -500.0f);
				GL.glVertex3f ((float)Math.Floor ((280.0f + 8.0f) * scaleFactor) + 0.5f, (float)Math.Floor ((7.0f - 5.0f) * scaleFactor) + 0.5f, -500.0f);
				GL.glEnd ();
			}
			
			GL.glLoadIdentity ();
			
			// Draw "shadow"
			GL.glEnable (GL.GL_CULL_FACE);
			if (bounceInsideWindow)
				GL.glTranslatef (xPos + 10, yPos - 2, -800.0f);
			else
				GL.glTranslatef (r + 10, r - 2, -800.0f);
			GL.glRotatef (-16.0f, 0.0f, 0.0f, 1.0f);
			GL.glRotatef (angle, 0.0f, 1.0f, 0.0f);
			GL.glScalef (1.05f, 1.05f, 1.05f);
			
			GL.glEnable (GL.GL_BLEND);
			GL.glBlendFunc (GL.GL_SRC_ALPHA_SATURATE, GL.GL_ONE_MINUS_SRC_ALPHA);
			
			GL.glBegin (GL.GL_QUADS);
			GL.glColor4f (0.0f, 0.0f, 0.0f, 0.4f);
			for (i = 0; i < 4 * 8 * 16; i++) {
				Vertex v1 = boingData[i];
				Vertex v2 = boingData[i &  ~3];
				GL.glNormal3f(v2.nx, v2.ny, v2.nz);
				GL.glVertex3f(v1.x, v1.y, v1.z);
				//GL.glNormal3fv (&boingData[i & ~3].nx);
				//GL.glVertex3fv (&boingData[i].x);
			}
			GL.glEnd ();
			
			// Draw real boing
			GL.glEnable (GL.GL_LIGHTING);
			GL.glEnable (GL.GL_CULL_FACE);
			GL.glEnable (GL.GL_DEPTH_TEST);
			GL.glDepthMask ((byte)GL.GL_TRUE);
			GL.glDepthFunc (GL.GL_LESS);
			GL.glDisable (GL.GL_BLEND);
			
			GL.glLoadIdentity ();
			
			if (bounceInsideWindow)
				GL.glTranslatef (xPos, yPos, -100.0f);
			else
				GL.glTranslatef (r, r, -100.0f);
			GL.glRotatef (-16.0f, 0.0f, 0.0f, 1.0f);
			GL.glRotatef (angle, 0.0f, 1.0f, 0.0f);
			
			GL.glBegin (GL.GL_QUADS);
			for (i = 0; i < 4 * 8 * 16; i++) {
				Vertex v1 = boingData[i];
				Vertex v2 = boingData[i &  ~3];
				GL.glColor4f(v1.r, v1.g, v1.b, v1.a);
				GL.glNormal3f(v2.nx, v2.ny, v2.nz);
				GL.glVertex3f(v1.x, v1.y, v1.z);
			}
			GL.glEnd ();
			
			GL.glDisable (GL.GL_LIGHTING);
			
			angle += angleDelta;
			if (angle < 0.0f)
				angle += 360.0f; else if (angle > 360.0f)
				angle -= 360.0f;
			
			this.OpenGLContext.FlushBuffer ();
		}

		[ObjectiveCMessage("mouseDragged:")]
		public override void MouseDragged (NSEvent theEvent)
		{
			NSPoint origin;
			
			origin = this.Window.Frame.origin;
			
			origin.x += theEvent.DeltaX;
			origin.y -= theEvent.DeltaY;
			
			this.Window.SetFrameOrigin (origin);
		}

		[ObjectiveCMessage("rightMouseDown:")]
		public override void RightMouseDown (NSEvent theEvent)
		{
			this.angleDelta = 2.0f - this.angleDelta;
		}

		public void Transition ()
		{
			// Now we are going to switch to window movement mode for the bounce.
			// Move window origin such that when the ball is locked at r,r the ball doesn't
			// appear to move from it's current location.
			NSRect frame;
			
			bounceInsideWindow = false;
			
			frame.origin = this.Window.Frame.origin;
			
			frame.origin.x += xPos - r;
			frame.origin.y += yPos - r;
			frame.size.width = r * 2 + 20;
			frame.size.height = r * 2 + 20;
			
			// Convert xPos,yPos to screen coordinates
			xPos = frame.origin.x + r;
			yPos = frame.origin.y + r;
			
			// Move window
			this.Window.SetFrameDisplay (frame, true);
			
			this.OpenGLContext.Update ();
			this.Display ();
			this.Window.FlushWindow ();
		}

		public void Animate ()
		{
			// Do bouncy stuff
			if (bounceInsideWindow) {
				yVelocity -= 0.05f;
				
				xPos += xVelocity * scaleFactor;
				yPos += yVelocity * scaleFactor;
				
				if (xPos < (r + 10.0f)) {
					if (doTransparency == 2)
						this.Transition ();
					else {
						xPos = r + 10.0f;
						xVelocity = -xVelocity;
						angleDelta = -angleDelta;
					}
				} else if (xPos > (310 * scaleFactor - r)) {
					if (doTransparency == 2)
						this.Transition ();
					else {
						xPos = 310 * scaleFactor - r;
						xVelocity = -xVelocity;
						angleDelta = -angleDelta;
					}
				}
				if (yPos < r) {
					if (doTransparency < 2) {
						yPos = r;
						yVelocity = -yVelocity;
					}
					if (doTransparency == 1) {
						doTransparency = 2;
					} else if (doTransparency == 2) {
						this.Transition ();
					}
				}
				if (bounceInsideWindow)
					this.Display ();
			} else {
				NSRect frame;
				
				yVelocity -= 0.1f;
				
				xPos += xVelocity * scaleFactor;
				yPos += yVelocity * scaleFactor;
				
				frame = this.Window.Frame;
				
				if (xPos < (r + 10.0f)) {
					xPos = r + 10.0f;
					xVelocity = -xVelocity;
					angleDelta = -angleDelta;
				} else if (xPos > ((NSRect.NSMaxX (screenBounds) - 10) - r)) {
					xPos = (NSRect.NSMaxX (screenBounds) - 10) - r;
					xVelocity = -xVelocity;
					angleDelta = -angleDelta;
				}
				if (yPos < r) {
					yPos = r;
					yVelocity = -yVelocity;
				}
				frame.origin.x = xPos - r;
				frame.origin.y = yPos - r;
				
				this.NeedsDisplay = true;
				this.Window.SetFrameOrigin (frame.origin);
			}
		}

		[ObjectiveCMessage("acceptsFirstResponder")]
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}

		[ObjectiveCMessage("becomeFirstResponder")]
		public override bool BecomeFirstResponder ()
		{
			return true;
		}

		[ObjectiveCMessage("resignFirstResponder")]
		public override bool ResignFirstResponder ()
		{
			return true;
		}

		public void DoEventForCharacterDownEvent (char ch, bool flag)
		{
			switch (ch) {
			case 's':
				doScale = true;
				break;
			case 'l':
				doLight = true;
				break;
			case 't':
				doTransparency = 1;
				break;
			case 'm':
				enableMultiSample = true;
				break;
			}
		}

		[ObjectiveCMessage("keyDown:")]
		public override void KeyDown (NSEvent theEvent)
		{
			NSString character = theEvent.CharactersIgnoringModifiers;
			char ch = character[0];
			
			DoEventForCharacterDownEvent (ch, true);
		}
	}
}

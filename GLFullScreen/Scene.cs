using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.OpenGL;

namespace GLFullScreen
{
	[ObjectiveCClass]
	public partial class Scene : NSObject
	{
		public static readonly Class SceneClass = Class.Get (typeof(Scene));

		private Texture texture;
		private uint textureName;

		private float animationPhase;
		private float rollAngle;
		private float sunAngle;
		private bool wireframe;

		private static float[] lightDirection = new float[] { -0.7071f, 0.0f, 0.7071f, 0.0f };
		private static float radius = 0.25f;
		private static float[] materialAmbient = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
		private static float[] materialDiffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

		public Scene ()
		{
		}

		public Scene (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("init")]
		public override Id Init ()
		{
			this.NativePointer = this.SendMessageSuper<IntPtr> (SceneClass, "init");
			
			this.textureName = 0;
			this.animationPhase = 0;
			this.rollAngle = 0;
			this.sunAngle = 135;
			this.wireframe = false;
			
			return this;
		}

		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this.texture.Release ();
			this.SendMessageSuper (SceneClass, "dealloc");
		}

		public float RollAngle {
			get { return this.rollAngle; }
			set { this.rollAngle = value; }
		}
		public float SunAngle {
			get { return this.sunAngle; }
			set { this.sunAngle = value; }
		}

		public void AdvanceTimeBy (float seconds)
		{
			float phaseDelta = seconds - (float)Math.Floor (seconds);
			float newAnimationPhase = animationPhase + 0.005f * phaseDelta;
			newAnimationPhase = newAnimationPhase - (float)Math.Floor (newAnimationPhase);
			this.AnimationPhase = newAnimationPhase;
		}

		public float AnimationPhase {
			get { return this.animationPhase; }
			set { this.animationPhase = value; }
		}

		public void ToggleWireframe ()
		{
			this.wireframe = !this.wireframe;
		}

		public void SetViewportRect (NSRect bounds)
		{
			GL.glViewport (0, 0, (int)bounds.size.width, (int)bounds.size.height);
			
			GL.glMatrixMode (GL.GL_PROJECTION);
			GL.glLoadIdentity ();
			GL.gluPerspective (30, bounds.size.width / bounds.size.height, 1.0, 1000.0);
			GL.glMatrixMode (GL.GL_MODELVIEW);
		}

		// This method renders our scene.
		// We could optimize it in any of several ways, including factoring out the repeated OpenGL initialization calls and 
		// hanging onto the GLU quadric object, but the details of how it's implemented aren't important here. 
		// The main thing to note is that we've factored the drawing code out of the NSView subclass so that
		// the full-screen and non-fullscreen views share the same states for rendering 
		// (and MainController can use it when rendering in full-screen mode on pre-10.6 systems).
		public void Render ()
		{
			IntPtr quadric = IntPtr.Zero;
			
			GL.glEnable (GL.GL_DEPTH_TEST);
			GL.glEnable (GL.GL_CULL_FACE);
			GL.glEnable (GL.GL_LIGHTING);
			GL.glEnable (GL.GL_LIGHT0);
			GL.glEnable (GL.GL_TEXTURE_2D);
			
			GL.glClearColor (0, 0, 0, 0);
			GL.glClear (GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
			
			// Upload the texture
			// Since we are sharing OpenGL objects between the full-screen and non-fullscreen contexts, we only need to do this once
			if (this.texture == null) {
				NSString path = NSBundle.MainBundle.PathForResourceOfType ("Earth", "jpg");
				this.texture = new Texture (path);
				this.textureName = this.texture.TextureName;
			}
			
			// Set up texturing parameters
			GL.glBindTexture (GL.GL_TEXTURE_2D, this.textureName);
			GL.glTexEnvf (GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_MODULATE);
			
			// Set up our single directional light (the Sun!)
			lightDirection[0] = (float)Math.Cos (sunAngle * Math.PI / 180.0);
			lightDirection[2] = (float)Math.Sin (sunAngle * Math.PI / 180.0);
			GL.glLightfv (GL.GL_LIGHT0, GL.GL_POSITION, lightDirection);
			
			GL.glPushMatrix ();
			
			// Back the camera off a bit
			GL.glTranslatef (0.0f, 0.0f, -1.5f);
			
			// Draw the Earth!
			quadric = GL.gluNewQuadric ();
			if (wireframe) {
				GL.gluQuadricDrawStyle (quadric, GL.GLU_LINE);
			}
			
			GL.gluQuadricTexture (quadric, (byte)GL.GL_TRUE);
			GL.glMaterialfv (GL.GL_FRONT, GL.GL_AMBIENT, materialAmbient);
			GL.glMaterialfv (GL.GL_FRONT, GL.GL_DIFFUSE, materialDiffuse);
			GL.glRotatef (rollAngle, 1.0f, 0.0f, 0.0f);
			GL.glRotatef (-23.45f, 0.0f, 0.0f, 1.0f);
			// Earth's axial tilt is 23.45 degrees from the plane of the ecliptic
			GL.glRotatef (animationPhase * 360.0f, 0.0f, 1.0f, 0.0f);
			GL.glRotatef (-90.0f, 1.0f, 0.0f, 0.0f);
			GL.gluSphere (quadric, radius, 48, 24);
			GL.gluDeleteQuadric (quadric);
			quadric = IntPtr.Zero;
			
			GL.glPopMatrix ();
			
			GL.glBindTexture (GL.GL_TEXTURE_2D, 0);
		}
	}
}

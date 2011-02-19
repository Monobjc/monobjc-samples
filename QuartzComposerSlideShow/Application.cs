using System;
using System.Runtime.InteropServices;
using Monobjc.Quartz;
using Monobjc.Foundation;
using Monobjc.AppKit;

// Pres ESC to quit application
namespace Monobjc.Samples.QuartzComposerSlideShow
{
	[ObjectiveCClass]
	public class SlideShowApplication : NSObject
	{
		public static readonly Class SlideShowApplicationClass = Class.Get(typeof(SlideShowApplication));
		
		private float 			kSlideShowInterval = 3.0f; // seconds for each transition slide
		private uint				_imageIndex = 0;
		private NSMutableArray 	_fileList;
		private NSOpenGLContext	_openGLContext;
		private QCRenderer 		_renderer;
		private static Random	_random;
		
		public SlideShowApplication (){}
		
		public SlideShowApplication (IntPtr nativePointer) : base(nativePointer){}
	
		[ObjectiveCMessage("init")]
		public override Id Init ()
		{		
				this.NativePointer = this.SendMessageSuper<IntPtr>(SlideShowApplicationClass, "init");

				return this;
		}

		[ObjectiveCMessage("applicationDidFinishLaunching:")]
		public void ApplicationDidFinishLaunching (NSNotification aNotification)
		{
			Console.WriteLine ("Application finished launching");
			
			NSArray 							imageFileTypes = NSImage.ImageFileTypes;
			int 								value = 1;
            NSOpenGLPixelFormatAttribute[] attributes = {
															NSOpenGLPixelFormatAttribute.NSOpenGLPFAFullScreen,
															NSOpenGLPixelFormatAttribute.NSOpenGLPFAScreenMask,
                                                           (NSOpenGLPixelFormatAttribute) CGDisplayIDToOpenGLDisplayMask(CGMainDisplayID()),
															NSOpenGLPixelFormatAttribute.NSOpenGLPFANoRecovery,
															NSOpenGLPixelFormatAttribute.NSOpenGLPFADoubleBuffer,
															NSOpenGLPixelFormatAttribute.NSOpenGLPFAAccelerated,
															NSOpenGLPixelFormatAttribute.NSOpenGLPFADepthSize,
															(NSOpenGLPixelFormatAttribute)24,
															(NSOpenGLPixelFormatAttribute)0
														};
            NSOpenGLPixelFormat format = new NSOpenGLPixelFormat(attributes).Autorelease<NSOpenGLPixelFormat>();
			NSOpenPanel						openPanel;
			NSDirectoryEnumerator			enumerator;
			NSString							basePath;
			NSString							subPath;
			
			//Ask the user for a directory of images
			openPanel = NSOpenPanel.OpenPanel;
			openPanel.AllowsMultipleSelection = false;
			openPanel.CanChooseFiles = false;
			openPanel.CanChooseDirectories = true;
			if (openPanel.RunModalForDirectoryFileTypes(null, null, null) != NSOpenPanel.NSOKButton) {
				Console.WriteLine ("No directory specified");
				NSApplication.SharedApplication.Terminate(null);
			}
						
			//Populate an array with all the image files in the directory (no recursivity)
			_fileList = new NSMutableArray();
			basePath = openPanel.Filenames.ObjectAtIndex(0).CastTo<NSString>();
			enumerator =  NSFileManager.DefaultManager.EnumeratorAtPath(basePath);
			while (enumerator.MoveNext()) {
				subPath = enumerator.Current.CastTo<NSString>();
				if (enumerator.FileAttributes.ObjectForKey(NSFileManager.NSFileType).CastTo<NSString>().IsEqualToString(NSFileManager.NSFileTypeDirectory)) {
					enumerator.SkipDescendents();	
					continue;
				}
				if (imageFileTypes.ContainsObject(subPath.PathExtension)) {
					_fileList.AddObject(basePath.StringByAppendingPathComponent(subPath));
				}
			}
			
			if (_fileList.Count < 2) {
				Console.WriteLine ("Directory contains less than 2 image files");
				NSApplication.SharedApplication.Terminate(null);
			}
			
			
			//Capture main scrren
			CGDisplayCapture(CGMainDisplayID());
			CGDisplayHideCursor(CGMainDisplayID());
			
			//Create the fullscreen OpenGL context on the main screen (double-buffered with color and depth buffers)

			_openGLContext = new NSOpenGLContext(format, null);
			if (_openGLContext == null) {
				Console.WriteLine ("Cannot create OpenGL context");
				NSApplication.SharedApplication.Terminate(null);
			}
			_openGLContext.SetFullScreen();
			_openGLContext.SetValuesForParameter(new int[] {value}, NSOpenGLContextParameter.NSOpenGLCPSwapInterval);
			
			//Create the QuartzComposer Renderer with that OpenGL context and the transition composition file
			_renderer = new QCRenderer(_openGLContext, format, NSBundle.MainBundle.PathForResourceOfType(NSString.NSPinnedString("Transition"),NSString.NSPinnedString("qtz"))); 

			
			if (_renderer == null) {
				Console.WriteLine ("Cannot create QCRenderer");
				NSApplication.SharedApplication.Terminate(null);
			}
			
			//NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("sendEvent:"),null, NSApplication.SharedApplication);
			
			//Run first transition as soon as possible
			PerformSelectorWithObjectAfterDelay(ObjectiveCRuntime.Selector("performTransition:"),null, 0.0);
		}

		[ObjectiveCMessage("performTransition:")]
		public void PerformTransition(Id param)
		{
			
			Console.WriteLine ("performTransition startet");
			double	time;
			NSImage	image;
			
			//Load next image
			image = new NSImage(NSData.DataWithContentsOfFile(_fileList.ObjectAtIndex(_imageIndex).CastTo<NSString>()));
			if (image == null) {
				Console.WriteLine ("Cannot load image at path {0}", _fileList.ObjectAtIndex(_imageIndex).CastTo<NSString>());
			}
	
			//Set transition source image (just get it from the previous destination image)
			_renderer.SetValueForInputKey(_renderer.ValueForInputKey("destination"), "source");

			//Set transition destination image (the new image)
			_renderer.SetValueForInputKey(image, "destination");
			
			// Release next image
			image.Release();
			
			// Render transition - FIXME: use a runloop timer
			for (time = 0.0; time < 1.0; time+=0.01) {
				if (!_renderer.RenderAtTimeArguments(time, null)) {
					Console.WriteLine ("Rendering failed at time {0}",time);
				}
				_openGLContext.FlushBuffer();
			}
			// this is necessary to make sure that the last image is rendered at time 1.0
			if (!_renderer.RenderAtTimeArguments(1.0,null)) {
				Console.WriteLine ("Rendering failed at time {0}",time);
				_openGLContext.FlushBuffer();
			}
			
			//Schedule next transition
			if (_imageIndex < _fileList.Count - 1) {
				_imageIndex++;
			}
			else {
				_imageIndex = 0;
			}
			PerformSelectorWithObjectAfterDelay(ObjectiveCRuntime.Selector("performTransition:"), null, kSlideShowInterval);
		}
	
		[ObjectiveCMessage("applicationWillTerminate:")]	
		public void ApplicationWillTerminate (NSNotification aNotification)
		{
			Console.WriteLine ("Application will terminate");
			
			//Destroy renderer
			_renderer.SafeRelease();
			
			//Destroy OpenGL context
            if (_openGLContext != null)
            {
                _openGLContext.ClearDrawable();
                _openGLContext.Release();
            }

		    //Release main screen
			if (CGDisplayIsCaptured(CGMainDisplayID())) {
				CGDisplayShowCursor(CGMainDisplayID());
				CGDisplayRelease(CGMainDisplayID());
			}
			
			//Release file list
			_fileList.SafeRelease();
		}
			
		public static int rand()
		{
			if (_random == null) {
				_random = new Random();
			}		
			return _random.Next();
		}

#region InterOp funtions "DO NOT MODIFY"
        [DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint = "CGDisplayIDToOpenGLDisplayMask")]
        public static extern UInt32 CGDisplayIDToOpenGLDisplayMask(UInt32 display);

        [DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint = "CGDisplayIsCaptured")]
		public static extern bool CGDisplayIsCaptured(UInt32 display);
		
		[DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint="CGMainDisplayID")]
		public static extern UInt32 CGMainDisplayID();
		
		[DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint="CGDisplayCapture")]
		public static extern Int32 CGDisplayCapture(UInt32 display);
		
		[DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint="CGDisplayShowCursor")]
		public static extern Int32 CGDisplayShowCursor(UInt32 display);
		
		[DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint="CGDisplayHideCursor")]
		public static extern Int32 CGDisplayHideCursor(UInt32 display);
		
		[DllImport("/System/Libraries/Frameworks/ApplicationServices.framework/ApplicationServices", EntryPoint="CGDisplayRelease")]
		public static extern Int32 CGDisplayRelease(UInt32 display);
#endregion
	}
}

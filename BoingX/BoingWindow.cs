using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.BoingX
{
	[ObjectiveCClass]
	public partial class BoingWindow : NSWindow
	{
		public static readonly Class BoingWindowClass = Class.Get (typeof(BoingWindow));

		public BoingWindow ()
		{
		}

		public BoingWindow (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		public BoingWindow (NSRect contentRect, NSWindowStyleMask windowStyle, NSBackingStoreType bufferingType, bool deferCreation) : base(contentRect, windowStyle, bufferingType, deferCreation)
		{
		}

		public BoingWindow (NSRect contentRect, NSWindowStyleMask windowStyle, NSBackingStoreType bufferingType, bool deferCreation, NSScreen screen) : base(contentRect, windowStyle, bufferingType, deferCreation, screen)
		{
		}

		// This is here just so that keyboards events work even though we're a borderless window (normally they would not).
		public override bool CanBecomeKeyWindow
		{
			[ObjectiveCMessage("canBecomeKeyWindow")]
			get
			{
				return true;
			}
		}
	}
}

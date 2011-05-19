using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace BoingX
{
	[ObjectiveCClass]
	public partial class AppDelegate : NSObject, INSNibAwaking
	{
		public static readonly Class AppDelegateClass = Class.Get (typeof(AppDelegate));

		public AppDelegate ()
		{
		}

		public AppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("awakeFromNib")]
		public void AwakeFromNib ()
		{
			// Do anything you want here
		}
	}
}


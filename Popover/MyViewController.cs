using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace net.monobjc.samples.Popover
{
    [ObjectiveCClass]
	public partial class MyViewController : NSViewController
	{
		public static readonly Class MyViewControllerClass = Class.Get (typeof(MyViewController));
		
		public MyViewController ()
		{
		}
		
		public MyViewController (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		partial void CheckBoxAction (Id sender)
		{
			// user has clicked the check box in the popover (or its detached window)
		}
	}
}

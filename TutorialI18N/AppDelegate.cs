using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace TutorialI18N
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

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}
		
		partial void ShowAlert (Id sender)
		{
			NSString messageTitle = FoundationFramework.NSLocalizedString("$$Hello World", null);
			NSString informativeText = FoundationFramework.NSLocalizedString("$$It speaks English", null);
			NSString defaultButton = FoundationFramework.NSLocalizedString("$$OK", null);
			NSAlert alert = NSAlert.AlertWithMessageTextDefaultButtonAlternateButtonOtherButtonInformativeTextWithFormat(messageTitle, defaultButton, null, null, informativeText);
			alert.RunModal();
		}
	}
}

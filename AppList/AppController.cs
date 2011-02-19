using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.AppList
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "AppController" /> class.
        /// </summary>
        public AppController() {}

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AppController" /> class.
        /// </summary>
        /// <param name = "nativePointer">The native pointer.</param>
        public AppController(IntPtr nativePointer)
            : base(nativePointer) {}

        public NSWorkspace Workspace
        {
            [ObjectiveCMessage("workspace")]
            get { return NSWorkspace.SharedWorkspace; }
        }

        public NSArray SortDescriptors
        {
            [ObjectiveCMessage("sortDescriptors")]
            get { return NSArray.ArrayWithObject(NSSortDescriptor.SortDescriptorWithKeyAscending("localizedName", true)); }
        }
		
		partial void HideAction (Id sender)
		{
            NSRunningApplication selectedApp = this.arrayController.SelectedObjects.ObjectAtIndex(0).CastTo<NSRunningApplication>();
            selectedApp.Hide();
		}

		partial void UnhideAction (Id sender)
		{
            NSRunningApplication selectedApp = this.arrayController.SelectedObjects.ObjectAtIndex(0).CastTo<NSRunningApplication>();
            selectedApp.Unhide();
		}

		partial void QuitAction (Id sender)
		{
            NSRunningApplication selectedApp = this.arrayController.SelectedObjects.ObjectAtIndex(0).CastTo<NSRunningApplication>();
            selectedApp.Terminate();
		}
    }
}
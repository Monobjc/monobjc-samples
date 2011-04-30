using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.ScriptingBridge.Finder;
using Monobjc.ScriptingBridge;

namespace SBSetFinderComment
{
	[ObjectiveCClass]
	public partial class Controller : NSObject
	{
		public static readonly Class ControllerClass = Class.Get (typeof(Controller));

		// Current file selection
		private NSURL selectedFinderItem;

		public Controller ()
		{
		}

		public Controller (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		/// <summary>
		/// Gets/sets the selected item. The setter notifies listener when value changed.
		/// </summary>
		public NSURL SelectedFinderItem {
			[ObjectiveCMessage("selectedFinderItem")]
			get { return selectedFinderItem; }
			[ObjectiveCMessage("setSelectedFinderItem:")]
			set {
				this.WillChangeValueForKey ("selectedFinderItem");
				NSURL newValue = value.SafeRetain<NSURL> ();
				this.selectedFinderItem.SafeRelease ();
				this.selectedFinderItem = newValue;
				this.DidChangeValueForKey ("selectedFinderItem");
			}
		}

		/// <summary>
		/// Returns an NSString containing the referenced item's finder comment (aka Spotlight comment) an item referenced by the file url.  Returns nil if an error occurs during processing.
		/// </summary>
		public NSString FinderCommentForFileURL (NSURL theFileURL)
		{
			NSString result = null;
			
			try {
				// Retrieve the Finder application Scripting Bridge object.
				FinderApplication finder = SBApplication.ApplicationWithBundleIdentifier ("com.apple.finder").CastAs<FinderApplication> ();
				
				// Retrieve a reference to our finder item asking for it by location 
				FinderItem theItem = finder.Items.ObjectAtLocation (theFileURL).CastAs<FinderItem> ();
				
				// Set the result.
				result = theItem.Comment;
			} catch (Exception e) {
				Console.WriteLine (e);
				result = null;
			}
			
			// return YES on success 
			return result;
		}

		/// <summary>
		/// Returns YES if it is able to change the finder comment (aka Spotlight comment) for an item referenced by the file url.  Returns NO if an error occurs during processing.
		/// </summary>
		public bool ChangeFinderCommentForFileURL (NSString comment, NSURL theFileURL)
		{
			bool result = false;
			
			try {
				// Retrieve the Finder application Scripting Bridge object.
				FinderApplication finder = SBApplication.ApplicationWithBundleIdentifier ("com.apple.finder").CastAs<FinderApplication> ();
				
				// Retrieve a reference to our finder item asking for it by location 
				FinderItem theItem = finder.Items.ObjectAtLocation (theFileURL).CastAs<FinderItem> ();
				
				// Attempt to set the comment for the Finder item.  
				theItem.Comment = comment;
				
				// Successful result 
				result = true;
			} catch (Exception e) {
				Console.WriteLine (e);
				result = false;
			}
			
			// Return YES on success 
			return result;
		}

		/// <summary>
		/// Returns true if it is able to reveal the indicated item in a Finder window.  The Finder will be switched into the forground during the processing of this call.  Returns NO if an error occurs during processing.
		/// </summary>
		public bool FinderRevealFileURL (NSURL theFileURL)
		{
			bool result = false;
			
			try {
				// Retrieve the Finder application Scripting Bridge object.
				FinderApplication finder = SBApplication.ApplicationWithBundleIdentifier ("com.apple.finder").CastAs<FinderApplication> ();
				
				// Retrieve a reference to our finder item asking for it by location 
				FinderItem theItem = finder.Items.ObjectAtLocation (theFileURL).CastAs<FinderItem> ();
				
				// Display the item
				theItem.Reveal ();
				
				// Activate the Finder application
				finder.Activate ();
				
				// successful result 
				result = true;
			} catch (Exception e) {
				Console.WriteLine (e);
				result = false;
			}
			
			// Return YES on success 
			return result;
		}

		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			this.SelectedFinderItem = null;
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}

		[ObjectiveCMessage("applicationDidBecomeActive:")]
		public virtual void ApplicationDidBecomeActive (NSNotification aNotification)
		{
			// If we have selected a file in the finder 
			if (this.SelectedFinderItem != null) {
				NSString theComment = this.FinderCommentForFileURL (this.SelectedFinderItem);
				if (theComment != null) {
					// Set the path in the display
					this.fileNameField.StringValue = this.SelectedFinderItem.Path;
					
					// Retrieve the finder comment
					NSUInteger p = this.commentField.TextStorage.String.Length;
					this.commentField.SetSelectedRange (new NSRange (0, p));
					this.commentField.InsertText (theComment);
				} else {
					this.ShowErrorMessageWithTitle ("Unable to update the finder comment for the selected item.", "Error getting comment");
				}
			}
		}

		partial void ChangeComment (Id sender)
		{
			// Retrieve the comment text from the window
			NSString commentText = commentField.TextStorage.String;
			
			// Verify that the comment is of a suitable length.
			// Radar rdar://problem/4857955 states that Finder comments are limited
			// to 750 Unicode characters. This is the current recommendation at
			// the time of this writing.
			if (commentText.Length > 750) {
				this.ShowErrorMessageWithTitle (String.Format ("Comments are limited to 750 characters.  The comment you entered is {0} characters long.", commentText.Length), "Comment too long");
			} else {
				// Attempt to change the finder comment.
				if (!this.ChangeFinderCommentForFileURL (commentText, this.SelectedFinderItem)) {
					this.ShowErrorMessageWithTitle ("Unable to set the finder comment.  Please re-select the file and try again.", "Error setting comment");
				}
			}
		}

		partial void RevealInFinder (Id sender)
		{
			if (!this.FinderRevealFileURL (this.SelectedFinderItem)) {
				this.ShowErrorMessageWithTitle ("Unable to reveal item in Finder.  Please re-select the file and try again.", "Error revealing item");
			}
		}

		partial void SelectFileForComment (Id sender)
		{
			NSOpenPanel theOpenPanel;
			NSInteger opResult;
			
			// Create an open panel
			theOpenPanel = NSOpenPanel.OpenPanel;
			theOpenPanel.Delegate = this;
			
			// Set the prompt and title 
			theOpenPanel.Message = "Select a file or folder for comment editing.";
			theOpenPanel.Title = "Choose File or Folder";
			
			// Directories okay, only one at a time
			theOpenPanel.CanChooseDirectories = true;
			theOpenPanel.AllowsMultipleSelection = false;
			
			// Run the panel 
			opResult = theOpenPanel.RunModalForDirectoryFileTypes (FoundationFramework.NSHomeDirectory (), null, null);
			if (NSPanel.NSOKButton == opResult) {
				// Get and save the path
				this.SelectedFinderItem = theOpenPanel.URLs.ObjectAtIndex (0).CastTo<NSURL> ();
				
				// Attempt to retrieve the comment 
				NSString theComment = this.FinderCommentForFileURL (this.SelectedFinderItem);
				if (theComment != null) {
					// set the path in the display
					this.fileNameField.StringValue = this.SelectedFinderItem.Path;
					
					// Retrieve the finder comment
					NSUInteger p = this.commentField.TextStorage.String.Length;
					this.commentField.SetSelectedRange (new NSRange (0, p));
					this.commentField.InsertText (theComment);
				} else {
					this.ShowErrorMessageWithTitle ("Unable to update the finder comment for the selected item.", "Error getting comment");
				}
			}
		}

		private void ShowErrorMessageWithTitle (NSString message, NSString title)
		{
			NSAlert.AlertWithMessageTextDefaultButtonAlternateButtonOtherButtonInformativeTextWithFormat (title, "OK", null, null, message).RunModal ();
		}
	}
}

using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;
using System.Runtime.InteropServices;

namespace SplitView
{
	[ObjectiveCClass]
	public partial class SplitView : NSView
	{
		public static readonly Class SplitViewClass = Class.Get (typeof(SplitView));
		private NSArray _viewStackConstraints;
		private NSArray _heightConstraints;
		private NSLayoutConstraint _draggingConstraint;

		public SplitView ()
		{
		}

		public SplitView (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public SplitView (NSRect frameRect) : base(frameRect)
		{
		}
		
		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this._draggingConstraint.SafeRelease ();
			this._heightConstraints.SafeRelease ();
			this._draggingConstraint.SafeRelease ();
			
			this.SendMessageSuper (SplitViewClass, "init");
		}
		
		public static new bool RequiresConstraintBasedLayout {
			[ObjectiveCMessage("requiresConstraintBasedLayout")]
			get {
				return true;
			}
		}

		public CGFloat DividerThickness {
			[ObjectiveCMessage("dividerThickness")]
			get {
				return 9;
			}
		}
		
		public void UpdateViewStackConstraints ()
		{
			if (this.ViewStackConstraints == null) {
				NSMutableArray stackConstraints = NSMutableArray.Array;
				NSDictionary metrics = NSDictionary.DictionaryWithObjectForKey (
					NSNumber.NumberWithDouble (this.DividerThickness),
					(NSString)"dividerThickness");
				NSMutableDictionary viewsDict = NSMutableDictionary.Dictionary;
				
				// iterate over our subviews from top to bottom
				NSView previousView = null;
				foreach (NSView currentView in this.Subviews.GetEnumerator<NSView>()) {
					viewsDict.SetObjectForKey (currentView, (NSString)"currentView");
					
					if (previousView == null) {
						// tie topmost view to the top of the container
						stackConstraints.AddObjectsFromArray (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:|[currentView]", 0, metrics, viewsDict));
					} else {
						// tie current view to the next one higher up
						viewsDict.SetObjectForKey (previousView, (NSString)"previousView");
						stackConstraints.AddObjectsFromArray (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:[previousView]-dividerThickness-[currentView]", 0, metrics, viewsDict));
					}

					// each view should fill the splitview horizontally
					stackConstraints.AddObjectsFromArray (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("H:|[currentView]|", 0, metrics, viewsDict));
            
					previousView = currentView;
				}				
        
				// tie the bottom view to the bottom of the splitview
				if (this.Subviews.Count > 0) {
					stackConstraints.AddObjectsFromArray (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:[currentView]|", 0, metrics, viewsDict));
				}

				this.SetViewStackConstraints (stackConstraints);
			}
		}
		
		public void SetViewStackConstraints (NSMutableArray stackConstraints)
		{
			if (this._viewStackConstraints != stackConstraints) {
				if (this._viewStackConstraints != null) {
					this.RemoveConstraints (this._viewStackConstraints);
				}

				this._viewStackConstraints.SafeRelease ();
				this._viewStackConstraints = stackConstraints.SafeRetain<NSMutableArray> ();
				
				if (this._viewStackConstraints != null) {
					this.AddConstraints (this._viewStackConstraints);
				} else {
					this.NeedsUpdateConstraints = true;
				}
			}
		}
		
		[ObjectiveCMessage("didAddSubview:")]
		public override void DidAddSubview (NSView subview)
		{
			subview.TranslatesAutoresizingMaskIntoConstraints = false;
			this.SetViewStackConstraints (null);
			this.SendMessageSuper (SplitViewClass, "didAddSubview:", subview);
		}
		
		[ObjectiveCMessage("willRemoveSubview:")]
		public override void WillRemoveSubview (NSView subview)
		{
			this.SetViewStackConstraints (null);
			this.SendMessageSuper (SplitViewClass, "willRemoveSubview:", subview);
		}
		
		public void SetHeightConstraints (NSArray heightConstraints)
		{
			if (this._heightConstraints != heightConstraints) {
				if (this._heightConstraints != null) {
					this.RemoveConstraints (this._heightConstraints);
				}
				this._heightConstraints.SafeRelease ();
				this._heightConstraints = heightConstraints.SafeRetain<NSArray> ();
				if (this._heightConstraints != null) {
					this.AddConstraints (this._heightConstraints);
				} else {
					this.NeedsUpdateConstraints = true;
				}
			}
		}
		
		public static int DistanceOfViewWithIndexFromDividerWithIndex (uint viewIndex, int dividerIndex)
		{
			return (int)(Math.Abs (viewIndex - (dividerIndex + 0.5)) - 0.5);
		}
		
		public NSArray ConstraintsForHeightsWithPrioritiesLowestAroundDivider (int dividerIndex)
		{
			NSMutableArray constraints = NSMutableArray.Array;
			NSArray views = this.Subviews;
			NSInteger numberOfViews = views.Count;
			
			CGFloat spaceForAllDividers = this.DividerThickness * (numberOfViews - 1);
			CGFloat spaceForAllViews = this.Bounds.Height - spaceForAllDividers;
			CGFloat priorityIncrement = 1.0 / numberOfViews;
			
			for (uint i = 0; i < numberOfViews; i++) {
				NSView currentView = views.ObjectAtIndex<NSView> (i);
				CGFloat percentOfTotalHeight = currentView.Frame.Height / spaceForAllViews;
				
				float constant = -((float)spaceForAllDividers * percentOfTotalHeight);
				
				NSLayoutConstraint heightConstraint = NSLayoutConstraint.ConstraintWithItemAttributeRelatedByToItemAttributeMultiplierConstant (
					currentView,
					NSLayoutAttribute.NSLayoutAttributeHeight, 
					NSLayoutRelation.NSLayoutRelationEqual,
					this,
					NSLayoutAttribute.NSLayoutAttributeHeight,
					percentOfTotalHeight,
					constant);
				
				if (dividerIndex == -2) {
					heightConstraint.Priority = NSLayoutPriority.NSLayoutPriorityDefaultLow;
				} else {
					float priority = (float)NSLayoutPriority.NSLayoutPriorityDefaultLow + (float)priorityIncrement * DistanceOfViewWithIndexFromDividerWithIndex (i, dividerIndex);
					heightConstraint.Priority = (NSLayoutPriority)priority;
				}
				
				constraints.AddObject (heightConstraint);
			}
			return constraints;
		}
		
		public void UpdateHeightConstraints ()
		{
			if (this.HeightConstraints == null) { 
				this.SetHeightConstraints (this.ConstraintsForHeightsWithPrioritiesLowestAroundDivider (-2)); 
			}
		}
		
		[ObjectiveCMessage("updateConstraints")]
		public override void UpdateConstraints ()
		{
			this.SendMessageSuper (SplitViewClass, "updateConstraints");
			this.UpdateViewStackConstraints ();
			this.UpdateHeightConstraints ();
		}
		
		public NSInteger dividerIndexForPoint (NSPoint point)
		{
			NSInteger dividerIndex = -1;
			Action<Id, NSUInteger, IntPtr > enumerator = delegate(Id id, NSUInteger i, IntPtr stop) {
				NSView view = id.CastTo<NSView> ();
				NSRect subviewFrame = view.Frame;
				if (point.y > subviewFrame.MaxY) {
					// the point is between us and the subview above
					dividerIndex = i - 1;
					Marshal.WriteByte (stop, 1); 
				} else if (point.y > subviewFrame.MinY) {
					// the point is in the interior of our view, not on a divider
					dividerIndex = -1;
					Marshal.WriteByte (stop, 1); 
				}
			};
			this.Subviews.EnumerateObjectsUsingBlock (enumerator);
			
			return dividerIndex;
		}
		
		[ObjectiveCMessage("mouseDown:")]
		public override void MouseDown (NSEvent theEvent)
		{
			NSPoint locationInSelf = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			int dividerIndex = this.dividerIndexForPoint (locationInSelf);
			
			if (dividerIndex != -1) {
				// First we lock the heights in place for the given dividerIndex
				this.SetHeightConstraints (this.ConstraintsForHeightsWithPrioritiesLowestAroundDivider (dividerIndex));
				
				// Now we add a constraint that forces the bottom edge of the view above the divider to align with the mouse location
				NSView viewAboveDivider = this.Subviews.ObjectAtIndex<NSView> ((uint)dividerIndex);
				this._draggingConstraint = NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:[viewAboveDivider]-100-|", 0, null, NSDictionary.DictionaryWithObjectForKey (viewAboveDivider, (NSString)"viewAboveDivider")).LastObject.Retain<NSLayoutConstraint> ();
				this._draggingConstraint.Priority = NSLayoutPriority.NSLayoutPriorityDragThatCannotResizeWindow;
				this._draggingConstraint .Constant = locationInSelf.y;
				
				this.AddConstraint (this._draggingConstraint);
			} else {
				this.SendMessageSuper (SplitViewClass, "mouseDown:", theEvent);
			}
		}
		
		[ObjectiveCMessage("mouseDragged:")]
		public override void MouseDragged (NSEvent theEvent)
		{
			if (this._draggingConstraint != null) {
				// update the dragging constraint for the new location
				NSPoint locationInSelf = this.ConvertPointFromView (theEvent.LocationInWindow, null);
				this._draggingConstraint .Constant = locationInSelf.y;
			} else {
				this.SendMessageSuper (SplitViewClass, "mouseDragged:", theEvent);
			}
		}
		
		[ObjectiveCMessage("mouseUp:")]
		public override void MouseUp (NSEvent theEvent)
		{
			if (this._draggingConstraint != null) {
				// update the dragging constraint for the new location
				this.RemoveConstraint (this._draggingConstraint);
				this._draggingConstraint.Release ();
				this._draggingConstraint = null;
					
				// We lock the current heights in place
				this.SetHeightConstraints (this.ConstraintsForHeightsWithPrioritiesLowestAroundDivider (-2));
			} else {
				this.SendMessageSuper (SplitViewClass, "mouseUp:", theEvent);
			}
		}
		
		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect (NSRect aRect)
		{
			NSColor.BlackColor.Set ();
			AppKitFramework.NSRectFill (aRect);
		}
		
		public NSArray ViewStackConstraints {
			get {
				return this._viewStackConstraints;
			}
		}
		
		public NSArray HeightConstraints {
			get {
				return this._heightConstraints;
			}
		}
	}
}

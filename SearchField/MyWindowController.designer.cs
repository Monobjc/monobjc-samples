// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Monobjc.Samples.SearchField {
	using Monobjc;
	using Monobjc.AppKit;
	using System;
	
	
	public partial class MyWindowController {
		
		partial void SheetDoneButtonAction(IntPtr sender);

		[IBOutlet()]
		[ObjectiveCIVar("searchField")]
		public virtual NSSearchField searchField {
			get {
				return this.GetInstanceVariable <NSSearchField>("searchField");
			}
			set {
				this.SetInstanceVariable <NSSearchField>("searchField", value);
			}
		}
		
		[IBOutlet()]
		[ObjectiveCIVar("simpleSheet")]
		public virtual NSWindow simpleSheet {
			get {
				return this.GetInstanceVariable <NSWindow>("simpleSheet");
			}
			set {
				this.SetInstanceVariable <NSWindow>("simpleSheet", value);
			}
		}
		
		[IBAction()]
		[ObjectiveCMessage("sheetDoneButtonAction:")]
		public virtual void __SheetDoneButtonAction(IntPtr sender) {
			this.SheetDoneButtonAction(sender);
		}
	}
}

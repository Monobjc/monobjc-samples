// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Monobjc.Samples.WhereIsMyMac {
	using Monobjc;
	using Monobjc.AppKit;
	using Monobjc.WebKit;
	
	
	public partial class WhereIsMyMacAppDelegate {
		
		partial void OpenInDefaultBrowser(Id sender);

		[IBOutlet()]
		[ObjectiveCIVar("accuracyLabel")]
		public virtual NSTextField accuracyLabel {
			get {
				return this.GetInstanceVariable <NSTextField>("accuracyLabel");
			}
			set {
				this.SetInstanceVariable <NSTextField>("accuracyLabel", value);
			}
		}
		
		[IBOutlet()]
		[ObjectiveCIVar("locationLabel")]
		public virtual NSTextField locationLabel {
			get {
				return this.GetInstanceVariable <NSTextField>("locationLabel");
			}
			set {
				this.SetInstanceVariable <NSTextField>("locationLabel", value);
			}
		}
		
		[IBOutlet()]
		[ObjectiveCIVar("webView")]
		public virtual WebView webView {
			get {
				return this.GetInstanceVariable <WebView>("webView");
			}
			set {
				this.SetInstanceVariable <WebView>("webView", value);
			}
		}
		
		[IBOutlet()]
		[ObjectiveCIVar("window")]
		public virtual NSWindow window {
			get {
				return this.GetInstanceVariable <NSWindow>("window");
			}
			set {
				this.SetInstanceVariable <NSWindow>("window", value);
			}
		}
		
		[IBAction()]
		[ObjectiveCMessage("openInDefaultBrowser:")]
		public virtual void __OpenInDefaultBrowser(Id sender) {
			this.OpenInDefaultBrowser(sender);
		}
	}
}

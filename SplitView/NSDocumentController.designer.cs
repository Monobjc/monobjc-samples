// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace SplitView {
	using Monobjc;
	using System;
	
	
	public partial class NSDocumentController {
		
		partial void ClearRecentDocuments(IntPtr sender);

		partial void NewDocument(IntPtr sender);

		partial void OpenDocument(IntPtr sender);

		partial void SaveAllDocuments(IntPtr sender);

		[IBAction()]
		[ObjectiveCMessage("clearRecentDocuments:")]
		public virtual void __ClearRecentDocuments(IntPtr sender) {
			this.ClearRecentDocuments(sender);
		}
		
		[IBAction()]
		[ObjectiveCMessage("newDocument:")]
		public virtual void __NewDocument(IntPtr sender) {
			this.NewDocument(sender);
		}
		
		[IBAction()]
		[ObjectiveCMessage("openDocument:")]
		public virtual void __OpenDocument(IntPtr sender) {
			this.OpenDocument(sender);
		}
		
		[IBAction()]
		[ObjectiveCMessage("saveAllDocuments:")]
		public virtual void __SaveAllDocuments(IntPtr sender) {
			this.SaveAllDocuments(sender);
		}
	}
}

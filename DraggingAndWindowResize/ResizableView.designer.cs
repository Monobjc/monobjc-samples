// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace net.monobjc.samples.DraggingAndWindowResize {
	using Monobjc;
	using net.monobjc.samples.DraggingAndWindowResize;
	
	
	public partial class ResizableView {
		
		[IBOutlet()]
		[ObjectiveCIVar("dragThumbView")]
		public virtual DragThumbView dragThumbView {
			get {
				return this.GetInstanceVariable <DragThumbView>("dragThumbView");
			}
			set {
				this.SetInstanceVariable <DragThumbView>("dragThumbView", value);
			}
		}
	}
}

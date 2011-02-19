using System;
using Monobjc.Foundation;
using Monobjc.AppKit;

namespace Monobjc.Samples.SimpleCocoaApp
{
    [ObjectiveCClass]
    public partial class HelloController : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelloController"/> class.
        /// </summary>
        public HelloController() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public HelloController(IntPtr nativePointer)
            : base(nativePointer) {}

        /// <summary>
        /// This method is called when the user picks a different message 
        /// to display by clicking a different radio button
        /// </summary>
        partial void SwitchMessage(NSMatrix sender)
        {
            // sender is the NSMatrix containing the radio buttons.
            // We ask the sender for which row (radio button) is selected and add one
            // to compensate for counting from zero.
            int which = sender.SelectedRow + 1;

            // We now set our NSButton's action to be the message corresponding to the radio button selection.
            // +[NSString stringWithFormat:...] is used to concatenate "message" and the message number.  
            // NSSelectorFromString converts the message name string to an actual message structure that
            // Objective-C can use.
            this.helloButton.Action = ObjectiveCRuntime.Selector(String.Format("message{0}:", which));
        }

        /// <summary>
        /// This method is called when the user picks a different object to 
        /// receive messages using the PopUp menu
        /// </summary>
        partial void SwitchObject(NSPopUpButton sender)
        {
            // sender is the NSPopUpMenu containing Hello object choices.
            // We ask the sender for which menu item is selected and add one
            // to compensate for counting from zero.
            int which = sender.IndexOfSelectedItem + 1;

            // Based on which menu item is selected, we set the target (the receiving object)
            // of the helloButton to point to either hello1 or hello2.
            if (which == 1)
            {
                this.helloButton.Target = this.hello1;
            }
            else
            {
                this.helloButton.Target = this.hello2;
            }
        }

        /// <summary>
        /// awakeFromNib is called when this object is done being unpacked from the nib file;
        /// at this point, we can do any needed initialization before turning app control over to the user
        /// </summary>
        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib() {}
    }
}
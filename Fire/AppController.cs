using System;
using Monobjc.AppKit;
using Monobjc.ApplicationServices;
using Monobjc.CoreFoundation;
using Monobjc.Foundation;
using Monobjc.QuartzCore;

namespace Monobjc.Samples.Fire
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        private CALayer rootLayer;
        private CAEmitterLayer fireEmitter;
        private CAEmitterLayer smokeEmitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        public AppController() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public AppController(IntPtr nativePointer)
            : base(nativePointer) { }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            //Create the root layer
            rootLayer = CALayer.Layer;

            //Set the root layer's background color to black
            rootLayer.BackgroundColor = CGColor.GetConstantColor(CGColor.kCGColorBlack);

            //Create the fire emitter layer
            fireEmitter = CAEmitterLayer.Layer;
            fireEmitter.EmitterPosition = new CGPoint(225, 50);
            fireEmitter.EmitterMode = CAEmitterLayer.kCAEmitterLayerOutline;
            fireEmitter.EmitterShape = CAEmitterLayer.kCAEmitterLayerLine;
            fireEmitter.RenderMode = CAEmitterLayer.kCAEmitterLayerAdditive;
            fireEmitter.EmitterSize = new CGSize(0, 0);

            //Create the smoke emitter layer
            smokeEmitter = CAEmitterLayer.Layer;
            smokeEmitter.EmitterPosition = new CGPoint(225, 50);
            smokeEmitter.EmitterMode = CAEmitterLayer.kCAEmitterLayerPoints;

            //Create the fire emitter cell
            CAEmitterCell fire = CAEmitterCell.EmitterCell;
            fire.EmissionLongitude = (float) Math.PI;
            fire.BirthRate = 0;
            fire.Velocity = 80;
            fire.VelocityRange = 30;
            fire.EmissionRange = 1.1f;
            fire.YAcceleration = 200;
            fire.ScaleSpeed = 0.3f;
            IntPtr color = CGColor.CreateGenericRGB(0.8f, 0.4f, 0.2f, 0.10f);
            fire.Color = color;
            CGColor.Release(color);
            fire.ContentsPointer = CGImageNamed("fire.png");

            //Name the cell so that it can be animated later using keypaths
            fire.Name = "fire";

            //Add the fire emitter cell to the fire emitter layer
            fireEmitter.EmitterCells = NSArray.ArrayWithObject(fire);

            //Create the smoke emitter cell
            CAEmitterCell smoke = CAEmitterCell.EmitterCell;
            smoke.BirthRate = 11;
            smoke.EmissionLongitude = (float)(Math.PI / 2);
            smoke.Lifetime = 0;
            smoke.Velocity = 40;
            smoke.VelocityRange = 20;
            smoke.EmissionRange = (float)(Math.PI / 4);
            smoke.Spin = 1;
            smoke.SpinRange = 6;
            smoke.YAcceleration = 160;
            smoke.ContentsPointer = CGImageNamed("smoke.png");
            smoke.Scale = 0.1f;
            smoke.AlphaSpeed = -0.12f;
            smoke.ScaleSpeed = 0.7f;

            //Name the cell so that it can be animated later using keypaths
            smoke.Name = "smoke";

            //Add the smoke emitter cell to the smoke emitter layer
            smokeEmitter.EmitterCells = NSArray.ArrayWithObject(smoke);

            //Add the two emitter layers to the root layer
            rootLayer.AddSublayer(smokeEmitter);
            rootLayer.AddSublayer(fireEmitter);

            //Set the view's layer to the base layer
            view.Layer = rootLayer;
            view.WantsLayer = true;

            //Set the fire simulation to reflect the initial slider position
            this.SlidersChanged(this);

            //Force the view to update
            view.NeedsDisplay = true;
        }

        partial void SlidersChanged(Id sender)
        {
            //Query the gasSlider's value
            float gas = gasSlider.IntValue / 100.0f;

            //Update the fire properties
            fireEmitter.SetValueForKeyPath(NSNumber.NumberWithInt((int)(gas * 1000)), "emitterCells.fire.birthRate");
            fireEmitter.SetValueForKeyPath(NSNumber.NumberWithFloat(gas), "emitterCells.fire.lifetime");
            fireEmitter.SetValueForKeyPath(NSNumber.NumberWithFloat(gas * 0.35f), "emitterCells.fire.lifetimeRange");
            fireEmitter.EmitterSize = new CGSize(50 * gas, 0);

            //Update the smoke properites
            smokeEmitter.SetValueForKeyPath(NSNumber.NumberWithInt((int)(gas * 4)), "emitterCells.smoke.lifetime");
            IntPtr color = CGColor.CreateGenericRGB(1.0f, 1.0f, 1.0f, gas * 0.3f);
            smokeEmitter.SetValueForKeyPath(new Id(color), "emitterCells.smoke.color");
            CGColor.Release(color);
        }

        //Return a CGImageRef from the specified image file in the app's bundle
        private static IntPtr CGImageNamed(NSString name)
        {
            NSURL url = NSBundle.MainBundle.URLForResourceWithExtension(name, null);
            IntPtr source = CGImageSource.CreateWithURL(url, null);
            IntPtr image = CGImageSource.CreateImageAtIndex(source, 0, null);
            CFType.CFRelease(source);
            return image;
        }
    }
}
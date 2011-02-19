using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.QuartzCore;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.Fireworks
{
	[ObjectiveCClass]
	public partial class AppController : NSObject, INSNibAwaking
	{
		public static readonly Class AppControllerClass = Class.Get (typeof(AppController));

		private CALayer rootLayer;
		private CAEmitterLayer mortor;

		public AppController ()
		{
		}

		public AppController (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("awakeFromNib")]
		public void AwakeFromNib ()
		{
			//Create the root layer
			this.rootLayer = CALayer.Layer;
			
			//Set the root layer's attributes
			this.rootLayer.Bounds = CGRect.CGRectMake (0, 0, 640, 480);
			IntPtr color = CGColor.CreateGenericRGB (0.0, 0.0, 0.0, 1);
			rootLayer.BackgroundColor = color;
			CGColor.Release (color);
			
			//Load the spark image for the particle
			String fileName = NSBundle.MainBundle.PathForResourceOfType ("tspark", "png");
			IntPtr dataProvider = CGDataProvider.CreateWithFilename (fileName);
			IntPtr img = CGImage.CreateWithPNGDataProvider (dataProvider, IntPtr.Zero, false, CGColorRenderingIntent.kCGRenderingIntentDefault);
			
			this.mortor = CAEmitterLayer.Layer.Retain<CAEmitterLayer>();
			this.mortor.EmitterPosition = CGPoint.CGPointMake (320, 0);
			this.mortor.RenderMode = CAEmitterLayer.kCAEmitterLayerAdditive;
			
			//Invisible particle representing the rocket before the explosion
			CAEmitterCell rocket = CAEmitterCell.EmitterCell.Retain<CAEmitterCell>();
			rocket.EmissionLongitude = Math.PI / 2;
			rocket.EmissionLatitude = 0;
			rocket.Lifetime = 1.6f;
			rocket.BirthRate = 1;
			rocket.Velocity = 400;
			rocket.VelocityRange = 100;
			rocket.YAcceleration = -250;
			rocket.EmissionRange = Math.PI / 4;
			color = CGColor.CreateGenericRGB (0.5, 0.5, 0.5, 0.5);
			rocket.Color = color;
			CGColor.Release (color);
			rocket.RedRange = 0.5f;
			rocket.GreenRange = 0.5f;
			rocket.BlueRange = 0.5f;
			
			//Name the cell so that it can be animated later using keypath
			rocket.Name = "rocket";
			
			//Flare particles emitted from the rocket as it flys
			CAEmitterCell flare = CAEmitterCell.EmitterCell;
			flare.ContentsPointer = img;
			flare.EmissionLongitude = (4 * Math.PI) / 2;
			flare.Scale = 0.4;
			flare.Velocity = 100;
			flare.BirthRate = 45;
			flare.Lifetime = 1.5f;
			flare.YAcceleration = -350;
			flare.EmissionRange = Math.PI / 7;
			flare.AlphaSpeed = -0.7f;
			flare.ScaleSpeed = -0.1;
			flare.ScaleRange = 0.1;
			flare.BeginTime = 0.01;
			flare.Duration = 0.7;
			
			//The particles that make up the explosion
			CAEmitterCell firework = CAEmitterCell.EmitterCell;
			firework.ContentsPointer = img;
			firework.BirthRate = 9999;
			firework.Scale = 0.6;
			firework.Velocity = 130;
			firework.Lifetime = 2;
			firework.AlphaSpeed = -0.2f;
			firework.YAcceleration = -80;
			firework.BeginTime = 1.5;
			firework.Duration = 0.1;
			firework.EmissionRange = 2 * Math.PI;
			firework.ScaleSpeed = -0.1;
			firework.Spin = 2;
			
			//Name the cell so that it can be animated later using keypath
			firework.Name = "firework";
			
			//preSpark is an invisible particle used to later emit the spark
			CAEmitterCell preSpark = CAEmitterCell.EmitterCell;
			preSpark.BirthRate = 80;
			preSpark.Velocity = firework.Velocity * 0.70;
			preSpark.Lifetime = 1.7f;
			preSpark.YAcceleration = firework.YAcceleration * 0.85;
			preSpark.BeginTime = firework.BeginTime - 0.2;
			preSpark.EmissionRange = firework.EmissionRange;
			preSpark.GreenSpeed = 100;
			preSpark.BlueSpeed = 100;
			preSpark.RedSpeed = 100;
			
			//Name the cell so that it can be animated later using keypath
			preSpark.Name = "preSpark";
			
			//The 'sparkle' at the end of a firework
			CAEmitterCell spark = CAEmitterCell.EmitterCell;
			spark.ContentsPointer = img;
			spark.Lifetime = 0.05f;
			spark.YAcceleration = -250;
			spark.BeginTime = 0.8;
			spark.Scale = 0.4;
			spark.BirthRate = 10;
			
			preSpark.EmitterCells = NSArray.ArrayWithObject (spark);
			rocket.EmitterCells = NSArray.ArrayWithObjects (flare, firework, preSpark, null);
			mortor.EmitterCells = NSArray.ArrayWithObject (rocket);
			this.rootLayer.AddSublayer (this.mortor);
			
			//Set the view's layer to the base layer
			theView.Layer = rootLayer;
			theView.WantsLayer = true;
			
			//Force the view to update
			theView.NeedsDisplay = true;
		}

		partial void SlidersMoved (Id sender)
		{
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat ((float)(rocketRange.FloatValue * Math.PI / 4)), "emitterCells.rocket.emissionRange");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat (rocketVelocity.FloatValue), "emitterCells.rocket.velocity");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat (rocketVelocityRange.FloatValue), "emitterCells.rocket.velocityRange");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat (-1 * rocketGravity.FloatValue), "emitterCells.rocket.yAcceleration");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat ((float) (fireworkRange.FloatValue * Math.PI / 4)), "emitterCells.rocket.emitterCells.firework.emissionRange");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat (fireworkVelocity.FloatValue), "emitterCells.rocket.emitterCells.firework.velocity");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat (fireworkVelocityRange.FloatValue), "emitterCells.rocket.emitterCells.firework.velocityRange");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat (-1 * fireworkGravity.FloatValue), "emitterCells.rocket.emitterCells.firework.yAcceleration");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat ((float)(fireworkVelocity.FloatValue * 0.70)), "emitterCells.rocket.emitterCells.preSpark.velocity");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat ((float)(fireworkGravity.FloatValue * -0.85)), "emitterCells.rocket.emitterCells.preSpark.yAcceleration");
			this.mortor.SetValueForKeyPath (NSNumber.NumberWithFloat ((float)(fireworkRange.FloatValue * Math.PI / 4)), "emitterCells.rocket.emitterCells.preSpark.emissionRange");
			
			this.mortor.Speed = animationSpeed.FloatValue / 100.0f;
		}

		partial void ResetSliders (Id sender)
		{
			rocketRange.IntValue = 1;
			rocketVelocity.IntValue = 400;
			rocketVelocityRange.IntValue = 100;
			rocketGravity.IntValue = 250;
			
			fireworkRange.IntValue = 8;
			fireworkVelocity.IntValue = 130;
			fireworkVelocityRange.IntValue = 0;
			fireworkGravity.IntValue = 80;
			
			animationSpeed.IntValue = 100;
			
			this.SlidersMoved (this);
		}
	}
}


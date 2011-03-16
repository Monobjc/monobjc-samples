using System;
using System.Runtime.InteropServices;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.SM2DGraphView;

namespace Monobjc.Samples.SM2DGraphing
{
    [ObjectiveCClass]
    public partial class SM2DGraphAppDelegate : NSObject
    {
        private static readonly Class SM2DGraphAppDelegateClass = Monobjc.Class.Get(typeof (SM2DGraphAppDelegate));

        private NSTimer _sm_addPointsTimer;
        private uint _sm_cacheNumPoints;
        private SMPieChartView _sm_clickedPie;
        private NSMutableData _sm_data_cos;
        private NSMutableData _sm_data_sin;
        private static NSPoint _sm_pointToAdd = new NSPoint(1.9f, 5.9f);
		
		[ObjectiveCIVar]
		public virtual SM2DGraphView.SM2DGraphView _sm_barGraph {
			get { return this.GetInstanceVariable <SM2DGraphView.SM2DGraphView>("_sm_barGraph"); }
			set { this.SetInstanceVariable <SM2DGraphView.SM2DGraphView>("_sm_barGraph", value); }
		}

		[ObjectiveCIVar]
		public virtual NSTextField _sm_clickedSlice {
			get { return this.GetInstanceVariable <NSTextField>("_sm_clickedSlice"); }
			set { this.SetInstanceVariable <NSTextField>("_sm_clickedSlice", value); }
		}

		[ObjectiveCIVar]
		public virtual NSTextField _sm_clickInfo {
			get { return this.GetInstanceVariable <NSTextField>("_sm_clickInfo"); }
			set { this.SetInstanceVariable <NSTextField>("_sm_clickInfo", value); }
		}

		[ObjectiveCIVar]
		public virtual SM2DGraphView.SM2DGraphView _sm_costTimeGraph {
			get { return this.GetInstanceVariable <SM2DGraphView.SM2DGraphView>("_sm_costTimeGraph"); }
			set { this.SetInstanceVariable <SM2DGraphView.SM2DGraphView>("_sm_costTimeGraph", value); }
		}

		[ObjectiveCIVar]
		public virtual SMPieChartView _sm_hardDrive {
			get { return this.GetInstanceVariable <SMPieChartView>("_sm_hardDrive"); }
			set { this.SetInstanceVariable <SMPieChartView>("_sm_hardDrive", value); }
		}

		[ObjectiveCIVar]
		public virtual NSSlider _sm_slider {
			get { return this.GetInstanceVariable <NSSlider>("_sm_slider"); }
			set { this.SetInstanceVariable <NSSlider>("_sm_slider", value); }
		}

		[ObjectiveCIVar]
		public virtual SMPieChartView _sm_timeChart {
			get { return this.GetInstanceVariable <SMPieChartView>("_sm_timeChart"); }
			set { this.SetInstanceVariable <SMPieChartView>("_sm_timeChart", value); }
		}

		[ObjectiveCIVar]
		public virtual SM2DGraphView.SM2DGraphView _sm_trigGraph {
			get { return this.GetInstanceVariable <SM2DGraphView.SM2DGraphView>("_sm_trigGraph"); }
			set { this.SetInstanceVariable <SM2DGraphView.SM2DGraphView>("_sm_trigGraph", value); }
		}

        public SM2DGraphAppDelegate() {}

        public SM2DGraphAppDelegate(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this._sm_data_sin.Release();
            this._sm_data_cos.Release();

            this.SendMessageSuper(SM2DGraphAppDelegateClass, "dealloc");
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this._sm_cacheNumPoints = (uint) this._sm_slider.IntValue;
            this._sm_recalcTrigData();

            // Inset the X Axis a bit to give enough room for the bars to display.
            this._sm_barGraph.SetAxisInsetForAxis(SM2DGraphView.SM2DGraphView.BarWidth, SM2DGraphAxis.kSM2DGraph_Axis_X);
            this._sm_barGraph.LiveRefresh = true;
            this._sm_barGraph.Title = @"Try this title!";

            this._sm_costTimeGraph.AttributedTitle = new NSAttributedString("Increasing Costs", NSDictionary.DictionaryWithObjectForKey(NSFont.BoldSystemFontOfSize(NSFont.SystemFontSize), NSAttributedString_AppKitAdditions.NSFontAttributeName));

            this._sm_barGraph.RefreshDisplay(this);
            this._sm_costTimeGraph.RefreshDisplay(this);
            this._sm_trigGraph.RefreshDisplay(this);

            this._sm_hardDrive.RefreshDisplay(this);
            this._sm_timeChart.RefreshDisplay(this);
        }

        [ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
        public bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication application)
        {
            return true;
        }

        [ObjectiveCMessage("changedSlider:")]
        public void ChangedSlider(Id sender)
        {
            // We're using a live slider, so this action method gets called a lot when the user grabs the slider knob.
            // However, it gets called TOO MUCH!  It's called even when the slider's intValue hasn't changed.
            // So, we cache the intValue to use and only update when we actually need to (when the value changes).
            if (this._sm_cacheNumPoints == this._sm_slider.IntValue)
            {
                return;
            }

            this._sm_cacheNumPoints = (uint) this._sm_slider.IntValue;
            this._sm_recalcTrigData();
            this._sm_trigGraph.ReloadData();
        }

        [ObjectiveCMessage("addPoints:")]
        public void AddPoints(Id sender)
        {
            if (this._sm_addPointsTimer == null)
            {
                this._sm_addPointsTimer = NSTimer.TimerWithTimeIntervalTargetSelectorUserInfoRepeats(0.3d, this, ObjectiveCRuntime.Selector("_sm_addPoint:"), null, true).Retain<NSTimer>();
                NSRunLoop.CurrentRunLoop.AddTimerForMode(this._sm_addPointsTimer, NSRunLoop.NSDefaultRunLoopMode);
                sender.CastTo<NSButton>().Title = "Stop";
            }
            else
            {
                this._sm_addPointsTimer.Invalidate();
                this._sm_addPointsTimer.Release();
                this._sm_addPointsTimer = null;

                this._sm_barGraph.RefreshDisplay(this);

                sender.CastTo<NSButton>().Title = "Add";
            }
        }

        [ObjectiveCMessage("_sm_addPoint:")]
        public void _sm_addPoint(NSTimer timer)
        {
            // Do some little bit of calculating to add a new point within the view of the graph.
            _sm_pointToAdd.x -= 0.1f;
            if (_sm_pointToAdd.x < 0.0f)
            {
                _sm_pointToAdd.x = 2.0f;
            }

            _sm_pointToAdd.y -= 0.1f;
            if (_sm_pointToAdd.y < 0.0f)
            {
                _sm_pointToAdd.y = 10.0f;
            }

            this._sm_barGraph.AddDataPointToLineIndex(_sm_pointToAdd, 1);
        }

        private void _sm_recalcTrigData()
        {
            int index;
            float radians;
            double constant1, constant2;

            if (this._sm_data_sin != null)
            {
                this._sm_data_sin.Release();
            }
            if (this._sm_data_cos != null)
            {
                this._sm_data_cos.Release();
            }

            this._sm_data_sin = new NSMutableData();
            this._sm_data_cos = new NSMutableData();

            constant1 = 720.0d/this._sm_cacheNumPoints; // Degrees per resolved scale (the higher the resolution the fewer degrees)
            constant2 = Math.PI/180.0d; // Radians per degree (pi radians is a half circle).

            for (index = 0; index <= this._sm_cacheNumPoints; index++)
            {
                NSPoint dataPoint = NSPoint.NSZeroPoint;

                // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                IntPtr data_sin = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (NSPoint)));
                IntPtr data_cos = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (NSPoint)));

                // The x axis scale is in degrees.
                dataPoint.x = (float) (index*constant1 + -360.0f);

                // The trig functions use radians...convert from degrees to radians.
                radians = (float) (dataPoint.x*constant2);

                // Get the sine.
                dataPoint.y = (float) Math.Sin(radians);

                // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                Marshal.StructureToPtr(dataPoint, data_sin, false);
                this._sm_data_sin.AppendBytesLength(data_sin, (uint) Marshal.SizeOf(typeof (NSPoint)));

                // Get the cosine.
                dataPoint.y = (float) Math.Cos(radians);

                // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                Marshal.StructureToPtr(dataPoint, data_cos, false);
                this._sm_data_cos.AppendBytesLength(data_cos, (uint) Marshal.SizeOf(typeof (NSPoint)));

                // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                Marshal.FreeHGlobal(data_sin);
                Marshal.FreeHGlobal(data_cos);
            }
        }
    }
}
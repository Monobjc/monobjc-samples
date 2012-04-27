using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.CorePlot;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.RangePlot
{
	[ObjectiveCClass]
	public partial class Controller : NSObject
	{
		public static readonly Class ControllerClass = Class.Get (typeof(Controller));
		
		public static readonly NSString PLOT_IDENTIFIER = new NSString("Range Plot");

		private CPTXYGraph graph;
		private NSArray plotData;
		private CPTFill areaFill;
		private CPTLineStyle barLineStyle;

		public Controller ()
		{
		}

		public Controller (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this.graph.SafeRelease ();
			this.plotData.SafeRelease ();
			this.areaFill.SafeRelease ();
			this.barLineStyle.SafeRelease ();
			this.SendMessageSuper (ControllerClass, "dealloc");
		}

		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			// If you make sure your dates are calculated at noon, you shouldn't have to 
			// worry about daylight savings. If you use midnight, you will have to adjust
			// for daylight savings time.
			NSDate refDate = NSDate.DateWithNaturalLanguageString ("12:00 Oct 27, 2010");
			double oneDay = 24 * 60 * 60;
			
			// Create graph from theme
			graph = new CPTXYGraph (CGRect.CGRectZero);
			CPTTheme theme = CPTTheme.ThemeNamed (CPTTheme.kCPTDarkGradientTheme);
			graph.ApplyTheme (theme);
			hostView.HostedLayer = graph;
			
			// Title
			CPTMutableTextStyle textStyle = CPTMutableTextStyle.TextStyle;
			textStyle.Color = CPTColor.WhiteColor;
			textStyle.FontSize = 18.0;
			textStyle.FontName = "Helvetica";
			graph.Title = "Click to Toggle Range Plot Style";
			graph.TitleTextStyle = textStyle;
			graph.TitleDisplacement = new CGPoint (0.0f, -20.0f);
			
			// Setup scatter plot space
			CPTXYPlotSpace plotSpace = graph.DefaultPlotSpace.CastTo<CPTXYPlotSpace> ();
			double xLow = oneDay * 0.5;
			plotSpace.XRange = CPTPlotRange.PlotRangeWithLocationLength (NSDecimal.FromDouble (xLow), NSDecimal.FromDouble (oneDay * 5.0));
			plotSpace.YRange = CPTPlotRange.PlotRangeWithLocationLength (NSDecimal.FromDouble (1.0), NSDecimal.FromDouble (3.0));
			
			// Axes
			CPTXYAxisSet axisSet = graph.AxisSet.CastTo<CPTXYAxisSet> ();
			CPTXYAxis x = axisSet.XAxis;
			x.MajorIntervalLength = NSDecimal.FromDouble (oneDay);
			x.OrthogonalCoordinateDecimal = NSDecimal.FromString ("2");
			x.MinorTicksPerInterval = 0;
			NSDateFormatter dateFormatter = new NSDateFormatter ().Autorelease<NSDateFormatter> ();
			dateFormatter.DateStyle = NSDateFormatterStyle.NSDateFormatterShortStyle;
			CPTTimeFormatter timeFormatter = new CPTTimeFormatter (dateFormatter).Autorelease<CPTTimeFormatter> ();
			timeFormatter.ReferenceDate = refDate;
			x.LabelFormatter = timeFormatter;
			
			CPTXYAxis y = axisSet.YAxis;
			y.MajorIntervalLength = NSDecimal.FromString (@"0.5");
			y.MinorTicksPerInterval = 5;
			y.OrthogonalCoordinateDecimal = NSDecimal.FromDouble (oneDay);
			
			// Create a plot that uses the data source method
			CPTRangePlot dataSourceLinePlot = new CPTRangePlot ().Autorelease<CPTRangePlot> ();
			dataSourceLinePlot.Identifier = PLOT_IDENTIFIER;
			
			// Add line style
			CPTMutableLineStyle lineStyle = CPTMutableLineStyle.LineStyle;
			lineStyle.LineWidth = 1.0;
			lineStyle.LineColor = CPTColor.GreenColor;
			barLineStyle = lineStyle.Retain<CPTLineStyle> ();
			dataSourceLinePlot.BarLineStyle = barLineStyle;
			
			// Bar properties
			dataSourceLinePlot.BarWidth = 10.0;
			dataSourceLinePlot.GapWidth = 20.0;
			dataSourceLinePlot.GapHeight = 20.0;
			dataSourceLinePlot.DataSource = this;
			
			// Add plot
			graph.AddPlot (dataSourceLinePlot);
			graph.DefaultPlotSpace.Delegate = this;
			
			// Store area fill for use later
			CPTColor transparentGreen = CPTColor.GreenColor.ColorWithAlphaComponent (0.2);
			areaFill = new CPTFill (transparentGreen);
			
			// Add some data
			NSMutableArray newData = new NSMutableArray();
			NSUInteger i;
			Random rand = new Random ();
			for (i = 0; i < 5; i++) {
				double xx = oneDay * (i + 1.0);
				double yy = 3.0 * rand.Next () / (double)Int32.MaxValue + 1.2;
				double rHigh = rand.Next () / (double)Int32.MaxValue * 0.5 + 0.25;
				double rLow = rand.Next () / (double)Int32.MaxValue * 0.5 + 0.25;
				double rLeft = (rand.Next () / (double)Int32.MaxValue * 0.125 + 0.125) * oneDay;
				double rRight = (rand.Next () / (double)Int32.MaxValue * 0.125 + 0.125) * oneDay;
				
				newData.AddObject (NSDictionary.DictionaryWithObjectsAndKeys (
				                                                              NSDecimalNumber.NumberWithDouble (xx), CPTRangePlot.FieldX, 
				                                                              NSDecimalNumber.NumberWithDouble (yy), CPTRangePlot.FieldY,
				                                                              NSDecimalNumber.NumberWithDouble (rHigh), CPTRangePlot.FieldHigh, 
				                                                              NSDecimalNumber.NumberWithDouble (rLow), CPTRangePlot.FieldLow, 
				                                                              NSDecimalNumber.NumberWithDouble (rLeft), CPTRangePlot.FieldLeft,
				                                                              NSDecimalNumber.NumberWithDouble (rRight), CPTRangePlot.FieldRight, 
				                                                              null));
			}
			
			plotData = newData;
		}

		[ObjectiveCMessage("numberForPlot:field:recordIndex:")]
		public virtual NSNumber NumberForPlotFieldRecordIndex (CPTPlot plot, NSUInteger fieldEnum, NSUInteger index)
		{
			NSNumber num = plotData.ObjectAtIndex<NSDictionary>(index).ObjectForKey<NSNumber>(NSNumber.NumberWithUnsignedInteger (fieldEnum));
			return num;
		}

		[ObjectiveCMessage("numberOfRecordsForPlot:")]
		public virtual NSUInteger NumberOfRecordsForPlot (CPTPlot plot)
		{
			return this.plotData.Count;
		}

		[ObjectiveCMessage("plotSpace:shouldHandlePointingDeviceUpEvent:atPoint:")]
		public virtual bool PlotSpaceShouldHandlePointingDeviceUpEventAtPoint (CPTPlotSpace space, Id @event, CGPoint point)
		{
			CPTRangePlot rangePlot = graph.PlotWithIdentifier(PLOT_IDENTIFIER).CastTo<CPTRangePlot>();
			rangePlot.AreaFill = ( rangePlot.AreaFill != null ? null : areaFill );
			rangePlot.BarLineStyle = ( rangePlot.BarLineStyle != null ? null : barLineStyle );
			return false;
		}
	}
}

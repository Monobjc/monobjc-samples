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

		private CPXYGraph graph;
		private NSArray plotData;
		private CPFill areaFill;
		private CPLineStyle barLineStyle;

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
			graph = new CPXYGraph (CGRect.CGRectZero);
			CPTheme theme = CPTheme.ThemeNamed (CPTheme.kCPDarkGradientTheme);
			graph.ApplyTheme (theme);
			hostView.HostedLayer = graph;
			
			// Title
			CPMutableTextStyle textStyle = CPMutableTextStyle.TextStyle;
			textStyle.Color = CPColor.WhiteColor;
			textStyle.FontSize = 18.0;
			textStyle.FontName = "Helvetica";
			graph.Title = "Click to Toggle Range Plot Style";
			graph.TitleTextStyle = textStyle;
			graph.TitleDisplacement = new CGPoint (0.0f, -20.0f);
			
			// Setup scatter plot space
			CPXYPlotSpace plotSpace = graph.DefaultPlotSpace.CastTo<CPXYPlotSpace> ();
			double xLow = oneDay * 0.5;
			plotSpace.XRange = CPPlotRange.PlotRangeWithLocationLength (NSDecimal.FromDouble (xLow), NSDecimal.FromDouble (oneDay * 5.0));
			plotSpace.YRange = CPPlotRange.PlotRangeWithLocationLength (NSDecimal.FromDouble (1.0), NSDecimal.FromDouble (3.0));
			
			// Axes
			CPXYAxisSet axisSet = graph.AxisSet.CastTo<CPXYAxisSet> ();
			CPXYAxis x = axisSet.XAxis;
			x.MajorIntervalLength = NSDecimal.FromDouble (oneDay);
			x.OrthogonalCoordinateDecimal = NSDecimal.FromString ("2");
			x.MinorTicksPerInterval = 0;
			NSDateFormatter dateFormatter = new NSDateFormatter ().Autorelease<NSDateFormatter> ();
			dateFormatter.DateStyle = NSDateFormatterStyle.NSDateFormatterShortStyle;
			CPTimeFormatter timeFormatter = new CPTimeFormatter (dateFormatter).Autorelease<CPTimeFormatter> ();
			timeFormatter.ReferenceDate = refDate;
			x.LabelFormatter = timeFormatter;
			
			CPXYAxis y = axisSet.YAxis;
			y.MajorIntervalLength = NSDecimal.FromString (@"0.5");
			y.MinorTicksPerInterval = 5;
			y.OrthogonalCoordinateDecimal = NSDecimal.FromDouble (oneDay);
			
			// Create a plot that uses the data source method
			CPRangePlot dataSourceLinePlot = new CPRangePlot ().Autorelease<CPRangePlot> ();
			dataSourceLinePlot.Identifier = PLOT_IDENTIFIER;
			
			// Add line style
			CPMutableLineStyle lineStyle = CPMutableLineStyle.LineStyle;
			lineStyle.LineWidth = 1.0;
			lineStyle.LineColor = CPColor.GreenColor;
			barLineStyle = lineStyle.Retain<CPLineStyle> ();
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
			CPColor transparentGreen = CPColor.GreenColor.ColorWithAlphaComponent (0.2);
			areaFill = new CPFill (transparentGreen);
			
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
				                                                              NSDecimalNumber.NumberWithDouble (xx), CPRangePlot.FieldX, 
				                                                              NSDecimalNumber.NumberWithDouble (yy), CPRangePlot.FieldY,
				                                                              NSDecimalNumber.NumberWithDouble (rHigh), CPRangePlot.FieldHigh, 
				                                                              NSDecimalNumber.NumberWithDouble (rLow), CPRangePlot.FieldLow, 
				                                                              NSDecimalNumber.NumberWithDouble (rLeft), CPRangePlot.FieldLeft,
				                                                              NSDecimalNumber.NumberWithDouble (rRight), CPRangePlot.FieldRight, 
				                                                              null));
			}
			
			plotData = newData;
		}

		[ObjectiveCMessage("numberForPlot:field:recordIndex:")]
		public virtual NSNumber NumberForPlotFieldRecordIndex (CPPlot plot, NSUInteger fieldEnum, NSUInteger index)
		{
			NSNumber num = plotData.ObjectAtIndex (index).CastTo<NSDictionary> ().ObjectForKey (NSNumber.NumberWithInt (fieldEnum)).CastTo<NSNumber> ();
			return num;
		}

		[ObjectiveCMessage("numberOfRecordsForPlot:")]
		public virtual NSUInteger NumberOfRecordsForPlot (CPPlot plot)
		{
			return this.plotData.Count;
		}

		[ObjectiveCMessage("plotSpace:shouldHandlePointingDeviceUpEvent:atPoint:")]
		public virtual bool PlotSpaceShouldHandlePointingDeviceUpEventAtPoint (CPPlotSpace space, Id @event, CGPoint point)
		{
			CPRangePlot rangePlot = graph.PlotWithIdentifier(PLOT_IDENTIFIER).CastTo<CPRangePlot>();
			rangePlot.AreaFill = ( rangePlot.AreaFill != null ? null : areaFill );
			rangePlot.BarLineStyle = ( rangePlot.BarLineStyle != null ? null : barLineStyle );
			return false;
		}
	}
}

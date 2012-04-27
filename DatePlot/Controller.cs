using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.ApplicationServices;
using Monobjc.CorePlot;
using Monobjc.Foundation;

namespace Monobjc.Samples.DatePlot
{
	[ObjectiveCClass]
	public partial class Controller : NSObject
	{
		public static readonly Class ControllerClass = Class.Get (typeof(Controller));

		public static readonly NSString PLOT_IDENTIFIER = new NSString("Date Plot");
		
		private CPTXYGraph graph;
		private NSArray plotData;
		
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
			CPTScatterPlot dataSourceLinePlot = new CPTScatterPlot ().Autorelease<CPTScatterPlot> ();
			dataSourceLinePlot.Identifier = PLOT_IDENTIFIER;
			
			// Add line style
			CPTMutableLineStyle lineStyle = CPTMutableLineStyle.LineStyle;
			lineStyle.LineWidth = 3.0;
			lineStyle.LineColor = CPTColor.GreenColor;
			dataSourceLinePlot.DataLineStyle = lineStyle;
			
			// Add plot
			graph.AddPlot (dataSourceLinePlot);
			dataSourceLinePlot.DataSource = this;
			
			// Add some data
			NSMutableArray newData = new NSMutableArray();
			NSUInteger i;
			Random rand = new Random ();
			for (i = 0; i < 5; i++) {
				double xx = oneDay * (i + 1.0);
				double yy = 3.0 * rand.Next () / (double)Int32.MaxValue + 1.2;

				newData.AddObject (NSDictionary.DictionaryWithObjectsAndKeys (
				                                                              NSDecimalNumber.NumberWithDouble (xx), CPTScatterPlot.FieldX, 
				                                                              NSDecimalNumber.NumberWithDouble (yy), CPTScatterPlot.FieldY,
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
	}
}

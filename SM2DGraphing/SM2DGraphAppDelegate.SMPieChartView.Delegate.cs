using System;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.SM2DGraphView;

namespace Monobjc.Samples.SM2DGraphing
{
    partial class SM2DGraphAppDelegate
    {
        [ObjectiveCMessage("pieChartView:didClickPoint:")]
        public void PieChartViewDidClickPoint(SMPieChartView inPieChartView, NSPoint inPoint)
        {
            int slice = inPieChartView.ConvertToSliceFromPointFromView(inPoint, inPieChartView);

            this._sm_clickedSlice.IntValue = slice;
            this._sm_clickedPie = inPieChartView;

            // We draw differently depending on what was clicked.
            this._sm_hardDrive.ReloadAttributes();
            this._sm_timeChart.ReloadAttributes();
        }

        [ObjectiveCMessage("pieChartView:labelForSliceIndex:")]
        public NSString PieChartViewLabelForSliceIndex(SMPieChartView inPieChartView, uint inSliceIndex)
        {
            return String.Format("Slice {0}", inSliceIndex);
        }

        [ObjectiveCMessage("pieChartViewCompletedDrawing:")]
        public void PieChartViewCompletedDrawing(SMPieChartView inPieChartView)
        {
            // This is just an example of what you could do...
            //if (inPieChartView == this._sm_hardDrive)
            //{
            //    Console.WriteLine("We're done drawing the hard drive usage chart.");
            //}
        }
    }
}
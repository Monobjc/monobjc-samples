// 
// Monobjc : a .NET/Objective-C bridge
// Copyright (C) 2007-2009  Laurent Etiemble
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
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
using System;
using System.ComponentModel;
using Monobjc.Foundation;

namespace Monobjc.Samples.BackgroundWorkerApp
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        private BackgroundWorker backgroundWorker;
        private int highestPercentageReached;
        private int numberToCompute;

        public AppController() {}

        public AppController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this.sliderIterations.DoubleValue = 40.0d;
            this.labelResult.StringValue = NSString.String;

            this.progressIndicator.DoubleValue = 0.0d;
            this.buttonStop.IsEnabled = false;

            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;

            this.backgroundWorker.DoWork += this.backgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
            this.backgroundWorker.ProgressChanged += this.backgroundWorker_ProgressChanged;
        }

        partial void Start(Id sender)
        {
            this.labelResult.StringValue = NSString.String;
            this.progressIndicator.DoubleValue = 0.0d;

            this.numberToCompute = (int) this.sliderIterations.DoubleValue;
            this.highestPercentageReached = 0;

            this.buttonStart.IsEnabled = false;
            this.buttonStop.IsEnabled = true;

            this.backgroundWorker.RunWorkerAsync(this.numberToCompute);
        }

        partial void Stop(Id sender)
        {
            this.backgroundWorker.CancelAsync();

            this.buttonStop.IsEnabled = false;
        }

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = this.ComputeFibonacci((int) e.Argument, worker, e);
        }

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NSAutoreleasePool pool = new NSAutoreleasePool();

            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                // TODO
            }
            else if (e.Cancelled)
            {
                this.labelResult.StringValue = "Canceled";
            }
            else
            {
                this.labelResult.StringValue = "Fibonacci result for " + this.numberToCompute + " iterations is " + e.Result;
            }

            this.progressIndicator.DoubleValue = 0.0d;

            this.buttonStart.IsEnabled = true;
            this.buttonStop.IsEnabled = false;

            pool.Release();
        }

        // This event handler updates the progress bar.
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            NSAutoreleasePool pool = new NSAutoreleasePool();

            this.progressIndicator.DoubleValue = e.ProgressPercentage;
            this.labelResult.StringValue = "Progress is " + e.ProgressPercentage + "%";

            pool.Release();
        }

        // This is the method that does the actual work. For this
        // example, it computes a Fibonacci number and
        // reports progress as it does its work.
        private long ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        {
            // The parameter n must be >= 0 and <= 91.
            // Fib(n), with n > 91, overflows a long.
            if ((n < 0) || (n > 91))
            {
                throw new ArgumentException("value must be >= 0 and <= 91", "n");
            }

            long result = 0;

            // Abort the operation if the user has canceled.
            // Note that a call to CancelAsync may have set 
            // CancellationPending to true just after the
            // last invocation of this method exits, so this 
            // code will not have the opportunity to set the 
            // DoWorkEventArgs.Cancel flag to true. This means
            // that RunWorkerCompletedEventArgs.Cancelled will
            // not be set to true in your RunWorkerCompleted
            // event handler. This is a race condition.
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                if (n < 2)
                {
                    result = 1;
                }
                else
                {
                    result = this.ComputeFibonacci(n - 1, worker, e) +
                             this.ComputeFibonacci(n - 2, worker, e);
                }

                // Report progress as a percentage of the total task.
                int percentComplete = (int) ((float) n/(float) this.numberToCompute*100);
                if (percentComplete > this.highestPercentageReached)
                {
                    this.highestPercentageReached = percentComplete;
                    worker.ReportProgress(percentComplete);
                }
            }

            return result;
        }
    }
}
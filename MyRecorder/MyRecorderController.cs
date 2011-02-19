using System;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.QTKit;

namespace Monobjc.Samples.MyRecorder
{
    [ObjectiveCClass]
    public partial class MyRecorderController : NSObject
    {
        private static readonly Class MyRecorderControllerClass = Class.Get(typeof (MyRecorderController));

        private QTCaptureSession mCaptureSession;
        private QTCaptureMovieFileOutput mCaptureMovieFileOutput;
        private QTCaptureDeviceInput mCaptureVideoDeviceInput;
        private QTCaptureDeviceInput mCaptureAudioDeviceInput;

        public MyRecorderController() {}

        public MyRecorderController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // Create the capture session    
            this.mCaptureSession = new QTCaptureSession();

            // Connect inputs and outputs to the session	
            bool success;
            NSError error;

            // Find a video device  
            QTCaptureDevice videoDevice = QTCaptureDevice.DefaultInputDeviceWithMediaType(QTMedia.QTMediaTypeVideo);
            if (videoDevice != null) 
			{
	            success = videoDevice.Open(out error);
			}
			else
			{
				success = false;
			}

            // If a video input device can't be found or opened, try to find and open a muxed input device
            if (!success)
            {
                videoDevice = QTCaptureDevice.DefaultInputDeviceWithMediaType(QTMedia.QTMediaTypeMuxed);
            }
			
            if (videoDevice != null) 
			{
	            success = videoDevice.Open(out error);
			}
			else
			{
				success = false;
			}

            if (!success)
            {
                videoDevice = null;
                // Handle error
            }

            if (videoDevice != null)
            {
                //Add the video device to the session as a device input
                this.mCaptureVideoDeviceInput = new QTCaptureDeviceInput(videoDevice);
                success = this.mCaptureSession.AddInputError(this.mCaptureVideoDeviceInput, out error);
                if (!success)
                {
                    // Handle error
                }

                // If the video device doesn't also supply audio, add an audio device input to the session
                if (!videoDevice.HasMediaType(QTMedia.QTMediaTypeSound) && !videoDevice.HasMediaType(QTMedia.QTMediaTypeMuxed))
                {
                    QTCaptureDevice audioDevice = QTCaptureDevice.DefaultInputDeviceWithMediaType(QTMedia.QTMediaTypeSound);
                    success = audioDevice.Open(out error);

                    if (!success)
                    {
                        audioDevice = null;
                        // Handle error
                    }

                    if (audioDevice != null)
                    {
                        this.mCaptureAudioDeviceInput = new QTCaptureDeviceInput(audioDevice);

                        success = this.mCaptureSession.AddInputError(this.mCaptureAudioDeviceInput, out error);
                        if (!success)
                        {
                            // Handle error
                        }
                    }
                }

                // Create the movie file output and add it to the session
                this.mCaptureMovieFileOutput = new QTCaptureMovieFileOutput();
                success = this.mCaptureSession.AddOutputError(this.mCaptureMovieFileOutput, out error);
                if (!success)
                {
                    // Handle error
                }

                this.mCaptureMovieFileOutput.Delegate = this;

                // Set the compression for the audio/video that is recorded to the hard disk.
                foreach (QTCaptureConnection connection in this.mCaptureMovieFileOutput.Connections.GetEnumerator<QTCaptureConnection>())
                {
                    NSString mediaType = connection.MediaType;
                    QTCompressionOptions compressionOptions = null;

                    // specify the video compression options
                    // (note: a list of other valid compression types can be found in the QTCompressionOptions.h interface file)
                    if (mediaType.IsEqualToString(QTMedia.QTMediaTypeVideo))
                    {
                        // use H.264
                        compressionOptions = QTCompressionOptions.CompressionOptionsWithIdentifier("QTCompressionOptions240SizeH264Video");
                        // specify the audio compression options
                    }
                    else if (mediaType.IsEqualToString(QTMedia.QTMediaTypeSound))
                    {
                        // use AAC Audio
                        compressionOptions = QTCompressionOptions.CompressionOptionsWithIdentifier("QTCompressionOptionsHighQualityAACAudio");
                    }

                    // set the compression options for the movie file output
                    this.mCaptureMovieFileOutput.SetCompressionOptionsForConnection(compressionOptions, connection);
                }

                // Associate the capture view in the UI with the session
                this.mCaptureView.CaptureSession = this.mCaptureSession;
                this.mCaptureSession.StartRunning();
            }
        }

        [ObjectiveCMessage("windowWillClose:")]
        public void WindowWillClose(NSNotification notification)
        {
            this.mCaptureSession.StopRunning();
            if (this.mCaptureVideoDeviceInput != null && this.mCaptureVideoDeviceInput.Device.IsOpen)
            {
                this.mCaptureVideoDeviceInput.Device.Close();
            }
            if (this.mCaptureAudioDeviceInput != null && this.mCaptureAudioDeviceInput.Device.IsOpen)
            {
                this.mCaptureAudioDeviceInput.Device.Close();
            }
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.mCaptureSession.SafeRelease();
            this.mCaptureVideoDeviceInput.SafeRelease();
            this.mCaptureAudioDeviceInput.SafeRelease();
            this.mCaptureMovieFileOutput.SafeRelease();

            this.SendMessageSuper(MyRecorderControllerClass, "dealloc");
        }

        partial void StartRecording(Id sender)
        {
            this.mCaptureMovieFileOutput.RecordToOutputFileURL(NSURL.FileURLWithPath("/Users/Shared/My Recorded Movie.mov"));
        }

        partial void StopRecording(Id sender)
        {
            this.mCaptureMovieFileOutput.RecordToOutputFileURL(null);
        }

        // Do something with your QuickTime movie at the path you've specified at /Users/Shared/My Recorded Movie.mov"
        [ObjectiveCMessage("captureOutput:didFinishRecordingToOutputFileAtURL:forConnections:dueToError:")]
        public void captureOutputDidFinishRecordingToOutputFileAtURLForConnectionsDueToError(QTCaptureFileOutput captureOutput, NSURL outputFileURL, NSArray connections, NSError error)
        {
            NSWorkspace.SharedWorkspace.OpenURL(outputFileURL);
        }
    }
}
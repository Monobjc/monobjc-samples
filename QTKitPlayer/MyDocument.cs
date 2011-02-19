using System;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.QTKit;

namespace Monobjc.Samples.QTKitPlayer
{
    [ObjectiveCClass]
    public partial class MyDocument : NSDocument
    {
        private static readonly Class MyDocumentClass = Class.Get(typeof (MyDocument));

        public MyDocument() {}

        public MyDocument(IntPtr nativePointer)
            : base(nativePointer) {}

        public override NSString WindowNibName
        {
            [ObjectiveCMessage("windowNibName")]
            get { return "MyDocument"; }
        }

        [ObjectiveCMessage("windowControllerDidLoadNib:")]
        public override void WindowControllerDidLoadNib(NSWindowController aController)
        {
            this.SendMessageSuper(MyDocumentClass, "windowControllerDidLoadNib:", aController);

            if (this.FileURL != null)
            {
                NSError error;
                QTMovie movie = QTMovie.MovieWithURLError(this.FileURL, out error);

                movie.SetAttributeForKey((NSNumber) true, QTMovie.QTMovieEditableAttribute);
                this.mMovieView.Movie = movie;
            }        }

        [ObjectiveCMessage("dataRepresentationOfType:")]
        public override NSData DataRepresentationOfType(NSString aType)
        {
            return null;
        }

        [ObjectiveCMessage("loadDataRepresentation:ofType:")]
        public bool LoadDataRepresentation(NSData data, NSString aType)
        {
            return true;
        }

        [ObjectiveCMessage("saveDocument:")]
        public override void SaveDocument(Id sender)
        {
            this.mMovieView.Movie.UpdateMovieFile();
            this.UpdateChangeCount(NSDocumentChangeType.NSChangeCleared);
        }
    }
}
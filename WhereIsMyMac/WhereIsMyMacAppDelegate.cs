using System;
using Monobjc.Foundation;
using Monobjc.CoreLocation;
using Monobjc.WebKit;
using Monobjc.AppKit;

namespace Monobjc.Samples.WhereIsMyMac
{
    [ObjectiveCClass]
    public partial class WhereIsMyMacAppDelegate : NSObject
    {
        private CLLocationManager locationManager;

        public WhereIsMyMacAppDelegate() {}

        public WhereIsMyMacAppDelegate(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            this.locationManager = new CLLocationManager();
            this.locationManager.Delegate = this;
            this.locationManager.StartUpdatingLocation();
        }

        [ObjectiveCMessage("latitudeRangeForLocation:")]
        public static double LatitudeRangeForLocation(CLLocation aLocation)
        {
            const double M = 6367000.0;
            const double metersToLatitude = 1.0/((Math.PI/180.0d)*M);
            const double accuracyToWindowScale = 2.0;
            return aLocation.HorizontalAccuracy*metersToLatitude*accuracyToWindowScale;
        }

        [ObjectiveCMessage("longitudeRangeForLocation:")]
        public static double LongitudeRangeForLocation(CLLocation aLocation)
        {
            double latitudeRange = LatitudeRangeForLocation(aLocation);
            return latitudeRange*Math.Cos(aLocation.Coordinate.latitude*Math.PI/180.0d);
        }

        partial void OpenInDefaultBrowser(Id sender)
        {
            CLLocation currentLocation = this.locationManager.Location;

            NSURL externalBrowserURL = NSURL.URLWithString(NSString.StringWithFormat(
                                                               "http://maps.google.com/maps?ll=%f,%f&amp;spn=%f,%f",
                                                               currentLocation.Coordinate.latitude,
                                                               currentLocation.Coordinate.longitude,
                                                               LatitudeRangeForLocation(currentLocation),
                                                               LongitudeRangeForLocation(currentLocation)));

            NSWorkspace.SharedWorkspace.OpenURL(externalBrowserURL);
        }

        [ObjectiveCMessage("locationManager:didUpdateToLocation:fromLocation:")]
        public void LocationManagerDidUpdateToLocationFromLocation(CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
        {
            // Ignore updates where nothing we care about changed
            if (oldLocation != null &&
                newLocation.Coordinate.longitude == oldLocation.Coordinate.longitude &&
                newLocation.Coordinate.latitude == oldLocation.Coordinate.latitude &&
                newLocation.HorizontalAccuracy == oldLocation.HorizontalAccuracy)
            {
                return;
            }

            // Load the HTML for displaying the Google map from a file and replace the
            // format placeholders with our location data
            NSError error;
            NSString htmlString = NSString.StringWithFormat(
                NSString.StringWithContentsOfFileEncodingError(
                NSBundle.MainBundle.PathForResourceOfType("HTMLFormatString", @"html"),
                NSStringEncoding.NSUTF8StringEncoding,out error),
                newLocation.Coordinate.latitude,
                newLocation.Coordinate.longitude,
                LatitudeRangeForLocation(newLocation),
                LongitudeRangeForLocation(newLocation));

            // Load the HTML in the WebView and set the labels
            this.webView.MainFrame.LoadHTMLStringBaseURL(htmlString, null);
            this.locationLabel.StringValue = NSString.StringWithFormat("%f, %f",
                                                                       newLocation.Coordinate.latitude, newLocation.Coordinate.longitude);
            this.accuracyLabel.StringValue = NSString.StringWithFormat("%f",
                                                                       newLocation.HorizontalAccuracy);
        }

        [ObjectiveCMessage("locationManager:didFailWithError:")]
        public void OpenInDefaultBrowser(CLLocationManager manager, NSError error)
        {
            this.webView.MainFrame.LoadHTMLStringBaseURL(NSString.StringWithFormat("Location manager failed with error: %@", error.LocalizedDescription), null);
            this.locationLabel.StringValue = NSString.String;
            this.accuracyLabel.StringValue = NSString.String;
        }

        [ObjectiveCMessage("applicationWillTerminate:")]
        public void applicationWillTerminate(NSNotification aNotification)
        {
            this.locationManager.StopUpdatingLocation();
            this.locationManager.Release();
        }
    }
}
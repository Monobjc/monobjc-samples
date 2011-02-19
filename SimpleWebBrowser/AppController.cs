using System;
using Monobjc.Foundation;
using Monobjc.WebKit;

namespace Monobjc.Samples.SimpleWebBrowser
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        public AppController() {}

        public AppController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this.webView.SetFrameLoadDelegate(d =>
                                                  {
                                                      d.WebViewDidStartProvisionalLoadForFrame += this.WebViewDidStartProvisionalLoadForFrame;
                                                      d.WebViewDidFinishLoadForFrame += this.WebViewDidFinishLoadForFrame;
                                                      d.WebViewDidReceiveTitleForFrame += this.WebViewDidReceiveTitleForFrame;
                                                  });
        }

        partial void Fetch(Id sender)
        {
            this.webView.MainFrame.LoadRequest(NSURLRequest.RequestWithURL(NSURL.URLWithString(this.urlText.StringValue)));
        }

        private void WebViewDidStartProvisionalLoadForFrame(WebView sender, WebFrame frame)
        {
            this.indicator.IsHidden = false;
            this.indicator.StartAnimation(null);
        }

        private void WebViewDidFinishLoadForFrame(WebView sender, WebFrame frame)
        {
            this.indicator.StopAnimation(null);
            this.indicator.IsHidden = true;
        }

        private void WebViewDidReceiveTitleForFrame(WebView sender, NSString title, WebFrame frame)
        {
            this.window.Title = title;
        }
    }
}
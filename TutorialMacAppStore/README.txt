Tutorial for MacAppStore

Before running this sample application, you need to change the bundle identifier and version to match the one you want to test.

Mac Dev Portal
==============
- Be sure the bundle identifier you want to use is declared in the App Ids section.

iTunes Connect Portal
=====================
- Be sure that an application with the same bundle identifier and version is defined.
- Be sure to have a test user defined.

MonoDevelop
===========
- ReceiptChecker.cs: Replace the bundle identifier and version values with the one you want.
- Info.plist: Replace the bundle identifier and version values with the one you want.
- Project Properties/Monobjc Deployment: select a valid signing identity.

Testing
=======
- Once the application is ready to be tested, build it under MonoDevelop
- Launch it FROM THE FINDER, to trigger the receipt generation.
- Once the receipt has been generated, you can debug the application.

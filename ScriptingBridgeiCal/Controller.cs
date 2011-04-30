using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.ScriptingBridge;
using Monobjc.ScriptingBridge.iCal;

namespace ScriptingBridgeiCal
{
	[ObjectiveCClass]
	public partial class Controller : NSObject
	{
		public static readonly Class ControllerClass = Class.Get (typeof(Controller));

		public Controller ()
		{
		}

		public Controller (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			this.time.DateValue = NSDate.Date;
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}

		partial void AddUpdateEvent (Id sender)
		{
			// Get the scripting bridge object for the target application.
			iCalApplication iCal = SBApplication.ApplicationWithBundleIdentifier ("com.apple.iCal").CastTo<iCalApplication>();
			
			// Bring the iCal application into view
			iCal.Activate ();
			
			// Reference to our calendar object
			iCalCalendar theCalendar = null;
			
			// Get the name of the calendar from the window.
			NSString calendarName = calendar.StringValue;
			
			// Get the calendar with the specified name using the -filteredArrayUsingPredicate: method.
			NSArray matchingCalendars = iCal.Calendars.FilteredArrayUsingPredicate (NSPredicate.PredicateWithFormat ("name == %@", calendarName));
			if (matchingCalendars.Count > 0) {
				theCalendar = matchingCalendars.ObjectAtIndex (0).CastTo<iCalCalendar> ();
			}
			
			// If no such calendar exists, then create a new one with that name. 
			if (theCalendar == null) {
				// Set up the properties for the new calendar 
				NSDictionary props = NSDictionary.DictionaryWithObjectForKey (calendarName, (NSString)"name");
				
				// Allocate and initialize the new calendar 
				Class calendarClass = iCal.ClassForScriptingClass ("calendar");
				// TODO: Change when constructor is available
				theCalendar = calendarClass.SendMessage<Id>("alloc").SendMessage<iCalCalendar>("initWithProperties:",props);
				
				// ...and add it to the list of calendars in the iCal application.
				iCal.Calendars.AddObject (theCalendar);
			}
			
			// get the event with the specified name from the calendar.  If no such event exists, then create a new one with that name.
			
			// reference to our event object
			iCalEvent theEvent;
			
			// get the name of the event from the window.
			NSString eventName = this.@event.StringValue;
			
			// calculate start and end times for a one hour event starting at the time displayed in the window.
			NSDate startDate = this.time.DateValue;
			
			// set the end date to the start time plus one hour (3600 seconds).
			NSDate endDate = new NSDate (3600, startDate);
			
			// the event summary contains the name displayed in the iCal calendar windows,
			// so we can't use [[[theCalendar events] objectWithName:eventName] exists]
			// to find out if the event exists. 
			//
			//So, instead of that we're going to use filteredArrayUsingPredicate to retrieve
			// an array of all of the events with a matching summary.	
			NSArray matchingEvents = theCalendar.Events.FilteredArrayUsingPredicate (NSPredicate.PredicateWithFormat ("summary == %@", eventName));
			
			// if we found at least one matching event, then we'll update the times for the first event with a matching summary
			if (matchingEvents.Count >= 1) {
				// get the first matching event out of the response
				theEvent = matchingEvents.ObjectAtIndex (0).CastTo<iCalEvent> ();
				
				// update the dates for the event
				theEvent.StartDate = startDate;
				theEvent.EndDate = endDate;
			} else {
				// otherwise, create a new event
				
				// set up the event properties.  Note, we're using kvc names from the iCal.h file to name the properties encoded in the NSDictonary.
				NSDictionary props = NSDictionary.DictionaryWithObjectsAndKeys (eventName, (NSString)"summary", startDate, (NSString)"startDate", endDate, (NSString)"endDate", null);
				
				// create the new event
				Class eventClass = iCal.ClassForScriptingClass ("event");
				// TODO: Change when constructor is available
				theEvent = eventClass.SendMessage<Id>("alloc").SendMessage<iCalEvent>("initWithProperties:",props);
				
				// add it to the list of events for this calendar.
				theCalendar.Events.AddObject (theEvent);
			}
		}
	}
}


// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Monobjc.ScriptingBridge.iCal {
    using System;
    using Monobjc;
    using Monobjc.Foundation;
    using Monobjc.AppKit;
    using Monobjc.ScriptingBridge;
    
    
    public enum iCalSaveOptions : uint {
        
        // 'yes ' => '0x79657320'
        iCalSaveOptionsYes = 2036691744u,
        
        // 'no  ' => '0x6E6F2020'
        iCalSaveOptionsNo = 1852776480u,
        
        // 'ask ' => '0x61736B20'
        iCalSaveOptionsAsk = 1634954016u,
    }
    
    public enum iCalPrintingErrorHandling : uint {
        
        // 'lwst' => '0x6C777374'
        iCalPrintingErrorHandlingStandard = 1819767668u,
        
        // 'lwdt' => '0x6C776474'
        iCalPrintingErrorHandlingDetailed = 1819763828u,
    }
    
    public enum iCalCALParticipationStatus : uint {
        
        // 'E6na' => '0x45366E61'
        iCalCALParticipationStatusUnknown = 1161195105u,
        
        // 'E6ap' => '0x45366170'
        iCalCALParticipationStatusAccepted = 1161191792u,
        
        // 'E6dp' => '0x45366470'
        iCalCALParticipationStatusDeclined = 1161192560u,
        
        // 'E6tp' => '0x45367470'
        iCalCALParticipationStatusTentative = 1161196656u,
    }
    
    public enum iCalCALStatusType : uint {
        
        // 'E4ca' => '0x45346361'
        iCalCALStatusTypeCancelled = 1161061217u,
        
        // 'E4cn' => '0x4534636E'
        iCalCALStatusTypeConfirmed = 1161061230u,
        
        // 'E4no' => '0x45346E6F'
        iCalCALStatusTypeNone = 1161064047u,
        
        // 'E4te' => '0x45347465'
        iCalCALStatusTypeTentative = 1161065573u,
    }
    
    public enum iCalCALPriorities : uint {
        
        // 'tdp0' => '0x74647030'
        iCalCALPrioritiesNoPriority = 1952739376u,
        
        // 'tdp9' => '0x74647039'
        iCalCALPrioritiesLowPriority = 1952739385u,
        
        // 'tdp5' => '0x74647035'
        iCalCALPrioritiesMediumPriority = 1952739381u,
        
        // 'tdp1' => '0x74647031'
        iCalCALPrioritiesHighPriority = 1952739377u,
    }
    
    public enum iCalCALViewTypeForScripting : uint {
        
        // 'E5da' => '0x45356461'
        iCalCALViewTypeForScriptingDayView = 1161127009u,
        
        // 'E5we' => '0x45357765'
        iCalCALViewTypeForScriptingWeekView = 1161131877u,
        
        // 'E5mo' => '0x45356D6F'
        iCalCALViewTypeForScriptingMonthView = 1161129327u,
    }
    
    public class iCalApplication : SBApplication {
        
        public iCalApplication() {
        }
        
        public iCalApplication(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual SBElementArray Calendars {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "calendars");
            }
        }
        
        public virtual SBElementArray Documents {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "documents");
            }
        }
        
        public virtual SBElementArray Windows {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "windows");
            }
        }
        
        public virtual Boolean Frontmost {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "frontmost");
            }
        }
        
        public virtual NSString Name {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "name");
            }
        }
        
        public virtual void CreateCalendarWithName(NSString withName) {
            ObjectiveCRuntime.SendMessage(this, "createCalendarWithName:", withName);
        }
        
        public virtual Boolean Exists(Id x) {
            return ObjectiveCRuntime.SendMessage <Boolean>(this, "exists:", x);
        }
        
        public virtual void GetURL(NSString x) {
            ObjectiveCRuntime.SendMessage(this, "GetURL:", x);
        }
        
        public virtual iCalDocument Open(Id x) {
            return ObjectiveCRuntime.SendMessage <iCalDocument>(this, "open:", x);
        }
        
        public virtual void PrintWithPropertiesPrintDialog(NSArray x, Id withProperties, Boolean printDialog) {
            ObjectiveCRuntime.SendMessage(this, "print:withProperties:printDialog:", x, withProperties, printDialog);
        }
        
        public virtual void QuitSaving(iCalSaveOptions saving) {
            ObjectiveCRuntime.SendMessage(this, "quitSaving:", saving);
        }
        
        public virtual void ReloadCalendars() {
            ObjectiveCRuntime.SendMessage(this, "reloadCalendars");
        }
        
        public virtual void SwitchViewTo(iCalCALViewTypeForScripting to) {
            ObjectiveCRuntime.SendMessage(this, "switchViewTo:", to);
        }
        
        public virtual void ViewCalendarAt(NSDate at) {
            ObjectiveCRuntime.SendMessage(this, "viewCalendarAt:", at);
        }
    }
    
    public class iCalDocument : SBObject {
        
        public iCalDocument() {
        }
        
        public iCalDocument(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual Id File {
            get {
                return ObjectiveCRuntime.SendMessage <Id>(this, "file");
            }
        }
        
        public virtual Boolean Modified {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "modified");
            }
        }
        
        public virtual NSString Name {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "name");
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalWindow : SBObject {
        
        public iCalWindow() {
        }
        
        public iCalWindow(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual NSRect Bounds {
            get {
                return ObjectiveCRuntime.SendMessage <NSRect>(this, "bounds");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setBounds:", value);
            }
        }
        
        public virtual Boolean Closeable {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "closeable");
            }
        }
        
        public virtual iCalDocument Document {
            get {
                return ObjectiveCRuntime.SendMessage <iCalDocument>(this, "document");
            }
        }
        
        public virtual NSInteger Id {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "id");
            }
        }
        
        public virtual NSInteger Index {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "index");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setIndex:", value);
            }
        }
        
        public virtual Boolean Miniaturizable {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "miniaturizable");
            }
        }
        
        public virtual Boolean Miniaturized {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "miniaturized");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setMiniaturized:", value);
            }
        }
        
        public virtual NSString Name {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "name");
            }
        }
        
        public virtual Boolean Resizable {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "resizable");
            }
        }
        
        public virtual Boolean Visible {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "visible");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setVisible:", value);
            }
        }
        
        public virtual Boolean Zoomable {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "zoomable");
            }
        }
        
        public virtual Boolean Zoomed {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "zoomed");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setZoomed:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalCalendar : SBObject {
        
        public iCalCalendar() {
        }
        
        public iCalCalendar(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual SBElementArray Events {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "events");
            }
        }
        
        public virtual SBElementArray Todos {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "todos");
            }
        }
        
        public virtual NSColor Color {
            get {
                return ObjectiveCRuntime.SendMessage <NSColor>(this, "color");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setColor:", value);
            }
        }
        
        public virtual NSString Name {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "name");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setName:", value);
            }
        }
        
        public virtual NSString Uid {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "uid");
            }
        }
        
        public virtual Boolean Writable {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "writable");
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalDisplayAlarm : SBObject {
        
        public iCalDisplayAlarm() {
        }
        
        public iCalDisplayAlarm(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual NSDate TriggerDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "triggerDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerDate:", value);
            }
        }
        
        public virtual NSInteger TriggerInterval {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "triggerInterval");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerInterval:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalMailAlarm : SBObject {
        
        public iCalMailAlarm() {
        }
        
        public iCalMailAlarm(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual NSDate TriggerDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "triggerDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerDate:", value);
            }
        }
        
        public virtual NSInteger TriggerInterval {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "triggerInterval");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerInterval:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalSoundAlarm : SBObject {
        
        public iCalSoundAlarm() {
        }
        
        public iCalSoundAlarm(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual NSString SoundFile {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "soundFile");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setSoundFile:", value);
            }
        }
        
        public virtual NSString SoundName {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "soundName");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setSoundName:", value);
            }
        }
        
        public virtual NSDate TriggerDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "triggerDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerDate:", value);
            }
        }
        
        public virtual NSInteger TriggerInterval {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "triggerInterval");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerInterval:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalOpenFileAlarm : SBObject {
        
        public iCalOpenFileAlarm() {
        }
        
        public iCalOpenFileAlarm(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual NSString Filepath {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "filepath");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setFilepath:", value);
            }
        }
        
        public virtual NSDate TriggerDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "triggerDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerDate:", value);
            }
        }
        
        public virtual NSInteger TriggerInterval {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "triggerInterval");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setTriggerInterval:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalAttendee : SBObject {
        
        public iCalAttendee() {
        }
        
        public iCalAttendee(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual NSString DisplayName {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "displayName");
            }
        }
        
        public virtual NSString Email {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "email");
            }
        }
        
        public virtual iCalCALParticipationStatus ParticipationStatus {
            get {
                return ObjectiveCRuntime.SendMessage <iCalCALParticipationStatus>(this, "participationStatus");
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalTodo : SBObject {
        
        public iCalTodo() {
        }
        
        public iCalTodo(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual SBElementArray DisplayAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "displayAlarms");
            }
        }
        
        public virtual SBElementArray MailAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "mailAlarms");
            }
        }
        
        public virtual SBElementArray OpenFileAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "openFileAlarms");
            }
        }
        
        public virtual SBElementArray SoundAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "soundAlarms");
            }
        }
        
        public virtual NSDate CompletionDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "completionDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setCompletionDate:", value);
            }
        }
        
        public virtual NSDate DueDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "dueDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setDueDate:", value);
            }
        }
        
        public virtual iCalCALPriorities Priority {
            get {
                return ObjectiveCRuntime.SendMessage <iCalCALPriorities>(this, "priority");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setPriority:", value);
            }
        }
        
        public virtual NSInteger Sequence {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "sequence");
            }
        }
        
        public virtual NSDate StampDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "stampDate");
            }
        }
        
        public virtual NSString Summary {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "summary");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setSummary:", value);
            }
        }
        
        public virtual NSString Uid {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "uid");
            }
        }
        
        public virtual NSString Url {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "url");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setUrl:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
    
    public class iCalEvent : SBObject {
        
        public iCalEvent() {
        }
        
        public iCalEvent(System.IntPtr pointer) : 
                base(pointer) {
        }
        
        public virtual SBElementArray Attendees {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "attendees");
            }
        }
        
        public virtual SBElementArray DisplayAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "displayAlarms");
            }
        }
        
        public virtual SBElementArray MailAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "mailAlarms");
            }
        }
        
        public virtual SBElementArray OpenFileAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "openFileAlarms");
            }
        }
        
        public virtual SBElementArray SoundAlarms {
            get {
                return ObjectiveCRuntime.SendMessage <SBElementArray>(this, "soundAlarms");
            }
        }
        
        public virtual Boolean AlldayEvent {
            get {
                return ObjectiveCRuntime.SendMessage <Boolean>(this, "alldayEvent");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setAlldayEvent:", value);
            }
        }
        
        public virtual NSDate EndDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "endDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setEndDate:", value);
            }
        }
        
        public virtual Id ExcludedDates {
            get {
                return ObjectiveCRuntime.SendMessage <Id>(this, "excludedDates");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setExcludedDates:", value);
            }
        }
        
        public virtual NSString Location {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "location");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setLocation:", value);
            }
        }
        
        public virtual NSString Recurrence {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "recurrence");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setRecurrence:", value);
            }
        }
        
        public virtual NSInteger Sequence {
            get {
                return ObjectiveCRuntime.SendMessage <NSInteger>(this, "sequence");
            }
        }
        
        public virtual NSDate StampDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "stampDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setStampDate:", value);
            }
        }
        
        public virtual NSDate StartDate {
            get {
                return ObjectiveCRuntime.SendMessage <NSDate>(this, "startDate");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setStartDate:", value);
            }
        }
        
        public virtual iCalCALStatusType Status {
            get {
                return ObjectiveCRuntime.SendMessage <iCalCALStatusType>(this, "status");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setStatus:", value);
            }
        }
        
        public virtual NSString Summary {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "summary");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setSummary:", value);
            }
        }
        
        public virtual NSString Uid {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "uid");
            }
        }
        
        public virtual NSString Url {
            get {
                return ObjectiveCRuntime.SendMessage <NSString>(this, "url");
            }
            set {
                ObjectiveCRuntime.SendMessage(this, "setUrl:", value);
            }
        }
        
        public virtual void CloseSavingSavingIn(iCalSaveOptions saving, Id savingIn) {
            ObjectiveCRuntime.SendMessage(this, "closeSaving:savingIn:", saving, savingIn);
        }
        
        public virtual void Delete() {
            ObjectiveCRuntime.SendMessage(this, "delete");
        }
        
        public virtual void DuplicateToWithProperties(SBObject to, NSDictionary withProperties) {
            ObjectiveCRuntime.SendMessage(this, "duplicateTo:withProperties:", to, withProperties);
        }
        
        public virtual void MoveTo(SBObject to) {
            ObjectiveCRuntime.SendMessage(this, "moveTo:", to);
        }
        
        public virtual void Show() {
            ObjectiveCRuntime.SendMessage(this, "show");
        }
    }
}
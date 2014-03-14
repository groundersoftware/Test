using System;
using Android.App;
using Android.Runtime;
using JobDispatch.Client.Core.Utils;

namespace MapTest
{
    [Application]
    public class App : Application
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            _log.Info("Application is starting");

            base.OnCreate();

            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser += UnhandledExceptionHandler;
        }

        protected override void Dispose(bool disposing)
        {
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser -= UnhandledExceptionHandler;

            base.Dispose(disposing);

            _log.Info("Application is disposed.");
        }

        /// <summary>
        /// When app-wide unhandled exceptions are hit, this will handle them. Be aware however, that typically
        /// android will be destroying the process, so there's not a lot you can do on the android side of things,
        /// but your xamarin code should still be able to work. so if you have a custom err logging manager or 
        /// something, you can call that here. You _won't_ be able to call Android.Util.Log, because Dalvik
        /// will destroy the java side of the process.
        /// </summary>
        protected void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;

            // log won't be available, because dalvik is destroying the process
            // Log.Debug (logTag, "MyHandler caught : " + e.Message);
            // instead, your err handling code should be run:

            _log.CrashDump("UNHANDLED 1", e);
        }

        protected void UnhandledExceptionHandler(object sender, RaiseThrowableEventArgs args)
        {
            // log won't be available, because dalvik is destroying the process
            // Log.Debug (logTag, "MyHandler caught : " + e.Message);
            // instead, your err handling code should be run:

            _log.CrashDump("UNHANDLED 2", args.Exception);
        }
    }
}

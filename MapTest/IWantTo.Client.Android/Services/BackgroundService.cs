using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using IWantTo.Client.Core.Utils;
using Java.Lang;

namespace IWantTo.Client.Android.Services
{
    [Service]
    [IntentFilter(new[] { "com.groundersoftware.backgroundservice" })]
    public class BackgroundService : Service
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Binder for this service. If a new activity need service it must at first binds service through this binder.</summary>
        private WantToServiceBinder _binder;

        /// <summary>Location service.</summary>
        private LocationService _locationService;

        public override void OnCreate()
        {
            _log.InfoFormat("Background Service Creating.");

        }

        /// <summary>
        /// Method is called only for started service (not only bounded).
        /// </summary>
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _log.InfoFormat("Background Service Starting.");

            // creates location service
            _locationService = new LocationService(this);

            // start main background thread
            Task.Factory.StartNew(BackgroundServiceThread, TaskCreationOptions.LongRunning);

            return StartCommandResult.NotSticky;
        }

        /// <summary>
        /// Main Background Thread
        /// </summary>
        private void BackgroundServiceThread()
        {
            _log.Info("The background service thread has been started.");

            while (true)
            {
                Thread.Sleep(5000);
            }

            _log.Info("The background service thread finished.");
        }

        public override IBinder OnBind(Intent intent)
        {
            _binder = new WantToServiceBinder(this);
            return _binder;
        }

        /// <summary>
        /// Method is called before end of service. For started and bounded services.
        /// </summary>
        public override void OnDestroy()
        {
            _log.Info("Background Service Destroying.");
            _locationService.EnablePositionTracking(false);

            base.OnDestroy();
        }

        #region Binder

        /// <summary>
        /// Binder class for Bound JobDispatch service.
        /// </summary>
        public class WantToServiceBinder : Binder
        {
            /// <summary>Service instance.</summary>
            private readonly BackgroundService _service;

            /// <summary>
            /// Constructs binder.
            /// </summary>
            /// <param name="service"></param>
            public WantToServiceBinder(BackgroundService service)
            {
                _service = service;
            }

            /// <summary>
            /// Gets Job Dispatch application service.
            /// </summary>
            /// <returns></returns>
            public BackgroundService GetService()
            {
                return _service;
            }
        }

        #endregion
    }
}
using System;
using Android.Content;
using Android.Locations;
using Android.OS;
using IWantTo.Client.Core.Services;
using IWantTo.Client.Core.Utils;

namespace IWantTo.Client.Android.Services
{
    public class LocationService : Java.Lang.Object, ILocationListener
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Unix Start Date.</summary>
        private static readonly DateTime _unixStartDate = new DateTime(1970, 1, 1);

        /// <summary>Ignoring new position if distance between new and old is less than this value. Default is 20 meters.</summary>
        private const float LOCATION_MIN_DISTANCE = 20;

        /// <summary>Min time parameter (in ms) in RequestLocationUpdates.</summary>
        private const long LOCATION_MIN_TIME = 10000;

        /// <summary>Android location manager.</summary>
        private readonly LocationManager _locationManager;

        /// <summary>Main looper of UI thread.</summary>
        private readonly Looper _mainLooper;

        /// <summary>Last valid location for Location manager with timestamp.</summary>
        private static Location _lastValidLocation;

        /// <summary>
        /// Construct Android Location Service.
        /// </summary>
        /// <param name="context">Application context.</param>
        public LocationService(Context context)
        {
            _log.Info("Location Service creating ...");

            // set main UI looper
            _mainLooper = context.MainLooper;

            // get location manager
            _locationManager = (LocationManager) context.GetSystemService(Context.LocationService);
            if (_locationManager == null)
            {
                _log.Error("Location manager missing from unknown reason!");
            }

            _log.Info("Location Service successfully created.");
        }

        /// <inheritdoc />
        public bool PositionTracking(bool enable)
        {
            // sanity check if location manager exist
            if (_locationManager == null)
            {
                return false;
            }

            if (enable)
            {
                _log.InfoFormat("Enabling position tracking");

                try
                {
                    if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                    {
                        // Updates Location every LOCATION_MIN_TIME ms and only when the location changed more than LOCATION_MIN_DISTANCE meters for GPS provider
                        _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, LOCATION_MIN_TIME, LOCATION_MIN_DISTANCE, this, _mainLooper);
                        _log.InfoFormat("Location provider: '{0}' enabled.", LocationManager.GpsProvider);
                    }

                    //if (_locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
                    //{
                    //    // Updates Location every 10 seconds and only when the location changed more than LOCATION_MIN_DISTANCE meters for Network provider
                    //    _locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, LOCATION_MIN_TIME, LOCATION_MIN_DISTANCE, this, _mainLooper);
                    //    _log.InfoFormat("Location provider: '{0}' enabled.", LocationManager.NetworkProvider);
                    //}
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("Exception during starting location service: {0}", ex.ToString());
                }
            }
            else
            {
                try
                {
                    _log.InfoFormat("Disabling position tracking");
                    _locationManager.RemoveUpdates(this);
                    _log.Info("Location service stopped.");
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("Exception during stopping location service: {0}", ex.ToString());
                }
            }

            return true;
        }

        /// <inheritdoc />
        public void DisposeLocationService()
        {
            try
            {
                _log.InfoFormat("Disabling position tracking...");

                // removed updates if exist
                if (_locationManager != null)
                {
                    _locationManager.RemoveUpdates(this);
                }

                _log.Info("Location service stopped.");
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Exception during stopping location service: {0}", ex.ToString());
            }
        }

        /// <inheritdoc />
        public void OnLocationChanged(Location location)
        {
            var userId = ConfigurationService.Instance.LastLoginId;

            _log.InfoFormat("New Location received: Provider:'{0}', Latitude:{1}, Longitude:{2}, Accuracy:{3}, Speed:{4}, Bearing:{5}, Provider:{6}, Date:{7} for User:{8}.", location.Provider, location.Latitude,
                            location.Longitude, location.Accuracy, location.Speed, location.Bearing, location.Provider, _unixStartDate.AddMilliseconds(location.Time).ToString("yyyy-MM-dd HH:mm:ss.fff"), userId);

            // process new location if is better than previous one
            if (IsBetterLocation(location, _lastValidLocation))
            {
                // Removing locations if distance is not more than LOCATION_MIN_DISTANCE - reducing not interesting lcoations and reducing "star" on map
                if (_lastValidLocation != null)
                {
                    var distance = location.DistanceTo(_lastValidLocation);
                    if (distance < LOCATION_MIN_DISTANCE / 2)           // 2 is only for sure that we are eliminating stars and not valid positions
                    {
                        _log.InfoFormat("Star detection: location is rejected. Star detection distance is {0} meters.", distance / 2);
                        return;
                    }
                }

                // set a new valid location 
                _lastValidLocation = location;
                _log.Info("New location is more precise than previous one. Prepared for sending to server.");

                // process position here

            }
            else
            {
                _log.InfoFormat("New location is not exactly better than actual one, previous will be used: Provider:'{0}', Latitude:{1}, Longitude:{2}, Accuracy:{3}, Speed:{4}, Bearing:{5}, Date:{6}", _lastValidLocation.Provider,
                                _lastValidLocation.Latitude, _lastValidLocation.Longitude, _lastValidLocation.Accuracy, _lastValidLocation.Speed, _lastValidLocation.Bearing,
                                _unixStartDate.AddMilliseconds(_lastValidLocation.Time).ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
        }

        /// <summary>
        /// Tests if new location (first argument) is better than previous one (second argument).
        /// Algoritmus based to http://developer.android.com/guide/topics/location/strategies.html
        /// with our changes.
        /// </summary>
        /// <param name="newLocation">New location.</param>
        /// <param name="currentBestLocation">Current best location.</param>
        /// <returns>True if new location is better than previous one, othewise false.</returns>
        private bool IsBetterLocation(Location newLocation, Location currentBestLocation)
        {
            // this is temporary solution just to test positions on the server side
            return true;

            // A new location is always better than no location
            if (currentBestLocation == null)
            {
                _log.Info("Current Location is null, new location is better.");
                return true;
            }

            // Check whether the new location is significantly newer or significantly older:
            //
            //              SignificantlyOlder <----                                    ----> SignificantlyNewer
            //                               ------+------------------+-----------------+------            
            //                         Current - (MIN_TIME*2)      Current       Current + (MIN_TIME*2)
            //
            var timeDelta = newLocation.Time - currentBestLocation.Time;
            var isSignificantlyNewer = timeDelta > LOCATION_MIN_TIME * 2;
            var isSignificantlyOlder = timeDelta < -LOCATION_MIN_TIME * 2;

            // If it's been more than one minute since the current location, use the new location
            // because the user has likely moved
            if (isSignificantlyNewer)
            {
                _log.InfoFormat("New location is significantly newer, new location is better.");
                return true;
            }
            if (isSignificantlyOlder)
            {
                _log.InfoFormat("New location is significantly older, new location is not better.");
                return false;
            }

            // Check whether the new location fix is more or less accurate
            var accuracyDelta = (int)(newLocation.Accuracy - currentBestLocation.Accuracy);
            var isSignificantlyMoreAccurate = accuracyDelta < -50;

            if (isSignificantlyMoreAccurate)
            {
                _log.InfoFormat("New location is significantly more accurate, new location is better.");
                return true;
            }

            // if we have location with better accurate in defined time interval it means that new location is better
            return false;
        }

        /// <inheritdoc />
        public void OnProviderDisabled(string provider)
        {
            _log.InfoFormat("Location provider {0} disabled.", provider);
        }

        /// <inheritdoc />
        public void OnProviderEnabled(string provider)
        {
            _log.InfoFormat("Location provider {0} enabled.", provider);
        }

        /// <inheritdoc />
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            _log.InfoFormat("Location provider {0} status changed to {1}.", provider, status.ToString());
        }
    }
}
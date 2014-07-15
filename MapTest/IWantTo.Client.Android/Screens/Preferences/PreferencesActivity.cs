using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.App;
using Android.Widget;
using IWantTo.Client.Core.Services;
using IWantTo.Client.Core.Utils;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;

namespace IWantTo.Client.Android.Screens.Preferences
{
    [Activity(Name = "iwantto.activity.preferencesactivity")]
    public class PreferencesActivity : SherlockPreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        /// <summary>Logger.</summary>
        private static readonly Logger _log = Logger.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // set activity title
            Title = Resources.GetString(Resource.String.PreferencesLayout_Title);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                AddPreferencesFromResource(Resource.Xml.preferences11after);
            }
            else
            {
                AddPreferencesFromResource(Resource.Xml.preferences11pre);
            }

            // set value from configuration file for language preference
            var language = ConfigurationService.Instance.Language;
            var languagePref = FindPreference("languages_list_preference");
            var languageValues = new List<string>(Resources.GetStringArray(Resource.Array.PreferencesLayout_Languages_List_Values));
            var languageNames = Resources.GetStringArray(Resource.Array.PreferencesLayout_Languages_List);

            if (!languageValues.Contains(language))
            {
                _log.WarnFormat("Not valid language '{0}' in configuration. Default value used instead.", language);
                language = "en";
            }

            // selects right radio button according to value from configuration
            ((ListPreference)languagePref).Value = language;
            languagePref.Summary = languageNames[languageValues.IndexOf(language)];

            var notificationVib = ConfigurationService.Instance.NotificationVibrating;
            var notificationVibPref = FindPreference("notification_vibration_preference");
            notificationVibPref.Summary = (notificationVib) ? Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Vibrate_Active) : Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Vibrate_Inactive);

            var notificationSound = ConfigurationService.Instance.NotificationSound;
            var notificationSoundPref = FindPreference("notification_sound_preference");
            notificationSoundPref.Summary = (notificationSound) ? Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Sound_Active) : Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Sound_Inactive);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                ((SwitchPreference)notificationVibPref).Checked = notificationVib;
                ((SwitchPreference)notificationSoundPref).Checked = notificationSound;
            }
            else
            {
                ((CheckBoxPreference)notificationVibPref).Checked = notificationVib;
                ((CheckBoxPreference)notificationSoundPref).Checked = notificationSound;
            }
        }

        /// <summary>
        /// Registers Preference Change listener
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(this).RegisterOnSharedPreferenceChangeListener(this);
        }

        /// <summary>
        /// UnRegisters Preference Change listener
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            PreferenceManager.GetDefaultSharedPreferences(this).UnregisterOnSharedPreferenceChangeListener(this);
        }

        /// <summary>
        /// NEcessary for correct UP button handling in Preferences Activity.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);
                    break;
            }

            return true;
        }


        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            switch (key)
            {
                case ("languages_list_preference"):
                {
                    // language changed

                    var languagePref = FindPreference("languages_list_preference");
                    var newLanguage = sharedPreferences.GetString(key, "en");

                    var languageValues = new List<string>(Resources.GetStringArray(Resource.Array.PreferencesLayout_Languages_List_Values));
                    var languageNames = Resources.GetStringArray(Resource.Array.PreferencesLayout_Languages_List);

                    if (languageValues.Contains(newLanguage))
                    {
                        ConfigurationService.Instance.Language = newLanguage;
                        languagePref.Summary = languageNames[languageValues.IndexOf(newLanguage)];
                    }
                    else
                    {
                        _log.ErrorFormat("Not valid language '{0}' in preferences dialog. Default value used instead.", newLanguage);
                        languagePref.Summary = languageNames[languageValues.IndexOf("en")];
                    }
                    _log.InfoFormat("Changed language to '{0}'", newLanguage);

                    // set a new language 
                    var languageIso = ConfigurationService.Instance.Language;
                    var locale = new Java.Util.Locale(languageIso);

                    Resources.Configuration.Locale = locale;
                    BaseContext.Resources.UpdateConfiguration(Resources.Configuration, BaseContext.Resources.DisplayMetrics);
                    RunOnUiThread(() => Toast.MakeText(this, Resources.GetString(Resource.String.PreferencesLayout_PreferenceChange_Information), ToastLength.Short).Show());

                    break;
                }
                case ("notification_vibration_preference"):
                {
                    // find a particular preference a retrieve value from check box preference or switch preference
                    var preference = FindPreference(key);
                    var enabled = (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb) ? ((SwitchPreference)preference).Checked : ((CheckBoxPreference)preference).Checked;

                    ConfigurationService.Instance.NotificationVibrating = enabled;
                    var notificationVibPref = FindPreference("notification_vibration_preference");
                    notificationVibPref.Summary = (enabled)
                            ? Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Vibrate_Active)
                            : Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Vibrate_Inactive);

                    break;
                }
                case ("notification_sound_preference"):
                {
                    // find a particular preference a retrieve value from check box preference or switch preference
                    var preference = FindPreference(key);
                    var enabled = (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb) ? ((SwitchPreference)preference).Checked : ((CheckBoxPreference)preference).Checked;

                    ConfigurationService.Instance.NotificationSound = enabled;
                    var notificationSoundPref = FindPreference("notification_sound_preference");
                    notificationSoundPref.Summary = (enabled)
                            ? Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Sound_Active)
                            : Resources.GetString(Resource.String.PreferencesLayout_NotificationsCategory_Sound_Inactive);

                    break;
                }
                default:
                {
                    _log.WarnFormat("Application error. Unknown preference in settings.");

                    break;
                }
            }
        }
    }
}
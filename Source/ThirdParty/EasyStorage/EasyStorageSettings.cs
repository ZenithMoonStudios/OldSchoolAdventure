using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EasyStorage
{
    /*
	 * This whole file is basically a big no-op on Windows Phone because none of the strings are used
	 * on that platform. However it is included so that games can have a cross-platform game using
	 * these APIs without having to use an #if !WINDOWS_PHONE around it.
	 */

    /// <summary>
    /// The languages supported by EasyStorage.
    /// </summary>
    public enum Language
    {
        German,
        Spanish,
        French,
        Italian,
        Japanese,
        English
    }

    /// <summary>
    /// Used to access settings for EasyStorage.
    /// </summary>
    public static class EasyStorageSettings
    {
        // map the two letter language value to our enumeration
        private static readonly Dictionary<string, Language> languageMap = new Dictionary<string, Language>
        {
            { "de", Language.German },
            { "es", Language.Spanish },
            { "fr", Language.French },
            { "it", Language.Italian },
            { "ja", Language.Japanese },
            { "en", Language.English }
        };

        // map our languages to string culture values for creating new CultureInfo objects. 
        // the only part that really matters to us is the language, so the region portion is
        // simply an acceptable value picked arbitrarily.
        private static readonly Dictionary<Language, string> cultureMap = new Dictionary<Language, string>
        {
            { Language.German, "de-DE" },
            { Language.Spanish, "es-ES" },
            { Language.French, "fr-FR" },
            { Language.Italian, "it-IT" },
            { Language.Japanese, "ja-JP" },
            { Language.English, "en-US" },
        };

        /// <summary>
        /// Restricts the EasyStorage system to the specified languages. If the system is currently
        /// set to a language not listed here, EasyStorage will use the first language given. This
        /// method does reset the SaveDevice strings, so it's best to call this before setting
        /// your strings explicitly.
        /// </summary>
        /// <param name="supportedLanguages">The set of supported languages.</param>
        public static void SetSupportedLanguages(params Language[] supportedLanguages)
        {
#if !WINDOWS_PHONE
            // make sure we didn't get null
            if (supportedLanguages == null)
                throw new ArgumentNullException("supportedLanguages");

            // make sure we didn't get an empty collection
            if (supportedLanguages.Length == 0)
                throw new ArgumentException("supportedLanguages");

            // make sure all languages specified are actually valid
            foreach (Language l in supportedLanguages)
            {
                if (l < Language.German || l > Language.English)
                {
                    throw new ArgumentException("supportedLanguages");
                }
            }

            // is the current language unsupported
            bool supportedLanguage = false;

            // try to find the current language
            Language currentLanguage;
            if (!languageMap.TryGetValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower(), out currentLanguage))
            {
                // if there is no language match, the box is running an unsupported language
                supportedLanguage = false;
            }
            else
            {
                // otherwise figure out whether our language is currently supported
                supportedLanguage = supportedLanguages.Contains(currentLanguage);
            }

            // if we're running a non-supported language, default to the first given language
            if (!supportedLanguage)
            {
                Strings.Culture = new CultureInfo(cultureMap[supportedLanguages[0]]);

                // since the Strings.Culture changed, we need to reset the strings to make sure
                // they are compliant with the desired supported languages
                ResetSaveDeviceStrings();
            }
#endif
        }

        /// <summary>
        /// Resets the SaveDevice strings to their default values.
        /// </summary>
        public static void ResetSaveDeviceStrings()
        {
#if !WINDOWS_PHONE
            SaveDevice.OkOption = Strings.Ok;
            SaveDevice.YesOption = Strings.Yes_Select_new_device;
            SaveDevice.NoOption = Strings.No_Continue_without_device;
            SaveDevice.DeviceOptionalTitle = Strings.Reselect_Storage_Device;
            SaveDevice.DeviceRequiredTitle = Strings.Storage_Device_Required;
            SaveDevice.ForceDisconnectedReselectionMessage = Strings.forceDisconnectedReselectionMessage;
            SaveDevice.PromptForDisconnectedMessage = Strings.promptForDisconnectedMessage;
            SaveDevice.ForceCancelledReselectionMessage = Strings.forceCanceledReselectionMessage;
            SaveDevice.PromptForCancelledMessage = Strings.promptForCancelledMessage;
#endif
        }
    }
}

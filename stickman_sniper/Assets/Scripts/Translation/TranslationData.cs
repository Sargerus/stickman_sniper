using Sirenix.OdinInspector;
using System;

namespace StickmanSniper.Translation
{

    [Serializable]
    public class TranslationData
    {
        public bool ruEnabled;
        [ShowIf("@ruEnabled==true")]
        public string ruTranslation;

        public bool enEnabled;
        [ShowIf("@enEnabled==true")]
        public string enTranslation;

        public bool trEnabled;
        [ShowIf("@trEnabled==true")]
        public string trTranslation;

        public string GetTranslation(string language)
        {
            if (language == "ru")
                return ruTranslation;
            else if (language == "tr")
                return trTranslation;
            else
                return enTranslation;
        }
    }
}
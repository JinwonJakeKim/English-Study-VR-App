namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    [System.Serializable]
    public class Config 
    {
        public string name;


        public int sampleRate;
        public bool isEnabledProfanityFilter;
        public int maxAlternatives;
        public Enumerators.AudioEncoding audioEncoding;
        public Enumerators.LanguageCode defaultLanguage;

        public Enumerators.SpeechRecognitionType recognitionType;

        public double voiceDetectionThreshold;

        public bool useVolumeMultiplier;
        public float audioVolumeMultiplier;
       
    }
}
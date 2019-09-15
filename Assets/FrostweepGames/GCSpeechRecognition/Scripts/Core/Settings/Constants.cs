namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class Constants
    {
        public const string LONG_RECOGNIZE_REQUEST_URL = "https://speech.googleapis.com/v1/speech:longrunningrecognize";
        public const string RECOGNIZE_REQUEST_URL = "https://speech.googleapis.com/v1/speech:recognize";
        public const string STREAMING_RECOGNIZE_REQUEST_URL = "https://speech.googleapis.com/v1/speech:recognize";
        public const string API_KEY_PARAM = "?key=";


        public const string GC_API_KEY = "AIzaSyDYuVTsEU1VuEm-rYTQCl98fa7y2JUgAIc"; // Google Cloud API Key. Only for test! Use your own API Key in Live!


        public const double AUDIO_DETECT_RATIO = 32768.0;
    }
}
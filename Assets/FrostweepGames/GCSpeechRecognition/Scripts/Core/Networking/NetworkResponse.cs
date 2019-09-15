namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class NetworkResponse
    {
        public long netPacketIndex;

        public string response;
        public string error;

        public NetworkResponse(string resp, string err, long index)
        {
            netPacketIndex = index;
            response = resp;
            error = err;
        }
    }
}
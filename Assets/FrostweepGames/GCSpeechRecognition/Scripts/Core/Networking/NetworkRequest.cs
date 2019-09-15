using UnityEngine.Networking;
using System.Text;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class NetworkRequest
    {
        public long netPacketIndex;

        public UnityWebRequest request;

        public NetworkRequest(string uri, string data, long index)
        {
            netPacketIndex = index;

            request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/text");
        }

        public void Send()
        {
            request.Send();
        }
    }
}
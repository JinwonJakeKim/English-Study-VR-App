using System;
using System.Collections.Generic;
using System.Threading;

//defines
//using grpc = Grpc.Core;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class Networking : IDisposable
    {
        public event Action<NetworkResponse> NetworkResponseEvent;

        private List<NetworkRequest> _networkRequests;
        private List<NetworkResponse> _networkResponses;

        private long _packetIndex = 0;


        public Networking()
        {
            _networkRequests = new List<NetworkRequest>();
            _networkResponses = new List<NetworkResponse>();
        }

        public void Update()
        {
            for(int i = 0; i < _networkRequests.Count; i++)
            {
                if (_networkRequests[i].request.isDone)
                {
                    NetworkResponse response = new NetworkResponse(_networkRequests[i].request.downloadHandler.text,
                                                                   _networkRequests[i].request.error,
                                                                   _networkRequests[i].netPacketIndex);

                    _networkResponses.Add(response);

                    if (NetworkResponseEvent != null)
                        NetworkResponseEvent(response);

                    _networkRequests.RemoveAt(i--);
                }
            }
        }

        public void Dispose()
        {
            _networkRequests.Clear();
            _networkResponses.Clear();
        }

        public long SendRequest(string uri, string data)
        {
            NetworkRequest netRequest = new NetworkRequest(uri, data, _packetIndex);
           
            _packetIndex++;

            _networkRequests.Add(netRequest);

            netRequest.Send();

            return _packetIndex - 1;
        }


        //public void GRPCSendRequest(string uri)
        //{
        //  grpc.Channel channel = new grpc.Channel(uri);
        //}
    }
}
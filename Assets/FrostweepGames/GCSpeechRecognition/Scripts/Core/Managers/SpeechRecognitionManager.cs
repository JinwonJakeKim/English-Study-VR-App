using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class SpeechRecognitionManager : IService, IDisposable, ISpeechRecognitionManager
    {
        public event Action<RecognitionResponse, long> RecognitionSuccessEvent;
        public event Action<string, long> RecognitionFailedEvent;

        private Config _currentConfig;

        private Networking _networking;

        public Config CurrentConfig { get { return _currentConfig; } }

        public void Init()
        {
            _networking = new Networking();
            _networking.NetworkResponseEvent += NetworkResponseEventHandler;
        }

        public void Update()
        {
            _networking.Update();
        }

        public void Dispose()
        {
            _networking.NetworkResponseEvent -= NetworkResponseEventHandler;
            _networking.Dispose();
        }

        public void SetConfig(Config config)
        {
            _currentConfig = config;
        }

        public void Recognize(AudioClip clip, List<string[]> contexts, Enumerators.LanguageCode language)
        {
            if(_currentConfig == null)
                throw new NotImplementedException("Config isn't seted! Use SetConfig method!");

            if (clip == null)
                throw new NotImplementedException("AudioClip isn't seted!");

            string postData = string.Empty; 
            string uri = string.Empty;

            switch (_currentConfig.recognitionType)
            {
                case Enumerators.SpeechRecognitionType.SYNC:
                    {
                        uri = Constants.RECOGNIZE_REQUEST_URL + Constants.API_KEY_PARAM + Constants.GC_API_KEY;

                        postData = JsonUtility.ToJson(GenerateSyncRequest(
                                   AudioConvert.Convert(clip, _currentConfig.audioEncoding, 
                                                              _currentConfig.useVolumeMultiplier, 
                                                              _currentConfig.audioVolumeMultiplier), contexts, language));
                    }
                    break;
                case Enumerators.SpeechRecognitionType.ASYNC:
                    {
                        Debug.Log("Async(Long) speech recognition isn't fully implemented!");

                        uri = Constants.LONG_RECOGNIZE_REQUEST_URL + Constants.API_KEY_PARAM + Constants.GC_API_KEY;

                        postData = JsonUtility.ToJson(GenerateSyncRequest(
                                    AudioConvert.Convert(clip, _currentConfig.audioEncoding,
                                                               _currentConfig.useVolumeMultiplier,
                                                               _currentConfig.audioVolumeMultiplier), contexts, language));
                    }
                    break;
                default:
                    throw new NotSupportedException(_currentConfig.recognitionType + " doesn't supported!");
            }

            _networking.SendRequest(uri, postData);
        }

        private void NetworkResponseEventHandler(NetworkResponse response)
        {
            if (GCSpeechRecognition.Instance.isFullDebugLogIfError)
                Debug.Log(response.error + "\n" + response.response);

            if(!string.IsNullOrEmpty(response.error))
            {
                if(RecognitionFailedEvent != null)
                    RecognitionFailedEvent(response.error, response.netPacketIndex);
            }
            else
            {
                if (response.response.Contains("error"))
                {
                    if (RecognitionFailedEvent != null)
                        RecognitionFailedEvent(response.response, response.netPacketIndex);
                }
                else if (response.response.Contains("results"))
                {
                    if (RecognitionSuccessEvent != null)
                        RecognitionSuccessEvent(JsonUtility.FromJson<RecognitionResponse>(response.response), response.netPacketIndex);
                }
                else if (response.response.Contains("{}"))
                {
                    if (RecognitionFailedEvent != null)
                        RecognitionFailedEvent("Any Words Not Detected!", response.netPacketIndex);
                }
                else
                {
                    if (RecognitionFailedEvent != null)
                        RecognitionFailedEvent("Process Parse Response failed with error: " + response.response, response.netPacketIndex);
                }          
            }
        }

        private RecognitionRequest GenerateSyncRequest(string content, List<string[]> contexts, Enumerators.LanguageCode language)
        {
            RecognitionRequest request = new RecognitionRequest();
            request.config.encoding = _currentConfig.audioEncoding.ToString();
            request.config.languageCode = language.ToString().Replace("_", "-");
            request.config.sampleRateHertz = _currentConfig.sampleRate;
            request.config.maxAlternatives = _currentConfig.maxAlternatives;
            request.config.profanityFilter = _currentConfig.isEnabledProfanityFilter;

            if (contexts != null)
            {
                request.config.speechContexts = new SpeechContext[contexts.Count];

                for (int i = 0; i < contexts.Count; i++)
                {
                    request.config.speechContexts[i] = new SpeechContext();
                    request.config.speechContexts[i].phrases = contexts[i];
                }
            }

            request.audio.content = content;

            return request;
        }
    }
}
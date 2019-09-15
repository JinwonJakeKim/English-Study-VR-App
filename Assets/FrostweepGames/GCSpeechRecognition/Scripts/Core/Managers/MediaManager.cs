using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class MediaManager : IService, IMediaManager
    {
        public event Action StartedRecordEvent;
        public event Action<AudioClip> FinishedRecordEvent;
        public event Action RecordFailedEvent;

        public event Action BeginTalkigEvent;
        public event Action<AudioClip> EndTalkigEvent;

        private AudioClip _microphoneWorkingAudioClip;
        private AudioClip _latestVoiceAudioClip;
        private string _microphoneDevice;

        private int _currentSamplePosition;
        private int _previousSamplePosition;
        private float[] _currentAudioSamples;
        private bool _isTalking;
        private List<float> _currentRecordingVoice;

        private ISpeechRecognitionManager _speechRecognitionManager;
        private IVoiceDetectionManager _voiceDetectionManager;


        public bool IsEnabledVoiceDetection { get; set; }
        public bool IsCanWork { get; set; }
        public bool IsRecording { get; set; }

        public void Init()
        {
            _speechRecognitionManager = GCSpeechRecognition.Instance.ServiceLocator.Get<ISpeechRecognitionManager>();
            _voiceDetectionManager = GCSpeechRecognition.Instance.ServiceLocator.Get<IVoiceDetectionManager>();

            CheckMicrophones();
        }

        public void StartRecord()
        {
            if (IsRecording || !IsCanWork)
                return;

            _currentRecordingVoice = new List<float>();

            if (_microphoneWorkingAudioClip != null)
                MonoBehaviour.Destroy(_microphoneWorkingAudioClip);

            if (_latestVoiceAudioClip != null)
                MonoBehaviour.Destroy(_latestVoiceAudioClip);

            _microphoneWorkingAudioClip = Microphone.Start(_microphoneDevice, true, 1, _speechRecognitionManager.CurrentConfig.sampleRate);

            _currentAudioSamples = new float[_microphoneWorkingAudioClip.samples * _microphoneWorkingAudioClip.channels];

            IsRecording = true;

            if (StartedRecordEvent != null)
                StartedRecordEvent();
        }


        public void StopRecord()
        {
            if (!IsRecording || !IsCanWork)
                return;

            IsRecording = false;

            Microphone.End(_microphoneDevice);


            if (!IsEnabledVoiceDetection)
                _latestVoiceAudioClip = MakeAudioClipFromSamples(_currentRecordingVoice.ToArray());

            if (_currentRecordingVoice != null)
                _currentRecordingVoice.Clear();

            _currentAudioSamples = null;
            _currentRecordingVoice = null;

            if (FinishedRecordEvent != null)
                FinishedRecordEvent(_latestVoiceAudioClip);
        }

        private void CheckMicrophones()
        {
            if (Microphone.devices.Length > 0)
            {
                _microphoneDevice = Microphone.devices[0];
                IsCanWork = true;
            }
            else
            {
                Debug.Log("Microphone devices not found!");
                IsCanWork = false;
            }
        }

        public void Update()
        {
            if (IsRecording)
            {
                _currentSamplePosition = Microphone.GetPosition(_microphoneDevice);
                _microphoneWorkingAudioClip.GetData(_currentAudioSamples, 0);

                if (IsEnabledVoiceDetection)
                {
                    bool isTalking = _voiceDetectionManager.CheckVoice(AudioClip2ByteConverter.FloatToByte(_currentAudioSamples));

                    if (!_isTalking && isTalking)
                    {
                        _isTalking = true;

                        if (BeginTalkigEvent != null)
                            BeginTalkigEvent();
                    }
                    else if (_isTalking && !isTalking)
                    {
                        _isTalking = false;

                        _latestVoiceAudioClip = MakeAudioClipFromSamples(_currentRecordingVoice.ToArray());

                        if (EndTalkigEvent != null)
                            EndTalkigEvent(_latestVoiceAudioClip);

                        _currentRecordingVoice.Clear();
                    }
                    else if (_isTalking)
                    {
                        AddVoiceSamples();
                    }
                }
                else
                {
                    AddVoiceSamples();
                }

                _previousSamplePosition = _currentSamplePosition;
            }
        }

        public void Dispose()
        {
            if (_microphoneWorkingAudioClip != null)
                MonoBehaviour.Destroy(_microphoneWorkingAudioClip);

            if (_latestVoiceAudioClip != null)
                MonoBehaviour.Destroy(_latestVoiceAudioClip); 
        }

        private void AddVoiceSamples()
        {
            if (_previousSamplePosition > _currentSamplePosition)
            {
                for (int i = _previousSamplePosition; i < _currentAudioSamples.Length; i++)
                    _currentRecordingVoice.Add(_currentAudioSamples[i]);

                _previousSamplePosition = 0;
            }

            for (int i = _previousSamplePosition; i < _currentSamplePosition; i++)
                _currentRecordingVoice.Add(_currentAudioSamples[i]);

            _previousSamplePosition = _currentSamplePosition;
        }

        private AudioClip MakeAudioClipFromSamples(float[] samples)
        {
            AudioClip clip;

            clip = AudioClip.Create("RecordedVoice", samples.Length, _microphoneWorkingAudioClip.channels, _speechRecognitionManager.CurrentConfig.sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }
    }
}
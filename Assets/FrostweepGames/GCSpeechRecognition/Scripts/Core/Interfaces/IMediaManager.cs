using System;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public interface IMediaManager
    {
        event Action StartedRecordEvent;
        event Action<AudioClip> FinishedRecordEvent;
        event Action RecordFailedEvent;

        event Action BeginTalkigEvent;
        event Action<AudioClip> EndTalkigEvent;

        bool IsEnabledVoiceDetection { get; set; }
        bool IsCanWork { get; set; }
        bool IsRecording { get; set; }

        void StopRecord();
        void StartRecord();
    }
}
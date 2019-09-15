using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public interface ISpeechRecognitionManager
    {
        event Action<RecognitionResponse, long> RecognitionSuccessEvent;
        event Action<string, long> RecognitionFailedEvent;

        Config CurrentConfig { get; }

        void SetConfig(Config config);
        void Recognize(AudioClip clip, List<string[]> contexts, Enumerators.LanguageCode language);
    }
}
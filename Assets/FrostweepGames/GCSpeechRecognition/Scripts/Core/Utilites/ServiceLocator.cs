using System;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class ServiceLocator : IDisposable
    {
        private Dictionary<Type, IService> _services;

        internal ServiceLocator()
        {
            _services = new Dictionary<Type, IService>();
            AddService<ISpeechRecognitionManager>(new SpeechRecognitionManager());
            AddService<IVoiceDetectionManager>(new VoiceDetectionManager());
            AddService<IMediaManager>(new MediaManager());
        }

        public void InitServices()
        {
            foreach (var service in _services)
                service.Value.Init();
        }

        public void Update()
        {
            foreach (var service in _services)
                service.Value.Update();
        }

        public void Dispose()
        {
            foreach (var service in _services)
                service.Value.Dispose();
        }

        public T Get<T>()
        {
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            else
                throw new NotImplementedException(typeof(T) + " not implemented!");
        }

        private void AddService<T>(IService service)
        {
            _services.Add(typeof(T), service);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class ThreadManager : IDisposable
    {
        private volatile List<Thread> _threads;
        private volatile Queue<Action> _mainThreadActions;

        public ThreadManager()
        {
            _threads = new List<Thread>();
            _mainThreadActions = new Queue<Action>();
        }

        public void Update()
        {
            if (_mainThreadActions.Count > 0)
            {
                for (int i = 0; i < _mainThreadActions.Count; i++)
                    _mainThreadActions.Dequeue().Invoke();
            }
        }

        public void Dispose()
        {
            AbortAllThreads();
        }

        public void RunInNewThread(Action method)
        {
            Thread thread = new Thread(new ThreadStart(method));
            thread.Start();

            _threads.Add(thread);
        }

        public void RunInMainThread(Action method)
        {
            _mainThreadActions.Enqueue(method);
        }

        /// <summary>
        /// Abort all threads.
        /// be sure to use it if threads count > 0 and if app will be closing
        /// </summary>
        public void AbortAllThreads()
        {
            foreach (var thread in _threads)
            {
#if UNITY_IOS
                thread.Interrupt();
#else
                thread.Abort();
#endif
            }

            _threads.Clear();
            _mainThreadActions.Clear();
        }
    }
}
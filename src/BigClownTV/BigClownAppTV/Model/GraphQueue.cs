using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.System.Threading;
using BigClownAppTV.Interfaces;

namespace BigClownAppTV.Model
{
    class GraphQueue<T> where T : IUnit
    {
        private Queue<T> _graphs;
        private ThreadPoolTimer _timer;
        private int _frequency;
        private DateTime _persisTime;

        /// <summary>
        /// Returns number of units in collection.
        /// </summary>
        public int Count => _graphs.Count;

        /// <summary>
        /// Returns switching frequncy between units in seconds.
        /// </summary>
        public int Frequency => _frequency;

        /// <summary>
        /// Returns Current element in collection.
        /// </summary>
        public T Current { get; set; }

        /// <summary>
        /// Returns data persistance in collection.
        /// </summary>
        public DateTime PersistTime => _persisTime;

        public delegate void GraphEventHandler(object sender, GraphQueueEventArgs<T> e);
        public event GraphEventHandler GraphHandler;


        public GraphQueue([Range(3,60)] int frequency, DateTime removeTime, params T[] graphs)
        {
            _graphs = new Queue<T>();

            _frequency = frequency;
            foreach (var a in graphs)
            {
                _graphs.Enqueue(a);
            }

            _timer = ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                if (_graphs.Count != 0 &&_graphs != null)
                {
                    Current = _graphs.Peek();
                    _graphs.Enqueue(_graphs.Dequeue());
                    OnGraphHandler(); // Dodělat...
                }

                while (true)
                {
                    if (_graphs.Peek().Time < DateTime.UtcNow.AddSeconds(-_frequency))
                    {
                        _graphs.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }

            }, TimeSpan.FromSeconds(frequency));
        }

        protected virtual void OnGraphHandler()
        {
            GraphHandler?.Invoke(this, new GraphQueueEventArgs<T>());
        }

        /// <summary>
        /// Generic eventargs that provides what is the current graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class GraphQueueEventArgs<T> : EventArgs // Dodělat...
        {
            public GraphQueueEventArgs()
            {
                
            }
        }
    }
}

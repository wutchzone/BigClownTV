using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.System.Threading;
using BigClownAppTV.Interfaces;
using Syncfusion.UI.Xaml.Diagram.Stencil;

namespace BigClownAppTV.Model
{
    class GraphQueue<T> where T : IUnit
    {
        private Queue<Queue<T>> _graphs;
        private ThreadPoolTimer _timer;
        private int _frequency;
        private int _persisTime;

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
        public Queue<T> Current { get; private set; }

        /// <summary>
        /// Returns data persistance in collection.
        /// </summary>
        public int PersistTime => _persisTime;

        public delegate void GraphEventHandler(object sender, GraphQueueEventArgs<T> e);
        public event GraphEventHandler GraphHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="removeTime">Remove time in seconds</param>
        /// <param name="graphs"></param>
        public GraphQueue([Range(3,60)] int frequency, int removeTime, params Queue<T>[] graphs)
        {
            _graphs = new Queue<Queue<T>>();
            _frequency = frequency;
            _persisTime = removeTime;
            foreach (var a in graphs)
            {
                _graphs.Enqueue(a);
            }

            _timer = ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                if (_graphs.Count != 0 &&_graphs != null )
                {
                    Current = _graphs.Peek();
                    _graphs.Enqueue(_graphs.Dequeue());

                    if (this.Current.Count == 0) return;
                    OnGraphHandler(Current, this.Current.Peek().Header, this.Current.Peek().Label);

                    while (true)
                    {
                        if (_graphs.Peek().Peek().Time < DateTime.UtcNow.Subtract(new TimeSpan(0,0,0,_persisTime)))
                        {
                            System.Diagnostics.Debug.WriteLine("Removed " + _graphs.Peek().Peek().Time);
                            _graphs.Peek().Dequeue();
                            
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Nothing to remove");
                            break;
                        }
                    }
                }              
            }, TimeSpan.FromSeconds(frequency));
        }

        protected virtual void OnGraphHandler(Queue<T> queue, string header, string label)
        {
            GraphHandler?.Invoke(this, new GraphQueueEventArgs<T>(queue, header, label));
        }

        /// <summary>
        /// Generic eventargs that provides what is the current graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class GraphQueueEventArgs<T> : EventArgs
        {
            public Queue<T> List { get; private set; } 
            public string Header { get; set; }
            public string Label { get; set; }

            public GraphQueueEventArgs(Queue<T> list, string header, string label)
            {
                this.List = list;
                this.Header = header;
                this.Label = label;
            }
        }
    }
}

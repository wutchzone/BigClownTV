using System;
using BigClownAppTV.Interfaces;

namespace BigClownAppTV.Model
{
    public class Unit : IUnit
    {
        public string Header { get; set; }
        public float Value { get; set; }
        public string Label { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            
            return string.Format("");
        }
    }
}

using System;

namespace BigClownAppTV.Interfaces
{
    public interface IUnit
    {
        int Header { get; set; }
        float Value { get; set; }
        string Label { get; set; }
        DateTime Time { get; set; }
    }
}
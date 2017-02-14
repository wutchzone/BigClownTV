using System;

namespace BigClownAppTV.Interfaces
{
    public interface IUnit
    {
        string Header { get; set; }
        float Value { get; set; }
        string Label { get; set; }
        DateTime Time { get; set; }
    }
}
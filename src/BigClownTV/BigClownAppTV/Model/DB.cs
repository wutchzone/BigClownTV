using System;
using System.Diagnostics;
using System.IO;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;

namespace BigClownAppTV.Model
{
    public class DB : SQLiteConnection
    {
        public DB() : base(new SQLitePlatformWinRT(), Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "database.sqlite"))
        {

            CreateTable<Temperature>();
            CreateTable<Humidity>();
            CreateTable<Lux>();
            CreateTable<Pressure>();
            CreateTable<Altitude>();
            Debug.WriteLine("DB loaded.");
        }

        public class Temperature : Default
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public float Value { get; set; }
            public string Label { get; set; }
            public DateTime Time { get; set; }
        }

        public class Humidity : Default
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public float Value { get; set; }
            public string Label { get; set; }
            public DateTime Time { get; set; }
        }

       public class Default
        {
            public string Header { get; set; }
        }
        public class Lux : Default
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public float Value { get; set; }
            public string Label { get; set; }
            public DateTime Time { get; set; }

        }

        public class Pressure : Default
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public float Value { get; set; }
            public string Label { get; set; }
            public DateTime Time { get; set; }
        }
        public class Altitude : Default
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public float Value { get; set; }
            public string Label { get; set; }
            public DateTime Time { get; set; }
        }
    }
}

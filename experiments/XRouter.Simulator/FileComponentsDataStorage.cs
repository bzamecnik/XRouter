using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using XRouter.ComponentWatching;

namespace XRouter.Simulator
{
    class FileComponentsDataStorage : IComponentsDataStorage
    {
        private static readonly string StorageDirectory = @"..\..\..\XRouter.CommonData\ComponentsDataStorage";

        private Simulation Simulation { get; set; }

        private readonly string filePath;
        private Dictionary<string, Point> locations = new Dictionary<string, Point>();

        public FileComponentsDataStorage(Simulation simulation)
        {
            Simulation = simulation;

            string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string storageDirectoryFullPath = Path.Combine(binPath, StorageDirectory);
            if (!Directory.Exists(storageDirectoryFullPath)) {
                Directory.CreateDirectory(storageDirectoryFullPath);
            }
            filePath = Path.Combine(storageDirectoryFullPath, GetSafeFileName(Simulation.Name) + ".txt");

            LoadFile();
        }

        private string GetSafeFileName(string name)
        {
            StringBuilder result = new StringBuilder(name);
            for (int i = 0; i < name.Length; i++) {
                char c = result[i];
                bool isSafeChar = char.IsLetterOrDigit(c) || (c == ' ') || (c == '_') || (c == '-');
                if (!isSafeChar) {
                    result[i] = '_';
                }
            }
            return result.ToString();
        }

        public Point GetLocation(string componentName)
        {
            if (locations.ContainsKey(componentName)) {
                Point result = locations[componentName];
                return result;
            } else {
                Point result = new Point();
                return result;
            }
        }

        public void SetLocation(string componentName, Point location)
        {
            if (locations.ContainsKey(componentName)) {
                locations[componentName] = location;
            } else {
                locations.Add(componentName, location);
            }
            SaveFile();
        }

        private void LoadFile()
        {
            if (!File.Exists(filePath)) {
                locations.Clear();
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines) {
                if (line.Trim().Length == 0) {
                    continue;
                }
                var parts = line.Split(';');
                string name = parts[0];
                double x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                locations.Add(name, new Point(x, y));
            }
        }

        private void SaveFile()
        {
            List<string> lines = new List<string>();
            foreach (var pair in locations) {
                string line = string.Format(CultureInfo.InvariantCulture, "{0};{1};{2}", pair.Key, pair.Value.X, pair.Value.Y);
                lines.Add(line);
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.IO;

namespace PrimesOpenTK
{
    public class Preferences
    {
        public PreferencesModel Model { get; set; }
        private readonly FileManager<PreferencesModel> fileManager;

        private static Preferences instance;
        public static Preferences Instance => instance ?? (instance = new Preferences());

        public Preferences()
        {
            this.fileManager = new FileManager<PreferencesModel>("UserSettings.json");
            this.Model = this.fileManager.Load() ?? new PreferencesModel();
        }

        public void Save()
        {
            if (this.Model == null)
            {
                this.Model = new PreferencesModel();
            }
            this.fileManager.Save(this.Model);
        }

        private class FileManager<T> where T : class
        {
            private readonly string filePath;

            public FileManager(string fileName)
            {
                this.filePath = this.GetLocalFilePath(fileName);
            }

            private string GetLocalFilePath(string fileName)
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appData, fileName);
            }

            public T Load()
            {
                return File.Exists(this.filePath) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(this.filePath)) : null;
            }

            public void Save(T model)
            {
                var json = JsonConvert.SerializeObject(model);
                File.WriteAllText(this.filePath, json);
            }
        }
    }
}

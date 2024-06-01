using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GBGame.Utilities
{
    public class JsonObjectLibrary<T> where T : IJsonObject
    {
        public static JsonObjectLibrary<T> LoadLibrary(string libraryFolder, string libraryName)
        {
            libraryName = $"{libraryName}.json";
            string lib_path = Path.Combine(Application.streamingAssetsPath, libraryFolder);
            string lib_file = Path.Combine(lib_path, libraryName);
            string data_path = Path.Combine(lib_path, $"Data");
            JsonObjectLibrary<T> result;
            if (!File.Exists(lib_file))
            {
                Debug.LogWarning($"Library file not found, generating new at {lib_file}");
                result = new JsonObjectLibrary<T>(lib_path, data_path, libraryName);

                if (!Directory.Exists(lib_path)) Directory.CreateDirectory(lib_path);
                if (!Directory.Exists(data_path)) Directory.CreateDirectory(data_path);
                result.SaveLibrary();
            }
            else
            {
                string json = File.ReadAllText(lib_file);
                result = JsonUtility.FromJson<JsonObjectLibrary<T>>(json);
            }
            result.LibPath = lib_path;
            result.DataPath = data_path;
            result.LibName = libraryName;
            return result;
        }


        [System.Serializable]
        private struct LibEntry
        {
            public string ID;
            public string FileName => $"{ID}.json";
            public string ExpectedType;

            public LibEntry(in T item)
            {
                ExpectedType = item.GetType().ToString();
                ID = item.JsonID;
            }
        }

        private string LibPath;
        private string DataPath;
        private string LibName;

        [SerializeField] private List<LibEntry> Entries = new List<LibEntry>();

        private string ItemPath(string item) => Path.Combine(LibPath, DataPath, item);
        private string LibraryFile => Path.Combine(LibPath, LibName);

        public JsonObjectLibrary(string libPath, string dataPath, string libName)
        {
            LibPath = libPath;
            DataPath = dataPath;
            LibName = libName;
        }

        private bool GetEntry(string key, out LibEntry result)
        {
            foreach (var item in Entries)
            {
                if (item.ID == key)
                {
                    result = item;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public void SaveLibrary()
        {
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(LibraryFile, json);
        }

        public bool GetItem(string id, out T result)
        {
            result = default;
            if (!GetEntry(id, out LibEntry entry)) return false;

            string json = File.ReadAllText(ItemPath(entry.FileName));

            result = JsonUtility.FromJson<T>(json);

            return true;
        }
        public bool SaveItem(in T item)
        {
            LibEntry entry;
            if (!GetEntry(item.JsonID, out entry))
            {
                entry = new LibEntry(item);
                Entries.Add(entry);
            }

            string json = JsonUtility.ToJson(item, true);

            File.WriteAllText(ItemPath(entry.FileName), json);
            SaveLibrary();

            return true;
        }

        public bool DeleteItem(string id)
        {
            if (!GetEntry(id, out LibEntry entry)) return false;
            File.Delete(ItemPath(entry.FileName));
            Entries.Remove(entry);
            SaveLibrary();
            return true;
        }

        public string[] GetItemIDs()
        {
            List<string> result = new List<string>();
            foreach (var item in Entries)
            {
                result.Add(item.ID);
            }
            result.Sort();
            return result.ToArray();
        }
    } 
}
using System.IO;
using System.Linq;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using DesignPatterns;
using ScriptableObjects;
using UnityTemplateProjects;

[RequireComponent(typeof())]
public class SaveManager : Singleton<SaveManager> {
    private string local_save_directory_path;
    private string local_save_data_directory_path;
    private string local_save_directory_file_path;

    private void OnEnable() {
        local_save_directory_path = Application.persistentDataPath + "/save";
        local_save_data_directory_path = Application.persistentDataPath + "/save/data";
        local_save_directory_file_path = Application.persistentDataPath + "/save/data/dat.txt";
    }

    private void Save<T>(T t) {
        if (!IsSaveDirectoryExisting())
            Directory.CreateDirectory(local_save_directory_path);
        if (!IsDataDirectoryExisting())
            Directory.CreateDirectory(local_save_data_directory_path);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(local_save_directory_file_path);

        string json = JsonUtility.ToJson(t);

        #if UNITY_EDITOR
            File.WriteAllText(local_save_data_directory_path + "/debug_json.json", json);
        #endif

        try {
            bf.Serialize(file, json);
            Debug.Log("Saved at : " + local_save_directory_file_path +  " TIME : " + System.DateTime.Now);
        } catch (FileNotFoundException e) {
            Debug.LogException(e);
            throw;
        } finally {
            file.Close();
        }
    }

    private void Load<T>(T t) {
        if (!IsSaveFileExisting())
            return;

        BinaryFormatter bf = new BinaryFormatter();

        try {
            FileStream file = File.Open(local_save_directory_file_path, FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file) as string, t);
            Debug.Log("Loaded: " + " TIME : " + System.DateTime.Now);
        } catch (FileNotFoundException e) {
            Debug.LogException(e);
            throw;
        }
    }

    private bool IsSaveDirectoryExisting() => Directory.Exists(local_save_directory_path);
    private bool IsDataDirectoryExisting() => Directory.Exists(local_save_data_directory_path);
    private bool IsSaveFileExisting() => File.Exists(local_save_directory_file_path);
}

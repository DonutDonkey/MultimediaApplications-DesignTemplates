using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveUtility {
	private Hashtable data;
	public Hashtable Data { get => data; set => data = value; }
        
	public SaveTools() => data = new Hashtable();

	public SaveTools Write<T>(string  in_key, T in_value) {
		data[in_key] = in_value;
		return this;
	}

	public SaveTools Read<T>(string in_key, ref T in_value) {
		in_value = data[in_key] is T ? (T) data[in_key] : default;
		return this;
	}

	public T Read<T>(string in_key) => data[in_key] is T ? (T) data[in_key] : default;

	// should be async?
	public void Save(string filePath, string fileName) {
		List<string> lines = 
			(from DictionaryEntry pair in data select $"{pair.Key}:{pair.Value}").ToList();
		FileStream fs = new FileStream(filePath + fileName, FileMode.Create);
		BinaryFormatter formatter = new BinaryFormatter();
		lines.Sort();
		lines.Insert(0, DateTime.Now.ToString(CultureInfo.InvariantCulture));
		
		try {
			formatter.Serialize(fs, data);
		} catch (SerializationException e) {
			Console.WriteLine("Failed to serialize. Reason: " + e.Message);
			throw;
		} finally {
			fs.Close();
		}
            
#if UNITY_EDITOR
		try {
			File.WriteAllLines(filePath + "DEBUG.TXT", lines);
		} catch (FileLoadException e) {
			Console.WriteLine("Failed to serialize. Reason: " + e.Message);
			throw;
		}
#endif
	}

	public Hashtable Load(string filePath) {
		FileStream fs = new FileStream(filePath, FileMode.Open);
		try {
			BinaryFormatter formatter = new BinaryFormatter();

			// Deserialize the hashtable from the file
			return formatter.Deserialize(fs) as Hashtable;
		} catch (SerializationException e) {
			Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
			throw;
		} finally {
			fs.Close();
		}
	}
}

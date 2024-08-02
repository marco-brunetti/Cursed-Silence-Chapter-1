using Newtonsoft.Json;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = string.Empty;
    private string fileName = string.Empty;
    private bool useEncryption = false;
    private readonly string encryptioncodeWord = "word";

    public FileDataHandler(string dataDirPath, string fileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.fileName = fileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, fileName);
        GameData loadedData = null;

        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = string.Empty;

                using(FileStream fileStream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(fileStream)) { dataToLoad = reader.ReadToEnd(); }
                }

                if (useEncryption) dataToLoad = EncryptDecrypt(dataToLoad);

                loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
                UnityEngine.Debug.Log($"Retrieved saved data from: {fullPath}.");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }

        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonConvert.SerializeObject(data);

            if(useEncryption) dataToStore = EncryptDecrypt(dataToStore);

            using(FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(fileStream)) { writer.Write(dataToStore); }

                UnityEngine.Debug.Log($"Saved data to: {fullPath}.");
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    ///A simple implementation of XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = string.Empty;

        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ encryptioncodeWord[i % encryptioncodeWord.Length]);
        }

        return modifiedData;
    }
}
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager {
    private readonly string saveFolder = Application.persistentDataPath + Path.DirectorySeparatorChar + "saveData";
    public void Init() {
        // create saveFolder if not exists
        Directory.CreateDirectory(saveFolder);
        
    }

    public bool FileExists(string fileName) {
        throw new System.NotImplementedException();
    }
}

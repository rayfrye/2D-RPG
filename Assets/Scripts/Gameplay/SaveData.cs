using UnityEngine;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveData : MonoBehaviour 
{
	public static void save(string saveGameName)
	{
		if (Application.platform != RuntimePlatform.WindowsWebPlayer) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Create (Application.persistentDataPath + "/" + saveGameName + ".gamedata");
			bf.Serialize (file, GameManager.Game.current);
			file.Close ();
		}
	}

	public static void load(string saveGameName)
	{
		if (Application.platform != RuntimePlatform.WindowsWebPlayer) 
		{
			if (File.Exists (Application.persistentDataPath + "/" + saveGameName + ".gamedata")) 
			{
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (Application.persistentDataPath + "/" + saveGameName + ".gamedata", FileMode.Open);
				GameManager.Game.current = (GameManager.Game)bf.Deserialize (file);
				file.Close ();
			}
		}
	}
}

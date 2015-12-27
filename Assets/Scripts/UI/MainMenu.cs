using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class MainMenu : MonoBehaviour 	
{
	#region GameObjects
	public GameObject pnl_SaveGameNameInputBG;
	public GameObject pnl_LoadSaveGames;
	public GameObject btn_GameSave;
	public GameObject content_GameList;
	public List<GameObject> loadGameButtons = new List<GameObject> ();
	#endregion GameObjects


	public void startNewGame(string SaveGameName)
	{
		string path = @"SaveData/" + SaveGameName;

		if (Directory.Exists (path)) 
		{
			Debug.Log ("File already exists!");
		}
		else 
		{
			createNewSaveGame (SaveGameName);
			pnl_SaveGameNameInputBG.SetActive(false);

			setCurrentSaveGame(SaveGameName);

			StartCoroutine(loadLevel(2.0f,"Interior - Player House"));

		}
	}

	IEnumerator loadLevel(float waitSeconds, string sceneName) 
	{
		yield return new WaitForSeconds(waitSeconds);
		Application.LoadLevel (sceneName);
	}
	
	public void setCurrentSaveGame(string SaveGameName)
	{
		string data = SaveGameName;

		var sr = File.CreateText ("SaveData/currentSaveGame.txt");
		sr.Write (data);
		sr.Close ();
	}

	public void createNewSaveGame(string SaveGameName)
	{
		//FileUtil.CopyFileOrDirectory (@"Assets/Resources/Data/GameData/SaveGameTemplate",@"Assets/Resources/Data/SaveData/" + SaveGameName);
		dirCopy ( @"Assets/Resources/Data/GameData/SaveGameTemplate", @"SaveData/" + SaveGameName, true);
	}

	public void dirCopy(string sourceDirName, string destDirName, bool copySubDirs)
	{
		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);
		
		if (!dir.Exists)
		{
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDirName);
		}
		
		DirectoryInfo[] dirs = dir.GetDirectories();
		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists(destDirName))
		{
			Directory.CreateDirectory(destDirName);
		}
		
		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files)
		{
			string temppath = Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, false);
		}
		
		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string temppath = Path.Combine(destDirName, subdir.Name);
				dirCopy(subdir.FullName, temppath, copySubDirs);
			}
		}
	}

	public void showLoadGameDialogue()
	{
		DirectoryInfo dir = new DirectoryInfo("SaveData");
		
		if (!dir.Exists) 
		{
			Directory.CreateDirectory("SaveData");
		}

		clearList (loadGameButtons);

		pnl_LoadSaveGames.SetActive (true);

		string[] gameSaves = Directory.GetDirectories(@"SaveData");

		foreach (string gameSave in gameSaves) 
		{
			int startIndex = gameSave.LastIndexOf("/") + 1;
			string saveGameName = gameSave.Substring(startIndex,gameSave.Length - startIndex);

			GameObject newBtn = (GameObject) Instantiate (btn_GameSave);
			newBtn.transform.SetParent (content_GameList.transform);
			newBtn.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

			newBtn.transform.FindChild("txt_SaveGameName").GetComponent<Text>().text = saveGameName;

			newBtn.GetComponent<Button>().onClick.AddListener(() => loadGame(saveGameName));

			loadGameButtons.Add (newBtn);
		}
	}

	void clearList(List<GameObject> l)
	{
		foreach (GameObject go in l) 
		{
			GameObject.Destroy (go);
		}

		l.Clear ();
	}

	public void loadGame(string saveGameName)
	{
		XmlDocument xml = new XmlDocument ();
		//string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + saveGameName + "/playerdata.xml");
		//xml.LoadXml( content );

		string content = File.ReadAllText ("SaveData/" + saveGameName + "/playerdata.xml");  
		xml.LoadXml( content );

		XmlNode node = xml.DocumentElement.SelectSingleNode ("/data");

		setCurrentSaveGame(saveGameName);

		StartCoroutine (loadLevel (5.0f, node ["scene"].InnerText));
	}
}

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
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
		string path = @"Assets/Resources/Data/SaveData/" + SaveGameName;

		if (Directory.Exists (path)) 
		{
			Debug.Log ("File already exists!");
		}
		else 
		{
			createNewSaveGame (SaveGameName);
			pnl_SaveGameNameInputBG.SetActive(false);
			Application.LoadLevel ("Interior - Player House");
		}
	}

	public void createNewSaveGame(string SaveGameName)
	{
		FileUtil.CopyFileOrDirectory (@"Assets/Resources/Data/GameData/SaveGameTemplate",@"Assets/Resources/Data/SaveData/" + SaveGameName);
	}

	public void showLoadGameDialogue()
	{
		clearList (loadGameButtons);

		pnl_LoadSaveGames.SetActive (true);

		string[] gameSaves = Directory.GetDirectories(@"Assets/Resources/Data/SaveData");

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
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + saveGameName + "/playerdata.xml");
		xml.LoadXml( content );
		XmlNode node = xml.DocumentElement.SelectSingleNode ("/data");

		Application.LoadLevel (node ["scene"].InnerText);
	}
}

using UnityEngine;
using UnityEngine.UI;
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
		createNewSaveGame (SaveGameName);
		pnl_SaveGameNameInputBG.SetActive(false);
	}

	public void createNewSaveGame(string SaveGameName)
	{
		GameManager.Game game = new GameManager.Game ();

		game.saveName = SaveGameName;
		game.quests = loadQuestData();
		game.characters = loadCharacterData ();
		game.player = loadPlayerData ();

		GameManager.Game.current = game;

		SaveData.save (SaveGameName);
		SaveData.save ("current_game");

		Application.LoadLevel (game.player.scene);
	}

	public void showLoadGameDialogue()
	{
		clearList (loadGameButtons);

		pnl_LoadSaveGames.SetActive (true);

		string[] gameSaves = Directory.GetFiles (Application.persistentDataPath);

		foreach (string gameSave in gameSaves) 
		{
			if(gameSave.Contains (".gamedata") && !gameSave.Contains("current_game"))
			{
				int startIndex = gameSave.LastIndexOf("/") + 1;
				int length = gameSave.Length - startIndex - 9;
				string saveGameName = gameSave.Substring(startIndex,length);

				GameObject newBtn = (GameObject) Instantiate (btn_GameSave);
				newBtn.transform.SetParent (content_GameList.transform);
				newBtn.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

				newBtn.transform.FindChild("txt_SaveGameName").GetComponent<Text>().text = saveGameName;

				newBtn.GetComponent<Button>().onClick.AddListener(() => loadGame(saveGameName));

				loadGameButtons.Add (newBtn);
			}
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
		SaveData.load (saveGameName);
		SaveData.save ("current_game");

		Application.LoadLevel (GameManager.Game.current.player.scene);
	}

	public List<GameManager.Quest> loadQuestData()
	{
		List <GameManager.Quest> quests = new List<GameManager.Quest> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = File.ReadAllText("./Assets/Resources/Data/GameData/SaveGameTemplate/questdata.xml");
		xml.LoadXml( content );
		
		foreach (XmlNode node in xml.DocumentElement.SelectNodes ("/data/quest")) 
		{
			GameManager.Quest quest = new GameManager.Quest();
			
			quest.id = int.Parse (node["id"].InnerText);
			quest.name = node["name"].InnerText;
			quest.desc = node["desc"].InnerText;
			quest.isComplete = returnTrueFalse(node["isComplete"].InnerText);
			
			quests.Add (quest);
		}
		
		return quests;
	}

	public GameManager.Player loadPlayerData()
	{
		GameManager.Player player = new GameManager.Player ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( "./Assets/Resources/Data/GameData/SaveGameTemplate/playerdata.xml");
		xml.LoadXml( content );
		
		XmlNode node = xml.DocumentElement.SelectSingleNode ("/data");
		
		player.name = node ["name"].InnerText;
		player.characterSprite = node ["characterSprite"].InnerText;
		player.scene = node ["scene"].InnerText;
		Vector3 pos = new Vector3 (float.Parse (node ["position_x"].InnerText), float.Parse (node ["position_y"].InnerText), float.Parse (node ["position_z"].InnerText));
		
		if (node ["use_coordinates"].InnerText.ToUpper () != "TRUE") 
		{
			GameObject go = GameObject.Find (node ["waypoint_name"].InnerText);
			
			if(go != null)
			{
				pos = go.transform.position;
			}
		}
		
		player.pos_x = pos.x;
		player.pos_y = pos.y;
		player.pos_z = pos.z;

		player.useCoordinates = false;
		player.spawnpoint = "waypoint_PlayerSpawn_Bed";
		player.scene = "Interior - Player House";
		
		player.partyIDs = new List<int> ();
		foreach (XmlNode partyNode in xml.DocumentElement.SelectNodes ("/data/party/id")) 
		{
			player.partyIDs.Add (int.Parse (partyNode.InnerText));
		}
		
		return player;
	}

	public List<GameManager.Character> loadCharacterData()
	{
		List <GameManager.Character> characters = new List<GameManager.Character> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( "./Assets/Resources/Data/GameData/SaveGameTemplate/characterdata.xml");
		xml.LoadXml( content );
		
		foreach (XmlNode node in xml.DocumentElement.SelectNodes ("/data/character")) 
		{
			GameManager.Character character = new GameManager.Character();
			
			character.id = int.Parse (node["id"].InnerText);
			character.name = node["name"].InnerText;
			character.level = int.Parse (node["level"].InnerText);
			character.experience = int.Parse (node["experience"].InnerText);
			character.characterClassID = int.Parse (node["characterClassID"].InnerText);
			character.currentHealth = int.Parse (node["currentHealth"].InnerText);
			character.totalHealth = int.Parse (node["totalHealth"].InnerText);
			character.baseStrength = int.Parse (node["baseStrength"].InnerText);
			character.totalStrength = int.Parse (node["totalStrength"].InnerText);
			character.baseDefense = int.Parse (node["baseDefense"].InnerText);
			character.totalDefense = int.Parse (node["totalDefense"].InnerText);
			character.baseSpeed = int.Parse (node["baseSpeed"].InnerText);
			character.totalSpeed = int.Parse (node["totalSpeed"].InnerText);
			character.baseIntelligence = int.Parse (node["baseIntelligence"].InnerText);
			character.totalIntelligence = int.Parse (node["totalIntelligence"].InnerText);
			character.baseCharisma = int.Parse (node["baseCharisma"].InnerText);
			character.totalCharisma = int.Parse (node["totalCharisma"].InnerText);
			
			foreach(XmlNode abilityNode in node.SelectNodes ("abilities/id"))
			{
				character.abilityIDs.Add (int.Parse (abilityNode.InnerText));
			}
			
			characters.Add (character);
		}
		
		return characters;
	}

	public bool returnTrueFalse(string s)
	{
		bool b = true;
		
		if (s.ToUpper () == "FALSE") 
		{
			b = false;
		}
		
		return b;
	}
}

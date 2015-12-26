using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Instantiator : MonoBehaviour 
{
	public GameObject GlobalGameManager;
	public GameManager gm;
	public string saveGame;
	public GameObject PC;

	void Start()
	{
		if (GameObject.Find ("GlobalGameManager"))
		{
			Debug.Log ("GlobalGameManager already exists, not instantiating a new one, that would be silly.");
		}
		else
		{
			Debug.Log ("Instantiating new GlobalGameManager");
			GameObject go = new GameObject ();
			setupGlobalGameManager (go);
			go.name = "GlobalGameManager";
		}
	}
	
	public GameManager setupGlobalGameManager(GameObject go)
	{
		GameManager newgm = go.AddComponent<GameManager> ();
		gm = newgm;

		gm.saveGame = saveGame;
		gm.instantiator = this.gameObject;
		gm.quests = loadQuestData ();
		gm.abilities = loadAbilityData ();
		gm.characterClasses = loadClassData ();
		gm.characters = loadCharacterData (gm);
		gm.player = loadPlayerData (gm);

		gm.setupLocalGameManager ();
		gm.setupPCGo ();

		return gm;
	}

	public List<GameManager.Quest> loadQuestData()
	{
		List <GameManager.Quest> quests = new List<GameManager.Quest> ();

		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + saveGame + "/questdata.xml");
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

	public List<GameManager.CharacterClass> loadClassData()
	{
		List <GameManager.CharacterClass> charClasses = new List<GameManager.CharacterClass> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/GameData/GlobalData/classdata.xml");
		xml.LoadXml( content );
		
		foreach (XmlNode node in xml.DocumentElement.SelectNodes ("/data/class")) 
		{
			GameManager.CharacterClass charClass = new GameManager.CharacterClass();
			
			charClass.id = int.Parse (node["id"].InnerText);
			charClass.name = node["name"].InnerText;
			charClass.healthMin = int.Parse (node["healthMin"].InnerText);
			charClass.healthMax = int.Parse (node["healthMax"].InnerText);
			charClass.strengthMin = int.Parse (node["strengthMin"].InnerText);
			charClass.strengthMax = int.Parse (node["strengthMax"].InnerText);
			charClass.defenseMin = int.Parse (node["defenseMin"].InnerText);
			charClass.defenseMax = int.Parse (node["defenseMax"].InnerText);
			charClass.speedMin = int.Parse (node["speedMin"].InnerText);
			charClass.speedMax = int.Parse (node["speedMax"].InnerText);
			charClass.intelligenceMin = int.Parse (node["intelligenceMin"].InnerText);
			charClass.intelligenceMax = int.Parse (node["intelligenceMax"].InnerText);
			charClass.charismaMin = int.Parse (node["charismaMin"].InnerText);
			charClass.charismaMax = int.Parse (node["charismaMax"].InnerText);

			charClasses.Add (charClass);
		}
		
		return charClasses;
	}

	public List<GameManager.Character> loadCharacterData(GameManager gm)
	{
		List <GameManager.Character> characters = new List<GameManager.Character> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + saveGame + "/characterdata.xml");
		xml.LoadXml( content );
		
		foreach (XmlNode node in xml.DocumentElement.SelectNodes ("/data/character")) 
		{
			GameManager.Character character = new GameManager.Character();
			
			character.id = int.Parse (node["id"].InnerText);
			character.name = node["name"].InnerText;
			character.level = int.Parse (node["level"].InnerText);
			character.experience = int.Parse (node["experience"].InnerText);
			character.characterClassID = int.Parse (node["characterClassID"].InnerText);
			character.characterClass = gm.characterClasses[int.Parse (node["characterClassID"].InnerText)];
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
				character.abilities.Add (gm.abilities[int.Parse (abilityNode.InnerText)]);
			}

			characters.Add (character);
		}
		
		return characters;
	}

	public GameManager.Player loadPlayerData(GameManager gm)
	{
		GameManager.Player player = new GameManager.Player ();

		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + saveGame + "/playerdata.xml");
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
				//pos = go.GetComponent<RectTransform>().position;
				pos = go.transform.position;
			}
		}

		player.pos = pos;

		player.party = new List<GameManager.Character> ();
		foreach (XmlNode partyNode in xml.DocumentElement.SelectNodes ("/data/party/id")) 
		{
			player.party.Add (gm.characters[int.Parse (partyNode.InnerText)]);
		}

		gm.enemy = new GameManager.Player ();
		gm.enemy.party = new List<GameManager.Character> ();
		foreach (XmlNode partyNode in xml.DocumentElement.SelectNodes ("/data/enemyparty/id")) 
		{
			gm.enemy.party.Add (gm.characters[int.Parse (partyNode.InnerText)]);
		}

		return player;
	}

	public List<GameManager.Ability> loadAbilityData()
	{
		List <GameManager.Ability> abilities = new List<GameManager.Ability> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/GameData/GlobalData/abilitydata.xml");
		xml.LoadXml( content );
		
		foreach (XmlNode node in xml.DocumentElement.SelectNodes ("/data/ability")) 
		{
			GameManager.Ability ability = new GameManager.Ability();
			
			ability.id = int.Parse (node["id"].InnerText);
			ability.name = node["name"].InnerText;
			ability.functionName = node["functionName"].InnerText;
			ability.abilityType = getAbilityType(node["abilityTypeText"].InnerText);
			ability.numSides = int.Parse(node["numSides"].InnerText);
			ability.numDice = int.Parse(node["numDice"].InnerText);

			abilities.Add (ability);
		}
		
		return abilities;
	}

	GameManager.AbilityType getAbilityType(string abilityTypeText)
	{
		abilityTypeText = abilityTypeText.ToUpper ();
		GameManager.AbilityType abilityType = GameManager.AbilityType.Melee;

		switch (abilityTypeText) 
		{
		case "MELEE":
		{
			abilityType = GameManager.AbilityType.Melee;
			break;
		}
		case "MAGIC":
		{
			abilityType = GameManager.AbilityType.Magic;
			break;
		}
		case "RANGED":
		{
			abilityType = GameManager.AbilityType.Ranged;
			break;
		}
		default:
		{
			break;
		}
		}

		return abilityType;
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
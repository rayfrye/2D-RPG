using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Instantiator : MonoBehaviour 
{
	public GameObject GlobalGameManager;
	public GameManager gm;
	public string saveGame;
	public GameObject PC;
	public SaveData saveData;

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

		SaveData.load ("current_game");

		gm.instantiator = this.gameObject;

		gm.saveGame = saveGame;
		gm.abilities = loadAbilityData ();
		gm.characterClasses = loadClassData ();

		gm.setupLocalGameManager ();
		gm.setupPCGo ();

		return gm;
	}

	public List<GameManager.CharacterClass> loadClassData()
	{
		List <GameManager.CharacterClass> charClasses = new List<GameManager.CharacterClass> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( "Assets/Resources/Data/GameData/GlobalData/classdata.xml");
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

	public List<GameManager.Ability> loadAbilityData()
	{
		List <GameManager.Ability> abilities = new List<GameManager.Ability> ();
		
		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( "Assets/Resources/Data/GameData/GlobalData/abilitydata.xml");
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
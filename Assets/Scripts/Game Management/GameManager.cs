using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	#region classes
	[System.Serializable]
	public class Game
	{
		public static Game current;

		public string saveName;

		public List<Quest> quests = new List<Quest>();
		public List<Character> characters = new List<Character>();
		public Player player;
		public Player enemy;
	}

	[System.Serializable]
	public class Quest
	{
		public int id;
		public string name;
		public string desc;
		public bool isComplete;
	}

	[System.Serializable]
	public class Player
	{
		public string name;
		public string characterSprite;
		public List<int> partyIDs = new List<int>();
		public string scene;
		public bool useCoordinates;
		public string spawnpoint;
		public float pos_x;
		public float pos_y;
		public float pos_z;
	}

	[System.Serializable]
	public class Character
	{
		public int id;
		public string name;
		public int level;
		public int experience;

		public int characterClassID;

		public int currentHealth;
		public int totalHealth;
		public int baseStrength;
		public int totalStrength;
		public int baseDefense;
		public int totalDefense;
		public int baseSpeed;
		public int totalSpeed;
		public int baseIntelligence;
		public int totalIntelligence;
		public int baseCharisma;
		public int totalCharisma;

		public List<int> abilityIDs = new List<int> ();

	}

	[System.Serializable]
	public class CharacterClass
	{
		public int id;
		public string name;
		
		public int healthMin;
		public int healthMax;
		
		public int strengthMin;
		public int strengthMax;
		
		public int defenseMin;
		public int defenseMax;
		
		public int speedMin;
		public int speedMax;
		
		public int intelligenceMin;
		public int intelligenceMax;
		
		public int charismaMin;
		public int charismaMax;
	}

	[System.Serializable]
	public class Ability
	{
		public int id;
		public string name;
		public string functionName;

		public AbilityType abilityType;

		public int numSides;
		public int numDice;
	}

	public enum AbilityType
	{
		Melee
		,Ranged
		,Magic
	}
	#endregion classes

	#region variables
	public GameObject instantiator;
	public string saveGame;
	public string currentScene;

	public List<string> possibleBattleScenes = new List<string>();

	public List<CharacterClass> characterClasses = new List<CharacterClass> ();
	public List<Ability> abilities = new List<Ability> ();
	
	public GameObject pcGo;

	#endregion variables

	#region components
	public GameObject localGameManager;
	#endregion components

	void Start()
	{
		DontDestroyOnLoad (this);
	}

	public Vector3 currentPClocation()
	{
		return new Vector3 (Game.current.player.pos_x, Game.current.player.pos_y, Game.current.player.pos_z);
	}

	void OnLevelWasLoaded()
	{
		setupLocalGameManager ();
		setupPCGo ();
	}

	public void setupLocalGameManager()
	{
		localGameManager = GameObject.Find ("LocalGameManager");
		localGameManager.SendMessage ("GlobalGameManagerDoneLoading",this.GetComponent<GameManager>());
	}

	public void setupPCGo()
	{
		pcGo = GameObject.FindGameObjectWithTag ("Player");

		if (pcGo) 
		{
			PlayerController pcGoPlayerController = pcGo.GetComponent<PlayerController> ();

			if(GameManager.Game.current.player.useCoordinates)
			{
				pcGo.GetComponent<RectTransform> ().localPosition = new Vector3(GameManager.Game.current.player.pos_x, GameManager.Game.current.player.pos_y, GameManager.Game.current.player.pos_z);
			}
			else
			{
				pcGo.GetComponent<RectTransform> ().localPosition = GameObject.Find(GameManager.Game.current.player.spawnpoint).transform.position;
			}

			pcGoPlayerController.spriteName = GameManager.Game.current.player.characterSprite;
			pcGoPlayerController.setupSprites ();
		}
	}

	public void assignCharacterStats(List<Character> cs)
	{
		foreach (Character character in cs) 
		{
			assignCharacterClassStats(character,character.characterClassID);
		}
	}

	public void assignCharacterClassStats(Character c, int characterClassID)
	{
		/*
		CharacterClass cc = characterClasses [characterClassID];
		c.characterClass = cc;

		c.totalHealth = cc.healthMax;
		c.currentHealth = cc.healthMax;

		for (int i = 0; i < c.abilities.Count; i++) 
		{
			int abilityID = c.abilities[i].id;
			Ability a = abilities [abilityID];

			c.abilities[i] = a;
		}
		*/
	}
}

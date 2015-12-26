using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	#region classes
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
		public List<Character> party = new List<Character>();
		public string scene;
		public Vector3 pos;
	}

	[System.Serializable]
	public class Character
	{
		public int id;
		public string name;
		public int level;
		public int experience;

		public int characterClassID;
		public CharacterClass characterClass;

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

		public List<Ability> abilities = new List<Ability> ();

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
	public List<Quest> quests = new List<Quest>();
	public string currentScene;
	public Vector3 currentPClocation;
	public List<string> possibleBattleScenes = new List<string>();

	public List<Character> characters = new List<Character>();
	public List<CharacterClass> characterClasses = new List<CharacterClass> ();
	public List<Ability> abilities = new List<Ability> ();
	
	public GameObject pcGo;

	public Player player;
	public Player enemy;

	#endregion variables

	#region components
	public GameObject localGameManager;
	#endregion components

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
			pcGo.GetComponent<RectTransform> ().localPosition = player.pos;
			pcGoPlayerController.spriteName = player.characterSprite;
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
	}
}

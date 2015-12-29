using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class LocalGameManagerBattle : MonoBehaviour 
{
	#region components
	public GameManager gameManager;

	#endregion components

	#region variables
	public List<GameObject> playerSpawnPoints = new List<GameObject>();
	public List<GameObject> enemySpawnPoints = new List<GameObject>();
	public GameObject battleCharacter;
	public Transform sprites_PC;
	public Transform sprites_NPC;

	public Transform pnl_FriendlyStatus;
	public List<GameObject> btn_friendlyStatuses = new List<GameObject> ();
	public Transform pnl_EnemyStatus;
	public List<GameObject> btn_enemyStatuses = new List<GameObject> ();
	public List<GameObject> btn_AllStatuses = new List<GameObject>();
	public GameObject btn_CharacterStatus;

	public List<GameObject> characterGameObjects = new List<GameObject> ();
	public Dictionary<int,int> battleOrder = new Dictionary<int,int>();

	public Transform pnl_Actions;
	public GameObject btn_Action;

	public GameObject pnl_ActionButtons;
	public GameObject pnl_AbilityButtons;
	public List<GameObject> btn_Abilities = new List<GameObject>();
	
	public bool battleStarted = false;
	public int currentTurnIndex;
	public NPCTurnActionTypes currentNPCTurnActionType = NPCTurnActionTypes.SetupTurn;
	public float timeStartedAction;
	public enum NPCTurnActionTypes
	{
		SetupTurn
		,StartPickAction
		,PickAction
		,StartPickAbility
		,StartDoAbility
		,DoAbility
		,EndTurn
	}

	public PlayerTurnActionTypes currentPlayerTurnActionType = PlayerTurnActionTypes.SetupTurn;
	public enum PlayerTurnActionTypes
	{
		SetupTurn
		,ChooseAction
		,ChooseAbilityTarget
		,EndTurn
	}
	public GameManager.Character currentTurnCharacter()
	{
		return GameManager.Game.current.characters[battleOrder.Keys.ElementAt(currentTurnIndex)];
	}
	#endregion variables

	void GlobalGameManagerDoneLoading(GameManager gm)
	{
		gameManager = gm;
		battleOrder = getBattleOrder ();
		instantiateCharacters ();

		battleStarted = true;
	}

	void Update()
	{
		if (battleStarted)
		{
			if(!isStillCharacters(GameManager.Game.current.player.partyIDs))
			{
				Debug.Log ("Player party all dead!");
				endBattle(false);
			}
			else
			{
				if(!isStillCharacters(GameManager.Game.current.enemy.partyIDs))
				{
					Debug.Log ("Enemy party all dead!");
					endBattle(true);
				}
				else
				{
					if(GameManager.Game.current.player.partyIDs.Contains (currentTurnCharacter ().id))
					{
						playerTakeTurn();
					}
					else
					{
						npcTakeTurn ();
					}
				}
			}
		}
	}

	void endBattle(bool playerWon)
	{
		if (!playerWon) 
		{
			//charactersWinBattle(gameManager.enemy.party);
			Application.LoadLevel ("Main Menu");
		}
		else 
		{
			charactersWinBattle(GameManager.Game.current.player.partyIDs);
			actionRunAway();
		}
	}

	void charactersWinBattle(List<int> characterIDs)
	{
		int xp = Random.Range (1,100);

		foreach (int characterID in characterIDs) 
		{
			GameManager.Game.current.characters[characterID].experience += xp;
		}
	}

	bool isStillCharacters(List<int> characterIDs)
	{
		bool b = false;

		foreach (int characterID in characterIDs) 
		{
			if(GameManager.Game.current.characters[characterID].currentHealth > 0)
			{
				return true;
			}
		}

		return b;
	}

	void playerTakeTurn()
	{
		switch (currentPlayerTurnActionType) 
		{
		case PlayerTurnActionTypes.SetupTurn:
		{
			resetUI();
			currentPlayerTurnActionType = PlayerTurnActionTypes.ChooseAction;
			break;
		}
		case PlayerTurnActionTypes.ChooseAction:
		{

			break;
		}
		case PlayerTurnActionTypes.ChooseAbilityTarget:
		{

			break;
		}
		case PlayerTurnActionTypes.EndTurn:
		{
			incrementTurn();
			break;
		}
		default:
		{
			break;
		}
		}
	}

	void npcTakeTurn()
	{
		switch (currentNPCTurnActionType) 
		{
		case NPCTurnActionTypes.SetupTurn:
		{
			hideUI();
			currentNPCTurnActionType = NPCTurnActionTypes.StartDoAbility;
			break;
		}
		case NPCTurnActionTypes.StartDoAbility:
		{
			timeStartedAction = Time.time;
			currentNPCTurnActionType = NPCTurnActionTypes.DoAbility;
			break;
		}
		case NPCTurnActionTypes.DoAbility:
		{
			npcDoAbility (2.5f);
			break;
		}
		case NPCTurnActionTypes.EndTurn:
		{
			incrementTurn();
			break;
		}
		default:
		{
			break;
		}
		}
	}

	void npcDoAbility(float timeToWait)
	{
		if (Time.time - timeToWait > timeStartedAction) 
		{
			int abilityID = currentTurnCharacter().abilityIDs[Random.Range (0,currentTurnCharacter().abilityIDs.Count)];
			NPCDoAbility (gameManager.abilities[abilityID]);

			Debug.Log ("NPC done doing ability");
			currentNPCTurnActionType = NPCTurnActionTypes.EndTurn;
		} 
		else 
		{
			//Debug.Log ("NPC doing ability");
		}
	}

	Dictionary<int,int> getBattleOrder()
	{
		Dictionary<int,int> tempBattleOrder = new Dictionary<int,int> ();
		List<int> tempCharacterIDs = new List<int> ();

		tempCharacterIDs.AddRange (GameManager.Game.current.player.partyIDs);
		tempCharacterIDs.AddRange (GameManager.Game.current.enemy.partyIDs);

		foreach (int characterID in tempCharacterIDs)
		{
			int roll = Random.Range (1,21);
			int adjustedRoll = roll + GameManager.Game.current.characters[characterID].totalSpeed;

			do
			{
				if(tempBattleOrder.ContainsValue (adjustedRoll))
				{
					Debug.Log ("rerolling");
					roll = Random.Range (1,21);
					adjustedRoll = roll + GameManager.Game.current.characters[characterID].totalSpeed;
				}
				else
				{
					Debug.Log ("adding " + GameManager.Game.current.characters[characterID].name + " to battle order");
					tempBattleOrder.Add (characterID,adjustedRoll);
				}
			}
			while(!tempBattleOrder.ContainsKey(characterID));
		}

		tempBattleOrder = sortBattleOrder (tempBattleOrder);

		return tempBattleOrder;
	}

	Dictionary<int,int> sortBattleOrder(Dictionary<int,int> d)
	{
		Dictionary<int,int> sorted = new Dictionary<int,int>();

		foreach (KeyValuePair<int,int> item in d.OrderBy(i => i.Value))
		{
			sorted.Add (item.Key,item.Value);
		}

		return sorted;
	}

	void instantiateCharacters()
	{
		drawCharacters (GameManager.Game.current.player.partyIDs, playerSpawnPoints, pnl_FriendlyStatus, btn_friendlyStatuses, PlayerController.Direction.East);
		drawCharacters (GameManager.Game.current.enemy.partyIDs, enemySpawnPoints, pnl_EnemyStatus,btn_enemyStatuses, PlayerController.Direction.West);
	}

	void updateCharacters()
	{
		foreach (GameObject go in characterGameObjects) 
		{
			Destroy (go);
		}

		characterGameObjects.Clear ();
		btn_friendlyStatuses.Clear ();
		btn_enemyStatuses.Clear ();
		btn_AllStatuses.Clear ();

		drawCharacters (GameManager.Game.current.player.partyIDs, playerSpawnPoints, pnl_FriendlyStatus, btn_friendlyStatuses, PlayerController.Direction.East);
		drawCharacters (GameManager.Game.current.enemy.partyIDs, enemySpawnPoints, pnl_EnemyStatus,btn_enemyStatuses, PlayerController.Direction.West);
	}

	void drawCharacters(List<int> partyIDs, List<GameObject> spawnPositions, Transform statusParent, List<GameObject> statusGrouping, PlayerController.Direction dir)
	{
		int posID = 0;
		
		foreach (int characterID in partyIDs) 
		{
			GameManager.Character character = GameManager.Game.current.characters[characterID];
			GameObject newCharacter = (GameObject) Instantiate(battleCharacter);
			newCharacter.transform.SetParent (spawnPositions[posID].transform);
			newCharacter.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
			//newCharacter.GetComponent<CharacterGameObjectStats>().gameManager = gameManager;
			newCharacter.GetComponent<CharacterGameObjectStats>().character = character;

			characterGameObjects.Add (newCharacter);

			GameObject btn = (GameObject) Instantiate (btn_CharacterStatus);
			btn.transform.SetParent (statusParent);
			btn.GetComponent<btn_CharacterStatus>().character = character;
			btn.GetComponent<btn_CharacterStatus>().characterName.text = character.name;
			btn.GetComponent<btn_CharacterStatus>().health.text = character.currentHealth + "/" + character.totalHealth; 
			RectTransform btn_rt = btn.GetComponent<RectTransform>();
			btn_rt.localScale = new Vector2(1,1);

			PlayerController pc = newCharacter.GetComponent<PlayerController>();
			pc.currentDirection = dir;

			statusGrouping.Add (btn);
			btn_AllStatuses.Add (btn);
			characterGameObjects.Add (btn);

            posID++;
		}
	}

	public void resetUI()
	{
		pnl_ActionButtons.SetActive (true);
		pnl_AbilityButtons.SetActive (false);
	}

	public void hideUI()
	{
		pnl_ActionButtons.SetActive (false);
		pnl_AbilityButtons.SetActive (false);
	}
	
	public void enableAbilityButtons(int id)
	{
		pnl_ActionButtons.SetActive (false);
		pnl_AbilityButtons.SetActive (true);

		for (int i = 0; i < btn_Abilities.Count; i++) 
		{
			if(i < GameManager.Game.current.characters[GameManager.Game.current.player.partyIDs[id]].abilityIDs.Count)
			{
				btn_Abilities[i].GetComponent<Button>().onClick.RemoveAllListeners();
				GameManager.Ability a = gameManager.abilities[GameManager.Game.current.characters[GameManager.Game.current.player.partyIDs[id]].abilityIDs[i]];
				btn_Abilities[i].GetComponentInChildren<Text>().text = a.name;
				btn_Abilities[i].GetComponent<Button>().onClick.AddListener(() => setupAbilityTargetButtons(a));
			}
			else
			{
				btn_Abilities[i].SetActive(false);
			}
		}
	}

	public void setupAbilityTargetButtons(GameManager.Ability a)
	{
		foreach (GameObject go in btn_AllStatuses) 
		{
			go.GetComponent<Button>().onClick.RemoveAllListeners();
			btn_CharacterStatus cs = go.GetComponent<btn_CharacterStatus>();
			go.GetComponent<Button>().onClick.AddListener(() => playerDoAbility(cs.character.id,a));
		}

		Debug.Log ("Choose Target");
	}

	public void playerDoAbility(int targetID, GameManager.Ability a)
	{
		doAbility(a, targetID);
		currentPlayerTurnActionType = PlayerTurnActionTypes.EndTurn;
	}

	public void NPCDoAbility(GameManager.Ability a)
	{
		int targetID = findTarget("ENEMY");

		doAbility(a, targetID);
	}

	public void doAbility(GameManager.Ability a, int targetID)
	{
		switch (a.id)
		{
		case 0:
		{
			Debug.Log ("Doing ability '" + a.name + "'");
			
			doDamage (currentTurnCharacter ().id,targetID,a.numDice,a.numSides);
			
			break;
		}
		default:
		{
			Debug.Log ("Ability ID not found.");
			break;
		}
		}
	}

	void incrementTurn()
	{
		currentNPCTurnActionType = NPCTurnActionTypes.SetupTurn;
		currentPlayerTurnActionType = PlayerTurnActionTypes.SetupTurn;

		currentTurnIndex++;
		currentTurnIndex %= battleOrder.Count;
	}

	public int findTarget(string abilityTargetFaction)
	{
		int characterID = 0;

		switch (abilityTargetFaction.ToUpper ()) 
		{
		case "FRIENDLY":
		{
			if (GameManager.Game.current.player.partyIDs.Contains (currentTurnCharacter ().id)) 
			{
				characterID = GameManager.Game.current.player.partyIDs[Random.Range (0,GameManager.Game.current.player.partyIDs.Count)];
			}
			else
			{
				characterID = GameManager.Game.current.enemy.partyIDs[Random.Range (0,GameManager.Game.current.enemy.partyIDs.Count)];
			}
			break;
		}
		case "ENEMY":
		{
			if (GameManager.Game.current.player.partyIDs.Contains (currentTurnCharacter ().id)) 
			{
				characterID = GameManager.Game.current.enemy.partyIDs[Random.Range (0,GameManager.Game.current.enemy.partyIDs.Count)];
			}
			else
			{
				characterID = GameManager.Game.current.player.partyIDs[Random.Range (0,GameManager.Game.current.player.partyIDs.Count)];
			}
			break;
		}
		default:
		{
			break;
		}
		}

		Debug.Log ("Targeting " + GameManager.Game.current.characters[characterID].name);

		return characterID;
	}

	public void actionRunAway()
	{
		//SaveData saveData = GameObject.Find ("Instantiator").GetComponent<SaveData> ();
		//saveData.saveCharacterData (gameManager);

		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + gameManager.saveGame + "/playerdata.xml");
		xml.LoadXml( content );

		XmlNode node = xml.DocumentElement.SelectSingleNode ("/data");

		Application.LoadLevel (node ["scene"].InnerText);
	}

	public void doDamage(int attackerID, int defenderID, int numDice, int numSides)
	{
		List<int> dice = new List<int> ();

		int totalDamage = GameManager.Game.current.characters[attackerID].totalStrength * numDice;

		for(int i = 1; i <= numDice; i++)
		{
			int rollValue = Random.Range (1,numSides+1);

			dice.Add (rollValue);

			totalDamage += rollValue;
		}

		int totalDefense = Random.Range (0, GameManager.Game.current.characters[defenderID].totalDefense);

		totalDamage = Mathf.Max (0,totalDamage - totalDefense);

		Debug.Log (GameManager.Game.current.characters[attackerID].name + " attacks " + GameManager.Game.current.characters[defenderID].name + " for " + totalDamage);

		if (totalDamage < GameManager.Game.current.characters[defenderID].currentHealth) 
		{
			GameManager.Game.current.characters[defenderID].currentHealth -= totalDamage;
		} 
		else
		{
			GameManager.Game.current.characters[defenderID].currentHealth = 0;
			battleOrder.Remove (GameManager.Game.current.characters[defenderID].id);

			int index = currentTurnIndex;
			for(int i = 0; i < battleOrder.Count; i++)
			{
				if(battleOrder.Keys.ElementAt(i) == GameManager.Game.current.characters[attackerID].id)
				{
					index = i;
				}
			}
			currentTurnIndex = index;
		}

		updateCharacters ();
	}
}

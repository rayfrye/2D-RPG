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
	public Dictionary<GameManager.Character,int> battleOrder = new Dictionary<GameManager.Character,int>();

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
		return battleOrder.Keys.ElementAt(currentTurnIndex);
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
			if(!isStillCharacters(gameManager.player.party))
			{
				Debug.Log ("Player party all dead!");
				endBattle(false);
			}
			else
			{
				if(!isStillCharacters(gameManager.enemy.party))
				{
					Debug.Log ("Enemy party all dead!");
					endBattle(true);
				}
				else
				{
					if(gameManager.player.party.Contains (currentTurnCharacter ()))
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
			charactersWinBattle(gameManager.player.party);
			actionRunAway();
		}
	}

	void charactersWinBattle(List<GameManager.Character> characters)
	{
		int xp = Random.Range (1,100);

		foreach (GameManager.Character character in characters) 
		{
			character.experience += xp;
		}


	}

	bool isStillCharacters(List<GameManager.Character> characters)
	{
		bool b = false;

		foreach (GameManager.Character character in characters) 
		{
			if(character.currentHealth > 0)
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
			GameManager.Ability ability = currentTurnCharacter().abilities[Random.Range (0,currentTurnCharacter().abilities.Count)];
			NPCDoAbility (ability);

			Debug.Log ("NPC done doing ability");
			currentNPCTurnActionType = NPCTurnActionTypes.EndTurn;
		} 
		else 
		{
			//Debug.Log ("NPC doing ability");
		}
	}

	Dictionary<GameManager.Character,int> getBattleOrder()
	{
		Dictionary<GameManager.Character,int> tempBattleOrder = new Dictionary<GameManager.Character,int> ();
		List<GameManager.Character> tempCharacters = new List<GameManager.Character> ();

		tempCharacters.AddRange (gameManager.player.party);
		tempCharacters.AddRange (gameManager.enemy.party);

		foreach (GameManager.Character character in tempCharacters)
		{
			int roll = Random.Range (1,21);
			int adjustedRoll = roll + character.totalSpeed;

			do
			{
				if(tempBattleOrder.ContainsValue (adjustedRoll))
				{
					Debug.Log ("rerolling");
					roll = Random.Range (1,21);
					adjustedRoll = roll + character.totalSpeed;
				}
				else
				{
					Debug.Log ("adding " + character.name + " to battle order");
					tempBattleOrder.Add (character,adjustedRoll);
				}
			}
			while(!tempBattleOrder.ContainsKey(character));
		}

		tempBattleOrder = sortBattleOrder (tempBattleOrder);

		return tempBattleOrder;
	}

	Dictionary<GameManager.Character,int> sortBattleOrder(Dictionary<GameManager.Character,int> d)
	{
		Dictionary<GameManager.Character,int> sorted = new Dictionary<GameManager.Character,int>();

		foreach (KeyValuePair<GameManager.Character,int> item in d.OrderBy(i => i.Value))
		{
			sorted.Add (item.Key,item.Value);
		}

		return sorted;
	}

	void instantiateCharacters()
	{
		drawCharacters (gameManager.player.party, playerSpawnPoints, pnl_FriendlyStatus, btn_friendlyStatuses, PlayerController.Direction.East);
		drawCharacters (gameManager.enemy.party, enemySpawnPoints, pnl_EnemyStatus,btn_enemyStatuses, PlayerController.Direction.West);
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

		drawCharacters (gameManager.player.party, playerSpawnPoints, pnl_FriendlyStatus, btn_friendlyStatuses, PlayerController.Direction.East);
		drawCharacters (gameManager.enemy.party, enemySpawnPoints, pnl_EnemyStatus,btn_enemyStatuses, PlayerController.Direction.West);
	}

	void drawCharacters(List<GameManager.Character> party, List<GameObject> spawnPositions, Transform statusParent, List<GameObject> statusGrouping, PlayerController.Direction dir)
	{
		int posID = 0;
		
		foreach (GameManager.Character character in party) 
		{
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
			if(i < gameManager.player.party[id].abilities.Count)
			{
				btn_Abilities[i].GetComponent<Button>().onClick.RemoveAllListeners();
				GameManager.Ability a = gameManager.player.party[id].abilities[i];
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
			go.GetComponent<Button>().onClick.AddListener(() => playerDoAbility(cs.character,a));
		}

		Debug.Log ("Choose Target");
	}

	public void playerDoAbility(GameManager.Character target, GameManager.Ability a)
	{
		doAbility(a, target);
		currentPlayerTurnActionType = PlayerTurnActionTypes.EndTurn;
	}

	public void NPCDoAbility(GameManager.Ability a)
	{
		GameManager.Character target = findTarget("ENEMY");

		doAbility(a, target);
	}

	public void doAbility(GameManager.Ability a, GameManager.Character target)
	{
		switch (a.id)
		{
		case 0:
		{
			Debug.Log ("Doing ability '" + a.name + "'");
			
			doDamage (currentTurnCharacter (),target,a.numDice,a.numSides);
			
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

	public GameManager.Character findTarget(string abilityTargetFaction)
	{
		GameManager.Character character = currentTurnCharacter ();

		switch (abilityTargetFaction.ToUpper ()) 
		{
		case "FRIENDLY":
		{
			if (gameManager.player.party.Contains (currentTurnCharacter ())) 
			{
				character = gameManager.player.party[Random.Range (0,gameManager.player.party.Count)];
			}
			else
			{
				character = gameManager.enemy.party[Random.Range (0,gameManager.enemy.party.Count)];
			}
			break;
		}
		case "ENEMY":
		{
			if (gameManager.player.party.Contains (currentTurnCharacter ())) 
			{
				character = gameManager.enemy.party[Random.Range (0,gameManager.player.party.Count)];
			}
			else
			{
				character = gameManager.player.party[Random.Range (0,gameManager.enemy.party.Count)];
			}
			break;
		}
		default:
		{
			break;
		}
		}

		Debug.Log ("Targeting " + character.name);

		return character;
	}

	public void actionRunAway()
	{
		SaveData saveData = GameObject.Find ("Instantiator").GetComponent<SaveData> ();
		saveData.saveCharacterData (gameManager);

		XmlDocument xml = new XmlDocument ();
		
		string content = System.IO.File.ReadAllText( Application.dataPath + "/Resources/Data/SaveData/" + gameManager.saveGame + "/playerdata.xml");
		xml.LoadXml( content );

		XmlNode node = xml.DocumentElement.SelectSingleNode ("/data");

		Application.LoadLevel (node ["scene"].InnerText);
	}

	public void doDamage(GameManager.Character attacker, GameManager.Character defender, int numDice, int numSides)
	{
		List<int> dice = new List<int> ();

		int totalDamage = attacker.totalStrength * numDice;

		for(int i = 1; i <= numDice; i++)
		{
			int rollValue = Random.Range (1,numSides+1);

			dice.Add (rollValue);

			totalDamage += rollValue;
		}

		int totalDefense = Random.Range (0, defender.totalDefense);

		totalDamage = Mathf.Max (0,totalDamage - totalDefense);

		Debug.Log (attacker.name + " attacks " + defender.name + " for " + totalDamage);

		if (totalDamage < defender.currentHealth) 
		{
			defender.currentHealth -= totalDamage;
		} 
		else
		{
			defender.currentHealth = 0;
			battleOrder.Remove (defender);

			int index = currentTurnIndex;
			for(int i = 0; i < battleOrder.Count; i++)
			{
				if(battleOrder.Keys.ElementAt(i) == attacker)
				{
					index = i;
				}
			}
			currentTurnIndex = index;
		}

		updateCharacters ();
	}
}

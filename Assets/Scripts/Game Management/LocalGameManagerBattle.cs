using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
	public Transform pnl_EnemyStatus;
	public GameObject btn_CharacterStatus;

	public List<GameObject> characterGameObjects = new List<GameObject> ();

	public Transform pnl_Actions;
	public GameObject btn_Action;

	public GameObject pnl_ActionButtons;
	public GameObject pnl_AbilityButtons;
	public List<GameObject> btn_Abilities = new List<GameObject>();
	
	#endregion variables

	void GlobalGameManagerDoneLoading(GameManager gm)
	{
		gameManager = gm;
		instantiateCharacters ();
	}

	void instantiateCharacters()
	{

		drawCharacters (gameManager.player.party, playerSpawnPoints, pnl_FriendlyStatus);
		drawCharacters (gameManager.enemy.party, enemySpawnPoints, pnl_EnemyStatus);
	}

	void updateCharacters()
	{
		foreach (GameObject go in characterGameObjects) 
		{
			Destroy (go);

		}

		characterGameObjects.Clear ();

		drawCharacters (gameManager.player.party, playerSpawnPoints, pnl_FriendlyStatus);
		drawCharacters (gameManager.enemy.party, enemySpawnPoints, pnl_EnemyStatus);
	}

	void drawCharacters(List<GameManager.Character> party, List<GameObject> spawnPositions, Transform statusParent)
	{
		int posID = 0;
		
		foreach (GameManager.Character character in party) 
		{
			GameObject newCharacter = (GameObject) Instantiate(battleCharacter);
			newCharacter.transform.SetParent (spawnPositions[posID].transform);
			newCharacter.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
			newCharacter.GetComponent<CharacterGameObjectStats>().gameManager = gameManager;
			newCharacter.GetComponent<CharacterGameObjectStats>().character = character;

			characterGameObjects.Add (newCharacter);
			
			GameObject btn = (GameObject) Instantiate (btn_CharacterStatus);
			btn.transform.SetParent (statusParent);
			btn.GetComponent<btn_CharacterStatus>().characterName.GetComponent<Text>().text = newCharacter.GetComponent<CharacterGameObjectStats>().character.name;
			btn.GetComponent<btn_CharacterStatus>().health.GetComponent<Text>().text = newCharacter.GetComponent<CharacterGameObjectStats>().character.currentHealth + "/" + newCharacter.GetComponent<CharacterGameObjectStats>().character.totalHealth;
			RectTransform btn_rt = btn.GetComponent<RectTransform>();
			btn_rt.localScale = new Vector2(1,1);

			characterGameObjects.Add (btn);

            posID++;
		}
	}

	

	public void enableAbilityButtons(int id)
	{
		pnl_ActionButtons.SetActive (false);
		pnl_AbilityButtons.SetActive (true);

		for (int i = 0; i < btn_Abilities.Count; i++) 
		{
			if(i < gameManager.player.party[id].abilities.Count)
			{
				GameManager.Ability a = gameManager.player.party[id].abilities[i];
				btn_Abilities[i].GetComponentInChildren<Text>().text = a.name;
				btn_Abilities[i].GetComponent<Button>().onClick.AddListener(() => doAbility(a.id));

			}
			else
			{
				btn_Abilities[i].SetActive(false);
			}
		}
	}

	public void doAbility(int id)
	{
		GameManager.Ability a = gameManager.abilities [id];

		switch (id) 
		{
		case 0:
		{
			Debug.Log ("Doing ability '" + a.name + "'");

			doDamage (gameManager.player.party[0],gameManager.enemy.party[0],a.numDice,a.numSides);

			break;
		}
		default:
		{
			Debug.Log ("Ability ID not found.");
			break;
		}
		}
	}

	public void doDamage(GameManager.Character attacker, GameManager.Character defender, int numDice, int numSides)
	{
		List<int> dice = new List<int> ();
		//string log = "";
		int totalDamage = attacker.totalStrength * numDice;

		for(int i = 1; i <= numDice; i++)
		{
			int rollValue = Random.Range (1,numSides+1);

			dice.Add (rollValue);

			totalDamage += rollValue;

			//log += "roll " + i + ": " + rollValue + "\n";
		}

		int totalDefense = Random.Range (0, defender.totalDefense);

		totalDamage = Mathf.Max (0,totalDamage - totalDefense);

		print (totalDamage);

		defender.currentHealth -= totalDamage;

		updateCharacters ();

	}
}

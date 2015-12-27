using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Dialogue : MonoBehaviour 
{
	#region classes
	[System.Serializable]
	public class DialogueItem
	{
		public int id;
		public string text;
		public List<DialogueResponse> dialogueResponses = new List<DialogueResponse> ();
		public List<int> questPrerequisiteIDs = new List<int> ();
	}

	[System.Serializable]
	public class DialogueResponse
	{
		public string text;
		public List<DialogueResponseAction> dialogueResponseActions = new List<DialogueResponseAction>();
	}

	[System.Serializable]
	public class DialogueResponseAction
	{
		public DialogueResponseActionTypes dialogueResponseActionType;
		public int dialogueResponseActionValue;

	}

	public enum DialogueResponseActionTypes
	{
		EndConversation
		,GoToDialogueItem
		,StartBattle
		,CompleteQuest
	}
	#endregion classes

	#region variables
	public LocalGameManager localGameManager;
	public GameManager gameManager;
	public List<DialogueItem> dialogueItems = new List<DialogueItem>();
	public DialogueCanvas dialogueCanvas;

	public int currentDialogueItemID;
	#endregion variables

	public void setupDialogue()
	{
		localGameManager.isNotPaused = 0;

		setDirection ();

		clearBtnResponses ();

		DialogueItem d = dialogueItems [currentDialogueItemID];

		if (currentDialogueItemID == 0) 
		{
			d = dialogueItems[getCurrentDialogueItem ()];
		}

		dialogueCanvas.canvasDialogue.SetActive (true);

		dialogueCanvas.txt_Dialogue.text = "";

		CharacterGameObjectStats charGoStats = transform.GetComponent<CharacterGameObjectStats> ();

		if (charGoStats != null) 
		{
			dialogueCanvas.txt_Dialogue.text = charGoStats.character.name + ":\n";
		}

		dialogueCanvas.txt_Dialogue.text += replaceSpecialText (d.text);

		if (d.dialogueResponses.Count > 0) 
		{
			dialogueCanvas.pnl_Responses.SetActive (true);

			foreach (DialogueResponse dialogueResponse in d.dialogueResponses)
			{
				GameObject btn = (GameObject) Instantiate (dialogueCanvas.btn_Response);
				btn.transform.SetParent(dialogueCanvas.pnl_Responses.transform);

				RectTransform rt = btn.GetComponent<RectTransform>();
				rt.localScale = new Vector2(1,1);

				btn.GetComponentInChildren<Text>().text = dialogueResponse.text;

				foreach(DialogueResponseAction action in dialogueResponse.dialogueResponseActions)
				{
					addDialogueAction(btn,action);
				}
			}
		}
	}

	void setDirection()
	{
		PlayerController.Direction playerDir = localGameManager.gameManager.pcGo.transform.GetComponent<PlayerController> ().currentDirection;
		PlayerController thisController = transform.GetComponent<PlayerController> ();

		if (thisController != null) 
		{
			switch (playerDir) 
			{
			case PlayerController.Direction.East:
			{
				thisController.currentDirection = PlayerController.Direction.West;
				break;
			}
			case PlayerController.Direction.West:
			{
				thisController.currentDirection = PlayerController.Direction.East;
				break;
			}
			case PlayerController.Direction.North:
			{
				thisController.currentDirection = PlayerController.Direction.South;
				break;
			}
			case PlayerController.Direction.South:
			{
				thisController.currentDirection = PlayerController.Direction.North;
				break;
			}
			default:
			{
				thisController.currentDirection = PlayerController.Direction.South;
				break;
			}
			}
		}
	}

	string replaceSpecialText(string s)
	{
		return s.Replace ("<PlayerName>", localGameManager.gameManager.saveGame);
	}

	public void clearBtnResponses()
	{
		foreach (Transform t in dialogueCanvas.pnl_Responses.transform)
		{
			Destroy (t.gameObject);
		}
	}

	public void addDialogueAction(GameObject btn, DialogueResponseAction action)
	{
		switch (action.dialogueResponseActionType) 
		{
		case DialogueResponseActionTypes.EndConversation:
		{
			btn.GetComponent<Button>().onClick.AddListener(() => endConversation());
			break;
		}
		case DialogueResponseActionTypes.GoToDialogueItem:
		{
			btn.GetComponent<Button>().onClick.AddListener(() => goToDialogueItem(action.dialogueResponseActionValue));
			break;
		}
		case DialogueResponseActionTypes.StartBattle:
		{
			btn.GetComponent<Button>().onClick.AddListener(() => startBattle());
			break;
		}
		case DialogueResponseActionTypes.CompleteQuest:
		{
			btn.GetComponent<Button>().onClick.AddListener(() => completeQuest (action.dialogueResponseActionValue));
			break;
		}
		default:
		{
			break;
		}
		}
	}

	int getCurrentDialogueItem()
	{
		int id = 0;

		foreach (DialogueItem dialogueItem in dialogueItems) 
		{
			bool prereqsComplete = false;

			if(dialogueItem.questPrerequisiteIDs.Count > 0)
			{
				foreach(int questPrerequisiteID in dialogueItem.questPrerequisiteIDs)
				{
					print (questPrerequisiteID);
					if(localGameManager.gameManager.quests[questPrerequisiteID].isComplete)
					{
						prereqsComplete = true;
					}
				}
			}

			if(prereqsComplete)
			{
				id = dialogueItem.id;
			}
		}
		print (id);
		return id;
	}

	public void endConversation()
	{
		clearBtnResponses ();
		currentDialogueItemID = 0;
		dialogueCanvas.canvasDialogue.SetActive (false);
		dialogueCanvas.pnl_Responses.SetActive (false);
		localGameManager.isNotPaused = 1;
	}

	public void goToDialogueItem(int dialogueID)
	{
		currentDialogueItemID = dialogueID;
		setupDialogue ();
	}

	public void startBattle()
	{
		endConversation ();

		localGameManager.gameManager.enemy.party = battleEnemies (new List<GameManager.Character> ());

		string sceneToLoad = localGameManager.possibleBattleScenes [Random.Range (0, localGameManager.possibleBattleScenes.Count)];
		Debug.Log ("Starting Battle at " + sceneToLoad);
		GameObject instantiator_go = GameObject.Find ("Instantiator");
		SaveData saveData = instantiator_go.GetComponent<SaveData> ();
		Instantiator instantiator = instantiator_go.GetComponent<Instantiator> ();
		Vector3 pos = instantiator.gm.pcGo.GetComponent<RectTransform> ().localPosition;
		saveData.saveData (instantiator.gm,instantiator.gm.player.scene,pos,"","FALSE");
		Application.LoadLevel (sceneToLoad);
	}

	public List<GameManager.Character> battleEnemies(List<GameManager.Character> enemies)
	{
		List<GameManager.Character> battleEnemies = new List<GameManager.Character>();

		if (enemies.Count == 0) 
		{
			List<int> availableEnemyIDs = localGameManager.possibleEnemyIDs;
			int numOfEnemies = Random.Range (1,5);

			for(int i = 0; i < numOfEnemies; i++)
			{
				int enemyIDIndex = Random.Range (0,availableEnemyIDs.Count);
				int enemyID = availableEnemyIDs[enemyIDIndex];
				localGameManager.gameManager.characters[enemyID].currentHealth = localGameManager.gameManager.characters[enemyID].totalHealth;
				battleEnemies.Add (localGameManager.gameManager.characters[enemyID]);
				availableEnemyIDs.Remove (enemyID);
			}
		}
		else 
		{
			battleEnemies = enemies;
		}

		return battleEnemies;
	}

	public void completeQuest(int questID)
	{
		localGameManager.gameManager.quests [questID].isComplete = true;
	}
}

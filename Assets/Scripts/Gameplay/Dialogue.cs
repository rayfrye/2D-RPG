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
	}
	#endregion classes

	#region variables
	public LocalGameManager localGameManager;
	public GameManager gameManager;
	public List<DialogueItem> dialogueItems = new List<DialogueItem>();
	public GameObject canvasDialogue;
	public Text txt_Dialogue;
	public GameObject pnl_Responses;
	public GameObject btn_Response;

	public int currentDialogueItemID;
	#endregion variables

	public void setupDialogue()
	{
		clearBtnResponses ();

		DialogueItem d = dialogueItems [currentDialogueItemID];

		if (currentDialogueItemID == 0) 
		{
			d = dialogueItems[getCurrentDialogueItem ()];
		}

		canvasDialogue.SetActive (true);
		txt_Dialogue.text = replaceSpecialText (d.text);

		if (d.dialogueResponses.Count > 0) 
		{
			pnl_Responses.SetActive (true);

			foreach (DialogueResponse dialogueResponse in d.dialogueResponses)
			{
				GameObject btn = (GameObject) Instantiate (btn_Response);
				btn.transform.SetParent(pnl_Responses.transform);

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

	string replaceSpecialText(string s)
	{
		return s.Replace ("<PlayerName>", localGameManager.gameManager.player.name);
	}

	public void clearBtnResponses()
	{
		foreach (Transform t in pnl_Responses.transform)
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
			if(dialogueItem.questPrerequisiteIDs.Count > 0)
			{
				bool prereqsComplete = true;

				foreach(int questPrerequisiteID in dialogueItem.questPrerequisiteIDs)
				{
					if(!localGameManager.gameManager.quests[questPrerequisiteID].isComplete)
					{
						prereqsComplete = false;
					}
				}

				if(prereqsComplete)
				{
					id = dialogueItem.id;
				}
			}
		}

		return id;
	}

	public void endConversation()
	{
		clearBtnResponses ();
		currentDialogueItemID = 0;
		canvasDialogue.SetActive (false);
		pnl_Responses.SetActive (false);
	}

	public void goToDialogueItem(int dialogueID)
	{
		currentDialogueItemID = dialogueID;
		setupDialogue ();
	}

	public void startBattle()
	{
		endConversation ();
		string sceneToLoad = gameManager.possibleBattleScenes [Random.Range (0, gameManager.possibleBattleScenes.Count)];
		Debug.Log ("Starting Battle at " + sceneToLoad);

		Application.LoadLevel (sceneToLoad);
	}
}

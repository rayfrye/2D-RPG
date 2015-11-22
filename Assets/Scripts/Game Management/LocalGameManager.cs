using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalGameManager : MonoBehaviour 
{
	#region components
	public GameManager gameManager;
	#endregion components
	
	#region variables
	public List<string> possibleBattleScenes = new List<string>();
	#endregion variables

	void GlobalGameManagerDoneLoading(GameManager gm)
	{
		gameManager = gm;

		gameManager.setupScene ();
		addBattleScenes ();
	}
	
	void addBattleScenes()
	{
		gameManager.possibleBattleScenes.Clear ();
		gameManager.possibleBattleScenes = possibleBattleScenes;
	}
}

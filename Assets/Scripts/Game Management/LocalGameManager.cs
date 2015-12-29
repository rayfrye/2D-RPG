using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LocalGameManager : MonoBehaviour 
{
	#region components
	public GameManager gameManager;
	#endregion components
	
	#region variables
	public List<string> possibleBattleScenes = new List<string>();
	public List<int> possibleEnemyIDs = new List<int>();
	public float time;
	public float timeScale = 1;
	public float isNotPaused = 1;
	#endregion variables

	void GlobalGameManagerDoneLoading(GameManager gm)
	{
		gameManager = gm;
		addBattleScenes ();

		//int startIndex = EditorApplication.currentScene.LastIndexOf ("/") + 1;
		//string sceneName = EditorApplication.currentScene.Substring (startIndex, EditorApplication.currentScene.Length - startIndex);

		string sceneName = Application.loadedLevelName;
		//sceneName = sceneName.Substring (0, sceneName.IndexOf ("."));

		GameObject sceneManager = GameObject.Find (GameManager.Game.current.player.scene);

		if (sceneManager)
		{
			sceneManager.SendMessage ("localGameManagerDoneLoading", gm);
		}
	}
	
	void addBattleScenes()
	{
		gameManager.possibleBattleScenes.Clear ();
		gameManager.possibleBattleScenes = possibleBattleScenes;
	}

	void Update()
	{
		keepTime ();
	}

	void keepTime()
	{
		time += Time.deltaTime * timeScale * isNotPaused;
	}
}

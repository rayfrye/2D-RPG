using UnityEngine;
using System.Collections;

public class Instantiator : MonoBehaviour 
{
	public GameObject GlobalGameManager;


	void Start()
	{
		if (GameObject.Find ("GlobalGameManager")) 
		{
			Debug.Log ("GlobalGameManager already exists, not instantiating a new one, that would be silly.");
		} 
		else 
		{
			GameObject go = (GameObject) Instantiate (GlobalGameManager);
			go.name = "GlobalGameManager";
		}
	}


	public GameManager setupGlobalGameManager(string saveGame)
	{
		GameManager gm = new GameManager();

		return gm;
	}
}

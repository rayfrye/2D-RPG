using UnityEngine;
using System.Collections;

public class Gwain : MonoBehaviour 
{
	public GameManager gameManager;

	public GameObject Guard1;
	public GameObject Guard2;

	void localGameManagerDoneLoading(GameManager gm)
	{
		gameManager = gm;
		//moveGuards ();
	}

	void moveGuards()
	{
		if (gameManager.quests [0].isComplete) 
		{
			Guard1.GetComponent<RectTransform>().localPosition = new Vector3(16,16,0);
			Guard2.GetComponent<RectTransform>().localPosition = new Vector3(19,16,0);
		}
	}
}

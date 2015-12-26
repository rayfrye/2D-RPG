using UnityEngine;
using System.Collections;

public class PlayerCastRay : MonoBehaviour 
{
	#region components
	public PlayerController playerController;
	#endregion components

	#region variables
	public float rayDistance = 1.5f;
	#endregion variables

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			castDialogueRay ();
		}
	}

	void castDialogueRay()
	{
		dialogueHits (getHits(.75f));
	}

	RaycastHit[] getHits(float distance)
	{
		Vector2 forward = dir ();
		Debug.DrawRay (transform.position, forward, Color.red);
		RaycastHit[] hits;
		Ray ray = new Ray (transform.position, forward);
		
		hits = Physics.RaycastAll (ray,distance);

		return hits;
	}

	Vector2 dir()
	{
		Vector2 dir = new Vector2 (rayDistance,0);

		switch (playerController.currentDirection) 
		{
		case PlayerController.Direction.North:
		{
			dir = new Vector2(0,rayDistance);
			break;
		}
		case PlayerController.Direction.South:
		{
			dir = new Vector2(0,-rayDistance);
			break;
		}
		case PlayerController.Direction.East:
		{
			dir = new Vector2(rayDistance,0);
			break;
		}
		case PlayerController.Direction.West:
		{
			dir = new Vector2(-rayDistance,0);
			break;
		}
		default:
		{
			Debug.Log ("Direction not defined for PC.");
			break;
		}
		}

		return dir;
	}

	void dialogueHits(RaycastHit[] hits)
	{
		foreach (RaycastHit hit in hits) 
		{
			Dialogue d = hit.transform.GetComponent<Dialogue>();

			if(d)
			{
				d.setupDialogue();
			}
			else
			{
				print ("No");
			}
		}
	}
}

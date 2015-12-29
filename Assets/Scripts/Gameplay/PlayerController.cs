using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NPCAction
{
	public float timeToNextAction;
	public ActionType actionType;

	public PlayerController.Direction turnDirection;
	public Transform moveDestinationTransform;
	public string action;


	public enum ActionType
	{
		Turn
		,Move
		,Action
	}
}

public class PlayerController : MonoBehaviour 
{
	#region variables
	public bool isPlayer = false;

	public bool moveEnabled = false;

	public float speed = 6.0F;
	public float gravity = 20.0F;
	
	public CharacterController controller;

	public string spriteName;

	public Direction currentDirection;

	public enum Direction
	{
		North
		,South
		,West
		,East
	}

	public int orderInLayer;
	#endregion variables

	#region NPCScheduling
	public List<NPCAction> normalActions = new List<NPCAction> ();
	public int normalActionID = 0;
	public float normalActionLastActionTime = 0;
	public List<NPCAction> specialActions = new List<NPCAction> ();
	#endregion NPCScheduling

	#region animations
	public List<string> animationNames;
	public List<string> spriteTypes;

	public GameObject walking_s;
	public GameObject idle_s;
	public GameObject attack_s;

	public GameObject walking_w;
	public GameObject idle_w;

	public GameObject walking_n;
	public GameObject idle_n;

	public GameObject walking_e;
	public GameObject idle_e;
	#endregion animations

	#region components
	public LocalGameManager localGameManager;
	#endregion components

	void Start()
	{
		localGameManager = GameObject.Find ("LocalGameManager").GetComponent<LocalGameManager> ();
		setupSprites ();
	}

	public void setupSprites()
	{
		for (int i = 0; i < animationNames.Count; i++)
		{
			GameObject spriteContainer = new GameObject ();
			spriteContainer.transform.SetParent (transform);
			spriteContainer.name = animationNames[i];
			spriteContainer.SetActive (false);

			RectTransform rectTransform = spriteContainer.AddComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(0,0,0);
			rectTransform.sizeDelta = new Vector2(0,0);

			Animation2DParent animation2DParent = spriteContainer.AddComponent<Animation2DParent>();

			for(int n = 0; n < spriteTypes.Count; n++)
			{
				GameObject sprite = new GameObject();
				sprite.transform.SetParent(spriteContainer.transform);
				sprite.name = spriteTypes[n];
				SpriteRenderer spriteRenderer = sprite.AddComponent<SpriteRenderer>();
				spriteRenderer.sortingOrder = orderInLayer;

				RectTransform spriteRectTransform = sprite.AddComponent<RectTransform>();
				spriteRectTransform.localPosition = new Vector3(0,0,0);
				spriteRectTransform.sizeDelta = new Vector2(0,0);

				Animation2D animation2D = sprite.AddComponent<Animation2D>();
				assignAnimation2DParentValues(spriteTypes[n],animation2DParent, animation2D, spriteContainer);

				animation2D.FPS = 5;
				animation2D.isLooping = true;
				animation2D.playOnStartup = true;

				if(spriteContainer.name.EndsWith("w"))
				{
					animation2D.flipSprite = true;
				}
			}
		}

		Sprite[] sprites = Resources.LoadAll <Sprite> ("Data/GameData/Sprites/Characters/" + spriteName);

		try
		{
			//North
			idle_n.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add(sprites [5]);

			walking_n.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [6]);
			walking_n.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [7]);

			//East
			idle_e.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [3]);

			walking_e.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [3]);
			walking_e.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add(sprites [4]);

			//South
			idle_s.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [0]);

			walking_s.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [1]);
			walking_s.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [2]);

			attack_s.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [8]);
			attack_s.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [9]);
			attack_s.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [10]);

			//West
			idle_w.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [3]);

			walking_w.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [3]);
			walking_w.GetComponent<Animation2DParent> ().body.GetComponent<Animation2D> ().frames.Add (sprites [4]);
		}
		catch
		{
			//Debug.Log ("Animations Not Assigned");
		}
	}

	void assignAnimation2DParentValues(string spriteType, Animation2DParent animation2DParent, Animation2D animation2D, GameObject spriteContainer)
	{
		switch(spriteType)
		{
		case "body":
		{
			animation2DParent.body = animation2D;
			assignAnimationGameObject(spriteContainer,animation2D);
			break;
		}
		default:
		{
			break;
		}
		}
	}

	void assignAnimationGameObject(GameObject spriteContainer, Animation2D animation2D)
	{
		switch (spriteContainer.name)
		{
		case "walking_s":
		{
			walking_s = spriteContainer;
			break;
		}
		case "idle_s":
		{
			idle_s = spriteContainer;
			break;
		}
		case "attack_s":
		{
			attack_s = spriteContainer;
			break;
		}
		case "walking_e":
		{
			walking_e = spriteContainer;
			break;
		}
		case "idle_e":
		{
			idle_e = spriteContainer;
			break;
		}
		case "walking_n":
		{
			walking_n = spriteContainer;
			break;
		}
		case "idle_n":
		{
			idle_n = spriteContainer;
			break;
		}
		case "walking_w":
		{
			walking_w = spriteContainer;
			break;
		}
		case "idle_w":
		{
			idle_w = spriteContainer;
			break;
		}
		default:
		{
			break;
		}
		}
	}
	
	void Update() 
	{
		if (isPlayer)
		{
			findCurrentAnimation (getCurrentAction ());
		}
		else
		{
			if(normalActions.Count > 0)
			{
				doNPCAction(normalActions[normalActionID]);

				if(localGameManager.time > normalActionLastActionTime + normalActions[normalActionID].timeToNextAction)
				{
					normalActionLastActionTime = localGameManager.time;
					normalActionID = (normalActionID + 1) % normalActions.Count;
				}
			}
			else
			{
				findCurrentAnimation("idle");
			}
		}
	}

	void doNPCAction(NPCAction npcAction)
	{
		switch (npcAction.actionType) 
		{
		case NPCAction.ActionType.Action:
		{
			break;
		}
		case NPCAction.ActionType.Move:
		{
			Vector2 distance = npcAction.moveDestinationTransform.GetComponent<RectTransform>().position - transform.GetComponent<RectTransform>().position;

			if(Mathf.Abs (distance.sqrMagnitude) > .05 && localGameManager.isNotPaused == 1)
			{
				findCurrentAnimation("move");
				move (distance.normalized);
			}
			else
			{
				findCurrentAnimation("idle");
			}
			break;
		}
		case NPCAction.ActionType.Turn:
		{
			break;
		}
		default:
		{
			findCurrentAnimation("idle");
			break;
		}
		}
	}

	string getCurrentAction()
	{
		string s = "";

		if (Input.GetKey (KeyCode.Space)) 
		{
			s = "attack";
		}
		else 
		{
			if((Mathf.Abs (Input.GetAxis("Horizontal")) > 0) || (Mathf.Abs (Input.GetAxis("Vertical")) > 0))
			{
				s = "move";
			}
			else
			{
				s = "idle";
			}
		}

		return s;
	}

	void move(Vector2 moveDirection)
	{
		// Use input up and down for direction, multiplied by speed
		//moveDirection = new Vector2(x,y);
		currentDirection = dirFacing (moveDirection);
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed * localGameManager.isNotPaused;

		controller.Move(moveDirection * Time.deltaTime);
	}

	public void turn(Vector2 turnDirection)
	{
		currentDirection = dirFacing (turnDirection);
	}

	public Direction dirFacing(Vector2 moveDirection)
	{
		Direction dir = currentDirection;

		if (Mathf.Abs (moveDirection.x) > 0 || Mathf.Abs (moveDirection.y) > 0) 
		{
			if(Mathf.Abs (moveDirection.x) > Mathf.Abs (moveDirection.y))
			{
				if(moveDirection.x > 0)
				{
					dir = Direction.East;
				}
				else
				{
					dir = Direction.West;
				}
			}
			else
			{
				if(Mathf.Abs (moveDirection.x) == Mathf.Abs (moveDirection.y))
				{
					//Debug.Log ("Diagonal");
				}
				else
				{
					if(moveDirection.y > 0)
					{
						dir = Direction.North;
					}
					else
					{
						dir = Direction.South;

					}
				}
			}
		}

		return dir;
	}

	void findCurrentAnimation(string action)
	{
		switch (action) 
		{
		case "idle":
		{
			switch (currentDirection) 
			{
			case Direction.South:
			{
				setAnimationActive ();
				idle_s.SetActive (true);
				break;
			}
			case Direction.West:
			{
				setAnimationActive ();
				idle_w.SetActive (true);
				break;
			}
			case Direction.North:
			{
				setAnimationActive ();
				idle_n.SetActive (true);
				break;
			}
			case Direction.East:
			{
				setAnimationActive ();
				idle_e.SetActive (true);
				break;
			}
			default:
			{
				break;
			}
			}
			break;
		}
		case "move":
		{
			if (localGameManager.isNotPaused == 1)
			{
				if(isPlayer)
				{
					move (new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")));
				}

				switch (currentDirection)
				{
				case Direction.South:
				{
					setAnimationActive ();
					walking_s.SetActive (true);
					break;
				}
				case Direction.West:
				{
					setAnimationActive ();
					walking_w.SetActive (true);
					break;
				}
				case Direction.North:
				{
					setAnimationActive ();
					walking_n.SetActive (true);
					break;
				}
				case Direction.East:
				{
					setAnimationActive ();
					walking_e.SetActive (true);
					break;
				}
				default:
				{
					break;
				}
				}
			}
			break;
		}
		case "attack":
		{
			switch (currentDirection) 
			{
			case Direction.South:
			{
				setAnimationActive ();

				attack_s.SetActive (true);
				break;
			}
			case Direction.West:
			{
				setAnimationActive ();
				walking_w.SetActive (true);
				break;
			}
			case Direction.North:
			{
				setAnimationActive ();
				walking_n.SetActive (true);
				break;
			}
			case Direction.East:
			{
				setAnimationActive ();
				walking_e.SetActive (true);
				break;
			}
			default:
			{
				break;
			}
			}
			break;
		}
		default:
		{
			break;
		}
		}
	}

	void setAnimationActive()
	{
		try
		{
			walking_s.SetActive (false);
			idle_s.SetActive (false);
			attack_s.SetActive (false);
			
			walking_w.SetActive (false);
			idle_w.SetActive (false);
			
			walking_n.SetActive (false);
			idle_n.SetActive (false);
			
			walking_e.SetActive (false);
			idle_e.SetActive (false);
		}
		catch
		{
			//Debug.Log ("Animations not assigned");
		}
	}
}
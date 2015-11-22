using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{

	#region variables
	public bool moveEnabled;

	public float speed = 6.0F;
	public float gravity = 20.0F;
	
	private Vector3 moveDirection = Vector3.zero;
	public CharacterController controller;

	public Direction currentDirection;

	public enum Direction
	{
		North
		,South
		,West
		,East
	}
	#endregion variables

	#region animations
	public GameObject walking_s;
	public GameObject idle_s;

	public GameObject walking_w;
	public GameObject idle_w;

	public GameObject walking_n;
	public GameObject idle_n;

	public GameObject walking_e;
	public GameObject idle_e;
	#endregion animations
	
	void Update() 
	{
		if (moveEnabled) 
		{
			move ();
		}

		findCurrentAnimation ();
	}

	void move()
	{
		// Use input up and down for direction, multiplied by speed
		moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		currentDirection = dirFacing (moveDirection);
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		
		controller.Move(moveDirection * Time.deltaTime);
	}

	Direction dirFacing(Vector2 moveDirection)
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

	void findCurrentAnimation()
	{
		if (moveDirection.magnitude > 0) 
		{
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
		else 
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
		}
	}

	void setAnimationActive()
	{
		walking_s.SetActive (false);
		idle_s.SetActive (false);
		
		walking_w.SetActive (false);
		idle_w.SetActive (false);
		
		walking_n.SetActive (false);
		idle_n.SetActive (false);
		
		walking_e.SetActive (false);
		idle_e.SetActive (false);
	}
}
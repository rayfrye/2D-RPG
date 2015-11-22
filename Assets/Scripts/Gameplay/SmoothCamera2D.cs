using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour 
{	
	#region variables
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public Camera cam;
	#endregion variables

	void Start()
	{
		followTarget ();
	}
	
	void FixedUpdate () 
	{
		followTarget ();
	}

	void followTarget()
	{
		if (target)
		{
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
			Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}
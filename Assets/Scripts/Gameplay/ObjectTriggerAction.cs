using UnityEngine;
using System.Collections;

public class ObjectTriggerAction : MonoBehaviour 
{
	#region components
	public LocalGameManager localGameManager;
	#endregion components

	#region variables

	public ColliderType colliderType;
	public string colliderValue;

	public enum ColliderType
	{
		door
	}
	#endregion variables

	void OnTriggerEnter(Collider c)
	{
		GameManager gameManager = localGameManager.gameManager;

		switch (colliderType) 
		{
		case ColliderType.door:
		{
			string scene = colliderValue.Substring(0,colliderValue.IndexOf("|"));
			string vs = colliderValue.Substring(colliderValue.IndexOf("|")+1,colliderValue.Length-colliderValue.IndexOf("|")-1);
			float x =  float.Parse (vs.Substring (vs.IndexOf ("x=")+2,vs.IndexOf ("y=")-vs.IndexOf ("x=")-2));
			float y =  float.Parse (vs.Substring (vs.IndexOf ("y=")+2,vs.IndexOf ("z=")-vs.IndexOf ("y=")-2));
			float z =  float.Parse (vs.Substring (vs.IndexOf ("z=")+2,vs.Length - vs.IndexOf ("z=")-2));


			gameManager.currentPClocation = new Vector3(x,y,z);
			gameManager.currentScene = scene;

			Application.LoadLevel(scene);

			Debug.Log ("Going to " + scene);

			break;
		}
		default:
		{
			Debug.Log ("No action created for selected collider type");
			break;
		}
		}
	}
}

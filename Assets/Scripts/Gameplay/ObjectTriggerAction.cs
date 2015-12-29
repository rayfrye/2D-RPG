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
		switch (colliderType) 
		{
		case ColliderType.door:
		{
			string scene = colliderValue.Substring(0,colliderValue.IndexOf("|"));
			string vs = colliderValue.Substring(colliderValue.IndexOf("|")+1,colliderValue.Length-colliderValue.IndexOf("|")-1);

			GameManager.Game.current.player.spawnpoint = vs;
			GameManager.Game.current.player.scene = scene;
						
			SaveData.save ("current_game");
			SaveData.save (GameManager.Game.current.saveName);

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

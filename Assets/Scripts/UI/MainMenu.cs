using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class MainMenu : MonoBehaviour 	
{
	#region GameObjects
	public GameObject pnl_SaveGameNameInputBG;

	#endregion GameObjects

	#region Components
	public GameManager gameManager;

	#endregion Components

	public void startNewGame(string SaveGameName)
	{
		string path = @"Assets/Resources/Data/SaveData/" + SaveGameName + ".xml";

		if (File.Exists (path)) 
		{
			Debug.Log ("File already exists!");
		}
		else 
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(path)) 
			{
				xmlWriter.WriteStartDocument ();
				xmlWriter.WriteElementString ("PlayerName",SaveGameName);
				xmlWriter.WriteEndDocument ();
			}

			gameManager.player.name = SaveGameName;

			pnl_SaveGameNameInputBG.SetActive(false);
		}
	}
}

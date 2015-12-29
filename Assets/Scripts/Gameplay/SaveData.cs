using UnityEngine;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveData : MonoBehaviour 
{
	public static void save(string saveGameName)
	{
		//SaveData.savedGames.Add(GameManager.Game.current);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/" + saveGameName + ".gamedata");
		bf.Serialize(file, GameManager.Game.current);
		file.Close();
	}

	public static void load(string saveGameName)
	{
		if(File.Exists(Application.persistentDataPath + "/" + saveGameName + ".gamedata")) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + saveGameName + ".gamedata", FileMode.Open);
			//SaveData.savedGames = (List<GameManager.Game>)bf.Deserialize(file);
			GameManager.Game.current = (GameManager.Game)bf.Deserialize(file);
			file.Close();
		}
	}
	/*
	public void saveData(GameManager gm, string scene, Vector3 position, string waypointName, string useCoordinates)
	{
		Debug.Log ("Saving " + gm.saveGame);
		savePlayerData (gm, scene, position, waypointName, useCoordinates);
		saveCharacterData (gm);
		saveQuestData (gm);
	}

	void savePlayerData(GameManager gm, string scene, Vector3 position, string waypointName, string useCoordinates)
	{
		string data = "<data>";
		data += "\n\t<name>" + GameManager.Game.current.player.name + "</name>";
		data += "\n\t<characterSprite>" + GameManager.Game.current.player.characterSprite + "</characterSprite>";
		data += "\n\t<scene>" + scene + "</scene>";
		data += "\n\t<waypoint_name>" + waypointName + "</waypoint_name>";
		data += "\n\t<use_coordinates>" + useCoordinates + "</use_coordinates>";
		data += "\n\t<position_x>" + position.x + "</position_x>";
		data += "\n\t<position_y>" + position.y + "</position_y>";
		data += "\n\t<position_z>" + position.z + "</position_z>";

		data += "\n\t<party>";
		foreach (int characterID in GameManager.Game.current.player.partyIDs) 
		{
			data += "\n\t\t<id>" + characterID + "</id>";
		}
		data += "\n\t</party>";

		data += "\n\t<enemyparty>";
		foreach (int characterID in GameManager.Game.current.enemy.partyIDs) 
		{
			data += "\n\t\t<id>" + characterID + "</id>";
		}
		data += "\n\t</enemyparty>";

		data += "\n</data>";

		writeFile (gm.saveGame, "playerdata", data);
	}

	public void saveCharacterData(GameManager gm)
	{
		string data = "<data>";

		foreach (GameManager.Character character in GameManager.Game.current.characters) 
		{
			data += "\n\t<character>";
			data += "\n\t\t<id>" + character.id + "</id>";
			data += "\n\t\t<name>" + character.name + "</name>";
			data += "\n\t\t<level>" + character.level + "</level>";
			data += "\n\t\t<experience>" + character.experience + "</experience>";
			data += "\n\t\t<characterClassID>" + character.characterClassID + "</characterClassID>";
			data += "\n\t\t<currentHealth>" + character.currentHealth + "</currentHealth>";
			data += "\n\t\t<totalHealth>" + character.totalHealth + "</totalHealth>";
			data += "\n\t\t<baseStrength>" + character.baseStrength + "</baseStrength>";
			data += "\n\t\t<totalStrength>" + character.totalStrength + "</totalStrength>";
			data += "\n\t\t<baseDefense>" + character.baseDefense + "</baseDefense>";
			data += "\n\t\t<totalDefense>" + character.totalDefense + "</totalDefense>";
			data += "\n\t\t<baseSpeed>" + character.baseSpeed + "</baseSpeed>";
			data += "\n\t\t<totalSpeed>" + character.totalSpeed + "</totalSpeed>";
			data += "\n\t\t<baseIntelligence>" + character.baseIntelligence + "</baseIntelligence>";
			data += "\n\t\t<totalIntelligence>" + character.totalIntelligence + "</totalIntelligence>";
			data += "\n\t\t<baseCharisma>" + character.baseCharisma + "</baseCharisma>";
			data += "\n\t\t<totalCharisma>" + character.totalCharisma + "</totalCharisma>";
			data += "\n\t\t<abilities>";

			foreach(int abilityID in character.abilityIDs)
			{
				data += "\n\t\t\t<id>" + abilityID + "</id>";
			}

			data += "\n\t\t</abilities>";
			data += "\n\t</character>";
		}

		data += "\n</data>";

		writeFile (gm.saveGame, "characterdata", data);
	}

	void saveQuestData(GameManager gm)
	{
		string data = "<data>";
		
		foreach (GameManager.Quest quest in GameManager.Game.current.quests) 
		{
			data += "\n\t<quest>";
			data += "\n\t\t<id>" + quest.id + "</id>";
			data += "\n\t\t<name>" + quest.name + "</name>";
			data += "\n\t\t<desc>" + quest.desc + "</desc>";
			data += "\n\t\t<isComplete>" + quest.isComplete + "</isComplete>";
			data += "\n\t</quest>";
		}
		
		data += "\n</data>";

		writeFile (gm.saveGame, "questdata", data);


	}

	void writeFile(string saveGame, string fileName, string data)
	{
		string filePath = "SaveData/" + saveGame + "/" + fileName + ".xml";

		var sr = File.CreateText(filePath);
		sr.WriteLine (data);
		sr.Close();
	}
	*/
}

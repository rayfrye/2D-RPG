﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterGameObjectStats : MonoBehaviour 
{
	#region components
	public GameManager gameManager;
	#endregion components

	#region variables
	public int characterID;
	public bool isNPC;
	public GameManager.Character character;
	#endregion variables

	void Start()
	{
		getCharacter ();
	}

	void getCharacter()
	{
		//character = gameManager.characters [characterID];
	}
}
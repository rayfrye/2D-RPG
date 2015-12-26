using UnityEngine;
using System.Collections;

// @NOTE the attached sprite's position should be "top left" or the children will not align properly
// Strech out the image as you need in the sprite render, the following script will auto-correct it when rendered in the game
[RequireComponent (typeof (SpriteRenderer))]

// Generates a nice set of repeated sprites inside a streched sprite renderer
// @NOTE Vertical only, you can easily expand this to horizontal with a little tweaking
public class SpriteTile : MonoBehaviour 
{
	SpriteRenderer sprite;
	
	void Awake ()
	{
		// Get the current sprite with an unscaled size
		sprite = GetComponent<SpriteRenderer>();
		Vector2 spriteSize = new Vector2(sprite.bounds.size.x / transform.localScale.x, sprite.bounds.size.y / transform.localScale.y);
		
		// Generate a child prefab of the sprite renderer
		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		childPrefab.transform.position = transform.position;
		childSprite.sprite = sprite.sprite;
		childSprite.sortingOrder = sprite.sortingOrder;
		
		// Loop through and spit out repeated tiles
		GameObject child;
		for (int y = 0; y < (int)Mathf.Round(sprite.bounds.size.y); y++) 
		{
			for (int x = 0; x < (int)Mathf.Round(sprite.bounds.size.x); x++) 
			{
				child = Instantiate(childPrefab) as GameObject;

				child.transform.position = transform.position - (new Vector3(-spriteSize.x * x, spriteSize.y  * y, 0));


				child.transform.parent = transform;
			}
		}
		
		// Set the parent last on the prefab to prevent transform displacement
		childPrefab.transform.parent = transform;
		
		// Disable the currently existing sprite component since its now a repeated image
		sprite.enabled = false;
	}
}
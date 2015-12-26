using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation2D: MonoBehaviour 
{

	public float FPS;
	public bool isLooping;
	public List<Sprite> frames = new List<Sprite>();
	public bool playOnStartup = false;
	public bool flipSprite = false;
	public SpriteRenderer outputRenderer;
	private float secondsToWait;
	
	private int currentFrame;
	private bool stopped = false;

	public void Start () 
	{
		outputRenderer = this.GetComponent<SpriteRenderer>();

		if (flipSprite)
		{
			this.GetComponent<RectTransform> ().localScale = new Vector3 (-1, 1, 1);
		}

		currentFrame = 0;
		if(FPS > 0) 
		secondsToWait = 1/FPS;
		else 
		secondsToWait = 0f;

		if(playOnStartup) 
		{
			Play(true);
		}
	}

	public void Play(bool reset = false) 
	{
		
		if(reset) 
		{
			currentFrame = 0;
		}
		
		stopped = false;
		outputRenderer.enabled = true;
		
		if(frames.Count > 1) 
		{
			Animate();
		}
		else if(frames.Count > 0) 
		{
			outputRenderer.sprite = frames[0];
		}
	}

	public virtual void Animate() 
	{
		CancelInvoke("Animate");
		if(currentFrame >= frames.Count) 
		{
			if(!isLooping) 
			{
				stopped = true;
			}
			else 
			{
				currentFrame = 0;
			}
		}
		
		outputRenderer.sprite = frames[currentFrame];
		
		if(!stopped) 
		{
			currentFrame++;
		}
		
		if(!stopped && secondsToWait > 0) 
		{
			Invoke("Animate", secondsToWait);
		}
	}
}

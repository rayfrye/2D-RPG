using UnityEngine;
using System.Collections;

public class Animation2D: MonoBehaviour 
{

	public float FPS;
	public bool isLooping;
	public Sprite[] frames;
	public bool playOnStartup = false;
	private SpriteRenderer outputRenderer;
	private float secondsToWait;
	
	private int currentFrame;
	private bool stopped = false;

	public void Awake () 
	{
		outputRenderer = this.GetComponent<SpriteRenderer>();

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
		
		if(frames.Length > 1) 
		{
			Animate();
		}
		else if(frames.Length > 0) 
		{
			outputRenderer.sprite = frames[0];
		}
	}

	public virtual void Animate() 
	{
		CancelInvoke("Animate");
		if(currentFrame >= frames.Length) 
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

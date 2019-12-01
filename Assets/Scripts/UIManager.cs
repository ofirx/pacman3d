using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
	public static UIManager instance;
	public Text scoreText, levelText, lifesText; 

	void Awake()
	{
		instance = this;
	}

	public void UpdateUI()
	{
		scoreText.text = "Score: " + GameManager.instance.ReadScore();
		levelText.text = "Level: " + GameManager.instance.ReadLevel();
		lifesText.text = "Lifes: " + GameManager.instance.ReadLifes();
	}
}

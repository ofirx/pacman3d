using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ReadScore : MonoBehaviour {

	public Text highscoreText, leveltext;


	void Start () 
	{
		highscoreText.text = "Highscore: " + ScoreHolder.score;
		leveltext.text = "Highest Level: " + ScoreHolder.level;
	}
	

}

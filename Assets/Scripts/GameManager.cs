using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	public static GameManager instance;

	private static int lifes = 3;
	private static int score;
	private static int cur_level = 1;
	private int pelletAmount;

	List<GameObject> ghostList = new List<GameObject>();
	GameObject pacmanObj;

	//CHASE TIME
	float cTimer = 20f;
	float curCTimer = 0;

	//SCATTER TIME
	float sTimer = 7f;
	float curSTimer = 0;

	//FRIGHTEND TIMER
	public float fTimer = 8f;
	public float curFTimer = 0f;

	//BOOLS
	bool chase;
	bool scatter;
	public bool frighten;

	static bool hasLost;
	//CountDOWN
	bool hasStartedGame;
	public GameObject countDownImage;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		if (hasLost)
		{
			score = 0;
			lifes = 3;
			cur_level = 1;
			hasLost = false;
		}


		if (score > (cur_level - 1) * 3000)
		{
			//Debug.Log("Lifes optained");
			lifes++;
		}
			
		//Debug.Log("score: " + score + "current level: " + cur_level);
		scatter = true;
		ghostList.AddRange(GameObject.FindGameObjectsWithTag("Ghost"));
		pacmanObj = GameObject.FindGameObjectWithTag("Player");

		UIManager.instance.UpdateUI();

		countDownImage.SetActive(true);

	}

	void Update()
	{
		Timing();
	}

	public void AddPellet()
	{
		pelletAmount++;
	}

	public void ReducePellet(int amount)
	{
		pelletAmount--;
		//score++
		score += amount;
		UIManager.instance.UpdateUI();
		if (pelletAmount <= 0)
		{
			//wincondition
			WinCondition();
		}

		for (int i = 0; i < ghostList.Count; i++)
		{
			Pathfinding pGhost = ghostList[i].GetComponent<Pathfinding>();
			if (score >= pGhost.pointsToCollect && !pGhost.released)
			{
				pGhost.state = Pathfinding.GhostStates.CHASE;
				pGhost.released = true;
			}
		}
	}

	void Timing()
	{
		UpdateStates();
		if (chase)
		{
			curCTimer = curCTimer + Time.deltaTime;
			if (curCTimer >= cTimer)
			{
				curCTimer = 0;
				chase = false;
				scatter = true;
			}
		}
		if (scatter)
		{
			curSTimer = curSTimer + Time.deltaTime;
			if (curSTimer >= sTimer)
			{
				curSTimer = 0;
				chase = true;
				scatter = false;
			}
		}
		if (frighten)
		{
			if (curCTimer != 0 || curSTimer != 0)
			{
				scatter = false;
				chase = false;
				curCTimer = 0;
				curSTimer = 0;
			}

			curFTimer = curFTimer + Time.deltaTime;
			if (curFTimer >= fTimer)
			{
				curFTimer = 0;
				chase = true;
				scatter = false;
				frighten = false;
			}
		}
	}

	void UpdateStates()
	{
		for (int i = 0; i < ghostList.Count; i++)
		{
			Pathfinding pGhost = ghostList[i].GetComponent<Pathfinding>();
			if (pGhost.state == Pathfinding.GhostStates.CHASE && scatter)
			{
				pGhost.state = Pathfinding.GhostStates.SCATTER;
			}
			else if (pGhost.state == Pathfinding.GhostStates.SCATTER && chase)
			{
				pGhost.state = Pathfinding.GhostStates.CHASE;
			}
			else if (frighten && pGhost.state != Pathfinding.GhostStates.HOME && pGhost.state != Pathfinding.GhostStates.GOT_EATEN)
			{
				pGhost.state = Pathfinding.GhostStates.FRIGHTEND;
			}
			else if (pGhost.state == Pathfinding.GhostStates.FRIGHTEND)
			{
				pGhost.state = Pathfinding.GhostStates.CHASE;
			}
		}
	}

	void WinCondition()
	{
		cur_level++;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void LoseLife()
	{
		lifes--;
		UIManager.instance.UpdateUI();

		//Debug.Log("lifes left: " + lifes);
		if (lifes <= 0)
		{
			// game over
			ScoreHolder.level = cur_level;
			ScoreHolder.score = score;

			hasLost = true;
			SceneManager.LoadScene("GameOver");
			return;
		}
		foreach (GameObject ghost in ghostList)
		{
			ghost.GetComponent<Pathfinding>().Reset();
		}
		pacmanObj.GetComponent<PacMan>().Reset();
		//COUNTDOWN
		countDownImage.SetActive(true);
		EndRound();
	}

	public void AddScore(int amount)
	{
		score += amount;
		UIManager.instance.UpdateUI();
	}

	public int ReadScore()
	{
		return score;
	}

	public int ReadLevel()
	{
		return cur_level;
	}

	public int ReadLifes()
	{
		return lifes;
	}

	public void StartRound()
	{
		hasStartedGame = true;
	}

	public void EndRound()
	{
		hasStartedGame = false;
	}

	public bool startCheck()
	{
		return hasStartedGame;
	}
}

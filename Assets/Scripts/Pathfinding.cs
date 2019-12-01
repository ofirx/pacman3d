using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pathfinding : MonoBehaviour {

	public enum Ghosts
	{
		BLINKY,
		CLYDE,
		INKY,
		PINKY
	}
	public Ghosts ghost;

	public Transform blinky;

	//PATHFINDING
	List<Node> path = new List<Node>();
	int D = 10;//HEURISTIC DISTANCE - COST PER STEP
	Node lastVisitedNode;
	public Grid grid;

	//TARGETS
	private Transform currentTarget;
	public Transform pacManTarget;
	public List<Transform> homeTarget = new List<Transform>();
	//public Transform frightendTarget;
	public List<Transform> scatterTarget = new List<Transform>();
	//public Transform[] scatterTarget;
	//public Transform leavingHomeTarget;//<<<<

	//MOVEMENT
	Vector3 nextPos,destination;
	float speed = 3f;

	//DIRECTION
	Vector3 up = Vector3.zero,
	right = new Vector3(0,90,0),
	down = new Vector3(0,180,0),
	left = new Vector3(0,270,0),
	currentDirection = Vector3.zero;

	//STATEMACHINE
	public enum GhostStates
	{
		HOME,
		LEAVING_HOME,
		CHASE,
		SCATTER,
		FRIGHTEND,
		GOT_EATEN,
	}
	public GhostStates state;

	//APPEARENCE
	int activeAppearance;//0 = NORMAL, 1 = FRIGHTENED, 2 = EYES ONLY
	public GameObject[] appearance;

	//RELEASE INFO
	public int pointsToCollect;
	public bool released = false;

	//HOME TIMER
	public float timer=3f;
	public float curTime = 0f;

	//RESET STATE
	Vector3 initPosition;
	GhostStates initState;

//	void Awake()
//	{
//		grid = GetComponent<Grid>();
//	}

	void Start()
	{
		initPosition = transform.position;
		initState = state;

		destination = transform.position;
		currentDirection = up;
		for (int i = 0; i < scatterTarget.Count; i++)
		{
			scatterTarget[i].GetComponent<MeshRenderer>().enabled = false;
		}
		for (int i = 0; i < homeTarget.Count; i++)
		{
			homeTarget[i].GetComponent<MeshRenderer>().enabled = false;
		}
	}

	void Update()
	{
		if(GameManager.instance.startCheck())
		{
			CheckState();
		}

	}

	void FindThePath()
	{
		Node startNode = grid.NodeRequest(transform.position);//current/ghost pos in grid
		Node goalNode = grid.NodeRequest(currentTarget.position);//pacmans pos in grid

		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();

		//add start node
		openList.Add(startNode);
		//keep looping
		while (openList.Count > 0)
		{
			Node currentNode = openList[0];
			for (int i = 1; i < openList.Count; i++)
			{
				if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
				{
					currentNode = openList[1];
				}
			}
			openList.Remove(currentNode);
			closedList.Add(currentNode);
			//goal has been found
			if (currentNode == goalNode)
			{
				//get path before we exit

				PathTracer(startNode,goalNode);
				return;
			}
				
			//CHECK ALL NEIGHBOR NODES - EXCEPT BACKWARDS!!
			foreach (Node neighbour in grid.GetNeighborNodes(currentNode))
			{
				if (!neighbour.walkable || closedList.Contains(neighbour) || neighbour == lastVisitedNode) 
				{
					continue;
				}

				int calcMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (calcMoveCost < neighbour.gCost || !openList.Contains(neighbour))
				{
					neighbour.gCost = calcMoveCost;
					neighbour.hCost = GetDistance(neighbour, goalNode);

					neighbour.parentNode = currentNode;
					if (!openList.Contains(neighbour))
					{
						openList.Add(neighbour);
					}
				}
			}
			lastVisitedNode = null;
		}
	}

	void PathTracer(Node startNode, Node goalNode)
	{
		lastVisitedNode = startNode;
		path.Clear();
		//List<Node> path = new List<Node>();
		Node currentNode = goalNode;
		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parentNode;
		}
		//reverse path to get it sorted right
		path.Reverse();

		//grid.path = path;
		if (ghost == Ghosts.BLINKY)
		{
			grid.blinkyPath = path;
		}
		if (ghost == Ghosts.PINKY)
		{
			grid.pinkyPath = path;
		}
		if (ghost == Ghosts.INKY)
		{
			grid.inkyPath = path;
		}if (ghost == Ghosts.CLYDE)
		{
			grid.clydePath = path;
		}
	}

	int GetDistance(Node a, Node b)
	{
		int distX = Mathf.Abs(a.posX - b.posX);
		int distZ = Mathf.Abs(a.posZ - b.posZ);

		return D * (distX + distZ);
	}

	void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
		if (Vector3.Distance(transform.position, destination) < 0.0001f)
		{
			
			//FIND PATH
			FindThePath();

			if (path.Count > 0)
			{
				//DESTINATION
				nextPos = grid.NextPathPoint(path[0]);
				destination = nextPos;
		
				//ROTATION
				SetDirection();
				transform.localEulerAngles = currentDirection;
			}
		}
	}

	void SetDirection()
	{
		int dirX = (int)(nextPos.x - transform.position.x);
		int dirZ = (int)(nextPos.z - transform.position.z);

		//UP
		if (dirX == 0 && dirZ > 0)
		{
			currentDirection = up;
		}
		//RIGHT
		else if (dirX > 0 && dirZ == 0)
		{
			currentDirection = right;
		}
		//LEFT
		else if (dirX < 0 && dirZ == 0)
		{
			currentDirection = left;
		}
		//DOWN
		else if (dirX == 0 && dirZ < 0)
		{
			currentDirection = down;
		}
	}

	void CheckState()
	{
		switch (state)
		{
			case GhostStates.HOME:
				activeAppearance = 0;
				SetAppearance();
				speed = 1.5f;
				if (!homeTarget.Contains(currentTarget))
				{
					currentTarget = homeTarget[0];
				}
				for (int i = 0; i < homeTarget.Count; i++)
				{
					if (Vector3.Distance(transform.position, homeTarget[i].position) < 0.0001f && currentTarget == homeTarget[i])
					{
						i++;
						if (i >= homeTarget.Count)
						{
							i = 0;
						}
						currentTarget = homeTarget[i];
						continue;
					}
				}

				if (released)
				{
					curTime += Time.deltaTime;
					if (curTime >= timer)
					{
						curTime = 0;
						state = GhostStates.CHASE;
					}
				}

				Move();
				break;
			case GhostStates.LEAVING_HOME:
				activeAppearance = 0;
				SetAppearance();
				break;
			//CHASE PACMAN
			case GhostStates.CHASE:
				activeAppearance = 0;
				SetAppearance();
				speed = 3f;
				if (ghost == Ghosts.CLYDE)
				{
					if (Vector3.Distance(transform.position, pacManTarget.position) <= 8)
					{
						if (!scatterTarget.Contains(currentTarget))
						{
							currentTarget = scatterTarget[0];
						}
						for (int i = 0; i < scatterTarget.Count; i++)
						{
							if (Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f && currentTarget == scatterTarget[i])
							{
								i++;
								if (i >= scatterTarget.Count)
								{
									i = 0;
								}
								currentTarget = scatterTarget[i];
								//continue;
							}
						}
					}
					else
					{
						currentTarget = pacManTarget;
					}
				}

				if (ghost == Ghosts.BLINKY)
				{
					currentTarget = pacManTarget;
				}

				if (ghost == Ghosts.PINKY)
				{
					PinkyBehavior();
				}

				if (ghost == Ghosts.INKY)
				{
					InkyBehavior();
				}

				Move();
				break;
			case GhostStates.SCATTER:
				activeAppearance = 0;
				SetAppearance();
				speed = 3f;

				currentTarget = scatterTarget[0];

				if (!scatterTarget.Contains(currentTarget))
				{
					currentTarget = scatterTarget[0];
				}
				for (int i = 0; i < scatterTarget.Count; i++)
				{
					if (Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f && currentTarget == scatterTarget[i])
					{
						i++;
						if(i>=scatterTarget.Count)
						{
							i = 0;
						}
						currentTarget = scatterTarget[i];
						continue;
					}
				}
				Move();
				break;
			case GhostStates.FRIGHTEND:
				activeAppearance = 1;
				SetAppearance();
				speed = 1.5f;
				if (!scatterTarget.Contains(currentTarget))
				{
					currentTarget = scatterTarget[0];
				}
				for (int i = 0; i < scatterTarget.Count; i++)
				{
					if (Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f && currentTarget == scatterTarget[i])
					{
						i++;
						if(i>=scatterTarget.Count)
						{
							i = 0;
						}
						currentTarget = scatterTarget[i];
						continue;
					}
				}

				Move();
				break;
			case GhostStates.GOT_EATEN:
				activeAppearance = 2;
				SetAppearance();
				speed = 10f;
				currentTarget = homeTarget[0];

				if (Vector3.Distance(transform.position, homeTarget[0].position) < 0.0001f)
				{
					state = GhostStates.HOME;
				}

				Move();
				break;
		}
	}

	void SetAppearance()
	{
		for (int i = 0; i < appearance.Length; i++)
		{
			appearance[i].SetActive(i == activeAppearance);
		}
	}

	void PinkyBehavior()
	{
		Transform aheadTarget = new GameObject().transform;
		int lookAhead = 4;
		//SET A TARGET
		aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
		for (int i = lookAhead; i > 0; i--)
		{
			if (!grid.CheckInsideGrid(aheadTarget.position))
			{
				lookAhead--;
				aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
			}
			else
			{
				break;
			}
		}
		aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;

		Debug.DrawLine(transform.position, aheadTarget.position);
		currentTarget = aheadTarget;

		Destroy(aheadTarget.gameObject);
	}

	void InkyBehavior()
	{
		Transform blinkyToPacman = new GameObject().transform;
		Transform target = new GameObject().transform;
		Transform goal = new GameObject().transform;

		blinkyToPacman.position = new Vector3(pacManTarget.position.x - blinky.position.x, 0, pacManTarget.position.z - blinky.position.z);
		target.position = new Vector3(pacManTarget.position.x + blinkyToPacman.position.x, 0, pacManTarget.position.z + blinkyToPacman.position.z);

		goal.position = grid.GetNearestNonWallNode(target.position);
		currentTarget = goal;

		Debug.DrawLine(transform.position, currentTarget.position);

		Destroy(target.gameObject);
		Destroy(blinkyToPacman.gameObject);
		Destroy(goal.gameObject);
	}

	public void Reset()
	{
		transform.position = initPosition;
		state= initState;

		destination = transform.position;
		currentDirection = up;
	}
}

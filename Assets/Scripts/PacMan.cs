using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

	public float speed = 5f;

	Vector3 up = Vector3.zero,
	right = new Vector3(0,90,0),
	down = new Vector3(0,180,0),
	left = new Vector3(0,270,0),
	currentDirection = Vector3.zero;

	Vector3 nextPos, destination;// direction;

	bool canMove;
	public LayerMask unwalkable;

	//RESET
	Vector3 initPosition;

	//TOUCH / MOUSE MOVEMENT
	Vector2 lastMousePosition;
	Vector2 startPosition;
	Vector2 swipeDelta;
	bool isDragging;
	bool swipeUp,swipeDown,swipeLeft,swipeRight;

	// Use this for initialization
	void Start () 
	{
		initPosition = transform.position;
		Reset();
	}

	public void Reset()
	{
		transform.position = initPosition;
		currentDirection = up;
		nextPos = Vector3.forward;
		destination = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GameManager.instance.startCheck())
		{
			Move();
		}
	}

	void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

		//KEYBOARD INPUT
		if (Input.GetKeyDown(KeyCode.W) || swipeUp)
		{
			nextPos = Vector3.forward;
			currentDirection = up;
		}
		else if (Input.GetKeyDown(KeyCode.S) || swipeDown)
		{
			nextPos = Vector3.back;
			currentDirection = down;
		}
		else if (Input.GetKeyDown(KeyCode.A) || swipeLeft)
		{
			nextPos = Vector3.left;
			currentDirection = left;
		}
		else if (Input.GetKeyDown(KeyCode.D) || swipeRight)
		{
			nextPos = Vector3.right;
			currentDirection = right;
		}

		//MOUSE / TOUCH INPUT
		swipeDown = swipeUp = swipeLeft = swipeRight = false;

		if (Input.GetMouseButtonDown(0))
		{
			startPosition = (Vector2)Input.mousePosition;
			isDragging = true;

		}
		else if(Input.GetMouseButtonUp(0))
		{
			isDragging = false;
		}

		swipeDelta = Vector2.zero;
		if(isDragging)
		{
			if( Input.GetMouseButton(0))
			{
				swipeDelta = (Vector2)Input.mousePosition - startPosition;
			}

			float x = swipeDelta.x;
			float y = swipeDelta.y;

			if (swipeDelta.magnitude > 50)
			{
				if (Mathf.Abs(x) > Mathf.Abs(y))
				{
					//left or right
					if (x < 0)
					{
						swipeLeft = true;
					}
					else
					{
						swipeRight = true;
					}
				}
				else
				{
					//up or down
					if (y < 0)
					{
						swipeDown = true;
					}
					else
					{
						swipeUp = true;
					}
				}
			}
		}




		if (Vector3.Distance(destination, transform.position) < 0.00001f)
		{
			transform.localEulerAngles = currentDirection;
			//if (canMove)
			{
				if (Valid())
				{
					destination = transform.position + nextPos;
					//direction = nextPos;
					//canMove = true;
				}
			}
		}
	}

	bool Valid()
	{
		Ray myRay = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
		RaycastHit myHit;

		if (Physics.Raycast(myRay, out myHit, 1f,unwalkable))
		{
			if (myHit.collider.tag == "Wall")
			{
				return false;
			}
		}
		return true;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Ghost")
		{
			Pathfinding pGhost = col.GetComponent<Pathfinding>();
			if (pGhost.state == Pathfinding.GhostStates.FRIGHTEND)
			{
				pGhost.state = Pathfinding.GhostStates.GOT_EATEN;
				//score ++ 
				GameManager.instance.AddScore(400);

			}
			else if (pGhost.state != Pathfinding.GhostStates.FRIGHTEND && pGhost.state != Pathfinding.GhostStates.GOT_EATEN)
			{
				//lose a Life
				GameManager.instance.LoseLife();
			}
		}
	}
}

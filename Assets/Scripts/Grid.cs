using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public GameObject bottomLeft, topRight;
	//DEBUG
	public GameObject start, goal;

	Node[,] myGrid;

	public List<Node> clydePath;
	public List<Node> blinkyPath;
	public List<Node> inkyPath;
	public List<Node> pinkyPath;

	//public List<Node> path;//tutorial Check


	public LayerMask unwalkable; //bitshifting needed!?! check bombs!!!

	//GRID INFO
	int xStart,zStart;

	int xEnd,zEnd;

	public int vCells,hCells;//amount of cells in grid

	int cellWidth = 1;
	int cellHeight = 1;

	void Awake()
	{
		MPGridCreate();
	}

	void MPGridCreate()
	{
		xStart = (int)bottomLeft.transform.position.x;
		zStart = (int)bottomLeft.transform.position.z;

		xEnd = (int)topRight.transform.position.x;
		zEnd = (int)topRight.transform.position.z;

		hCells = (int)((xEnd - xStart) / cellWidth);
		vCells = (int)((zEnd - zStart) / cellHeight);

		myGrid = new Node[hCells + 1, vCells + 1];

		UpdateGrid();
	}

	public void UpdateGrid()
	{
		for (int i = 0; i <= hCells; i++)
		{
			for (int j = 0; j <= vCells; j++) 
			{
				bool walkable = !(Physics.CheckSphere(new Vector3(xStart + i, 0, zStart + j),0.4f,unwalkable));

				myGrid[i, j] = new Node(i, j, 0, walkable);
			}
		}
	}

	void OnDrawGizmos()
	{
		if (myGrid != null)
		{
			foreach (Node node in myGrid)
			{
				Gizmos.color = (node.walkable) ? Color.white : Color.red;

				if (clydePath != null)
				{
					if (clydePath.Contains(node))
					{
						Gizmos.color = Color.yellow;
					}
				}
				if (inkyPath != null)
				{
					if (inkyPath.Contains(node))
					{
						Gizmos.color = Color.cyan;
					}
				}

				if (blinkyPath != null)
				{
					if (blinkyPath.Contains(node))
					{
						Gizmos.color = Color.red;
					}
				}
				if (pinkyPath != null)
				{
					if (pinkyPath.Contains(node))
					{
						Gizmos.color = Color.magenta;
					}
				}
				//Tutorial Check
//				if (path != null)
//				{
//					if (path.Contains(node))
//					{
//						Gizmos.color = Color.green;
//					}
//				}

				Gizmos.DrawWireCube(new Vector3(xStart + node.posX, 0.5f, zStart + node.posZ), new Vector3(0.8f, 0.8f, 0.8f));
			}
		}
	}

	public Node NodeRequest(Vector3 pos)
	{
		int gridX = (int)Vector3.Distance(new Vector3(pos.x, 0, 0), new Vector3(xStart, 0, 0));
		int gridZ = (int)Vector3.Distance(new Vector3(0, 0, pos.z), new Vector3(0, 0, zStart));


		return myGrid[gridX, gridZ];
	}

	public Vector3 NextPathPoint(Node node)
	{
		int gridX = (int)(xStart+node.posX);
		int gridZ = (int)(zStart+node.posZ);

		return new Vector3(gridX, 0, gridZ);
	}
		

	public List<Node> GetNeighborNodes(Node node)
	{
		List<Node> neighbours = new List<Node>();
		//find all neighbors in a 3 x3 square around current node
		for (int x = -1; x <= 1; x++)
		{
			for (int z = -1; z <= 1; z++) 
			{
				//ignore foollowing fields
				//CENTER - WHERE YOU ARE
				if (x == 0 && z == 0)
				{
					continue;
				}
				//ignore top left
				if (x == -1 && z == 1)
				{
					continue;
				}
				//igore top right
				if (x == 1 && z == 1)
				{
					continue;
				}
				//igore bottom left
				if (x == 1 && z == -1)
				{
					continue;
				}
				//igore bottom right
				if (x == -1 && z == -1)
				{
					continue;
				}

				int checkPosX = node.posX + x;
				int checkPosZ = node.posZ + z;

				//INSIDE PLAYFIELD CHECK
				if (checkPosX >= 0 && checkPosX <= (hCells) && checkPosZ >= 0 && checkPosZ <= (vCells) )
				{
					neighbours.Add(myGrid[checkPosX, checkPosZ]);
				}
			}
		}
		return neighbours;
	}

	public bool CheckInsideGrid(Vector3 requestedPosition)
	{
		int gridX = (int)(requestedPosition.x - xStart);
		int gridZ = (int)(requestedPosition.z - zStart);


		if (gridX > hCells)
		{
			return false;
		}
		else if (gridX < 0)
		{
			return false;
		}
		else if (gridZ > vCells)
		{
			return false;
		}
		else if (gridZ < 0)
		{
			return false;
		}

		if (!NodeRequest(requestedPosition).walkable)
		{
			return false;
		}
		return true;
	}

	public Vector3 GetNearestNonWallNode(Vector3 target)
	{
		float min = 1000;
		int minIndexI = 0;
		int minIndexJ = 0;

		for (int i = 0; i < hCells; i++)
		{
			for (int j = 0; j < vCells; j++) 
			{
				if (myGrid[i, j].walkable)
				{
					Vector3 nextPoint = NextPathPoint(myGrid[i, j]);
					float distance = Vector3.Distance(nextPoint, target);
					if (distance < min)
					{
						min = distance;
						minIndexI = i;
						minIndexJ = j;
					}
				}
			}
		}
		return NextPathPoint(myGrid[minIndexI, minIndexJ]);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour {

	int score = 3;

	void Start () 
	{
		GameManager.instance.AddPellet();
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			GameManager.instance.ReducePellet(score);
			Destroy(gameObject);

		}
	}
}

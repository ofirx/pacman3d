using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : MonoBehaviour {

	int score = 10;

	void Start () 
	{
		GameManager.instance.AddPellet();
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			GameManager.instance.ReducePellet(score);
			GameManager.instance.frighten = true;
			Destroy(gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class AgedBullet : MonoBehaviour
{
	public float LifeOfBullet;
	private float _age;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		_age += Time.deltaTime;
		if(_age > LifeOfBullet)
		{
			Destroy(gameObject);
		}
	}
}


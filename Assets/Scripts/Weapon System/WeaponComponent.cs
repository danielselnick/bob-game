using UnityEngine;
using System.Collections;

public class WeaponComponent : MonoBehaviour
{
	public GameObject BulletPrototype;
	public float ReloadTime;
	public float FireRate;
	public float FireForce = 20;
	public int ClipSize;
	private float _timeSinceLastFire = 0;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		_timeSinceLastFire += Time.deltaTime;
		if(CanFire())
		{
			FireWeapon();
		}
	}
	
	private bool CanFire()
	{
		if(_timeSinceLastFire > FireRate)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private void FireWeapon()
	{
		var bulletClone = Instantiate(BulletPrototype) as GameObject;
		if(bulletClone == null)
		{
			return;
		}
		bulletClone.transform.position = this.transform.position;		
		bulletClone.rigidbody.AddForce(FireForce, 0, 0);
		_timeSinceLastFire = 0;
	}
}


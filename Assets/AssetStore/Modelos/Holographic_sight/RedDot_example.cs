using UnityEngine;
using System.Collections;

public class RedDot_example : MonoBehaviour {

	public GameObject redDot;
	public bool isAiming = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Mouse1)) {
			isAiming = true;

		} else {
			isAiming = false;
		}
	
{

		  if (isAiming == true) {
				redDot.GetComponent<Renderer>().enabled = true;
			}
			else 
			{
				redDot.GetComponent<Renderer>().enabled = false;
			}
		


		}


	
	}
}





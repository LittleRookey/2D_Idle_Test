using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
	public bool rotate=true;
	[SerializeField] private float Speed = 10;
	void Start() {
		rotate = true;
	}

	// Update is called once per frame
	void Update () {
		if(rotate == true){
			transform.Rotate ( Vector3.forward * (Speed * Time.deltaTime ) );
		}
	}
}

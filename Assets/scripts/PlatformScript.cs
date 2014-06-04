using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {
	
	private bool destroyOnNextUpdate = false;
	
	void Start () {
	
	}
	
	void FixedUpdate () {
		if (MainScript.running) {
			if(destroyOnNextUpdate) {
				Destroy (gameObject);
				return;
			}
			
			float step;		
			if (MainScript.fastForward) {
				step = MainScript.maxPlatformStep;
			} else {
				step = MainScript.platformSpeed * Time.deltaTime;
				step = System.Math.Min(step, MainScript.maxPlatformStep);
			}
			
			rigidbody.MovePosition(rigidbody.position + Vector3.up * step);
			
			if (transform.position.y >= MainScript.height) {
				 Destroy (gameObject);
			}
		}
	}
	
	public void DestroyOnNextUpdate() {
		destroyOnNextUpdate = true;
	}
}

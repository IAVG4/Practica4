using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour {
	 
	public Vector2 positionInMatrix;

	void OnMouseDown() {
		GameManager.instance.OnClick(this.gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aliado : MonoBehaviour {

    Vector2 _posicion;
    int _id;
    public void ConstructoraAliado(Vector2 posicion, int id)
    {
        _posicion = posicion;
        _id = id;
    }

    public int my_Id() {
        return _id;
    }

    public Vector2 getPosicion()
    {
        return _posicion;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

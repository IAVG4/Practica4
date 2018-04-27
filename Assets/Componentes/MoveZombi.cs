using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveZombi : MonoBehaviour {

    Vector2Int dondeTengoQueIr;
    Vector2Int dondeEstoyYendo;

    Vector2 _posicion;
    int _id;
    public void ConstructoraZombi(Vector2 posicion, int id)
    {
        _posicion = posicion;
        _id = id;
    }

    public int my_Id()
    {
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

    IEnumerator movimientoZombi()
    {
        for (int i = 0; i < GameManager.instance.listaAliados.Count; i++)
        {

        }
        yield return new WaitForSeconds(0.5f);
    }
}

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

    public void InvokeMovimientoZombi()
    {
        Debug.Log("He entrado en el Inoke");
        StartCoroutine("MovimientoZombi", 0.5f);
    }

    public IEnumerator MovimientoZombi()
    {
        //for (int i = 0; i < GameManager.instance.listaAliados.Count; i++)
        //{
            Debug.Log(this.gameObject.transform.position);
            this.gameObject.transform.Translate(new Vector3(-1, 0, 0));
            _posicion = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
            Debug.Log(this.gameObject.transform.position);
        //}
        yield return new WaitForSeconds(0.5f);
    }
}

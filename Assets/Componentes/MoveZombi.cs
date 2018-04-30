using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveZombi : MonoBehaviour {

    

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
        
        StartCoroutine("MovimientoZombi", 0.5f);
    }

    public IEnumerator MovimientoZombi()
    {
        if (GameManager.instance.listaAliados.Count > 0)
        {
            Vector3 AliadoMasCercano = GameManager.instance.listaAliados[0].transform.position;
            int distancia = (int)Mathf.Abs(this.gameObject.transform.position.x - AliadoMasCercano.x) + 
                (int)Mathf.Abs(-this.gameObject.transform.position.y - (-AliadoMasCercano.y));

            for (int i = 1; i < GameManager.instance.listaAliados.Count; i++)
            {
                int distAux = (int)Mathf.Abs(this.gameObject.transform.position.x 
                    - GameManager.instance.listaAliados[i].transform.position.x) +
                (int)Mathf.Abs(-this.gameObject.transform.position.y 
                - (-GameManager.instance.listaAliados[i].transform.position.y));

                if (distAux < distancia)
                {
                    distancia = distAux;
                    AliadoMasCercano = GameManager.instance.listaAliados[1].transform.position;
                }
                Debug.Log(i);
            }

            if (this.gameObject.transform.position.y == AliadoMasCercano.y)
            {
                if (this.gameObject.transform.position.x > AliadoMasCercano.x)
                {
                    this.gameObject.transform.Translate(new Vector3(-1, 0, 0));
                    
                }

                else if (this.gameObject.transform.position.x < AliadoMasCercano.x)
                {
                    this.gameObject.transform.Translate(new Vector3(1, 0, 0));
                }
            }
            else
            {
                if (this.gameObject.transform.position.y > AliadoMasCercano.y)
                {
                    this.gameObject.transform.Translate(new Vector3(0, -1, 0));

                }

                else
                {
                    this.gameObject.transform.Translate(new Vector3(0, 0, 0));
                }
            }
        }

        _posicion = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

        yield return new WaitForSeconds(0.5f);
    }
}

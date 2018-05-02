using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

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
		// por inicializarlos a algo
		Vector3 AliadoMasCercano = new Vector3 (0, 0, 0);
		int distancia = 1000;
        if (GameManager.instance.listaAliados.Count > 0)
        {
            AliadoMasCercano = GameManager.instance.listaAliados[0].transform.position;
            distancia = (int)Mathf.Abs(this.gameObject.transform.position.x - AliadoMasCercano.x) + 
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
                    AliadoMasCercano = GameManager.instance.listaAliados[i].transform.position;
                }
                //Debug.Log(i);
			}
		}
		// Despues de comprobar a los aliados vamos a comprobar al heroe
		int distToHeroe = (int)Mathf.Abs(this.gameObject.transform.position.x 
			- GameManager.instance.heroe.transform.position.x) +
			(int)Mathf.Abs(-this.gameObject.transform.position.y 
				- (-GameManager.instance.heroe.transform.position.y));
		if (distToHeroe < distancia) {
			Debug.Log ("target heroe");
			AliadoMasCercano = GameManager.instance.heroe.transform.position;
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
        
        _posicion = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

		PersonajeCasilla pc = GameManager.instance.getPersonaje (_posicion); 
		if (pc.hayHeroe || pc.hayAliado) {
			int num =Random.Range(0, 100);
			bool deDia = GameManager.instance.esDeDia ();
			if (deDia) {
				if (GameManager.instance.getNumAliados () >= 2 && num < 90) // muere zombie
					Debug.Log ("Muere Zombie 1");
				else if (GameManager.instance.getNumAliados () == 1 && num < 50) // muere zombie
					Debug.Log ("Muere Zombie 2");
				else if (GameManager.instance.getNumAliados () == 0 && num < 20) // muere zombie
					Debug.Log ("Muere Zombie 3");
				else // muere soldado
					Debug.Log ("Muere Sodado");
				
			} else {
				if (GameManager.instance.getNumAliados () >= 2 && num < 80) // muere zombie
					Debug.Log ("Muere Zombie 1");
				else if (GameManager.instance.getNumAliados () == 1 && num < 40) // muere zombie
					Debug.Log ("Muere Zombie 2");
				else if (GameManager.instance.getNumAliados () == 0 && num < 10) // muere zombie
					Debug.Log ("Muere Zombie 3");
				else // muere soldado
					Debug.Log ("Muere Sodado");
			}
		}
			
		
        yield return new WaitForSeconds(0.5f);
    }
}

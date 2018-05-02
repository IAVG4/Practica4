using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHeroe : MonoBehaviour {

	public enum situacion {muchosZombies, muchosAliados, neutro}
	public enum destreza {buena, regular, mala}
	public enum decision {avanzar, retroceder, esperar}
	decision myDecision;
	Vector2 _posicion;

	public void ConstructoraHeroe(Vector2 posicion)
	{
		_posicion = new Vector2(-posicion.y, posicion.x);
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

	public void InvokeMovimientoHeroe()
	{
		myDecision = makeDecision ();
		Debug.Log (myDecision);
		StartCoroutine("MovimientoHeroe", 0.5f);
		//MovimientoHeroe();
	}

	decision makeDecision(){
		int numZombies = GameManager.instance.getNumZombies ();
		int numAliados = GameManager.instance.getNumAliados ();
		bool deDia = GameManager.instance.esDeDia ();

		situacion mySituacion = situacion.neutro;
		if(numZombies == 0 && numAliados == 0)
			mySituacion = situacion.neutro;
		else if(numZombies == 0 && numAliados == 1)
			mySituacion = situacion.muchosAliados;
		else if(numZombies == 0 && numAliados > 1)
			mySituacion = situacion.muchosAliados;
		else if(numZombies <= 5 && numAliados == 0)
			mySituacion = situacion.muchosZombies;
		else if(numZombies <= 5 && numAliados == 1)
			mySituacion = situacion.neutro;
		else if(numZombies <= 5 && numAliados > 1)
			mySituacion = situacion.neutro;
		else if(numZombies > 5 && numAliados == 0)
			mySituacion = situacion.muchosZombies;
		else if(numZombies > 5 && numAliados == 1)
			mySituacion = situacion.muchosZombies;
		else if(numZombies > 5 && numAliados > 1)
			mySituacion = situacion.muchosZombies;

		destreza myDestreza = destreza.regular;
		if (deDia && numAliados == 0)
			myDestreza = destreza.mala;
		else if (deDia && numAliados == 1)
			myDestreza = destreza.regular;
		else if (deDia && numAliados > 1)
			myDestreza = destreza.buena;
		else if (!deDia && numAliados == 0)
			myDestreza = destreza.mala;
		else if (!deDia && numAliados == 1)
			myDestreza = destreza.regular;
		else if (!deDia && numAliados > 1)
			myDestreza = destreza.buena;

		if (mySituacion == situacion.muchosZombies) {
			if (myDestreza == destreza.buena)
				return decision.avanzar;
			else if (myDestreza == destreza.regular)
				return decision.esperar;
			else
				return decision.retroceder;
			
		} else if (mySituacion == situacion.muchosAliados) {
			if (myDestreza == destreza.buena)
				return decision.esperar;
			else if (myDestreza == destreza.regular)
				return decision.retroceder;
			else
				return decision.retroceder;
			
		} else {
			if (myDestreza == destreza.buena)
				return decision.retroceder;
			else if (myDestreza == destreza.regular)
				return decision.avanzar;
			else
				return decision.retroceder;
		}
	}	

	public IEnumerator MovimientoHeroe()
	//public void MovimientoHeroe()
	{
		Vector3 target = new Vector3(0, 0, 0);
		if (myDecision == decision.avanzar) {
			int distancia = 1000;
			if (GameManager.instance.listaZombis.Count > 0)
			{
				target = GameManager.instance.listaZombis[0].transform.position;
				distancia = (int)Mathf.Abs(this.gameObject.transform.position.x - target.x) +
					(int)Mathf.Abs(-this.gameObject.transform.position.y - (-target.y));

				for (int i = 1; i < GameManager.instance.listaZombis.Count; i++)
				{
					int distAux = (int)Mathf.Abs(this.gameObject.transform.position.x
						- GameManager.instance.listaZombis[i].transform.position.x) +
						(int)Mathf.Abs(-this.gameObject.transform.position.y
							- (-GameManager.instance.listaZombis[i].transform.position.y));

					if (distAux < distancia)
					{
						distancia = distAux;
						target = GameManager.instance.listaZombis[i].transform.position;
					}

				}
			}

		} else if (myDecision == decision.retroceder) {
			target = GameManager.instance.refugio.transform.position;
		}

		if (myDecision != decision.esperar) {
			if (this.gameObject.transform.position.y == target.y)
			{
				if (this.gameObject.transform.position.x > target.x)
				{
					this.gameObject.transform.Translate(new Vector3(-1, 0, 0));
				}

				else if (this.gameObject.transform.position.x < target.x)
				{
					this.gameObject.transform.Translate(new Vector3(1, 0, 0));
				}
			}
			else if (this.gameObject.transform.position.x == target.x)
			{
				if (this.gameObject.transform.position.y > target.y)
					this.gameObject.transform.Translate(new Vector3(0, -1, 0));
				else
					this.gameObject.transform.Translate(new Vector3(0, 1, 0));
			}
			else
			{
				if (Mathf.Abs(this.gameObject.transform.position.x - target.x) > 
					Mathf.Abs(-this.gameObject.transform.position.y - (-target.y)))
				{
					if (this.gameObject.transform.position.y < target.y)
					{
						this.gameObject.transform.Translate(new Vector3(0, 1, 0));
					}
					else this.gameObject.transform.Translate(new Vector3(0, -1, 0));
				}

				else
				{
					if (this.gameObject.transform.position.x < target.x)
					{
						this.gameObject.transform.Translate(new Vector3(1, 0, 0));
					}
					else this.gameObject.transform.Translate(new Vector3(-1, 0, 0));
				}

			}

			_posicion = new Vector2 (-this.gameObject.transform.position.y, this.gameObject.transform.position.x);
			Debug.Log (_posicion);
			GameManager.instance.tablero [(int)_posicion.x, (int)_posicion.y].hayHeroe = true;
		}

		yield return new WaitForSeconds(0.5f);
	}
}

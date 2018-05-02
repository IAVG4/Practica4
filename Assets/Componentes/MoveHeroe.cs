using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHeroe : MonoBehaviour {

	public enum situacion {muchosZombies, muchosAliados, neutro}
	public enum destreza {buena, regular, mala}
	public enum decision {avanzar, retroceder, esperar}
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

	public void InvokeMovimientoHeroe()
	{
		decision d = makeDecision ();
		StartCoroutine(MovimientoHeroe(d));
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

		decision myDecision = decision.esperar;
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

	public IEnumerator MovimientoHeroe(decision myDecision)
	{
		Vector2 target = new Vector2(_posicion.x, _posicion.y);
		if (myDecision == decision.avanzar) {
			List<Vector2> zombies = GameManager.instance.getZombies ();
			target = zombies [0];
			int distancia = Mathf.Abs (((int)zombies [0].x - (int)_posicion.x) + ((int)zombies [0].y - (int)_posicion.y));

			for (int i = 1; i < zombies.Count; i++) {
				int distAux = Mathf.Abs (((int)zombies [i].x - (int)_posicion.x) + ((int)zombies [i].y - (int)_posicion.y));


				if (distAux < distancia) {
					distancia = distAux;
					target = zombies [i];
				}
				Debug.Log (i);
			}

		} else if (myDecision == decision.retroceder) {
			target = GameManager.instance.getRefugio ();
		}

		if (myDecision != decision.esperar) {
			if (this.gameObject.transform.position.y == target.y) {
				if (this.gameObject.transform.position.x > target.x) {
					this.gameObject.transform.Translate (new Vector3 (-1, 0, 0));

				} else if (this.gameObject.transform.position.x < target.x) {
					this.gameObject.transform.Translate (new Vector3 (1, 0, 0));
				}
			} else {
				if (this.gameObject.transform.position.y > target.y) {
					this.gameObject.transform.Translate (new Vector3 (0, -1, 0));

				} else {
					this.gameObject.transform.Translate (new Vector3 (0, 0, 0));
				}
			}

			_posicion = new Vector2 (this.gameObject.transform.position.x, this.gameObject.transform.position.y);
		}

		yield return new WaitForSeconds(0.5f);
	}
}

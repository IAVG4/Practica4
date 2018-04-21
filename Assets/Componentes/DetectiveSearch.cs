using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum type {vacia, casa, cadaver, sangre, agujero, barro, sangreBarro, arma, desconocido}

public class DetectiveSearch : MonoBehaviour {

    public static DetectiveSearch instance;


    bool[,] marked;
	type [,] knowledge;
	type [,] board;
	bool [,] seguridad;

	bool cadaverEncontrado, armaencontrada, muerto;
	Vector2 posicionCadaver;
	// Use this for initialization
	void Start () {
        instance = this;
		marked = new bool[5, 10];
		knowledge = new type[5, 10];
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 10; ++j)
				knowledge [i, j] = type.desconocido;
		}
		board = new type[5, 10];
		seguridad = new bool[5, 10]; 

		cadaverEncontrado = false;
		armaencontrada = false;
		muerto = false;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public bool buscaSolucion(type [,] tablero, Vector2 posCasa){
		board = tablero;
		knowledge [(int)posCasa.x, (int)posCasa.y] = type.casa;
		Vector2 posInicio = posCasa;
		while(!muerto){
			if(advance(posInicio)) // Si lo ha encontrado te sales
				break;
			posInicio = arriesgate();
			// Mientras se arriesgue a un barro que se siga arriesgando
			while(board[(int)posInicio.x, (int)posInicio.y] == type.barro){
				posInicio = arriesgate();
			}
			// Si se ha arriesgado a un agujero pues muere
			if(board[(int)posInicio.x, (int)posInicio.y] == type.agujero)
				muerto = true;
		}
		return muerto;
	}

	bool advance(Vector2 actual){
		knowledge [(int)actual.x, (int)actual.y] = board [(int)actual.x, (int)actual.y];
        GameManager.instance.destapaCasilla((int)actual.y, (int)actual.x);
		marked [(int)actual.x, (int)actual.y] = true;
		// Si esta vacia doy el paso y vuelvo a llamar a advance() con la nueva posicion
		if (knowledge [(int)actual.x, (int)actual.y] == type.vacia) {
			if (actual.y + 1 < board.GetLength (1) && !marked [(int)actual.x, (int)actual.y + 1]) // derecha
			if (advance (new Vector2 (actual.x, actual.y + 1)))
				return true;
			if (actual.x - 1 >= 0 && !marked [(int)actual.x - 1, (int)actual.y]) // arriba
				if (advance (new Vector2 (actual.x - 1, actual.y)))
				return true;
			if (actual.x + 1 < board.GetLength (0) && !marked [(int)actual.x + 1, (int)actual.y]) // abajo
				if (advance (new Vector2 (actual.x + 1, actual.y)))
				return true;
			if (actual.y - 1 >= 0 && !marked [(int)actual.x, (int)actual.y - 1]) // izquierda
				if (advance (new Vector2 (actual.x, actual.y - 1)))
				return true;
		} 
		// Si es barro no me arriesgo y me doy la vuelta
		else if (knowledge [(int)actual.x, (int)actual.y] == type.barro || knowledge [(int)actual.x, (int)actual.y] == type.sangreBarro) {
			//buscaAgujero (new Vector2 ((int)actual.x, (int)actual.y));
			seguridad [(int)actual.x, (int)actual.y] = true;
			// Si es sangre barro intento deducir donde esta el cadaver
			if (knowledge [(int)actual.x, (int)actual.y] == type.sangreBarro) {
				if (deduzcoPosiocionCadaver (new Vector2 ((int)actual.x, (int)actual.y))) {
					// si lo consigo deducir avanzo hasta el cadaver para poder buscar el arma desde ahi
					buscaArmaDesdeCadaver (posicionCadaver);
					return true;
				}
			}
			return false;
		}
		// Si encuentra una casilla con sangre o el arma paso a inspeccionar la zona
		else if (knowledge [(int)actual.x, (int)actual.y] == type.sangre || knowledge [(int)actual.x, (int)actual.y] == type.arma) {
			inspeccionar (new Vector2 ((int)actual.x, (int)actual.y));
			return true;
		}
		return false;
	}

	void inspeccionar(Vector2 actual){
		// Si encuentra el arma compruebo las cuatro direcciones para deducir donde esta el cadaver
		if (knowledge [(int)actual.x, (int)actual.y] == type.arma) {
			armaencontrada = true;
			// Si no esta marcado (no lo conozco), voy para conocerlo y me lo apunto en la lista
			if (actual.x - 1 >= 0 && !marked [(int)actual.x - 1, (int)actual.y]) {
				marked [(int)actual.x - 1, (int)actual.y] = true;
				knowledge [(int)actual.x - 1, (int)actual.y] = board [(int)actual.x - 1, (int)actual.y];
				// Importante meterlo en la lista ya que lo estas comprobando (pasas por el)
				GameManager.instance.destapaCasilla((int)actual.y, (int)actual.x - 1);
			}
			if (actual.x + 1 < marked.GetLength(0) && !marked [(int)actual.x + 1, (int)actual.y]) {
				marked [(int)actual.x + 1, (int)actual.y] = true;
				knowledge [(int)actual.x + 1, (int)actual.y] = board [(int)actual.x + 1, (int)actual.y];
				// Importante meterlo en la lista ya que lo estas comprobando (pasas por el)
				GameManager.instance.destapaCasilla((int)actual.y, (int)actual.x + 1);
			}
			if (actual.y - 1 >= 0 && !marked [(int)actual.x, (int)actual.y - 1]) {
				marked [(int)actual.x, (int)actual.y - 1] = true;
				knowledge [(int)actual.x, (int)actual.y - 1] = board [(int)actual.x, (int)actual.y - 1];
				// Importante meterlo en la lista ya que lo estas comprobando (pasas por el)
				GameManager.instance.destapaCasilla((int)actual.y - 1, (int)actual.x);
			}
			if (actual.y + 1 < marked.GetLength(1) && !marked [(int)actual.x, (int)actual.y + 1]) {
				marked [(int)actual.x, (int)actual.y + 1] = true;
				knowledge [(int)actual.x, (int)actual.y + 1] = board [(int)actual.x, (int)actual.y + 1];
				// Importante meterlo en la lista ya que lo estas comprobando (pasas por el)
				GameManager.instance.destapaCasilla((int)actual.y + 1, (int)actual.x);
			}

			// Cuando ya hemos estudiado el entorno ya podemos deducir donde esta el cadaver
			// Si el arma esta abajo derecha respecto del cadaver
			if((knowledge[(int)actual.x, (int)actual.y - 1] == type.sangre || knowledge[(int)actual.x, (int)actual.y - 1] == type.sangreBarro) && (knowledge[(int)actual.x - 1, (int)actual.y] == type.sangre || knowledge[(int)actual.x - 1, (int)actual.y] == type.sangreBarro)){
				// posicion del cadaver: (actual.x - 1, actual.y - 1)
				posicionCadaver = new Vector2(actual.x - 1, actual.y - 1);			
				knowledge[(int)actual.x - 1, (int)actual.y - 1] = type.cadaver;
				GameManager.instance.destapaCasilla((int)actual.y - 1, (int)actual.x - 1);
				//knowledge[(int)actual.x - 2, (int)actual.y - 1] = type.sangre;
				//knowledge[(int)actual.x - 1, (int)actual.y - 2] = type.sangre;
				cadaverEncontrado = true;
				return;
			}
			// Si el arma esta abajo izquierda respecto del cadaver
			else if((knowledge[(int)actual.x, (int)actual.y + 1] == type.sangre || knowledge[(int)actual.x, (int)actual.y + 1] == type.sangreBarro) && (knowledge[(int)actual.x - 1, (int)actual.y] == type.sangre || knowledge[(int)actual.x - 1, (int)actual.y] == type.sangreBarro)){
				// posicion del cadaver: (actual.x - 1, actual.y + 1)
				posicionCadaver = new Vector2(actual.x - 1, actual.y + 1);			
				knowledge[(int)actual.x - 1, (int)actual.y + 1] = type.cadaver;
				GameManager.instance.destapaCasilla((int)actual.y + 1, (int)actual.x - 1);
				//knowledge[(int)actual.x - 2, (int)actual.y + 1] = type.sangre;
				//knowledge[(int)actual.x - 1, (int)actual.y + 2] = type.sangre;
				cadaverEncontrado = true;
				return;
			}
			// Si el arma esta arriba derecha respecto del cadaver
			else if((knowledge[(int)actual.x, (int)actual.y - 1] == type.sangre || knowledge[(int)actual.x, (int)actual.y - 1] == type.sangreBarro) && (knowledge[(int)actual.x + 1, (int)actual.y] == type.sangre || knowledge[(int)actual.x + 1, (int)actual.y] == type.sangreBarro)){
				// posicion del cadaver: (actual.x + 1, actual.y - 1)
				posicionCadaver = new Vector2(actual.x + 1, actual.y - 1);			
				knowledge[(int)actual.x + 1, (int)actual.y - 1] = type.cadaver;
				GameManager.instance.destapaCasilla((int)actual.y - 1, (int)actual.x + 1);
				//knowledge[(int)actual.x + 2, (int)actual.y - 1] = type.sangre;
				//knowledge[(int)actual.x + 1, (int)actual.y - 2] = type.sangre;
				cadaverEncontrado = true;
				return;
			}
			// Si el arma esta arriba izquierda respecto del cadaver
			else if((knowledge[(int)actual.x, (int)actual.y + 1] == type.sangre ||knowledge[(int)actual.x, (int)actual.y + 1] == type.sangreBarro) && (knowledge[(int)actual.x + 1, (int)actual.y] == type.sangre || knowledge[(int)actual.x + 1, (int)actual.y] == type.sangreBarro)){
				// posicion del cadaver: (actual.x + 1, actual.y - 1)
				posicionCadaver = new Vector2(actual.x + 1, actual.y - 1);			
				knowledge[(int)actual.x + 1, (int)actual.y + 1] = type.cadaver;
				GameManager.instance.destapaCasilla((int)actual.y + 1, (int)actual.x + 1);
				//knowledge[(int)actual.x + 2, (int)actual.y + 1] = type.sangre;
				//knowledge[(int)actual.x + 1, (int)actual.y + 2] = type.sangre;
				cadaverEncontrado = true;
				return;
			}
		}

		// Si encontramos sangre
		else if (knowledge [(int)actual.x, (int)actual.y] == type.sangre) {
			
			// Intenta deducir donde esta el cadaver a traves de lo que ya save
			if (!deduzcoPosiocionCadaver(new Vector2((int)actual.x, (int)actual.y))){
				// si no podemos deducir que casillas es pues probamos suerte siguiendo la busqueda
				if (actual.x - 1 >= 0 && !marked [(int)actual.x - 1, (int)actual.y]){ // arriba
					knowledge[(int)actual.x - 1, (int)actual.y] = board[(int)actual.x - 1, (int)actual.y];
					GameManager.instance.destapaCasilla((int)actual.y, (int)actual.x - 1);
					if (knowledge [(int)actual.x - 1, (int)actual.y] == type.arma) {
						armaencontrada = true;
					}
					if (knowledge [(int)actual.x - 1, (int)actual.y] == type.cadaver) {
						posicionCadaver = new Vector2((int)actual.x - 1, (int)actual.y);			
						cadaverEncontrado = true;
						if(!armaencontrada)
							buscaArmaDesdeCadaver (new Vector2(actual.x - 1, actual.y));
						return;
					}
				}
				if (actual.x + 1 < board.GetLength (0) && !marked [(int)actual.x + 1, (int)actual.y]){ // abajo
					knowledge[(int)actual.x + 1, (int)actual.y] = board[(int)actual.x + 1, (int)actual.y];
					GameManager.instance.destapaCasilla((int)actual.y, (int)actual.x + 1);
					if (knowledge [(int)actual.x + 1, (int)actual.y] == type.arma) {
						armaencontrada = true;
					}
					if (knowledge [(int)actual.x + 1, (int)actual.y] == type.cadaver) {
						posicionCadaver = new Vector2((int)actual.x + 1, (int)actual.y);			
						cadaverEncontrado = true;
						if(!armaencontrada)
							buscaArmaDesdeCadaver (new Vector2(actual.x + 1, actual.y));
						return;
					}
				}
				if (actual.y - 1 >= 0 && !marked [(int)actual.x, (int)actual.y - 1]){ // izquierda
					knowledge[(int)actual.x, (int)actual.y - 1] = board[(int)actual.x, (int)actual.y - 1];
					GameManager.instance.destapaCasilla((int)actual.y - 1, (int)actual.x);
					if (knowledge [(int)actual.x, (int)actual.y - 1] == type.arma) {
						armaencontrada = true;
					}
					if (knowledge [(int)actual.x, (int)actual.y - 1] == type.cadaver) {
						posicionCadaver = new Vector2 ((int)actual.x, (int)actual.y - 1);			
						cadaverEncontrado = true;
						if(!armaencontrada)
							buscaArmaDesdeCadaver (new Vector2(actual.x, actual.y - 1));
						return;
					}
				}
				if (actual.y + 1 < board.GetLength (1) && !marked [(int)actual.x, (int)actual.y + 1]){ // derecha
					knowledge[(int)actual.x, (int)actual.y + 1] = board[(int)actual.x, (int)actual.y + 1];
					GameManager.instance.destapaCasilla((int)actual.y + 1, (int)actual.x);
					if (knowledge [(int)actual.x, (int)actual.y + 1] == type.arma) {
						armaencontrada = true;
					}
					if (knowledge [(int)actual.x, (int)actual.y + 1] == type.cadaver) {
						posicionCadaver = new Vector2 ((int)actual.x, (int)actual.y + 1);			
						cadaverEncontrado = true;
						if(!armaencontrada)
							buscaArmaDesdeCadaver (new Vector2(actual.x, actual.y + 1));
						return;
					}
				}

			}
		} 
				
	
	}

	bool deduzcoPosiocionCadaver(Vector2 actual){
		if (actual.x - 2 >= 0 && (knowledge [(int)actual.x - 2, (int)actual.y] == type.sangreBarro || knowledge [(int)actual.x - 2, (int)actual.y] == type.sangre)) {
			// posicion del cadaver: (actual.x - 1, actual.y)
			knowledge [(int)actual.x - 1, (int)actual.y] = type.cadaver;
			GameManager.instance.destapaCasilla ((int)actual.y, (int)actual.x - 1);
			posicionCadaver = new Vector2((int)actual.x - 1, (int)actual.y);			
			cadaverEncontrado = true;
			return true;
		} else if (actual.x + 2 < board.GetLength (0) && (knowledge [(int)actual.x + 2, (int)actual.y] == type.sangreBarro || knowledge [(int)actual.x + 2, (int)actual.y] == type.sangre)) {
			// posicion del cadaver: (actual.x + 1, actual.y)
			knowledge [(int)actual.x + 1, (int)actual.y] = type.cadaver;
			GameManager.instance.destapaCasilla ((int)actual.y, (int)actual.x + 1);
			posicionCadaver = new Vector2((int)actual.x + 1, (int)actual.y);			
			cadaverEncontrado = true;
			return true;
		} else if (actual.y - 2 >= 0 && (knowledge [(int)actual.x, (int)actual.y - 2] == type.sangreBarro || knowledge [(int)actual.x, (int)actual.y - 2] == type.sangre)) {
			// posicion del cadaver: (actual.x, actual.y - 1) 
			knowledge [(int)actual.x, (int)actual.y - 1] = type.cadaver;
			GameManager.instance.destapaCasilla ((int)actual.y - 1, (int)actual.x);
			posicionCadaver = new Vector2((int)actual.x, (int)actual.y - 1);			
			cadaverEncontrado = true;
			return true;
		} else if (actual.y + 2 < board.GetLength (1) && (knowledge [(int)actual.x, (int)actual.y + 2] == type.sangreBarro || knowledge [(int)actual.x, (int)actual.y + 2] == type.sangre)) {
			// posicion del cadaver: (actual.x, actual.y + 1) 
			knowledge [(int)actual.x + 1, (int)actual.y] = type.cadaver;
			GameManager.instance.destapaCasilla ((int)actual.y + 1, (int)actual.x);
			posicionCadaver = new Vector2((int)actual.x, (int)actual.y + 1);			
			cadaverEncontrado = true;
			return true;
		} else
			return false;
	}
	
	void buscaArmaDesdeCadaver(Vector2 cadaver){
		if (cadaver.x - 1 >= 0) {
			//if (!marked [(int)cadaver.x - 1, (int)cadaver.y - 1] || !marked [(int)cadaver.x - 1, (int)cadaver.y + 1]) { // arriba
				// se mueve a (cadaver.x - 1, cadaver.y)
				knowledge [(int)cadaver.x - 1, (int)cadaver.y] = board [(int)cadaver.x - 1, (int)cadaver.y];
				GameManager.instance.destapaCasilla ((int)cadaver.y, (int)cadaver.x - 1);
				// si no es barro sangre sigo mirando, porque si lo fuese no me la jugaria
				if (knowledge [(int)cadaver.x - 1, (int)cadaver.y] != type.sangreBarro) {
					if (cadaver.y - 1 >= 0 && !marked [(int)cadaver.x - 1, (int)cadaver.y - 1]) { // se mueve a (cadaver.x - 1, cadaver.y - 1) si no se ha mirado ya
						knowledge [(int)cadaver.x - 1, (int)cadaver.y - 1] = board [(int)cadaver.x - 1, (int)cadaver.y - 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y - 1, (int)cadaver.x - 1);
						if (knowledge [(int)cadaver.x - 1, (int)cadaver.y - 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}
			
					if (cadaver.y + 1 < marked.GetLength(1) && !marked [(int)cadaver.x - 1, (int)cadaver.y + 1]) { // se mueve a (cadaver.x - 1, cadaver.y + 1) si no se ha mirado ya
						knowledge [(int)cadaver.x - 1, (int)cadaver.y + 1] = board [(int)cadaver.x - 1, (int)cadaver.y + 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y + 1, (int)cadaver.x - 1);
						if (knowledge [(int)cadaver.x - 1, (int)cadaver.y + 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}
				}
			//}
		}
		if (cadaver.x + 1 < marked.GetLength (0)) {
			//if (!marked [(int)cadaver.x + 1, (int)cadaver.y - 1] || !marked [(int)cadaver.x + 1, (int)cadaver.y + 1]) { // abajo
				// se mueve a (cadaver.x + 1, cadaver.y)
				knowledge [(int)cadaver.x + 1, (int)cadaver.y] = board [(int)cadaver.x + 1, (int)cadaver.y];
				GameManager.instance.destapaCasilla ((int)cadaver.y, (int)cadaver.x + 1);
				// si no es barro sangre sigo mirando, porque si lo fuese no me la jugaria
				if (knowledge [(int)cadaver.x + 1, (int)cadaver.y] != type.sangreBarro) {
					if (cadaver.y - 1 >= 0 && !marked [(int)cadaver.x + 1, (int)cadaver.y - 1]) { // se mueve a (cadaver.x + 1, cadaver.y - 1) si no se ha mirado ya
						knowledge [(int)cadaver.x + 1, (int)cadaver.y - 1] = board [(int)cadaver.x + 1, (int)cadaver.y - 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y - 1, (int)cadaver.x + 1);
						if (knowledge [(int)cadaver.x + 1, (int)cadaver.y - 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}

					if (cadaver.y + 1 < marked.GetLength(1) && !marked [(int)cadaver.x + 1, (int)cadaver.y + 1]) { // se mueve a (cadaver.x + 1, cadaver.y + 1) si no se ha mirado ya
						knowledge [(int)cadaver.x + 1, (int)cadaver.y + 1] = board [(int)cadaver.x + 1, (int)cadaver.y + 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y + 1, (int)cadaver.x + 1);
						if (knowledge [(int)cadaver.x + 1, (int)cadaver.y + 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}
				}
			//}
		} 
		if (cadaver.y - 1 >= 0) {
			//if (!marked [(int)cadaver.x - 1, (int)cadaver.y - 1] || !marked [(int)cadaver.x + 1, (int)cadaver.y - 1]) { // izquierda
				// se mueve a (cadaver.x, cadaver.y - 1)
				knowledge [(int)cadaver.x, (int)cadaver.y - 1] = board [(int)cadaver.x, (int)cadaver.y - 1];
				GameManager.instance.destapaCasilla ((int)cadaver.y - 1, (int)cadaver.x);
				// si no es barro sangre sigo mirando, porque si lo fuese no me la jugaria
				if (knowledge [(int)cadaver.x, (int)cadaver.y - 1] != type.sangreBarro) {
					if (cadaver.x - 1 >= 0 && !marked [(int)cadaver.x - 1, (int)cadaver.y - 1]) { // se mueve a (cadaver.x - 1, cadaver.y - 1) si no se ha mirado ya
						knowledge [(int)cadaver.x - 1, (int)cadaver.y - 1] = board [(int)cadaver.x - 1, (int)cadaver.y - 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y - 1, (int)cadaver.x - 1);
						if (knowledge [(int)cadaver.x - 1, (int)cadaver.y - 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}

					if (cadaver.x + 1 < marked.GetLength(0) && !marked [(int)cadaver.x + 1, (int)cadaver.y - 1]) { // se mueve a (cadaver.x + 1, cadaver.y - 1) si no se ha mirado ya
						knowledge [(int)cadaver.x + 1, (int)cadaver.y - 1] = board [(int)cadaver.x + 1, (int)cadaver.y - 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y - 1, (int)cadaver.x + 1);
						if (knowledge [(int)cadaver.x + 1, (int)cadaver.y - 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}
				}
			//}
		}
		if (cadaver.y + 1 < marked.GetLength (1)) {
			//if (!marked [(int)cadaver.x + 1, (int)cadaver.y + 1] || !marked [(int)cadaver.x - 1, (int)cadaver.y + 1]) { // derecha
				// se mueve a (cadaver.x, cadaver.y + 1)
				knowledge [(int)cadaver.x, (int)cadaver.y + 1] = board [(int)cadaver.x, (int)cadaver.y + 1];
				GameManager.instance.destapaCasilla ((int)cadaver.y + 1, (int)cadaver.x);
				// si no es barro sangre sigo mirando, porque si lo fuese no me la jugaria
				if (knowledge [(int)cadaver.x, (int)cadaver.y + 1] != type.sangreBarro) {
					if (cadaver.x + 1 < marked.GetLength(0) && !marked [(int)cadaver.x + 1, (int)cadaver.y + 1]) { // se mueve a (cadaver.x + 1, cadaver.y + 1) si no se ha mirado ya
						knowledge [(int)cadaver.x + 1, (int)cadaver.y + 1] = board [(int)cadaver.x + 1, (int)cadaver.y + 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y + 1, (int)cadaver.x + 1);
						if (knowledge [(int)cadaver.x + 1, (int)cadaver.y + 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}

					if (cadaver.x - 1 >= 0 && !marked [(int)cadaver.x - 1, (int)cadaver.y + 1]) { // se mueve a (cadaver.x - 1, cadaver.y + 1) si no se ha mirado ya
						knowledge [(int)cadaver.x - 1, (int)cadaver.y + 1] = board [(int)cadaver.x - 1, (int)cadaver.y + 1];
						GameManager.instance.destapaCasilla ((int)cadaver.y + 1, (int)cadaver.x - 1);
						if (knowledge [(int)cadaver.x - 1, (int)cadaver.y + 1] == type.arma) {
							armaencontrada = true;
							return;
						}
					}
				}
			//}
		} 				
	} 

	void buscaAgujero(Vector2 actual){
		// compruebo si es el barro de la derecha
		if (actual.x - 2 > 0 && (knowledge [(int)actual.x - 2, (int)actual.y] == type.sangreBarro || knowledge [(int)actual.x - 2, (int)actual.y] == type.barro)) {
			if((actual.x > 0 && knowledge [(int)actual.x - 1, (int)actual.y - 1] == type.barro || knowledge [(int)actual.x - 1, (int)actual.y - 1] == type.sangreBarro)||
				(actual.x < knowledge.GetLength(0) && knowledge [(int)actual.x + 1, (int)actual.y - 1] == type.barro || knowledge [(int)actual.x + 1, (int)actual.y - 1] == type.sangreBarro)){			
				knowledge [(int)actual.x - 1, (int)actual.y] = type.agujero;
			}
		} 
		// compruebo si es el barro de la izquierda
		if (actual.x + 2 <= board.GetLength (0) && (knowledge [(int)actual.x + 2, (int)actual.y] == type.sangreBarro || knowledge [(int)actual.x + 2, (int)actual.y] == type.barro)) {
			if((actual.x > 0 && knowledge [(int)actual.x - 1, (int)actual.y + 1] == type.barro || knowledge [(int)actual.x - 1, (int)actual.y + 1] == type.sangreBarro)||
				(actual.x < knowledge.GetLength(0) && knowledge [(int)actual.x + 1, (int)actual.y + 1] == type.barro || knowledge [(int)actual.x + 1, (int)actual.y + 1] == type.sangreBarro)){
				knowledge [(int)actual.x + 1, (int)actual.y] = type.agujero;
			}
		} 
		// compruebo si es el barro de la abajo
		if (actual.y - 2 > 0 && (knowledge [(int)actual.x, (int)actual.y - 2] == type.sangreBarro || knowledge [(int)actual.x, (int)actual.y - 2] == type.barro)) {
			if((actual.y > 0 && knowledge [(int)actual.x - 1, (int)actual.y - 1] == type.barro || knowledge [(int)actual.x - 1, (int)actual.y - 1] == type.sangreBarro)||
				(actual.y < knowledge.GetLength(1) && knowledge [(int)actual.x - 1, (int)actual.y + 1] == type.barro || knowledge [(int)actual.x - 1, (int)actual.y + 1] == type.sangreBarro)){
				knowledge [(int)actual.x, (int)actual.y - 1] = type.agujero;
			}
		} 
		// compruebo si es el barro de la arriba
		if (actual.x + 2 <= board.GetLength (1) && (knowledge [(int)actual.x, (int)actual.y + 2] == type.sangreBarro || knowledge [(int)actual.x, (int)actual.y + 2] == type.barro)){ 
			if((actual.y > 0 && knowledge [(int)actual.x + 1, (int)actual.y - 1] == type.barro || knowledge [(int)actual.x + 1, (int)actual.y - 1] == type.sangreBarro)||
				(actual.y < knowledge.GetLength(1) && knowledge [(int)actual.x + 1, (int)actual.y + 1] == type.barro || knowledge [(int)actual.x + 1, (int)actual.y + 1] == type.sangreBarro)){
				knowledge [(int)actual.x, (int)actual.y + 1] = type.agujero;
			}
		}
	}
	
	Vector2 arriesgate(){
		for(int i = 0; i < seguridad.GetLength(0); ++i){
			for (int j = 0; j < seguridad.GetLength (1); ++j) {
				// si no es peligrosa continuamos
				if (!seguridad [i, j])
					continue;
				// si lo es vemos si nos podemos arriesgar
				if (i - 1 >= 0 && knowledge [i - 1, j] == type.desconocido) {
					GameManager.instance.destapaCasilla (j, i - 1);
					return new Vector2 (i - 1, j);
				}
				if (j + 1 <= knowledge.GetLength (1) && knowledge [i, j + 1] == type.desconocido) {
					GameManager.instance.destapaCasilla (j + 1, i);
					return new Vector2 (i, j + 1);
				}
				if (j - 1 >= 0 && knowledge [i, j - 1] == type.desconocido) {
					GameManager.instance.destapaCasilla (j - 1, i);
					return new Vector2 (i, j - 1);
				}
				if (i + 1 < knowledge.GetLength (0) && knowledge [i + 1, j] == type.desconocido) {
					GameManager.instance.destapaCasilla (j, i + 1);
					return new Vector2 (i + 1, j);
				}
			}
		}
		return new Vector2(0,0);
	}

	public type[,] getKnowledge(){
		return knowledge;
	}
}	
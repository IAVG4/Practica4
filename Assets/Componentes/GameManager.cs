using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour {


    public static GameManager instance;

    struct PersonajeCasilla {
        public bool hayRefugio;
        public bool hayHeroe;
        public bool hayAliado;
        public bool hayZombi;
        public int numZumbis;
    };

    PersonajeCasilla[,] tablero = new PersonajeCasilla[5,10];

	public GameObject suelo;
    public GameObject refugio;
    public GameObject heroe;
    public GameObject aliado;
    public GameObject zombi;

    

    public GameObject ButtonReiniciar;
    public GameObject ButtonComenzar;

    bool heroeNoColocado = false;
    bool colocarCadaver = false;

	Vector2Int posHeroe;
    Vector2Int posRefugio;

	// Use this for initialization
	void Start () {
		instance = this;
		CreaTablero ();
		PonComisaria ();
	}
	
	// Update is called once per frame
	void Update () {
        
    }
    
	void CreaTablero() {
		int y = 0;
		for(int filas = 0; filas < tablero.GetLength(0); ++filas) {
			for(int columnas = 0; columnas < tablero.GetLength(1); ++columnas) {

                suelo.name = "Casilla_" + (filas + (columnas* tablero.GetLongLength(0)));

                tablero[filas, columnas].hayRefugio = false;
                tablero[filas, columnas].hayAliado = false;
                tablero[filas, columnas].hayHeroe = false;
                tablero[filas, columnas].hayZombi = false;
                tablero[filas, columnas].numZumbis = 0;

                suelo.transform.position = new Vector2(columnas, y);
                Instantiate (suelo, this.transform);
			}
			y = -filas - 1;
		}
	}

	void PonComisaria() {
		int randomFila = Random.Range(0, 5/2);
		int randomColumna = Random.Range (0, 10/2);

        tablero[randomFila, randomColumna].hayRefugio = true;
        refugio.transform.position = new Vector2(randomColumna, -randomFila);
		posRefugio = new Vector2Int(randomColumna, -randomFila);
		Instantiate(refugio, this.transform);
    }

	

	

    public void OnButtonReiniciarClick() { }

    public void OnClick(GameObject casillaPulsada)
    {
        if (!heroeNoColocado)
        {
            tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe = true;
            heroe.transform.position = new Vector2(casillaPulsada.transform.position.x, casillaPulsada.transform.position.y);
            posHeroe = new Vector2Int((int)casillaPulsada.transform.position.x, (int)casillaPulsada.transform.position.y);
            aliado.name = "Heroe_" + ((int)-casillaPulsada.transform.position.y + ((int)casillaPulsada.transform.position.x * tablero.GetLongLength(0)));
            Instantiate(heroe);
            heroeNoColocado = true;
        }

        else
        {
            // Si no hay aliado colocado en esa casilla
            if (!tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado &&
                !tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi)
            {
                tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe = false;
                tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado = true;
                aliado.transform.position = new Vector2(casillaPulsada.transform.position.x, casillaPulsada.transform.position.y);
                aliado.name = "Aliado_" + ((int)-casillaPulsada.transform.position.y + ((int)casillaPulsada.transform.position.x * tablero.GetLongLength(0)));
                Instantiate(aliado);
            }

            else
            {
                // Si no hay zombi colocado en esa casilla
                if (!tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi)
                {
                    GameObject aliade = GameObject.Find("Aliado_" + ((int)-casillaPulsada.transform.position.y + ((int)casillaPulsada.transform.position.x * tablero.GetLongLength(0))) + "(Clone)");
                    Destroy(aliade);
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi = true;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].numZumbis = 1;
                    zombi.transform.position = new Vector2(casillaPulsada.transform.position.x, casillaPulsada.transform.position.y);
                    zombi.name = "Zombi_" + ((int)-casillaPulsada.transform.position.y + ((int)casillaPulsada.transform.position.x * tablero.GetLongLength(0)));
                    Instantiate(zombi);
                }

                // Si habia zombi, vaciamos la casilla
                else
                {
                    GameObject zombie = GameObject.Find("Zombi_" + ((int)-casillaPulsada.transform.position.y + ((int)casillaPulsada.transform.position.x * tablero.GetLongLength(0))) + "(Clone)");
                    DestroyObject(zombie);
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].numZumbis = 0;
                }
            }


        }
    }

	int [,] traducirMatriz(int [,] original){
		int[,] traduccion = new int[original.GetLength (0), original.GetLength (1)];
		for (int i = 0; i < original.GetLength (0); ++i) {
			for (int j = 0; j < original.GetLength (1); ++j) {
				traduccion [j, i] = original [i, j];
			}
		}
		return traduccion;
	}

    Vector2Int traducirVector(Vector2Int vec){
		return new Vector2Int(vec.y, vec.x);
	}
		
	void printResult(List<Vector2Int> list){
		foreach (Vector2Int elem in list)
			Debug.Log (elem);
	}

	
	
}
	
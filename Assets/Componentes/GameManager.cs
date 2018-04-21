using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour {


	public static GameManager instance;

	public enum tipoCasilla { bloqueada, embarrada, embarrada_luz, embarrada_luz_sangre,
								embarrada_sangre, normal, normal_luz, normal_luz_sangre,
								normal_sangre }
	tipoCasilla [,] tablero = new tipoCasilla[5, 10];
	public bool[,] tablero01 = new bool[5, 10];
	string [,] tagCasillas = new string[5, 10];

	public GameObject casilla;
    public GameObject desconocida;

    public GameObject dead_body;
	public GameObject Karambit;
	public GameObject policeStation;
	public GameObject vigilante;

	public Sprite bloqueada;
	public Sprite embarrada;
	public Sprite embarrada_luz;
	public Sprite embarrada_luz_sangre;
	public Sprite embarrada_sangre;
	public Sprite normal;
	public Sprite normal_luz;
	public Sprite normal_luz_sangre;
	public Sprite normal_sangre;


	public GameObject ButtonAgujero;
    public GameObject ButtonCadaver;
    public GameObject ButtonReiniciar;
    public GameObject ButtonComenzar;

    bool colocarAgujeros = false;

	bool colocarCadaver = false;
    int numCadaver = 0;

	Vector2Int posCadaver;
    Vector2Int posComisaria;
    Vector2Int posKarambit;
	Vector2Int posVigilante;

	Queue<Vector2Int> caminoACadaver;
	Queue<Vector2Int> caminoAComisaria;

	// Use this for initialization
	void Start () {
		instance = this;
		caminoACadaver = new Queue<Vector2Int> ();
		caminoAComisaria = new Queue<Vector2Int> ();
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
				casilla.name = "Casilla_" + (filas + (columnas* tablero.GetLongLength(0)));
				casilla.GetComponent<SpriteRenderer>().sprite = normal;
				casilla.tag = "normal";
				tagCasillas[filas, columnas] = "normal";
				tablero[filas, columnas] = tipoCasilla.normal;
				casilla.transform.position = new Vector2(columnas, y);
                Instantiate (casilla, this.transform);
			}
			y = -filas - 1;
		}
	}

	void PonComisaria() {
		int randomFila = Random.Range(0, 5);
		int randomColumna = Random.Range (0, 10);

		policeStation.transform.position = new Vector2(randomColumna, -randomFila);
		posComisaria = new Vector2Int(randomColumna, -randomFila);
		Instantiate(policeStation, this.transform);
    }

	IEnumerator hacerElCamino() {

		foreach (Vector2Int posicion in caminoACadaver) {
		 //if (caminoACadaver.Count > 0) {
		 	//Vector2Int posicion = caminoACadaver.Dequeue();
			casilla = GameObject.Find("Casilla_" + (posicion.y + (posicion.x * tablero.GetLongLength(0))) + "(Clone)");
			desconocida = GameObject.Find("desconocida_" + (posicion.y + (posicion.x * tablero.GetLongLength(0))) + "(Clone)");
			DestroyObject(desconocida);
			posVigilante = posicion;
			vigilante = GameObject.Find ("vigilante(Clone)");
			vigilante.transform.position = new Vector3 (posicion.x, -posicion.y, 0);
			Karambit = GameObject.Find ("Karambit(Clone)");
			/*if(vigilante.transform.position == Karambit.transform.position) {
				DestroyObject (Karambit);
			}*/

			switch (tablero[posicion.y, posicion.x])
			{
			case tipoCasilla.normal:
				casilla.GetComponent<SpriteRenderer>().sprite = normal_luz;
				casilla.tag = "normal_luz";
				tagCasillas[posicion.y, posicion.x] = "normal_luz";
				tablero[posicion.y, posicion.x] = tipoCasilla.normal_luz;
			break;

			case tipoCasilla.embarrada:
				casilla.GetComponent<SpriteRenderer>().sprite = embarrada_luz;
				casilla.tag = "embarrada_luz";
				tagCasillas[posicion.y, posicion.x] = "embarrada_luz";
				tablero[posicion.y, posicion.x] = tipoCasilla.embarrada_luz;
			break;

			case tipoCasilla.normal_sangre:
				casilla.GetComponent<SpriteRenderer>().sprite = normal_luz_sangre;
				casilla.tag = "normal_luz_sangre";
				tagCasillas[posicion.y, posicion.x] = "normal_luz_sangre";
				tablero[posicion.y, posicion.x] = tipoCasilla.normal_luz_sangre;
			break;

			case tipoCasilla.embarrada_sangre:
				casilla.GetComponent<SpriteRenderer>().sprite = embarrada_luz_sangre;
				casilla.tag = "embarrada_luz_sangre";
				tagCasillas[posicion.y, posicion.x] = "embarrada_luz_sangre";
				tablero[posicion.y, posicion.x] = tipoCasilla.embarrada_luz_sangre;
			break;
			}
			yield return new WaitForSeconds (0.5f);
		}
	}

    public void destapaCasilla(int x, int y)
    {
		caminoACadaver.Enqueue (new Vector2Int(x, y));
    }
    public void TapaTablero()
    {
        ButtonComenzar.SetActive(false);
        ButtonReiniciar.SetActive(false);
		colocarAgujeros = false;
        int y = 0;
        for (int filas = 0; filas < tablero.GetLength(0); ++filas)
        {
            for (int columnas = 0; columnas < tablero.GetLength(1); ++columnas)
            {
                if (filas != -posComisaria.y || columnas != posComisaria.x) {
                    desconocida.name = "desconocida_" + (filas + (columnas * tablero.GetLongLength(0)));
                    desconocida.transform.position = new Vector2(columnas, y);
                    Instantiate(desconocida, this.transform);
                }
            }
            y = -filas - 1;
        }

        casilla = GameObject.Find("Casilla_" + (-posComisaria.y + (posComisaria.x * tablero.GetLongLength(0))) + "(Clone)");

        switch (tablero[-posComisaria.y, posComisaria.x])
        {
            case tipoCasilla.normal:
                casilla.GetComponent<SpriteRenderer>().sprite = normal_luz;
                casilla.tag = "normal_luz";
                tagCasillas[-posComisaria.y, posComisaria.x] = "normal_luz";
                tablero[-posComisaria.y, posComisaria.x] = tipoCasilla.normal_luz;
                break;

            case tipoCasilla.embarrada:
                casilla.GetComponent<SpriteRenderer>().sprite = embarrada_luz;
                casilla.tag = "embarrada_luz";
                tagCasillas[-posComisaria.y, posComisaria.x] = "embarrada_luz";
                tablero[-posComisaria.y, posComisaria.x] = tipoCasilla.embarrada_luz;
                break;

            case tipoCasilla.normal_sangre:
                casilla.GetComponent<SpriteRenderer>().sprite = normal_luz_sangre;
                casilla.tag = "normal_luz_sangre";
                tagCasillas[-posComisaria.y, posComisaria.x] = "normal_luz_sangre";
                tablero[-posComisaria.y, posComisaria.x] = tipoCasilla.normal_luz_sangre;
                break;

            case tipoCasilla.embarrada_sangre:
                casilla.GetComponent<SpriteRenderer>().sprite = embarrada_luz_sangre;
                casilla.tag = "embarrada_luz_sangre";
                tagCasillas[-posComisaria.y, posComisaria.x] = "embarrada_luz_sangre";
                tablero[-posComisaria.y, posComisaria.x] = tipoCasilla.embarrada_luz_sangre;
                break;
        }
		//new Vector3(posComisaria.x, posComisaria.y, 0)
		//Instantiate (vigilante, new Vector3(posComisaria.x, posComisaria.y, 0), this.transform);
		Instantiate(vigilante, new Vector3(posComisaria.x, posComisaria.y, 0), transform.rotation, this.transform);
		if(!this.GetComponent<DetectiveSearch>().buscaSolucion(traduceTablero(tablero), new Vector2 ((int)Mathf.Abs(posComisaria.y) , (int) posComisaria.x)))
			volverACasa ();
		StartCoroutine("hacerElCamino");
    }

	public void OnButtonAgujeroClick() {
		colocarAgujeros = true;
	}

	public void OnButtonCadaverClick() {
		colocarCadaver = true;
	}

    public void OnButtonReiniciarClick()
    {
        GameObject dead_body = GameObject.Find("dead_body(Clone)");
        DestroyObject(dead_body);
        GameObject karambit = GameObject.Find("Karambit(Clone)");
        DestroyObject(karambit);
        // GameObject policeStationClone = GameObject.Find("policeStation(Clone)");
        // DestroyObject(policeStationClone);
        for (int filas = 0; filas < tablero.GetLength(0); ++filas)
        {
            for (int columnas = 0; columnas < tablero.GetLength(1); ++columnas)
            {
                casilla = GameObject.Find("Casilla_" + (filas + (columnas * tablero.GetLongLength(0))) + "(Clone)");
                casilla.GetComponent<SpriteRenderer>().sprite = normal;
				casilla.tag = "normal";
				tagCasillas[filas, columnas] = "normal";
				tablero[filas, columnas] = tipoCasilla.normal;
            }

        }
        // PonComisaria();
        ButtonReiniciar.SetActive(false);
		ButtonAgujero.SetActive (false);
		ButtonCadaver.SetActive (true);
		numCadaver = 0;
		colocarCadaver = false;
		colocarAgujeros = false;
    }

    void ColocarCuchillo() {
        int randomPosCuchillo;
        bool cuchilloColocado = false;
        do
        {
            randomPosCuchillo = Random.Range(0, 4);
            switch(randomPosCuchillo)
            {
                case 0:
                    // Abajo Izquierda
                    if (((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) % 5 != 0 && ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) >= 0)
                    {
                       
                        casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1 - 5) + "(Clone)");
                        posKarambit = new Vector2Int((int)casilla.transform.position.x, (int)casilla.transform.position.y);
                        //Instantiate(Karambit, casilla.transform);
						Instantiate(Karambit, new Vector3(posKarambit.x, posKarambit.y, 0), transform.rotation, this.transform);
                        cuchilloColocado = true;
                    }
                    break;
                case 1:
                    // Abajo Derecha
                    if (((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) % 5 != 0 && ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) < 50)
                    {

                        casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1 + 5) + "(Clone)");
                        posKarambit = new Vector2Int((int)casilla.transform.position.x, (int)casilla.transform.position.y);
					Instantiate(Karambit, new Vector3(posKarambit.x, posKarambit.y, 0), transform.rotation, this.transform);
                        cuchilloColocado = true;
                    }
                    break;
                case 2:
                    // Arriba Izquierda
                    if (posCadaver.y + 1 <= 0 && ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) >= 0)
                    {

                        casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 1 - 5) + "(Clone)");
                        posKarambit = new Vector2Int((int)casilla.transform.position.x, (int)casilla.transform.position.y);
					Instantiate(Karambit, new Vector3(posKarambit.x, posKarambit.y, 0), transform.rotation, this.transform);
                        cuchilloColocado = true;
                    }
                    break;
                case 3:
                    // Arriba Derecha
                    if (posCadaver.y + 1 <= 0 && ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) < 50)
                    {

                        casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 1 + 5) + "(Clone)");
                        posKarambit = new Vector2Int((int)casilla.transform.position.x, (int)casilla.transform.position.y);
					Instantiate(Karambit, new Vector3(posKarambit.x, posKarambit.y, 0), transform.rotation, this.transform);
                        cuchilloColocado = true;
                    }
                    break;
            }
        }
        while (!cuchilloColocado);
           
    }
    public void OnClick(GameObject casillaPulsada) {
		if (colocarAgujeros) {
            if ((Vector2)casillaPulsada.transform.position != posComisaria && (Vector2)casillaPulsada.transform.position != posCadaver && (Vector2)casillaPulsada.transform.position != posKarambit)
            {

                Vector2Int posAgujero = new Vector2Int((int)casillaPulsada.transform.position.x, (int)casillaPulsada.transform.position.y);

                casilla = GameObject.Find("Casilla_" + (-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + "(Clone)");
                casilla.GetComponent<SpriteRenderer>().sprite = bloqueada;
				casilla.tag = "bloqueada";
				tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "bloqueada";
				tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.bloqueada;

                // Abajo
                if (((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 1) % 5 != 0)
                {
                    casilla = GameObject.Find("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 1) + "(Clone)");
                    // Con la de abajo
                    if ("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 1) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 1) + "(Clone)" ||
                        // Con la de la izquierda
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 1) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) + "(Clone)" ||
                        // Con la de la derecha
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 1) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) + "(Clone)")
                    {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {
							casilla.GetComponent<SpriteRenderer> ().sprite = embarrada_sangre;
							casilla.tag = "embarrada_sangre";
							tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada_sangre";
							tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada_sangre;
						}
                    }
                    
                    else {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {
							casilla.GetComponent<SpriteRenderer> ().sprite = embarrada;
							casilla.tag = "embarrada";
							tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada";
							tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada;
						}
						
                    }
                }

                //Arriba
                if (posAgujero.y + 1 <= 0)
                {
                    casilla = GameObject.Find("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 1) + "(Clone)");
                        // Con la de arriba
                    if ("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 1) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) + "(Clone)" ||
                        // Con la de la izquierda
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 1) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) + "(Clone)" ||
                        // Con la de la derecha
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 1) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) + "(Clone)")
                    {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {
							casilla.GetComponent<SpriteRenderer> ().sprite = embarrada_sangre;
							casilla.tag = "embarrada_sangre";
							tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada_sangre";
							tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada_sangre;
						}
					}

					else {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {
							casilla.GetComponent<SpriteRenderer> ().sprite = embarrada;
							casilla.tag = "embarrada";
							tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada";
							tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada;
						}

					}
                }

                //Izquierda
                if (((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 5) >= 0)
                {
                    // Con la izquierda
                    casilla = GameObject.Find("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 5) + "(Clone)");
                    if ("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 5) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) + "(Clone)" ||
                        // Con lo de arriba
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 5) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) + "(Clone)" ||
                        // Con lo de abajo
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) - 5) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 1) + "(Clone)")
                    {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {

                            if (posCadaver.y + 1 <= 0) {
                                casilla.GetComponent<SpriteRenderer>().sprite = embarrada_sangre;
                                casilla.tag = "embarrada_sangre";
                                tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada_sangre";
                                tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada_sangre;
                            }

                            else
                            {
                                casilla.GetComponent<SpriteRenderer>().sprite = embarrada;
                                casilla.tag = "embarrada";
                                tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada";
                                tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada;
                            }
						}
					}

					else {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {
							casilla.GetComponent<SpriteRenderer> ().sprite = embarrada;
							casilla.tag = "embarrada";
							tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada";
							tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada;
						}

					}
                }

                //Derecha
                if (((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 5) < 50)
                {
                    casilla = GameObject.Find("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 5) + "(Clone)");
                    
                    // Con la derecha
                    if ("Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 5) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) + "(Clone)" ||
                        // Con lo de arriba
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 5) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) + "(Clone)" ||
                        // Con lo de abajo
                        "Casilla_" + ((-posAgujero.y + (posAgujero.x * tablero.GetLongLength(0))) + 5) + "(Clone)" ==
                        "Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 1) + "(Clone)")
                    {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {

                            if (((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) % 5 != 0)
                            {
                                casilla.GetComponent<SpriteRenderer>().sprite = embarrada_sangre;
                                casilla.tag = "embarrada_sangre";
                                tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada_sangre";
                                tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada_sangre;
                            }

                            else
                            {
                                casilla.GetComponent<SpriteRenderer>().sprite = embarrada;
                                casilla.tag = "embarrada";
                                tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada";
                                tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada;
                            }
						}

					}

					else {
						if (casilla.GetComponent<SpriteRenderer> ().sprite != bloqueada) {
							casilla.GetComponent<SpriteRenderer> ().sprite = embarrada;
							casilla.tag = "embarrada";
							tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "embarrada";
							tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.embarrada;
						}

					}
                }

                ButtonAgujero.SetActive(false);
                ButtonComenzar.SetActive(true);
            }
        }
		if (colocarCadaver) {
            if ((Vector2)casillaPulsada.transform.position != posComisaria && numCadaver < 1)
            {
                Instantiate(dead_body, casillaPulsada.transform);
                posCadaver = new Vector2Int((int)casillaPulsada.transform.position.x, (int)casillaPulsada.transform.position.y);
                //Vamos a poner las casillas de sagre

                // Abajo
                if (((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) % 5 != 0)
                {

                    casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 1) + "(Clone)");
                    casilla.GetComponent<SpriteRenderer>().sprite = normal_sangre;
					casilla.tag = "normal_sangre";
					tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "normal_sangre";
					tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.normal_sangre;
                }
                
                //Arriba
                if (posCadaver.y + 1 <= 0)
                {
                    casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 1) + "(Clone)");
                    casilla.GetComponent<SpriteRenderer>().sprite = normal_sangre;
					casilla.tag = "normal_sangre";
					tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "normal_sangre";
					tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.normal_sangre;
                }

                //Izquierda
                if (((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) >= 0)
                {
                    casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) - 5) + "(Clone)");
                    casilla.GetComponent<SpriteRenderer>().sprite = normal_sangre;
					casilla.tag = "normal_sangre";
					tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "normal_sangre";
					tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.normal_sangre;
                }

                //Derecha
                if (((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) < 50)
                {
                    casilla = GameObject.Find("Casilla_" + ((-posCadaver.y + (posCadaver.x * tablero.GetLongLength(0))) + 5) + "(Clone)");
                    casilla.GetComponent<SpriteRenderer>().sprite = normal_sangre;
					casilla.tag = "normal_sangre";
					tagCasillas[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = "normal_sangre";
					tablero[(int)-casilla.transform.position.y, (int)casilla.transform.position.x] = tipoCasilla.normal_sangre;
                }

                ColocarCuchillo();

                numCadaver++;
                colocarCadaver = false;
                ButtonCadaver.SetActive(false);
                ButtonReiniciar.SetActive(true);
                ButtonAgujero.SetActive(true);
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

	type[,] traduceTablero(tipoCasilla [,] tablero){
		type[,] traduccion = new type[5, 10];
		for (int i = 0; i < 5; ++i) {
			for (int j = 0; j < 10; ++j) {
				switch (tablero [i, j]) 
				{
				case tipoCasilla.bloqueada:
					traduccion [i, j] = type.agujero;
					break;
				case tipoCasilla.embarrada:
					traduccion [i, j] = type.barro;
					break;
				case tipoCasilla.embarrada_sangre:
					traduccion [i, j] = type.sangreBarro;
					break;
				case tipoCasilla.normal:
					traduccion [i, j] = type.vacia;
					break;
				case tipoCasilla.normal_sangre:
					traduccion [i, j] = type.sangre;
					break;
				}
			}
		}
		traduccion [(int)Mathf.Abs(posComisaria.y), (int)posComisaria.x] = type.vacia;
		traduccion [(int)Mathf.Abs(posCadaver.y), (int)posCadaver.x] = type.cadaver;
		traduccion [(int)Mathf.Abs(posKarambit.y), (int)posKarambit.x] = type.arma;

		return traduccion;
	}

	// Rellena la lista caminoAComisaria con el camino de vuelta a la comisaria
	void volverACasa(){
		type[,] knowledge = this.GetComponent<DetectiveSearch> ().getKnowledge ();
		int[,] traduccion = new int[knowledge.GetLength (0), knowledge.GetLength (1)];
		for (int j = 0; j < knowledge.GetLength (1); ++j) {
			for (int i = 0; i < knowledge.GetLength (0); ++i) {
				if (knowledge [i, j] == type.agujero || knowledge [i, j] == type.desconocido)
					traduccion [i, j] = 0;
				else
					traduccion [i, j] = 1;
			}
		}
		List<Vector2> caminoACasa;
		caminoACasa = new List<Vector2>();
		caminoACasa = this.GetComponent<AStar> ().calculatePath (traduccion, new Vector2 ((int)-posKarambit.y, (int)posKarambit.x), new Vector2((int)-posComisaria.y, (int)posComisaria.x));
		foreach(Vector2 coord in caminoACasa){
			caminoACadaver.Enqueue (new Vector2Int((int)coord.y, (int)coord.x));
		}
	}
}
	
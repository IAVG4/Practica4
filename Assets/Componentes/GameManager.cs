using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public struct PersonajeCasilla {
	public bool hayRefugio;
	public bool hayHeroe;
	public bool hayAliado;
	public bool hayZombi;
	public int numZumbis;
};

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public int puntuacion = 0;  // +1 por zombi muerto (ó +5 si lo mató el héroe)
                                // ○ -10 por aliado muerto (ó -50 si muere el héroe)
    bool finPartida = false;    // La partida termina al morir el héroe, o al entrar este en el refugio

    PersonajeCasilla[,] tablero = new PersonajeCasilla[5,10];

	public GameObject suelo;
    public GameObject refugio;
    public GameObject heroe;

    public GameObject aliado;
    public List<GameObject> listaAliados;

    public GameObject zombi;
    public List<GameObject> listaZombis;

    const int MAX_ALIADOS = 5;
    const int MAX_ZOMBIS = 20;

    int num_aliados = 0;
    int num_zombis = 0;

    public GameObject ButtonReiniciar;
    public GameObject ButtonComenzar;
    public Text textPuntuacion;

    bool heroeNoColocado = false;

	Vector2Int posHeroe;
    Vector2Int posRefugio;

    public bool deDia = true;

	// Use this for initialization
	void Start () {
        ButtonComenzar.SetActive(false);
        ButtonReiniciar.SetActive(false);
		instance = this;
		CreaTablero ();
		PonComisaria ();
	}
	
	// Update is called once per frame
	void Update () {
        // Si pulsamos click derecho, cambiamos entre de dia y de noche
        if (Input.GetMouseButtonDown(1))
            deDia = !deDia;

        

        textPuntuacion.text = "Puntos: " + puntuacion;
    }

    void TurnoZombis()
    {
        for (int i = 0; i < listaZombis.Count; i++)
        {
            GameObject zombiAt = listaZombis[i];

            tablero[(int)-zombiAt.transform.position.y, (int)zombiAt.transform.position.x].numZumbis--;
            if (tablero[(int)-zombiAt.transform.position.y, (int)zombiAt.transform.position.x].numZumbis == 0)
                tablero[(int)-zombiAt.transform.position.y, (int)zombiAt.transform.position.x].hayZombi = false;

            zombiAt.GetComponent<MoveZombi>().InvokeMovimientoZombi();
        }
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

                suelo.transform.position = new Vector3(columnas, y, 0);
                Instantiate (suelo, this.transform);
                
			}
			y = -filas - 1;
		}

        
	}

	void PonComisaria() {
		int randomFila = Random.Range(0, 5/2);
		int randomColumna = Random.Range (0, 10/2);

        tablero[randomFila, randomColumna].hayRefugio = true;
        refugio.transform.position = new Vector3(randomColumna, -randomFila, 0);
		posRefugio = new Vector2Int(randomColumna, -randomFila);
		Instantiate(refugio, this.transform);
    }

	public void ComienzaLaPartida() {
        for (int filas = 0; filas < tablero.GetLength(0); ++filas)
        {
            for (int columnas = 0; columnas < tablero.GetLength(1); ++columnas)
            {
                GameObject casilla = GameObject.Find("Casilla_" + (filas + (columnas * tablero.GetLongLength(0))) + "(Clone)");
                casilla.GetComponent<BoxCollider2D>().enabled = false;

            }
        }

        TurnoZombis();
    }

    public void OnButtonReiniciarClick() {
        for (int filas = 0; filas < tablero.GetLength(0); ++filas)
        {
            for (int columnas = 0; columnas < tablero.GetLength(1); ++columnas)
            {
                GameObject casilla = GameObject.Find("Casilla_" + (filas + (columnas * tablero.GetLongLength(0))) + "(Clone)");
                casilla.GetComponent<BoxCollider2D>().enabled = true;

                tablero[filas, columnas].hayRefugio = false;
                tablero[filas, columnas].hayAliado = false;
                tablero[filas, columnas].hayHeroe = false;
                tablero[filas, columnas].hayZombi = false;
                tablero[filas, columnas].numZumbis = 0;

                GameObject heroee = GameObject.Find("Heroe(Clone)");
                DestroyObject(heroee);
                heroeNoColocado = false;

                for (int i = 0; i < listaAliados.Count; i++)
                {
                        GameObject aliadoDestruido = listaAliados[i];

                        tablero[(int)-aliadoDestruido.transform.position.y, (int)aliadoDestruido.transform.position.x].hayAliado = false;

                        listaAliados.Remove(aliadoDestruido);
                        num_aliados--;
                        Destroy(aliadoDestruido);
                }

                for (int i = 0; i < listaZombis.Count; i++)
                {
                        GameObject zombiDestruido = listaZombis[i];

                        tablero[(int)-zombiDestruido.transform.position.y, (int)zombiDestruido.transform.position.x].hayAliado = false;
                        tablero[(int)-zombiDestruido.transform.position.y, (int)zombiDestruido.transform.position.x].numZumbis = 0;

                        listaZombis.Remove(zombiDestruido);
                        num_zombis--;
                        Destroy(zombiDestruido);          
                }

            }
        }
        ButtonComenzar.SetActive(false);
        ButtonReiniciar.SetActive(false);
    }

    public void OnClick(GameObject casillaPulsada)
    {
       
        if (!heroeNoColocado)
        {
            tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe = true;
            heroe.transform.position = new Vector2(casillaPulsada.transform.position.x, casillaPulsada.transform.position.y);
            posHeroe = new Vector2Int((int)casillaPulsada.transform.position.x, (int)casillaPulsada.transform.position.y);
            heroe.name = "Heroe";
            Instantiate(heroe);
            heroeNoColocado = true;
            ButtonComenzar.SetActive(true);
            ButtonReiniciar.SetActive(true);

        }

        else
        {
            // Si no hay aliado colocado en esa casilla
            if (!tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe &&
                !tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado &&
                !tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi && 
                num_aliados < MAX_ALIADOS)
            {
      
                tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado = true;

                num_aliados++;
                
                aliado.transform.position = new Vector2(casillaPulsada.transform.position.x, casillaPulsada.transform.position.y);
                aliado.name = "Aliado_" + num_aliados;

                GameObject CopiaAliado = Instantiate(aliado);
                CopiaAliado.GetComponent<Aliado>().ConstructoraAliado((Vector2)aliado.transform.position, num_aliados);
                CopiaAliado.name = "Aliado_" + num_aliados;
                listaAliados.Add(CopiaAliado);

            }

            else
            {
                
                // Si no hay zombi colocado en esa casilla
                if (!tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi && 
                    num_zombis < MAX_ZOMBIS)
                {
                    for (int i = 0; i < listaAliados.Count; i++)
                    {
                        
                        if (listaAliados[i].GetComponent<Aliado>().getPosicion() == (Vector2)casillaPulsada.transform.position)
                        {
                            GameObject aliadoDestruido = listaAliados[i];
                            
                            listaAliados.Remove(aliadoDestruido);
                            num_aliados--;
                            Destroy(aliadoDestruido);
                        }

                        else
                            listaAliados[i].name = "Aliado_" + (i + 1);
                    }

                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayHeroe = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayAliado = false;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].hayZombi = true;
                    tablero[(int)-casillaPulsada.transform.position.y, (int)casillaPulsada.transform.position.x].numZumbis = 1;

                    num_zombis++;

                    zombi.transform.position = new Vector2(casillaPulsada.transform.position.x, casillaPulsada.transform.position.y);
                    zombi.name = "Zombi_" + num_zombis;

                    GameObject CopiaZombi = Instantiate(zombi);
                    CopiaZombi.GetComponent<MoveZombi>().ConstructoraZombi((Vector2)zombi.transform.position, num_zombis);

                    listaZombis.Add(CopiaZombi);
                }

                // Si habia zombi, vaciamos la casilla
                else
                {
                    for (int i = 0; i < listaZombis.Count; i++)
                    {

                        if (listaZombis[i].GetComponent<MoveZombi>().getPosicion() == (Vector2)casillaPulsada.transform.position)
                        {
                            GameObject zombiDestruido = listaZombis[i];

                            listaZombis.Remove(zombiDestruido);
                            num_zombis--;
                            Destroy(zombiDestruido);
                        }

                        else
                            listaZombis[i].name = "Zombi_" + (i + 1);
                    }
                    
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

	public List<Vector2> getZombies(){
		List<Vector2> zombies = new List<Vector2> ();
		for (int i = 0; i < listaZombis.Count; ++i)
			zombies.Add (new Vector2 (-listaZombis [i].transform.position.y, listaZombis [i].transform.position.x));
		
			return zombies;
	}
	public Vector2 getRefugio(){
		return new Vector2 ((int)-refugio.transform.position.y, (int)refugio.transform.position.x);
	}

	public int getNumZombies(){
		return num_zombis;
	}

	public int getNumAliados(){
		return num_aliados;
	}
	public bool esDeDia(){
		return deDia;
	}

	public PersonajeCasilla getPersonaje(Vector2 pos){
		return tablero [(int)-pos.y, (int)pos.x];
	}
}
	
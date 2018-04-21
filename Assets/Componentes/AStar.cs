using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;


public class AStar : MonoBehaviour {
	// Use this for initialization
	// Variables para mantener las dimensiones del tablero
	public static int tableroWidth = 10;
	public static int tableroHeight = 5;
	public static int tamañoTablero = 50;

	Node[,] tableroNode;
	bool [,] marked;

	// Lista de Node abiertos
	FastPriorityQueue<Node> open;


    //Vector2 pathStart, pathEnd;


    // Hay que inicializarlo
    int [,] tablero = new int[5, 10];


	public class Node : FastPriorityQueueNode
	{
		public enum state {normal, open, closed};
		public Node(int _x, int _y, int _h, int _cost)
        {
			x = _x;
			y = _y;
			pos = new Vector2(x,y);
			h = _h;
			cost = _cost;
			f = h + cost;
			estado = state.normal;
			Parent = null;
        }

		public Vector2 pos;
		int x, y; // posicion
		public int h; // Distance from a node to the target node
		public int cost; // lo que cuesta pasar
		public int g; // parent.g + cost;
		public int f; // h + g;
		public state estado;
		public Node Parent;
	};
	
	void Start () {
		
		tableroWidth = 10;
		tableroHeight = 5;
		tamañoTablero = 50;

		tableroNode = new Node[tablero.GetLength (0), tablero.GetLength (1)];
		marked = new bool [5, 10];

		open = new FastPriorityQueue<Node> (tablero.GetLength (0) * tablero.GetLength (1));
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Hay que resetear la Astar cada vez que se llame, reseatear todo las variables a valor del start
	public List<Vector2> calculatePath(int [,] tablero, Vector2 pathStart, Vector2 pathEnd) {
		fillMarked (tablero);
		fillTableroNode (tablero, pathEnd);

		tableroNode [(int)pathStart.x, (int)pathStart.y].g = 0;
		tableroNode [(int)pathStart.x, (int)pathStart.y].estado = Node.state.open;
		// referencia a un nodo que se va a considerar a continuacion
		Node myNode = tableroNode [(int)pathStart.x, (int)pathStart.y];
		// Nodo que vamos a usar para guardar el nodo final
		Node aux = null;
		open.Enqueue (myNode, myNode.g);
		while (open.Count != 0) {
			myNode = open.Dequeue ();
			myNode.estado = Node.state.closed;
			marked [(int)myNode.pos.x, (int)myNode.pos.y] = true;
			aux = addNeighbours (myNode, pathEnd);
			if (aux != null) {
				open.Clear ();
				return getPath(aux);
			}
		}
		return null;
	}

	int ManhattanDistance(Vector2 start, Vector2 goal) {
		return Mathf.Abs ((int)start.x -(int) goal.x) + Mathf.Abs ((int)start.y - (int)goal.y);
	}

	Node addNeighbours(Node node, Vector2 goal){
		Vector2 N = node.pos + new Vector2 (0, -1);
		Vector2 S = node.pos + new Vector2(0, 1);
		Vector2 E = node.pos + new Vector2(1, 0);
		Vector2 W = node.pos + new Vector2(-1, 0);
	bool myN = N.y >= 0 && !marked[(int)N.x, (int)N.y];
	bool myS = S.y < tableroWidth && !marked[(int)S.x, (int)S.y];
	bool myE = E.x < tableroHeight && !marked[(int)E.x, (int)E.y];
	bool myW = W.x >= 0 && !marked[(int)W.x, (int)W.y];

		int auxg = 0, auxf = 0;
		if (myN) {
		if (tableroNode [(int)N.x, (int)N.y].estado == Node.state.normal) {
			tableroNode [(int)N.x, (int)N.y].g = node.g + tableroNode [(int)N.x, (int)N.y].cost;
			tableroNode [(int)N.x, (int)N.y].f = tableroNode [(int)N.x, (int)N.y].g + tableroNode [(int)N.x, (int)N.y].h; 
			tableroNode [(int)N.x, (int)N.y].estado = Node.state.open;
			tableroNode [(int)N.x, (int)N.y].Parent = node;
			open.Enqueue (tableroNode [(int)N.x, (int)N.y], tableroNode [(int)N.x, (int)N.y].f);
			} else {
			auxg = node.g + tableroNode [(int)N.x, (int)N.y].cost;
			auxf = auxg + tableroNode [(int)N.x, (int)N.y].h; 
			if (auxf < tableroNode [(int)N.x, (int)N.y].f) {
				tableroNode [(int)N.x, (int)N.y].g = auxg;
				tableroNode [(int)N.x, (int)N.y].f = auxf;
				tableroNode [(int)N.x, (int)N.y].Parent = node;
				}
			}
		if (tableroNode [(int)N.x, (int)N.y].pos == goal)
			return tableroNode [(int)N.x, (int)N.y];
		}

		if (myS) {
		if (tableroNode [(int)S.x, (int)S.y].estado == Node.state.normal) {
			tableroNode [(int)S.x, (int)S.y].g = node.g + tableroNode [(int)S.x, (int)S.y].cost;
			tableroNode [(int)S.x, (int)S.y].f = tableroNode [(int)S.x, (int)S.y].g + tableroNode [(int)S.x, (int)S.y].h; 
			tableroNode [(int)S.x, (int)S.y].estado = Node.state.open;
			tableroNode [(int)S.x, (int)S.y].Parent = node;
			open.Enqueue (tableroNode [(int)S.x, (int)S.y], tableroNode [(int)S.x, (int)S.y].f);
			} else {
			auxg = node.g + tableroNode [(int)S.x, (int)S.y].cost;
			auxf = auxg + tableroNode [(int)S.x, (int)S.y].h; 
			if (auxf < tableroNode [(int)S.x, (int)S.y].f) {
				tableroNode [(int)S.x, (int)S.y].g = auxg;
				tableroNode [(int)S.x, (int)S.y].f = auxf;
				tableroNode [(int)S.x, (int)S.y].Parent = node;
				}
			}
		if (tableroNode [(int)S.x, (int)S.y].pos == goal)
			return tableroNode [(int)S.x, (int)S.y];
		}

		if (myE) {
			if (tableroNode [(int)E.x, (int)E.y].estado == Node.state.normal) {
				tableroNode [(int)E.x, (int)E.y].g = node.g + tableroNode [(int)E.x, (int)E.y].cost;
				tableroNode [(int)E.x, (int)E.y].f = tableroNode [(int)E.x, (int)E.y].g + tableroNode [(int)E.x, (int)E.y].h; 
				tableroNode [(int)E.x, (int)E.y].estado = Node.state.open;
				tableroNode [(int)E.x, (int)E.y].Parent = node;
				open.Enqueue (tableroNode [(int)E.x, (int)E.y], tableroNode [(int)E.x, (int)E.y].f);
			} else {
				auxg = node.g + tableroNode [(int)E.x, (int)E.y].cost;
				auxf = auxg + tableroNode [(int)E.x, (int)E.y].h; 
				if (auxf < tableroNode [(int)E.x, (int)E.y].f) {
					tableroNode [(int)E.x, (int)E.y].g = auxg;
					tableroNode [(int)E.x, (int)E.y].f = auxf;
					tableroNode [(int)E.x, (int)E.y].Parent = node;
				}
			}
			if (tableroNode [(int)E.x, (int)E.y].pos == goal)
				return tableroNode [(int)E.x, (int)E.y];
		}
		if (myW) {
		if (tableroNode [(int)W.x,(int) W.y].estado == Node.state.normal) {
			tableroNode [(int)W.x, (int)W.y].g = node.g + tableroNode [(int)W.x, (int)W.y].cost;
			tableroNode [(int)W.x, (int)W.y].f = tableroNode [(int)W.x, (int)W.y].g + tableroNode [(int)W.x, (int)W.y].h; 
			tableroNode [(int)W.x, (int)W.y].estado = Node.state.open;
			tableroNode [(int)W.x, (int)W.y].Parent = node;
			open.Enqueue (tableroNode [(int)W.x, (int)W.y], tableroNode [(int)W.x, (int)W.y].f);
			} else {
			auxg = node.g + tableroNode [(int)W.x, (int)W.y].cost;
			auxf = auxg + tableroNode [(int)W.x, (int)W.y].h; 
			if (auxf < tableroNode [(int)W.x, (int)W.y].f) {
				tableroNode [(int)W.x, (int)W.y].g = auxg;
				tableroNode [(int)W.x, (int)W.y].f = auxf;
				tableroNode [(int)W.x, (int)W.y].Parent = node;
				}
			}
		if (tableroNode [(int)W.x, (int)W.y].pos == goal)
			return tableroNode [(int)W.x, (int)W.y];
		}
		Node empty = null;
		return empty;
	} 

	void fillMarked(int [,] tablero){
		for (int i = 0; i < tablero.GetLength (0); ++i) {
			for(int j = 0; j < tablero.GetLength(1); ++j){
				if (tablero [i, j] == 0)
					marked [i, j] = true;
				else
					marked [i, j] = false;
			}
		}
	}

	void fillTableroNode(int [,] tablero, Vector2 fin){
		for (int i = 0; i < tablero.GetLength (0); ++i) {
			for(int j = 0; j < tablero.GetLength(1); ++j){
				if (!marked [i, j])
					tableroNode [i, j] = 
						new Node (i, j, ManhattanDistance (new Vector2 (i, j), fin), tablero[i,j]);
			}
		}
	}

	List<Vector2> getPath(Node node){
		List<Vector2> result = new List<Vector2>();
		while(node.Parent != null){
			result.Add (node.pos);
			node = node.Parent;
		}
		// Introducir el nodo inicial ya que al tener padre null no es introducido en el bucle anterior
		result.Add (node.pos);
		result.Reverse ();
		return result;
	} 
}

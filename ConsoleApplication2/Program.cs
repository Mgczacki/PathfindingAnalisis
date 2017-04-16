using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public class Node
    {
        public Dictionary<Node, int> ac;
        public int posX, posY;

    }
    public class Rectangle
    {
        public void assign(int posaX, int posaY, int posbX, int posbY)
        {
            this.posaX = posaX;
            this.posaY = posaY;
            this.posbX = posbX;
            this.posbY = posbY;
        }
        public int posaX, posaY, posbX, posbY;
        public bool containsStart, containsGoal;
        public int getSignificantX(int startx, int goalx)
        {
            if (containsStart)
            {
                return startx;
            }
            if (containsGoal)
            {
                return goalx;
            }
            return posaX + ((posbX - posaX) / 2);
        }
        public int getSignificantY(int starty, int goaly)
        {
            if (containsStart)
            {
                return starty;
            }
            if (containsGoal)
            {
                return goaly;
            }
            return posaY + ((posbY - posaY) / 2);
        }
    }
    public class grafo
    {
        public Node[,] nodes;
        public int size;
        public Node start, goal;
    }
    public class Mapa
    {
        public static Random r = new Random();
        public bool[,] array;
        public int size, entradax, entraday, salidax, saliday;
        public Mapa Clone()
        {
            Mapa m = new Mapa();
            m.array = (bool[,])array.Clone();
            m.size = size;
            m.entradax = entradax;
            m.entraday = entraday;
            m.salidax = salidax;
            m.saliday = saliday;
            return m;
        }
        public void randomizeMapa()
        {//Genera un mapa con obstaculos aleatorios y entrada y salida no obstaculos.
            //Console.WriteLine("RSTART");
            entradax = r.Next(0, size);
            entraday = r.Next(0, size);
            array = new bool[size, size];
            do
            {
                salidax = r.Next(0, size);
                saliday = r.Next(0, size);
            } while (salidax == entradax && saliday == entraday);
            int root = r.Next(1, 10);//Porcentaje de posibilidad de generar un obstaculo... Aleatorio nomas porque si.
            for (int x = 0; x < size; x++)//Generacion de obstaculos...
            {
                for (int y = 0; y < size; y++)
                {

                    if (r.Next(0, 10) < root)
                    {
                        array[x, y] = true;
                    }

                }
            }
            array[entradax, entraday] = false;
            array[salidax, saliday] = false;
            //Console.WriteLine("REND");
        }
        public grafo convertToGrafo()
        {
            //Console.WriteLine("GSTART");
            grafo res = new grafo();
            res.nodes = new Node[size, size];
            res.size = size;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (!array[x, y])//true is obstacle, therefore false is free = node.
                    {
                        res.nodes[x, y] = new Node();
                        res.nodes[x, y].ac = new Dictionary<Node, int>();
                        res.nodes[x, y].posX = x;
                        res.nodes[x, y].posY = y;
                    }
                    else
                    {
                        res.nodes[x, y] = null;
                    }

                }
            }
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (res.nodes[x, y] != null)
                    {
                        int xs = -1;
                        int xe = 1;
                        int ys = -1;
                        int ye = 1;
                        if (x == 0)
                        {
                            xs = 0;
                        }
                        if (x == size - 1)
                        {
                            xe = 0;
                        }
                        if (y == 0)
                        {
                            ys = 0;
                        }
                        if (y == size - 1)
                        {
                            ye = 0;
                        }
                        for (int p = x + xs; p <= x + xe; p++)
                        {
                            for (int q = y + ys; q <= y + ye; q++)
                            {
                                if (res.nodes[p, q] != null && !(p == x && q == y))
                                {
                                    /*if (p != x && q != y)
                                    {
                                        res.nodes[x, y].ac.Add(res.nodes[p, q], 14);
                                    }
                                    else
                                    {
                                        res.nodes[x, y].ac.Add(res.nodes[p, q], 10);
                                    }*/
                                    if (!(p != x && q != y))
                                    {
                                        res.nodes[x, y].ac.Add(res.nodes[p, q], 1);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            /* //Si llega a existir un nodo completamente aislado, se elimina...
             for (int x = 0; x < size; x++)
             {
                 for (int y = 0; y < size; y++)
                 {
                     if(res.nodes[x,y]!= null && res.nodes[x,y].ac.Count == 0)
                     {
                         res.nodes[x, y] = null;
                     }
                 }
             }*/
            res.start = res.nodes[entradax, entraday];
            res.goal = res.nodes[salidax, saliday];
            //Console.WriteLine("GEND");
            return res;
        }
    }
    class Program
    {
        public static List<int> posNodes = new List<int>();
        static public int getManhattan(int posaX, int posbX, int posaY, int posbY)
        {
            return (Math.Abs(posaX - posbX) + Math.Abs(posaY - posbY));
        }
        static public List<Node> Dijkstra(grafo g)
        {
            List<Node> unvisited = new List<Node>();
            Node goal = g.goal;
            Node start = g.start;
            Node current = start;
            List<Node> discovered = new List<Node>();
            Node[,] parent = new Node[g.size, g.size];
            int mindistance;
            foreach (Node n in g.nodes)
            {
                if (n != null)
                {
                    unvisited.Add(n);
                }
            }
            int[,] gVal = new int[g.size, g.size];
            for (int x = 0; x < g.size; x++)
            {
                for (int y = 0; y < g.size; y++)
                {
                    /*if (g.nodes[x, y] != null)
                    {*/
                    gVal[x, y] = 100000000;
                    //}
                }
            }
            gVal[current.posX, current.posY] = 0;
            parent[current.posX, current.posY] = current;
            discovered.Add(current);
            do
            {
                foreach (KeyValuePair<Node, int> n in current.ac)
                {
                    if (!discovered.Contains(n.Key))
                    {
                        discovered.Add(n.Key);
                    }
                    if (n.Value + gVal[current.posX, current.posY] < gVal[n.Key.posX, n.Key.posY])
                    {
                        gVal[n.Key.posX, n.Key.posY] = n.Value + gVal[current.posX, current.posY];
                        parent[n.Key.posX, n.Key.posY] = current;
                    }
                }
                unvisited.Remove(current);
                mindistance = 100000000;
                foreach (Node n in unvisited)
                {
                    if (gVal[n.posX, n.posY] < mindistance)
                    {
                        mindistance = gVal[n.posX, n.posY];
                        current = n;
                    }
                }
                if (mindistance == 100000000 && unvisited.Contains(goal))
                {
                    Console.WriteLine("DERROR:Unreachable destination");
                    return null;
                }
            } while (unvisited.Contains(goal));
            List<Node> Path = new List<Node>();
            current = goal;
            do
            {
                Path.Add(current);
                current = parent[current.posX, current.posY];
            } while (current != start);
            Path.Add(start);
            posNodes.Add(discovered.Count);
            return Path;
        }
        static public List<Node> AStar(grafo g)
        {
            List<Node> unvisited = new List<Node>();
            //int sNodes = 0;
            Node goal = g.goal;
            Node start = g.start;
            Node current = g.start;
            int mindistance;
            int[,] gVal = new int[g.size, g.size];
            int[,] fVal = new int[g.size, g.size];
            int[,] hVal = new int[g.size, g.size];
            Node[,] parent = new Node[g.size, g.size];
            foreach (Node n in g.nodes)
            {
                if (n != null)
                {
                    unvisited.Add(n);
                    gVal[n.posX, n.posY] = 100000000;
                    fVal[n.posX, n.posY] = 100000000;
                    hVal[n.posX, n.posY] = getManhattan(n.posX, goal.posX, n.posY, goal.posY);
                }
            }
            gVal[current.posX, current.posY] = 0;
            fVal[current.posX, current.posY] = hVal[current.posX, current.posY] + gVal[current.posX, current.posY];
            parent[current.posX, current.posY] = current;
            do
            {
                foreach (KeyValuePair<Node, int> n in current.ac)
                {
                    if (n.Value + hVal[n.Key.posX, n.Key.posY] + gVal[current.posX, current.posY] < fVal[n.Key.posX, n.Key.posY])
                    {
                        gVal[n.Key.posX, n.Key.posY] = n.Value + gVal[current.posX, current.posY];
                        fVal[n.Key.posX, n.Key.posY] = gVal[n.Key.posX, n.Key.posY] + hVal[n.Key.posX, n.Key.posY];
                        parent[n.Key.posX, n.Key.posY] = current;
                    }
                }
                unvisited.Remove(current);
                mindistance = 100000000;
                foreach (Node n in unvisited)
                {
                    if (fVal[n.posX, n.posY] < mindistance)
                    {
                        mindistance = fVal[n.posX, n.posY];
                        current = n;
                    }
                }
                if (mindistance == 100000000 && unvisited.Contains(goal))
                {
                    Console.WriteLine("AERROR:Unreachable destination");
                    return null;
                }
            } while (unvisited.Contains(goal));
            List<Node> Path = new List<Node>();
            current = goal;
            do
            {
                Path.Add(current);
                current = parent[current.posX, current.posY];
            } while (current != start);
            Path.Add(start);
            return Path;
        }
        static public List<Node> HPAStar(grafo g)
        {
            //Generación de supernodos...

            //TODO
            //Generar lista secundaria de owners... buscar alrededor de cada rectangulo y los owners ligarlos...
            int startx = g.start.posX;
            int starty = g.start.posY;
            int goalx = g.goal.posX;
            int goaly = g.goal.posY;
            List<Rectangle> rects = new List<Rectangle>();
            Rectangle[,] owner = new Rectangle[g.size, g.size];
            for (int y = 0; y < g.size; y++)
            {
                for (int x = 0; x < g.size; x++)
                {
                    if (g.nodes[x, y] != null)
                    {//If the node can be grouped... Groups it with its siblings...
                        Rectangle r = new Rectangle();
                        int addlength = 0;
                        while (x + addlength < g.size && g.nodes[x + addlength, y] != null)/* && !(x+length+1 == m.entradax && y == m.entraday) && !(x + length + 1 == m.salidax && y == m.saliday)*/
                        {
                            ++addlength;
                        }
                        //Base length calculated...
                        bool obstructed = false;
                        int height = 0;

                        for (int j = y; j < g.size; j++)
                        {
                            for (int i = x; i < x + addlength; i++)
                            {
                                if (g.nodes[i, j] == null)
                                {
                                    obstructed = true;
                                    break;
                                }
                            }
                            if (obstructed)
                            {
                                break;
                            }
                            else
                            {
                                for (int i = x; i < x + addlength; i++)
                                {
                                    g.nodes[i, j] = null;
                                    owner[i, j] = r;
                                }
                            }
                            ++height;
                        }
                        //Height calculated...
                        r.assign(x, y, x + addlength - 1, y + height - 1);
                        if (startx >= r.posaX && startx <= r.posbX && starty >= r.posaY && starty <= r.posbY)
                        {
                            r.containsStart = true;
                            //Console.WriteLine("fs: "+m.entradax+" "+m.entraday+"   "+r.posaX+" " + r.posaY + " " + r.posbX + " " + r.posbY + " ");
                        }
                        if (goalx >= r.posaX && goalx <= r.posbX && goaly >= r.posaY && goaly <= r.posbY)
                        {
                            r.containsGoal = true;
                            //Console.WriteLine("fg");
                        }
                        if (r.containsStart && r.containsGoal)//Se tendria que devolver la distancia con esto...
                        {
                            getManhattan(startx, goalx, starty, goaly);//Se haria algo con esto si fuera necesario saber la ruta... Solo realizo la operación por el tiempo de procesamiento.
                            return new List<Node>();//Realmente es trivial el moverse dentro de un rectangulo.
                        }
                        //Console.WriteLine(addlength + " " + height);
                        rects.Add(r);
                    }
                }
            }
            //Console.WriteLine("dividido listo " + rects.Count);
            //Ya tengo todo dividido en rectangulos... Falta volver a los rectángulos nodos, ya se cuales contienen entrada y salida...
            //Tenemos asegurado que el mismo rectangulo no contiene entrada y salida. Ese caso de salida ya se dió...

            //Revisar los recuadros alrededor de cada rectangulo, owner!!!!

            Node[,] nodos = new Node[g.size, g.size];
            foreach (Rectangle a in rects)
            {
                List<Rectangle> adj = new List<Rectangle>();
                int ax = -1;
                int ay = -1;
                int bx = 1;
                int by = 1;
                if (a.posaX == 0)
                {
                    ax = 0;
                }
                if (a.posaY == 0)
                {
                    ay = 0;
                }
                if (a.posbX == g.size - 1)
                {
                    bx = 0;
                }
                if (a.posbY == g.size - 1)
                {
                    by = 0;
                }
                if (ay != 0)
                {
                    for (int i = a.posaX; i <= a.posbX; i++)
                    {
                        if (owner[i, a.posaY + ay] != null && !adj.Contains(owner[i, a.posaY + ay]))
                        {
                            adj.Add(owner[i, a.posaY + ay]);
                        }
                    }
                }
                if (by != 0)
                {
                    for (int i = a.posaX; i <= a.posbX; i++)
                    {
                        if (owner[i, a.posbY + by] != null && !adj.Contains(owner[i, a.posbY + by]))
                        {
                            adj.Add(owner[i, a.posbY + by]);
                        }
                    }
                }
                if (ax != 0)
                {
                    for (int i = a.posaY; i <= a.posbY; i++)
                    {
                        if (owner[a.posaX + ax, i] != null && !adj.Contains(owner[a.posaX + ax, i]))
                        {
                            adj.Add(owner[a.posaX + ax, i]);
                        }
                    }
                }
                if (bx != 0)
                {
                    for (int i = a.posaY; i <= a.posbY; i++)//OJO
                    {
                        if (owner[a.posbX + bx, i] != null && !adj.Contains(owner[a.posbX + bx, i]))
                        {
                            adj.Add(owner[a.posbX + bx, i]);
                        }
                    }
                }
                int px = a.getSignificantX(startx, goalx);
                int py = a.getSignificantY(starty, goaly);
                if (nodos[px, py] == null)
                {
                    nodos[px, py] = new Node();
                    nodos[px, py].ac = new Dictionary<Node, int>();
                    nodos[px, py].posX = px;
                    nodos[px, py].posY = py;
                }

                foreach (Rectangle b in adj)
                {
                    int qx = b.getSignificantX(startx, goalx);
                    int qy = b.getSignificantY(starty, goaly);
                    if (!(nodos[qx, qy] != null && nodos[px, py].ac.ContainsKey(nodos[qx, qy])))
                    {
                        int caso = 0;
                        if ((b.posaX >= a.posaX && b.posaX <= a.posbX) || (b.posbX >= a.posaX && b.posbX <= a.posbX))
                        //Si se cumple cualquiera de los casos, sabemos que 
                        {//Casos 1 y 2... b arriba o abajo de a...
                            if (a.posaY - b.posbY == 1)//Caso 1, b sobre a.
                            {
                                caso = 1;
                            }
                            else if (b.posaY - a.posbY == 1)//Caso 2, b bajo a.
                            {
                                caso = 2;
                            }
                        }
                        else if ((b.posaY >= a.posaY && b.posaY <= a.posbY) || (b.posbY >= a.posaY && b.posbY <= a.posbY))
                        //Casos 3 y 4.... b a la derecha o izquierda de a...
                        {
                            if (a.posaX - b.posbX == 1)//Caso 3, b a la izquierda de a.
                            {
                                caso = 3;
                            }
                            else if (b.posaX - a.posbX == 1)//Caso 4, b a la derecha de a.
                            {
                                caso = 4;
                            }
                        }
                        //Console.WriteLine(ax + " " + ay + " " + bx + " " + by);

                        if (caso != 0)
                        {
                            int distancia = 0;
                            if (b != a)
                            {
                                if (nodos[qx, qy] == null)
                                {
                                    nodos[qx, qy] = new Node();
                                    nodos[qx, qy].ac = new Dictionary<Node, int>();
                                    nodos[qx, qy].posX = qx;
                                    nodos[qx, qy].posY = qy;
                                }
                                distancia = getManhattan(px, qx, py, qy);
                                if (!(nodos[px, py].ac.ContainsKey(nodos[qx, qy])))
                                {
                                    nodos[px, py].ac.Add(nodos[qx, qy], distancia);
                                    nodos[qx, qy].ac.Add(nodos[px, py], distancia);
                                }
                            }
                        }
                    }
                }
            }
            int gsize = g.size;
            g = new grafo();
            g.goal = nodos[goalx, goaly];
            g.start = nodos[startx, starty];
            g.size = gsize;
            g.nodes = nodos;
            //Console.WriteLine("ASTAR CALLED");
            return AStar(g);
        }
        static void Main(string[] args)
        {
            double[] aEff = new double[1500];
            System.IO.StreamWriter file =
           new System.IO.StreamWriter("experiment.txt", true);
            Console.WriteLine("Iniciando programa... asegurese de cambiar la prioridad a runtime...");
            Console.ReadKey();
            file.WriteLine("Maps:");
            List<Mapa> mList = new List<Mapa>();
            int[] shortLength = new int[1500];
            for (int i = 0; i < 1500; i++)
            {
                Mapa m = new Mapa();
                if (i < 500)
                {
                    m.size = 10;
                }
                else if (i < 1000)
                {
                    m.size = 50;
                }
                else
                {
                    m.size = 100;
                }
                bool valid = true;
                do
                {
                    valid = true;
                    Console.WriteLine("New map " + i + " is invalid, regenerating...");
                    m.randomizeMapa();
                    List<Node> r;
                    foreach (Mapa map in mList)
                    {
                        if (map.Equals(m))
                        {
                            Console.WriteLine("MERROR: Map already exists.");
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        if ((r = Dijkstra(m.convertToGrafo())) != null)
                        {
                            shortLength[i] = r.Count - 1;
                        }
                        else
                        {

                            valid = false;
                        }
                    }
                } while (!valid);
                //mList.Add(m);
                //Console.WriteLine(shortLength[i]);
                file.WriteLine("<map " + i + ">");
                aEff[i] = ((double)(posNodes[i]) / (double)shortLength[i]);//Calculo de la eficiencia de A* en el algoritmo... Nodos procesados/# de nodos de la respuesta
                Console.WriteLine("" + (double)aEff[i]);
                for (int y = 0; y < m.size; y++)
                {
                    for (int x = 0; x < m.size; x++)
                    {
                        if (x == m.entradax && y == m.entraday)
                        {
                            file.Write("E");
                        }
                        else if (x == m.salidax && y == m.saliday)
                        {
                            file.Write("S");
                        }
                        else if (m.array[x, y])
                        {
                            file.Write("O");
                        }
                        else
                        {
                            file.Write(" ");
                        }
                    }
                    file.WriteLine("");

                }
                file.WriteLine("</map>");
                mList.Add(m);
            }
            //file.Close();//Se escriben los 1500 mapas a un archivo de texto para futura referencia...
            file.WriteLine("Testing:\n\tChecked Nodes\tSolution Length\tEfficiency\tDijksrtaT\tAT*\tHPAT*");
            Console.WriteLine("Los 1500 mapas distintos fueron generados...");
            Console.WriteLine("Iniciando pruebas:");
            for (int i = 0; i < 1500; i++)
            {
                double timeD = 0;
                double timeA = 0;
                double timeH = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine("map " + i);
                grafo g;
                for (int j = 0; j < 5; j++)
                {
                    g = mList[i].convertToGrafo();
                    watch.Restart();
                    Dijkstra(g);
                    watch.Stop();
                    timeD += (double)watch.ElapsedTicks;
                    watch.Restart();
                    AStar(g);
                    watch.Stop();
                    timeA += (double)watch.ElapsedTicks;
                    watch.Restart();
                    HPAStar(g);
                    watch.Stop();
                    timeH += (double)watch.ElapsedTicks;
                }

                file.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", posNodes[i],shortLength[i], aEff[i], timeD / (5*TimeSpan.TicksPerMillisecond*1000), timeA / (5 * TimeSpan.TicksPerMillisecond * 1000), timeH / (5 * TimeSpan.TicksPerMillisecond * 1000)));
            }
            file.Close();
            //Console.ReadKey();
        }
    }
}

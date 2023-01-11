using Wolstencroft;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;
class Program
{
    #region CustomComponents
    #region Controller
    #endregion

    class PlayerController : Component
    {
        float fAxisY = 0;
        float fSpeed = .05f;
        float fScale = 0.1f;
        public override void OnUpdate()
        {
            if (Game.IsKeyPressed(Keyboard.Key.W))
            {
                fAxisY -= fScale;
            }
            else if (Game.IsKeyPressed(Keyboard.Key.S))
            {
                fAxisY += fScale;
            }
            else if (fAxisY != 0)
            {
                fAxisY = 0;
            }


            fAxisY = Math.Clamp(fAxisY, -1, 1);

            //move the player towards the mouse

            //get direction
            Vector2f _vDir = entity.transform.position - (Vector2f)Mouse.GetPosition(Game.Instance.window);

            Vector2f _vDirNorm = WMaths.Normalize(_vDir);
            Vector2f _vResult = _vDirNorm * fAxisY * fSpeed;


            entity.transform.position += (_vResult);
        }
    }

    class Projectile : Component
    {
        public Vector2f DirectionToFire;
        public float fSpeed = .25f;
        public float fTimeToWait = 2f;

        public override void OnStart()
        {
            DestroyObject();
        }

        public override void OnUpdate()
        {

            transform.position -= DirectionToFire * fSpeed;
        }

        async void DestroyObject()
        {
            await Task.Delay((int)(fTimeToWait * 1000f));
            Game.Destroy(entity);

        }

    }

    class ProjectileHandler : Component
    {
        public Vector2f DirectionToFire;

        bool bCanFire = true;
        public float fTimeToWait = 0.25f;


        public override void OnStart()
        {
        }
        public override void OnUpdate()
        {
            DirectionToFire = entity.transform.position - (Vector2f)Mouse.GetPosition(Game.Instance.window);
            DirectionToFire = WMaths.Normalize(DirectionToFire);

            if (Mouse.IsButtonPressed(Mouse.Button.Left) && bCanFire)
            {
                Shoot();
            }
        }

        async void Shoot()
        {
            bCanFire = false;
            ProjectileOBJ projectile = new ProjectileOBJ();
            projectile.transform.position = transform.position;
            projectile.projectile.DirectionToFire = DirectionToFire;
            Game.Instantiate(projectile);
            await Task.Delay((int)(fTimeToWait * 1000f));
            bCanFire = true;

        }
    }
    #endregion
    #region CustomEntities
    #region Test

    class ProjectileOBJ : Entity
    {
        public Projectile projectile;
        public Renderable renderable;
        public ProjectileOBJ()
        {
            //reference the projectile
            projectile = AddComponent<Projectile>();
            renderable = AddComponent<Renderable>();

            transform.size = new Vector2f(5, 5);
        }
    }

    class Player : Entity
    {
        PlayerController playerController;
        Renderable renderable;
        ProjectileHandler projectileHandler;

        public Player()
        {
            transform.size = new Vector2f(10, 10);

            playerController = AddComponent<PlayerController>();
            renderable = AddComponent<Renderable>();
            projectileHandler = AddComponent<ProjectileHandler>();


            transform.position += transform.size / 2;

        }
    }

    #endregion
    #endregion

    #region MazeGen
    #region Components

    class Cell : Component
    {
        public bool bIsVisited = false;
        public bool bIsSolid = true;
        public bool bIsOrigin = false;

        public Cell()
        {
            bIsVisited = false;
            bIsSolid = true;
            bIsOrigin = false;
        }



    };


    #endregion


    #region OBJ
    class CellOBJ : Entity
    {
        public Vector2i pos;
        public Cell cell;
        public Renderable renderable;
        public CellOBJ()
        {
            cell = AddComponent<Cell>();
            renderable = AddComponent<Renderable>();


        }

        public void SetVisited(bool bVal)
        {
            Game.Log($"{pos} Was visited");
            if (bVal)
            {
                renderable.Body.FillColor = new Color(0, 200, 0);
            }
            else
            {
                renderable.Body.FillColor = new Color(0, 0, 255);

            }
        }


    };

    class MazeOBJ : Entity
    {
        public CellOBJ[,] Cells;
        float fSpace = 12f;
        public int iAmmount = 18;
        public Vector2i Origin;
        public MazeSolver solver;

        public MazeOBJ()
        {
            solver = AddComponent<MazeSolver>();

            solver.obj = this;

            Cells = new CellOBJ[iAmmount, iAmmount];
            for (int x = 0; x < iAmmount; x++)
            {
                for (int y = 0; y < iAmmount; y++)
                {
                    Cells[x, y] = new CellOBJ();
                    (Cells[x, y]).transform.size = new Vector2f(10, 10);
                    Cells[x, y].pos = new Vector2i(x, y);
                }
            }
        }

        public void AddCellsToGame()
        {
            for (int x = 0; x < iAmmount; x++)
            {
                for (int y = 0; y < iAmmount; y++)
                {
                    (Cells[x, y]).transform.position = new Vector2f(x * fSpace, y * fSpace);
                    (Cells[x, y]).renderable.Body.FillColor = new SFML.Graphics.Color(0, 0, (byte)(255));

                    Entity obj = Game.Instantiate((Cells[x, y]));



                }
            }
        }

        public void SetOrigin(Vector2i coord)
        {
            Origin = coord;
            Cells[coord.X, coord.Y].cell.bIsOrigin = true;
            Cells[coord.X, coord.Y].GetComponent<Renderable>().Body.FillColor = new Color(255, 0, 0);
        }
    };

    class MazeSolver : Component
    {

        public CellOBJ[,] Cells;


        Stack<Vector2i> MazeStack = new Stack<Vector2i>();
        int iMaxPlacesToVisit, iPlacesVisited;

        public MazeOBJ obj;

        bool bRunOnce = false;

        public override void OnStart()
        {

            iMaxPlacesToVisit = (int)(obj.iAmmount * obj.iAmmount);
            obj.Cells[obj.Origin.X, obj.Origin.Y].SetVisited(true);
            MazeStack.Push(obj.Origin);
            iPlacesVisited = 1;


        }

        public override void OnUpdate()
        {
          if(!bRunOnce)
            {
                bRunOnce = true;
                CheckCell();
            }



        }

        void CheckCell()
        {
            var offset = (int x, int y) => { return (MazeStack.Peek().Y + y) * obj.iAmmount - 1 + (MazeStack.Peek().X + x); };

            if (iPlacesVisited < obj.iAmmount * obj.iAmmount)
            {

                List<int> iNeighbours = new List<int>();

                //check neighbours
                //north
                if (MazeStack.Peek().Y > 0 && !obj.Cells[offset(0, -1), offset(0, -1)].cell.bIsVisited)
                {
                    iNeighbours.Add(0);
                }
                //east
                if (MazeStack.Peek().X < obj.iAmmount - 1 && !obj.Cells[offset(1, 0), offset(1, 0)].cell.bIsVisited)
                {
                    iNeighbours.Add(1);

                }
                //south
                if (MazeStack.Peek().Y < obj.iAmmount - 1 && !obj.Cells[offset(0, 1), offset(0, 1)].cell.bIsVisited)
                {
                    iNeighbours.Add(2);

                }
                //west
                if (MazeStack.Peek().X > 0 && !obj.Cells[offset(-1, 0), offset(-1, 0)].cell.bIsVisited)
                {
                    iNeighbours.Add(3);
                }

                if (iNeighbours.Count() != 0)
                {
                    Random rand = new Random();

                    int iNextCellDir = iNeighbours[rand.Next(0, iNeighbours.Count())];

                    switch (iNextCellDir)
                    {
                        case 0:
                        //m_stack.push(make_pair((m_stack.top().first + 0), (m_stack.top().second - 1)));
                            obj.Cells[offset(0, -1), offset(0, -1)].SetVisited(true);
                            MazeStack.Push(new Vector2i(MazeStack.Peek().X , MazeStack.Peek().Y - 1));
                            Game.Log(0);
                            break;
                        case 1:
                            obj.Cells[offset(1, 0), offset(1, 0)].SetVisited(true);
                            MazeStack.Push(new Vector2i(MazeStack.Peek().X + 1 , MazeStack.Peek().Y));
                            Game.Log(1);

                            break;
                        case 2:
                            obj.Cells[offset(0, 1), offset(0, 1)].SetVisited(true);
                            MazeStack.Push(new Vector2i(MazeStack.Peek().X , MazeStack.Peek().Y + 1));
                            Game.Log(2);

                            break;
                        case 3:
                            obj.Cells[offset(-1, 0), offset(-1, 0)].SetVisited(true);
                            MazeStack.Push(new Vector2i(MazeStack.Peek().X - 1 , MazeStack.Peek().Y ));
                            Game.Log(3);

                            break;

                    }
                    iPlacesVisited++;


                }
                else
                {
                    MazeStack.Pop();

                }

                CheckCell();
            }
        }

    }
    #endregion


    #endregion

    /*
        Given a current cell as a parameter
        Mark the current cell as visited
        While the current cell has any unvisited neighbour cells
            Choose one of the unvisited neighbours
            Remove the wall between the current cell and the chosen cell
            Invoke the routine recursively for the chosen cell
				// Create a path between the neighbour and the current cell
				switch (next_cell_dir)
				{
				case 0: // North
					m_maze[offset(0, -1)] |= CELL_VISITED | CELL_PATH_S;
					m_maze[offset(0,  0)] |= CELL_PATH_N;
					m_stack.push(make_pair((m_stack.top().first + 0), (m_stack.top().second - 1)));
					break;

				case 1: // East
					m_maze[offset(+1, 0)] |= CELL_VISITED | CELL_PATH_W;
					m_maze[offset( 0, 0)] |= CELL_PATH_E;
					m_stack.push(make_pair((m_stack.top().first + 1), (m_stack.top().second + 0)));
					break;

				case 2: // South
					m_maze[offset(0, +1)] |= CELL_VISITED | CELL_PATH_N;
					m_maze[offset(0,  0)] |= CELL_PATH_S;
					m_stack.push(make_pair((m_stack.top().first + 0), (m_stack.top().second + 1)));
					break;

				case 3: // West
					m_maze[offset(-1, 0)] |= CELL_VISITED | CELL_PATH_E;
					m_maze[offset( 0, 0)] |= CELL_PATH_W;
					m_stack.push(make_pair((m_stack.top().first - 1), (m_stack.top().second + 0)));
					break;

				}

				m_nVisitedCells++;
			}
			else
			{
				// No available neighbours so backtrack!
				m_stack.pop();
			}
		}

     */



    static Game game = new Game();

    static MazeOBJ Maze = new MazeOBJ();

    static void Main()
    {
        //declare two new floats
        float[] fInputArray = new float[2];
        //output
        Console.WriteLine("Enter a 2 dimensional vector: example 0,1");

        //input
        string sInput = Console.ReadLine();
        //split the input so the input can look like "1,2"
        fInputArray = sInput.Split(',').Select(Convert.ToSingle).ToArray();

        Game.Instantiate(Maze);
        Maze.AddCellsToGame();

        Maze.SetOrigin(new Vector2i((int)(fInputArray[0] - 1), (int)(fInputArray[1]) - 1));



        game.Start();


    }


};


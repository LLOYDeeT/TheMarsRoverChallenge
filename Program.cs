using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marts
{
    enum RoverStatus
    {
        NotMoved,
        Moved,
        CannotMove
    }
    enum RoverDirections
    {
        N = 1,
        E = 2,
        S = 3,
        W = 4
    }
    class Rover
    {
        public int Xpos { get; set; }
        public int Ypos { get; set; }
        public RoverDirections Direction { get; set; }
        public RoverStatus RoverStatus { get; set; }

        public Rover(int x, int y, RoverDirections direction)
        {
            Xpos = x;
            Ypos = y;
            Direction = direction;
            RoverStatus = RoverStatus.NotMoved;
        }
    }
    internal class Program
    {
        static List<int[,]> plateauList = new List<int[,]>();
        static List<Rover> Rovers = new List<Rover>();
        static void Main(string[] args)
        {
            Addplateau(5, 5);
            AddRover(1, 2, 'n');
            AddRover(3, 3, 'e');
            string res = CommandRover("lmLMLMLMM");
            Console.WriteLine(res);
            res = CommandRover("mmrMMRMRRM");
            Console.WriteLine(res);
            Console.ReadKey();
        }

        static bool ValidDirection(char direction)
        {
            if (char.IsLetter(direction))
            {
                direction = char.ToUpper(direction);
                if (direction.Equals('N') || direction.Equals('S') || direction.Equals('W') || direction.Equals('E'))
                    return true;
            }

            return false;
        }
        static bool ValidCommand(char direction)
        {
            if (char.IsLetter(direction))
            {
                direction = char.ToUpper(direction);
                if (direction.Equals('L') || direction.Equals('R') || direction.Equals('M'))
                    return true;
            }

            return false;
        }
        static void AddRover(int x, int y, char direction)
        {
            if (ValidDirection(direction))
            {
                Rover rover = new Rover(x, y, GetRoverDirections(direction));
                Rovers.Add(rover);
            }
        }
        static void Addplateau(int x, int y)
        {
            int[,] plateau = Initializeplateau(x + 1, y + 1);
            if (plateau.Length > 0)
                plateauList.Add(plateau);
        }

        static Rover FindRoverToMove()
        {
            //Assumption, we can only move the rover if it is not moved yet in the list of rovers
            foreach (Rover rover in Rovers)
            {
                if (rover.RoverStatus != RoverStatus.Moved)
                {
                    return rover;
                }
            }
            return null;
        }

        static int[,] FindPlateau(int x, int y)
        {
            //find a Plateau where a rover can be successfully be positioned
            foreach (int[,] p in plateauList)
            {
                if (p.GetLength(0) >= x && p.GetLength(1) >= y)
                {
                    if (p[x, y] == -1)
                        return p;
                }
            }
            return Initializeplateau(0, 0);
        }

        static void MoveRover(Rover rover, int[,] plateau)
        {
            //move the rover in the rover direction: N = up, S = down, W = left and E = right 
            switch (rover.Direction)
            {
                case RoverDirections.N:
                    if (rover.Ypos + 1 <= plateau.GetLength(1) && plateau[rover.Xpos, rover.Ypos + 1] == -1)//move up
                        rover.Ypos += 1;
                    break;
                case RoverDirections.S:
                    if (rover.Ypos - 1 >= 0 && plateau[rover.Xpos, rover.Ypos - 1] == -1)//move down
                        rover.Ypos -= 1;
                    break;
                case RoverDirections.W:
                    if (rover.Xpos - 1 >= 0 && plateau[rover.Xpos - 1, rover.Ypos] == -1)//move left
                        rover.Xpos -= 1;
                    break;
                case RoverDirections.E:
                    if (rover.Xpos + 1 <= plateau.GetLength(0) && plateau[rover.Xpos + 1, rover.Ypos] == -1)//move right
                        rover.Xpos += 1;
                    break;
            }
        }

        static void ChangeRoverDirection(Rover rover, char direction)
        {
            //Change the direction of the rover without moving it
            if (char.IsLetter(direction))
            {
                direction = char.ToUpper(direction);
                switch (direction)
                {
                    case 'L':
                        int lpos = (int)rover.Direction - 1;
                        if (lpos == 0)//the posible pos is 4,3,2,1 and repeat
                            lpos = 4;
                        rover.Direction = (RoverDirections)lpos;
                        break;
                    case 'R':
                        int rpos = (int)rover.Direction + 1;
                        if (rpos > 4)//the posible pos is 1,2,3,4 and repeat
                            rpos = 1;
                        rover.Direction = (RoverDirections)rpos;
                        break;
                }
            }
        }

        static RoverDirections GetRoverDirections(char c)
        {
            //convert the char direction to RoverDirections
            if (char.IsLetter(c))
            {
                c = char.ToUpper(c);
                switch (c)
                {
                    case 'N':
                        return RoverDirections.N;
                    case 'S':
                        return RoverDirections.S;
                    case 'W':
                        return RoverDirections.W;
                    case 'E':
                        return RoverDirections.E;
                }
            }

            return RoverDirections.N;
        }
        static string CommandRover(string args)
        {
            //This methord command the rover if there is avalable rover that can move, then find a plateau where the rover can be position successfully then move the rovr based in the user commands
            if (Rovers.Count > 0)
            {
                Rover rovertoMove = FindRoverToMove();
                if (rovertoMove != null)
                {
                    int[,] plateau = FindPlateau(rovertoMove.Xpos, rovertoMove.Ypos);
                    if (plateau.Length > 0)
                    {
                        rovertoMove.RoverStatus = RoverStatus.Moved;
                        foreach (char c in args)
                        {
                            if (ValidCommand(c))
                            {
                                if(char.IsLetter(c))
                                {
                                    switch (char.ToUpper(c))
                                    {
                                        case 'M':
                                            MoveRover(rovertoMove, plateau);
                                            break;
                                        case 'L':
                                        case 'R':
                                            ChangeRoverDirection(rovertoMove, c);
                                            break;
                                    }
                                }                               
                            }
                        }

                        plateau[rovertoMove.Xpos, rovertoMove.Ypos] = 0;
                        return String.Format("{0} {1} {2}", rovertoMove.Xpos, rovertoMove.Ypos, rovertoMove.Direction);
                    }
                    else
                        return string.Empty;
                }
            }

            return string.Empty;
        }
        static int[,] Initializeplateau(int x, int y)
        {
            //This methord intialize the new plateau, meaning -1 all the position are available for the rover to move.
            if (x >= 0 && y >= 0)
            {
                int[,] plateau = new int[x, y];

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        plateau[i, j] = -1;
                    }
                }
                return plateau;
            }
            else
            {
                return Initializeplateau(0, 0);
            }
        }
    }
}

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

    class TheMarsRoverChallenge
    {
        List<int[,]> plateauList;
        List<Rover> Rovers;

        public TheMarsRoverChallenge()
        {
            plateauList = new List<int[,]>();
            Rovers = new List<Rover>();
        }
        bool ValidDirection(char direction)
        {
            if (char.IsLetter(direction))
            {
                direction = char.ToUpper(direction);
                if (direction.Equals('N') || direction.Equals('S') || direction.Equals('W') || direction.Equals('E'))
                    return true;
            }

            return false;
        }
        bool ValidCommand(char direction)
        {
            if (char.IsLetter(direction))
            {
                direction = char.ToUpper(direction);
                if (direction.Equals('L') || direction.Equals('R') || direction.Equals('M'))
                    return true;
            }

            return false;
        }
        public void AddRover(int x, int y, char direction)
        {
            if (x >= 0 && y >= 0 && ValidDirection(direction))
            {
                Rover rover = new Rover(x, y, GetRoverDirections(direction));
                Rovers.Add(rover);
            }
            else
                Console.WriteLine("\nRover cannot be added, ensure you captured the correct data");
        }
        public void Addplateau(int x, int y)
        {
            int[,] plateau = Initializeplateau(x + 1, y + 1);
            if (plateau.Length > 0)
                plateauList.Add(plateau);
            else
                Console.WriteLine("\nPlateau cannot be added, ensure you captured the correct data");
        }

        Rover FindRoverToMove()
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

        int[,] FindPlateau(int x, int y)
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

        bool MoveRover(Rover rover, int[,] plateau)
        {
            //move the rover in the rover direction: N = up, S = down, W = left and E = right 
            switch (rover.Direction)
            {
                case RoverDirections.N:
                    if (rover.Ypos + 1 <= plateau.GetLength(1) && plateau[rover.Xpos, rover.Ypos + 1] == -1)//move up
                    {
                        rover.Ypos += 1;
                        return true;
                    }
                    break;
                case RoverDirections.S:
                    if (rover.Ypos - 1 >= 0 && plateau[rover.Xpos, rover.Ypos - 1] == -1)//move down
                    {
                        rover.Ypos -= 1;
                        return true;
                    }
                    break;
                case RoverDirections.W:
                    if (rover.Xpos - 1 >= 0 && plateau[rover.Xpos - 1, rover.Ypos] == -1)//move left
                    {
                        rover.Xpos -= 1;
                        return true;
                    }
                    break;
                case RoverDirections.E:
                    if (rover.Xpos + 1 <= plateau.GetLength(0) && plateau[rover.Xpos + 1, rover.Ypos] == -1)//move right
                    {
                        rover.Xpos += 1;
                        return true;
                    }
                    break;
            }

            return false;
        }

        void ChangeRoverDirection(Rover rover, char direction)
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

        RoverDirections GetRoverDirections(char c)
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
        public string CommandRover(string args)
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
                        foreach (char c in args)
                        {
                            if (ValidCommand(c))
                            {
                                if (char.IsLetter(c))
                                {
                                    switch (char.ToUpper(c))
                                    {
                                        case 'M':
                                            if (MoveRover(rovertoMove, plateau) && rovertoMove.RoverStatus != RoverStatus.Moved)
                                            {
                                                rovertoMove.RoverStatus = RoverStatus.Moved;//Assumeption the rover is moved when a succussfully moved has been completed, the rover can still return to its original location but I will regards it as moved.
                                            }
                                            break;
                                        case 'L':
                                        case 'R':
                                            ChangeRoverDirection(rovertoMove, c);//Assumeption; if the rover change orentation will regards it as not moved
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

        int[,] Initializeplateau(int x, int y)
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
    internal class Program
    {
        static void Main(string[] args)
        {
            #region hard coded test
            TheMarsRoverChallenge theMarsRoverChallenge = new TheMarsRoverChallenge();
            theMarsRoverChallenge.Addplateau(5, 5);
            theMarsRoverChallenge.AddRover(1, 2, 'n');
            theMarsRoverChallenge.AddRover(3, 3, 'e');
            string res = theMarsRoverChallenge.CommandRover("lmLMLMLMM");
            Console.WriteLine("output: {0}", res);
            res = theMarsRoverChallenge.CommandRover("mmrMMRMRRM");
            Console.WriteLine("output: {0}",res);
            #endregion
            #region user command test
            bool endapp = false;
            Console.WriteLine("Welcome to The Mars Rover Challenge App");
            theMarsRoverChallenge = new TheMarsRoverChallenge();
            while (!endapp)
            {
                Console.WriteLine("\nPress 1 to Add Add Plateau, 2 to Add Rover, 3 to commaand Rover, 4 to exit the app");
                char input = Console.ReadKey().KeyChar;
                if (char.IsDigit(input))
                {
                    switch (input)
                    {
                        case '1':
                            Console.WriteLine("\nPlease enter the plateau in a format X Y Direction e.g. 0 0");
                            string plateaustr = Console.ReadLine();
                            string[] plateauA = plateaustr.Split(' ');
                            if (plateauA.Length == 2)
                            {
                                if (int.TryParse(plateauA[0], out int x) && int.TryParse(plateauA[1], out int y))
                                {
                                    theMarsRoverChallenge.Addplateau(x, y);
                                }
                                else
                                    Console.WriteLine("\nInvalid format");
                            }
                            else
                            {
                                Console.WriteLine("\nInvalid format");
                            }
                            break;
                        case '2':
                            Console.WriteLine("\nPlease enter the rover in a format X Y Direction e.g. 0 0 N:");
                            string rvr = Console.ReadLine();
                            string[] RoverA = rvr.Split(' ');
                            if (RoverA.Length == 3)
                            {
                                if (int.TryParse(RoverA[0], out int x) && int.TryParse(RoverA[1], out int y) && RoverA[2].Length == 1)
                                {
                                    theMarsRoverChallenge.AddRover(x, y, RoverA[2][0]);
                                }
                                else
                                    Console.WriteLine("\nInvalid format");
                            }
                            else
                            {
                                Console.WriteLine("\nInvalid format");
                            }
                            break;
                        case '3':
                            Console.WriteLine("\nPlease enter the rover command:");
                            string cmd = Console.ReadLine();
                            res = theMarsRoverChallenge.CommandRover(cmd);
                            Console.WriteLine("output: {0}", res);
                            break;
                        case '4':
                            endapp = true;
                            break;
                        default:
                            Console.WriteLine("\nInvalid option");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid option");
                }
            }
            Console.ReadKey();
            #endregion
        }
    }
}

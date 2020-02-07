using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace laser_maze_challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get input file name from user
                Console.Write("Welcome to the Laser Maze!  What is the name of your definition file? \n(Ex: input1.txt): ");
                string strFilePath = GetFileName();
                
                if (strFilePath != "")
                {
                    // Load the input file
                    string[] arrLines = File.ReadAllLines(strFilePath);
                    string  strGridLayout = arrLines[0],
                            strEntryPoint = arrLines[^2];

                    // Determine exit point
                    string strExitPoint = GetExitPoint(arrLines);

                    // Finalize 
                    Console.WriteLine("Grid Layout: " + strGridLayout);
                    Console.WriteLine("Laser Start Position: " + strEntryPoint);
                    Console.WriteLine("Laser Exit Position: " + strExitPoint);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        static string GetFileName()
        {
            try
            {
                string strFileName = Console.ReadLine();
                if (!strFileName.Contains(".txt"))
                {
                    strFileName += ".txt";
                }

                while (!File.Exists("../../../input/" + strFileName))
                {
                    Console.Write("The file " + strFileName + " could not be found.  Please try again: ");
                    strFileName = Console.ReadLine();
                    if (!strFileName.Contains(".txt"))
                    {
                        strFileName += ".txt";
                    }
                }

                return "../../../input/" + strFileName;
            }
            catch (IOException e)
            {
                Console.WriteLine("There was an error reading the file:");
                Console.WriteLine(e.Message);
                return "";
            }
        }

        static string GetExitPoint(string[] arrData)
        {
            try
            {
                string strExitPoint = "";

                // Get mirror coordinates
                var mirrors = new List<string> { };
                for (int i = 2; i < arrData.Length - 3; i++)
                {
                    mirrors.Add(arrData[i]);
                }

                // Get Grid X,Y
                string[] arrGridLayout = arrData[0].Split(',');
                int gridX = Int32.Parse(arrGridLayout[0]),
                    gridY = Int32.Parse(arrGridLayout[1]);

                // Determine start point and direction
                string[] arrStart = arrData[^2].Split(',');
                int currentX = Int32.Parse(arrStart[0]),
                    currentY = Int32.Parse(arrStart[1].Substring(0, arrStart.Length - 1));
                string startOrientation = arrStart[1].Substring(arrStart.Length - 1, 1),
                       currentDirection = GetStartDirection(currentX, currentY, startOrientation); // Up, Down, Left or Right;

                // Shoot the laser, and determine the exit point
                while (strExitPoint == "")
                {
                    // Check for Mirror in current location, then change direction based on it's reflection and orientation
                    foreach (string mirror in mirrors)
                    {
                        int mirrorX = Int32.Parse(mirror.Split(",")[0]);
                        int mirrorY = Int32.Parse(mirror.Split(",")[1].Replace("R", "").Replace("L", ""));
                        string lean = Regex.Replace(mirror.Split(",")[1], @"\d", "").Substring(0, 1);
                        string reflection = (Regex.Replace(mirror.Split(",")[1], @"\d", "").Length == 2 ? Regex.Replace(mirror.Split(",")[1], @"\d", "").Substring(1, 1) : "");

                        if (currentX == mirrorX && currentY == mirrorY)
                        {
                            
                            // Mirror is in this box
                            switch (currentDirection)
                            {
                                case "Up":
                                    if (lean == "R" && reflection != "L" )
                                    {
                                        currentDirection = "Right";
                                    } else if (lean == "L" && reflection != "R")
                                    {
                                        currentDirection = "Left";
                                    }
                                    break;
                                case "Down":
                                    if (lean == "R" && reflection != "R")
                                    {
                                        currentDirection = "Left";
                                    }
                                    else if (lean == "L" && reflection != "L")
                                    {
                                        currentDirection = "Right";
                                    }
                                    break;
                                case "Left":
                                    if (lean == "R" && reflection != "L")
                                    {
                                        currentDirection = "Down";
                                    }
                                    else if (lean == "L" && reflection != "R")
                                    {
                                        currentDirection = "Up";
                                    }
                                    break;
                                case "Right":
                                    if (lean == "R" && reflection != "R")
                                    {
                                        currentDirection = "Up";
                                    }
                                    else if (lean == "L" && reflection != "L")
                                    {
                                        currentDirection = "Down";
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }
                        }
                    }

                    // Increment currentX and currentY based on direction, and see if we reached an exit point
                    if (currentDirection == "Up")
                    {
                        if (currentY == gridY-1)
                        {
                            strExitPoint = currentX + "," + currentY + "V";
                        } else
                        {
                            currentY++;
                        }
                    }
                    else if (currentDirection == "Down")
                    {
                        if (currentY == 0)
                        {
                            strExitPoint = currentX + "," + currentY + "V";
                        }
                        else
                        {
                            currentY--;
                        }
                    }
                    else if (currentDirection == "Right")
                    {
                        if (currentX == gridX-1)
                        {
                            strExitPoint = currentX + "," + currentY + "H";
                        }
                        else
                        {
                            currentX++;
                        }
                    }
                    else if (currentDirection == "Left")
                    {
                        if (currentX == 0)
                        {
                            strExitPoint = currentX + "," + currentY + "H";
                        }
                        else
                        {
                            currentX--;
                        }
                    }

                    Console.WriteLine("D: " + currentDirection + " X: " + currentX + " Y: " + currentY);
                }

                return strExitPoint;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return "";
            }
        }

        static string GetStartDirection(int x, int y, string orientation)
        {
            string direction = "";
            if (orientation == "H")
            {
                direction = (x == 0 ? "Right" : "Left");
            } else
            {
                direction = (y == 0 ? "Up" : "Down");
            }

            return direction;
        }
    }
}

using System;
using System.IO;
using System.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

using RogueLikeWPF2.Core.Tiles;

// IDEAS:
// IDEA: Make variants of tiles automaticaly, so that we don't need a new tile for every changed pixel.
// IDEA: Multi coloured tiles. Example: use two base colour or more to change out? i.e. white, gray.

namespace RogueLikeWPF2.Core.Levels
{
    /// <summary>
    /// Gets and sets the levels by iterating through every file in the LevelData directory.
    /// </summary>
    public static class LevelInitializer
    {
        public static Level[] levelsArray;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns an array of levels.</returns>
        public static void LoadLevels(string levelsPath = "./Core/Levels/LevelData/") // Can set custom path. With a default value of: "./Core/Levels/LevelData/".
        {
            string[] currentLevelString; // string array of current level.
            string[] levelPaths = Directory.EnumerateFiles(levelsPath, "*.txt").Select(l => l).ToArray(); // Path to all level files stored in the LevelData directory.
            Level[] levels = new Level[levelPaths.Length]; // Array of all levels
            Level currentLevel; // Current level information input object.
            StreamReader fileReader; // Get content of file.
            string[] tempStorage;
            bool isWalkable;

            // Goes through every file of the levelsPath(path<string>) directory.
            foreach (string filePath in levelPaths)
            {
                // Read content of current file.
                fileReader = new StreamReader(filePath);

                // Converting (string)file to (string[])currentLevelString.
                // And reads the whole current file.
                currentLevelString = fileReader.ReadToEnd().Split(
                    new[] {Environment.NewLine}, StringSplitOptions.None // new string[]   
                );
                fileReader.Dispose(); // Removed reader from memory.

                // New Level and new tile array.               
                tempStorage = currentLevelString[5].Split(' ');
                currentLevel = new Level()
                {
                    ID = int.Parse(currentLevelString[1]),
                    Name = currentLevelString[3],
                    Height = int.Parse(tempStorage[1]),
                    Width = int.Parse(tempStorage[0]),
                };
                currentLevel.Tiles = new Tile[currentLevel.Height, currentLevel.Width];

                currentLevelString = currentLevelString.Skip(6).ToArray(); // remove used elements from array.

                
                for (int i = 0; i < currentLevelString.Length; i++)
                {
                    switch (currentLevelString[i])
                    {
                        case "tiles":
                            i++;
                            for (int j = 0; j < currentLevel.Height; j++)
                            {
                                try
                                {
                                    tempStorage = currentLevelString[i].Split(' ');
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    throw new Exception("LevelID: " + currentLevel.ID + ", LevelName: " + currentLevel.Name + ", is most likely not the correct size in relation to tiles, colors, and/or functions.");
                                }

                                for (int k = 0; k < currentLevel.Width; k++)
                                {
                                    currentLevel.Tiles[j, k] = new Tile() {
                                        X = k,
                                        Y = j,
                                        tileSymbol = int.Parse(tempStorage[k])
                                    };
                                }
                                i++;
                            }
                            i--; // fix - do not remove (if you do, you'll need to fix the other issue)
                            break;
                        case "colors":
                            i++;
                            for (int j = 0; j < currentLevel.Height; j++)
                            {
                                try
                                {
                                    tempStorage = currentLevelString[i].Split(' ');
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    throw new Exception("LevelID: " + currentLevel.ID + ", LevelName: " + currentLevel.Name + ", is most likely not the correct size in relation to tiles, colors, and/or functions.");
                                }

                                for (int k = 0; k < currentLevel.Width; k++)
                                {
                                    currentLevel.Tiles[j, k].tileColor = int.Parse(tempStorage[k]);
                                }
                                i++;
                            }
                            i--; // fix - do not remove (if you do, you'll need to fix the other issue)
                            break;
                        case "functions":
                            i++;
                            for (int j = 0; j < currentLevel.Height; j++)
                            {
                                try
                                {
                                    tempStorage = currentLevelString[i].Split(' ');
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    throw new Exception("LevelID: " + currentLevel.ID + ", LevelName: " + currentLevel.Name + ", is most likely not the correct size in relation to tiles, colors, and/or functions.");
                                }

                                for (int k = 0; k < currentLevel.Width; k++)
                                {
                                    currentLevel.Tiles[j, k].tileFunction = int.Parse(tempStorage[k]);

                                    //Debug.WriteLine("id: " + currentLevel.ID + ", symbol: " + currentLevel.Tiles[j, k].tileSymbol + ", color: " + currentLevel.Tiles[j, k].tileColor + ", func: " + currentLevel.Tiles[j, k].tileFunction);
                                }
                                i++;
                            }
                            i--;
                            break;
                        case "walkable":
                            i++;
                            for (int j = 0; j < currentLevel.Height; j++)
                            {
                                try
                                {
                                    tempStorage = currentLevelString[i].Split(' ');
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    throw new Exception("LevelID: " + currentLevel.ID + ", LevelName: " + currentLevel.Name + ", is most likely not the correct size in relation to tiles, colors, functions, etc.");
                                }

                                for (int k = 0; k < currentLevel.Width; k++)
                                {
                                    if (int.Parse(tempStorage[k]) == 1)
                                    {
                                        isWalkable = true;
                                    }
                                    else {
                                        isWalkable = false;
                                    }
                                    currentLevel.Tiles[j, k].isWalkable = isWalkable;
                                }
                                i++;
                            }
                            i--;
                            break;
                        case "entities":
                            i++;
                            for (int j = 0; j < currentLevel.Height; j++)
                            {
                                try
                                {
                                    tempStorage = currentLevelString[i].Split(' ');
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    throw new Exception("LevelID: " + currentLevel.ID + ", LevelName: " + currentLevel.Name + ", is most likely not the correct size in relation to tiles, colors, and/or functions.");
                                }

                                for (int k = 0; k < currentLevel.Width; k++)
                                {
                                    currentLevel.Tiles[j, k].tileType = int.Parse(tempStorage[k]);
                                }
                                i++;
                            }
                            i--;
                            break;
                        default:
                            break;
                    }
                }

                levels[currentLevel.ID - 1] = currentLevel; // Adds current level to the level array.
            }

            levelsArray = levels;
        }
    }
}

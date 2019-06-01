using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

using RogueLikeWPF2.Core.Levels;
using RogueLikeWPF2.Core.Tiles;
using RogueLikeWPF2.Core.Entities.Player;

// Duplicate refrences
using _Brushes = System.Windows.Media.Brushes;
using _Rectangle = System.Drawing.Rectangle;
using _PixelFormat = System.Drawing.Imaging.PixelFormat;
using _Color = System.Drawing.Color;

namespace RogueLikeWPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Game Sizes
        public static readonly int gameWidth = 32; // FIX: game not supporting levels over 32x32.
        public static readonly int gameHeight = 32; // FIX: game not supporting levels over 32x32.
        public static readonly int tileWidth = 16; // TODO: Make automatic? 
        public static readonly int tileHeight = 16; // TODO: Make automatic? 
        public static readonly int windowWidth = (gameWidth * tileWidth) + (gameWidth / 2);
        public static readonly int windowHeight = (gameWidth * tileHeight) + (gameHeight + 47); // TODO: Make more acurate, and automiatic.

        //public static readonly int sumTileWidth = tileset.Width / _tilesetTileWidth;
        //public static readonly int sumTileHeight = sumTileWidth;

        // Grid Positions
        private static readonly int _statusbarHeight = 40;
        private static readonly int _statusbarGridPosition = 0;
        private static readonly int _gameGridPosition = 1;

        // TILESET AND COLORSET
        // Tileset
        private static readonly string _tilesetURI = "./Core/Tiles/tileset_16x16_128.png"; // IDEA: Add animated tiles. Example: water, fire.
        private static readonly int _tilesetTileWidth = 16;
        private static readonly int _tilesetTileHeight = 16;
        private static List<Bitmap> _tiles;
        // Colorset
        private static readonly string _colorsetURI = "./Core/Tiles/ColorTileset.png";
        private static readonly int _colorsetTileWidth = 1;
        private static readonly int _colorsetTileHeight = 1;
        private static List<_Color> _colors;

        // COMPONENTS
        // Grids
        private static Grid mainGrid;
        private static Grid gameGrid;
        // Rows
        private static RowDefinition mainRow;
        private static RowDefinition mainGameRow;
        // Panels
        public static StackPanel statusPanel;
        // Grid Definisions
        private static RowDefinition rowIndDef;
        private static ColumnDefinition colIndDef;


        // Game
        //private static int gameStatus = 1; // 0 - Start Menu, 1 - Game, 2 - Pause? // NOT USED

        // Testing // REMOVE
        private static Player playerOne;
        private static StackPanel testPlayerPanel;
        private static int LevelSelector = 0; // TODO: Implement level selection

        /// <summary>
        /// Constructor for MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Roguelike";
            this.Width = windowWidth;
            this.Height = windowHeight;
            this.PreviewKeyDown += Window_PreviewKeyDown; // Key presses method

            LevelInitializer.LoadLevels(); // Loads levels from files.

            _tiles = CreateTilesFromTileset( new Bitmap( _tilesetURI) ); // TODO: Way to rotate images, so that we don't need the same symbol/tile four times rotated.
            _colors = CreateColorFromColorTileSet( new Bitmap( _colorsetURI) );

            InitializeGameWindow();
        }

        /// <summary>
        /// Adds components to the window (and stuff?).
        /// </summary>
        private void InitializeGameWindow()
        {
            //Main grid for the application
            mainGrid = new Grid
            {
                Background = _Brushes.Black
            };
            // Main grid definition rows and cols
            mainRow = new RowDefinition // Status bar space
            {
                Name = "MainRow",
                Height = new GridLength(_statusbarHeight)
            };
            mainGameRow = new RowDefinition
            {
                Name = "MainGameRow",
                Height = new GridLength(1, GridUnitType.Star) // 1 for percentage of * (or something), but 1 is 100%.
            };
            mainGrid.RowDefinitions.Add(mainRow);
            mainGrid.RowDefinitions.Add(mainGameRow);
            this.AddChild(mainGrid);

            // Status bar (top space)
            statusPanel = new StackPanel
            {
                Name = "StatusPanel",
                Background = _Brushes.Black
            };
            statusPanel.SetValue(Grid.RowProperty, _statusbarGridPosition);
            mainGrid.Children.Add(statusPanel);

            // Game screen / grid / grid inside mainGrid
            gameGrid = new Grid
            {
                Name = "GameGrid"
            };
            gameGrid.SetValue(Grid.RowProperty, _gameGridPosition);
            mainGrid.Children.Add(gameGrid);


            // Generate content of game grid
            // Generate rows for the game grid.
            // Rows
            for (int i = 0; i < gameHeight; i++)
            {

                rowIndDef = new RowDefinition
                {
                    Name = "row" + i,
                    Height = new GridLength(tileHeight)
                };
                gameGrid.RowDefinitions.Add(rowIndDef);
            }
            // Cols
            for (int i = 0; i < gameWidth; i++)
            {
                colIndDef = new ColumnDefinition
                {
                    Name = "colum" + i,
                    Width = new GridLength(tileWidth)
                };
                gameGrid.ColumnDefinitions.Add(colIndDef);
            }

            // TESTING // REMOVE // DEBUGGING

            // Width does not work after 32, everything beyond 32 will be put in the same place.
            //this.Width = (LevelInitializer.levelsArray[levelTestSelector].Width * tileWidth) + (gameWidth / 2); // FIX: Maths
            //this.Height = (LevelInitializer.levelsArray[levelTestSelector].Height * tileHeight) + (gameHeight + 47); // FIX: Maths.   47 - size of status panel?

            for (int i = 0; i < LevelInitializer.levelsArray[LevelSelector].Height; i++)
            {
                for (int j = 0; j < LevelInitializer.levelsArray[LevelSelector].Width; j++)
                {
                    RenderTile(LevelInitializer.levelsArray[LevelSelector].Tiles[i, j], "tile");
                    //Debug.WriteLine("Tile: " + LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j].tileSymbol + ", Color: " + LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j].tileColor + ", Func: " + LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j].tileFunction);
                }
            }

            // TODO: Automatic Player Creation, based on level map and integer of max number of players.
            //PLAYER TESTING
            playerOne = new Player("Tonnes", "Knight", 0, 1, 1, 59, 21);
            testPlayerPanel = new StackPanel() {
                Name = "PlayerOne",
                Background = GetTileSetTile(playerOne.tile.tileSymbol, _tiles, playerOne.tile.tileColor, _colors)
            };
            Grid.SetRow(testPlayerPanel, playerOne.tile.Y);
            Grid.SetColumn(testPlayerPanel, playerOne.tile.X);
            gameGrid.Children.Add(testPlayerPanel);
        }

        #region Button Press
        /// <summary>
        /// Keyboard button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Key Press Down Event</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Tile nextTile;

            switch (e.Key)
            {
                case Key.Up: // TODO: Change when implementing more than one player. Add controls for individual players.
                case Key.W:
                    if (playerOne.tile.Y > 0)
                    {
                        nextTile = LevelInitializer.levelsArray[LevelSelector].Tiles[playerOne.tile.Y - 1, playerOne.tile.X];
                        if (nextTile.isWalkable)
                        {
                            playerOne.tile.Y--;
                        }
                        //Debug.WriteLine("symbol: " + nextTile.tileSymbol + ", x: " + nextTile.X + ", y:" + nextTile.Y + ", walk: " + nextTile.isWalkable + ", color: " + nextTile.tileColor + ", type: " + nextTile.tileType);
                    }
                    break;
                case Key.Left:
                case Key.A:
                    if (playerOne.tile.X > 0)
                    {
                        nextTile = LevelInitializer.levelsArray[LevelSelector].Tiles[playerOne.tile.Y, playerOne.tile.X - 1];
                        if (nextTile.isWalkable)
                        {
                            playerOne.tile.X--;
                        }
                        //Debug.WriteLine("symbol: " + nextTile.tileSymbol + ", x: " + nextTile.X + ", y:" + nextTile.Y + ", walk: " + nextTile.isWalkable + ", color: " + nextTile.tileColor + ", type: " + nextTile.tileType);
                    }
                    break;
                case Key.Down:
                case Key.S:
                    if (playerOne.tile.Y < gameWidth - 1)
                    {
                        nextTile = LevelInitializer.levelsArray[LevelSelector].Tiles[playerOne.tile.Y + 1, playerOne.tile.X];
                        if (nextTile.isWalkable)
                        {
                            playerOne.tile.Y++;
                        }
                        //Debug.WriteLine("symbol: " + nextTile.tileSymbol + ", x: " + nextTile.X + ", y:" + nextTile.Y + ", walk: " + nextTile.isWalkable + ", color: " + nextTile.tileColor + ", type: " + nextTile.tileType);
                    }
                    break;
                case Key.Right:
                case Key.D:
                    if (playerOne.tile.X < gameWidth - 1)
                    {
                        nextTile = LevelInitializer.levelsArray[LevelSelector].Tiles[playerOne.tile.Y, playerOne.tile.X + 1];
                        if (nextTile.isWalkable)
                        {
                            playerOne.tile.X++;
                        }
                        //Debug.WriteLine("symbol: " + nextTile.tileSymbol + ", x: " + nextTile.X + ", y:" + nextTile.Y + ", walk: " + nextTile.isWalkable + ", color: " + nextTile.tileColor + ", type: " + nextTile.tileType);
                    }
                    break;
            }
            MoveTile(testPlayerPanel, playerOne.tile.X, playerOne.tile.Y);

            //Debug.WriteLine("x: " + player.tile.X + ", y: " + player.tile.Y);
        }
        #endregion

        /// <summary>
        /// Renders a tile based on a Tile class's information.
        /// </summary>
        /// <param name="tileInformation"></param>
        private void RenderTile(Tile tileInformation, string type)
        {
            string name = type + "x" + tileInformation.X + "y" + tileInformation.Y;            

            StackPanel tile = new StackPanel
            {
                Name = name,
                Background = GetTileSetTile(tileInformation.tileSymbol, _tiles, tileInformation.tileColor, _colors)
            };

            Grid.SetRow(tile, tileInformation.Y); // Areas where it says "Changed --", has been changed. Might have som unintended results.
            Grid.SetColumn(tile, tileInformation.X); // Changed --

            gameGrid.Children.Add(tile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileToMove"></param>
        private void MoveTile(StackPanel tileToMove, int toX, int toY)
        {
            Grid.SetRow(tileToMove, toY);
            Grid.SetColumn(tileToMove, toX);
        }

        #region Creation of tile map and color-pallet map.
        /// <summary>
        /// Create the tiles from an image.
        /// </summary>
        /// <param name="tileset"></param>
        /// <returns></returns>
        private List<Bitmap> CreateTilesFromTileset(Bitmap tileset)
        {
            List<Bitmap> rects = new List<Bitmap>();
            _Rectangle rect;
            _PixelFormat format = tileset.PixelFormat;

            for (int i = 0; i < tileset.Width; i += tileWidth) // Changed --
            {
                for (int j = 0; j < tileset.Height; j += tileHeight) // Changed --
                {
                    rect = new _Rectangle( j, i, _tilesetTileWidth, _tilesetTileHeight ); // Changed --

                    try
                    {
                        rects.Add(tileset.Clone(rect, format));
                    }
                    catch (OutOfMemoryException e) // .Clone throws out of memory if the reactangle is outisde of the bitmap's size.
                    {
                        throw e;
                    }
                }
            }

            tileset.Dispose();

            return rects;
        }

        /// <summary>
        /// Creates an list of colors, based on a pallet image.
        /// </summary>
        /// <returns></returns>
        private List<_Color> CreateColorFromColorTileSet(Bitmap colorset)
        {
            List<_Color> colors = new List<_Color>();
            _Color color;

            for (int i = 0; i < colorset.Height; i += _colorsetTileHeight)
            {
                for (int j = 0; j < colorset.Width; j += _colorsetTileWidth)
                {
                    try
                    {
                        color = colorset.GetPixel(j, i);
                        colors.Add(color);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            colorset.Dispose();

            return colors;
        }
        #endregion

        #region Tile getting
        /// <summary>
        /// Gets a single tile from the tilemap.
        /// </summary>
        /// <param name="tileSymbol"></param>
        /// <returns></returns>
        private ImageBrush GetTileSetTile(int tileSymbol, List<Bitmap> tiles, int tileColor, List<_Color> colors)
        {
            Bitmap tile;
            _Color newColor;

            // Getting a symbol for the tile
            if (tileSymbol <= tiles.Count())
            {
                tile = tiles[tileSymbol];
            } 
            else
            {
                throw new Exception("Tile symbol index does not exist. Symbol index outside of range?");
            }

            // Coloring of tiles
            if (tileColor != 1 || tileSymbol != 7) // Second color equals the whole x or y spectrum except for the four corners. // Different symbols have different colors? 
            {
                for (int i = 0; i < tile.Height; i++)
                {
                    for (int j = 0; j < tile.Width; j++)
                    {
                        if (tile.GetPixel(i, j).A > 0) // TODO: Make less resources intensive by not doing the same symbol more than once. // TODO2: And maybe use pointers instead of Get/Set Pixels, it's slow.
                        {
                            newColor = _Color.FromArgb(tile.GetPixel(i, j).A, colors[tileColor].R, colors[tileColor].G, colors[tileColor].B);
                            tile.SetPixel(i, j, newColor);
                        }
                    }
                }
            }

            // Creating a imagebrush for use in the background of stackpanels.
            ImageBrush brush = new ImageBrush()
            {
                ImageSource = ConvertBitmapToBitmapSource(tile) // Converts to bitmapsource
            };

            return brush;
        }
        #endregion

        #region Helper methods

        /// <summary>
        /// Used to delete Interpolated "integers".
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Converts Bitmap to BitmapSource
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private BitmapSource ConvertBitmapToBitmapSource(Bitmap image)
        {
            IntPtr hBitmap = image.GetHbitmap();
            BitmapSource convertedImage;

            try
            {
                convertedImage = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
            }
            catch (Exception e)
            {
                throw e; // TODO: Implement better error handling.
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return convertedImage;
        }

        #endregion
    }
}

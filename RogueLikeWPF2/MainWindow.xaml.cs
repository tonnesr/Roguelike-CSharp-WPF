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
        public static readonly int gameWidth = 32;
        public static readonly int gameHeight = 32;
        public static readonly int tileWidth = 16;
        public static readonly int tileHeight = 16;
        public static readonly int windowWidth = gameWidth * tileWidth;
        public static readonly int windowHeight = gameWidth * tileHeight;

        //public static readonly int sumTileWidth = tileset.Width / _tilesetTileWidth;
        //public static readonly int sumTileHeight = sumTileWidth;

        // Grid Positions
        private static readonly int _statusbarHeight = 40;
        private static readonly int _statusbarGridPosition = 0;
        private static readonly int _gameGridPosition = 1;

        // TILESET AND COLORSET
        // Tileset
        private static readonly string _tilesetURI = "./Core/Tiles/tileset_16x16_128.png";
        private static readonly int _tilesetTileWidth = 16;
        private static readonly int _tilesetTileHeight = 16;
        private static List<Bitmap> _tiles;
        // Colorset
        private static readonly string _colorsetURI = "./Core/Tiles/ColorTileset.png";
        private static readonly int _colorsetTileWidth = 1;
        private static readonly int _colorsetTileHeight = 1;
        private static List<_Color> _colors;

        // Grids
        public static Grid mainGrid;
        public static Grid gameGrid;


        // Game
        private static int gameStatus = 1; // 0 - Start Menu, 1 - Game, 2 - Pause?
        // Testing/
        private Player playerOne;
        private static Bitmap playerOneSymbol;

        /// <summary>
        /// Constructor for MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Roguelike";
            this.Width = windowWidth;
            this.Height = windowHeight;
            this.PreviewKeyDown += Window_PreviewKeyDown;

            LevelInitializer.LoadLevels();

            _tiles = CreateTilesFromTileset( new Bitmap( _tilesetURI) ); // TODO: Way to rotate images, so that we don't need the same symbol/tile four times rotated.
            _colors = CreateColorFromColorTileSet( new Bitmap( _colorsetURI) );

            InitializeGameWindow();


            // TESTING // DEBUGGING // REMOVE
            playerOneSymbol = new Bitmap("./Core/Entities/Player/player_16x16_16x16.png"); // REMOVE
            playerOne = new Player(1, 1, "Tonnes", "Knight", 1, 0); // REMOVE
            //MainGameLoop(); // REMOVE
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
            RowDefinition mainRow = new RowDefinition // Status bar space
            {
                Name = "MainRow",
                Height = new GridLength( _statusbarHeight )
            };
            RowDefinition mainGameRow = new RowDefinition
            {
                Name = "MainGameRow",
                Height = new GridLength( 1, GridUnitType.Star ) // 1 for percentage of * (or something), but 1 is 100%.
            };
            mainGrid.RowDefinitions.Add( mainRow );
            mainGrid.RowDefinitions.Add( mainGameRow );
            this.AddChild(mainGrid);

            // Status bar (top space)
            StackPanel statusPanel = new StackPanel
            {
                Name = "StatusPanel",
                Background = _Brushes.Black
            };
            statusPanel.SetValue( Grid.RowProperty, _statusbarGridPosition );
            mainGrid.Children.Add( statusPanel );

            // Game screen / grid / grid inside mainGrid
            gameGrid = new Grid
            {
                Name = "GameGrid"
            };
            gameGrid.SetValue( Grid.RowProperty, _gameGridPosition );
            mainGrid.Children.Add( gameGrid );


            // Second grid (game grid) content
            RowDefinition rowIndDef;
            ColumnDefinition colIndDef;

            // Generate content of game grid
            // Generate rows for the game grid.
            for (int i = 0; i < gameHeight; i++)
            {
                // Rows
                rowIndDef = new RowDefinition
                {
                    Name = "row" + i,
                    Height = new GridLength(tileHeight, GridUnitType.Star) // Grid type thing .Star (might cause problems) //GridUnitType.Star
                };
                gameGrid.RowDefinitions.Add(rowIndDef);
            }
            for (int i = 0; i < gameWidth; i++)
            {
                // Cols
                colIndDef = new ColumnDefinition
                {
                    Name = "colum" + i,
                    Width = new GridLength(tileWidth, GridUnitType.Star) // Grid type thing .Star (might cause problems) //GridUnitType.Star
                };
                gameGrid.ColumnDefinitions.Add(colIndDef);
            }


            // RENDERING TEST / DEBUGGING / REMOVE
            int levelTestSelector = 0; // TODO: Add level progression.
            for (int i = 0; i < LevelInitializer.levelsArray[levelTestSelector].Height; i++)
            {
                for (int j = 0; j < LevelInitializer.levelsArray[levelTestSelector].Width; j++)
                {
                    RenderTile(LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j]);
                    //Debug.WriteLine("Tile: " + LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j].tileSymbol + ", Color: " + LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j].tileColor + ", Func: " + LevelInitializer.levelsArray[levelTestSelector].Tiles[i, j].tileFunction);
                }
            }
        }

        private void MainGameLoop()
        {
            //StackPanel playerPanel = new StackPanel() {
            //    Name = "PlayerOne",    
            //};

            do // WE DO NOT NEED A LOOP. THE PROGRAM IS ALREADY IN A LOOP. (WPF).
            {
                Debug.WriteLine(playerOne.X + ", " + playerOne.Y);
            } while (gameStatus == 1);
        }

        #region Button Press
        /// <summary>
        /// Keyboard button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Key Press Down Event</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            // REMOVE // TESTING // DEBUGGING
            switch(e.Key)
            {
                case Key.W:
                    playerOne.Y--;
                    break;
                case Key.A:
                    playerOne.X--;
                    break;
                case Key.S:
                    playerOne.Y++;
                    break;
                case Key.D:
                    playerOne.X++;
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Renders a tile based on a Tile class's information.
        /// </summary>
        /// <param name="tileInformation"></param>
        private void RenderTile(Tile tileInformation)
        {
            string name = "Tile" + tileInformation.X + tileInformation.Y;            

            StackPanel tile = new StackPanel
            {
                Name = name,
                Background = GetTileSetTile(tileInformation.tileSymbol, _tiles, tileInformation.tileColor, _colors)
            };

            Grid.SetRow(tile, tileInformation.Y); // Areas where it says "Changed --", has been changed. Might have som unintended results.
            Grid.SetColumn(tile, tileInformation.X); // Changed --

            gameGrid.Children.Add(tile);
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

using GameEngine2D.Engine.Source.Global;
using GameEngine2D.Engine.Source.Level;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Layer;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GameEngine2D.Source.Level
{
    public class LDTKMap
    {

        string path = "SpriteSheets/MagicCliffsEnvironment/";
        Dictionary<string, Texture2D> spriteSheets = new Dictionary<string, Texture2D>();

        public LDTKMap(LDTKJson json)
        {
            //Globals.Camera.LevelGridCountH = 256;
            //Globals.Camera.LevelGridCountW = 256;

            foreach (GameEngine2D.Engine.Source.Level.Level level in json.Levels)
            {
                Array.Reverse(level.LayerInstances);
                foreach (GameEngine2D.Engine.Source.Level.LayerInstance layerInstance in level.LayerInstances)
                {
                    Layer.Layer currentLayer = null;
                    string tileSet = null;
                    if (layerInstance.Identifier.StartsWith("Colliders"))
                    {
                        currentLayer = RootContainer.Instance.EntityLayer;
                        tileSet = null;
                        //continue;
                    } else if (layerInstance.Identifier.StartsWith("Background"))
                    {
                        //currentLayer = RootContainer.Instance.BackgroundLayer;
                        currentLayer = RootContainer.Instance.AddScrollableLayer();
                        tileSet = "tileset";
                    }
                    else
                    {
                        //continue;
                        if (layerInstance.Identifier.StartsWith("Island"))
                        {
                            tileSet = "far-grounds";
                        } else if(layerInstance.Identifier.StartsWith("Sea"))
                        {
                            tileSet = "sea";
                        }
                        else if (layerInstance.Identifier.StartsWith("Sky"))
                        {
                            tileSet = "sky";
                        }
                        else if (layerInstance.Identifier.StartsWith("Clouds"))
                        {
                            tileSet = "clouds";
                        }
                        currentLayer = RootContainer.Instance.AddScrollableLayer();
                    }
                    if (tileSet != null && !spriteSheets.ContainsKey(path+tileSet)) {
                        spriteSheets.Add(path + tileSet, SpriteUtil.LoadTexture("SpriteSheets/MagicCliffsEnvironment/" + tileSet));
                    }
                    if (layerInstance.Identifier.StartsWith("Colliders"))
                    {
                        //public Dictionary<string, dynamic>[] IntGrid { get; set; }
                        foreach (Dictionary<string, dynamic> dict in layerInstance.IntGrid )
                        {
                            int y = (int)Math.Floor((decimal)dict["coordId"] / layerInstance.CWid);
                            int x = (int)(dict["coordId"] - y * layerInstance.CWid);
                            Entity e = new Entity(currentLayer, null, new Vector2(x, y) * Config.GRID, SpriteUtil.CreateRectangle(Config.GRID, Color.Black), true);
                            e.Visible = false;
                        }

                    } else
                    {
                        foreach (TileInstance tile in layerInstance.GridTiles)
                        {
                            //Logger.Log("Tile: " + tile.);

                            long tileId = tile.T;
                            int atlasGridBaseWidth = (int)layerInstance.GridSize;
                            int padding = 0;
                            int spacing = 0;
                            int gridSize = 16;

                            int gridTileX = (int)tileId - atlasGridBaseWidth * (int)Math.Floor((decimal)(tileId / atlasGridBaseWidth));
                            int pixelTileX = padding + gridTileX * (gridSize + spacing);

                            int gridTileY = (int)Math.Floor((decimal)tileId / atlasGridBaseWidth);
                            var pixelTileY = padding + gridTileY * (gridSize + spacing);

                            Entity e = new Entity(currentLayer, null, new Vector2(tile.Px[0], tile.Px[1]), spriteSheets["SpriteSheets/MagicCliffsEnvironment/" + tileSet]);
                            e.SourceRectangle = new Rectangle((int)tile.Src[0], (int)tile.Src[1], gridSize, gridSize);
                            //e.Pivot = new Vector2(gridSize / 2, gridSize / 2);

                        }
                    }
                }
            }
        }

        /*private readonly string LEVEL = "Level";
        private readonly string COLLIDERS = "Colliders";
        private readonly string BACKGROUND = "Background";
        private readonly string MOSSY = "MossyLayer";

        public HashSet<Vector2> Colliders = new HashSet<Vector2>();
        public HashSet<HashSet<Vector2>> Backgrounds = new HashSet<HashSet<Vector2>>();

        private Dictionary<string, Defs.Tileset> tiles = new Dictionary<string, Defs.Tileset>();

        public void LoadMap()
        {
            foreach (Defs.LDTKLayer layer in defs.layers)
            {
                Logger.Log("Layer: " + layer.identifier);
            }

            foreach (Defs.Tileset tileset in defs.tilesets)
            {
                tiles.Add(tileset.identifier, tileset);
                //Logger.Log("Tileset: " + tileset.identifier);
            }

            foreach (Level level in levels)
            {
                foreach (LayerInstance layer in level.layerInstances)
                {
                    if (layer.__identifier.Equals(COLLIDERS))
                    {
                        foreach (LayerInstance.IntGrid grid in layer.intGrid)
                        {
                            //var coordId = gridBasedX + gridBasedY * gridBasedWidth;
                            //var gridBasedY = Math.floor( coordId / gridBasedWidth );
                            //var gridBasedX = coordId - gridBasedY * gridBasedWidth
                            //cx / cy for a grid based X / Y
                            //cWid / cHei for a grid based width / height

                            int y = (int)Math.Floor((decimal)grid.coordId / layer.__cWid);
                            int x = grid.coordId - y * layer.__cWid;

                            Colliders.Add(new Vector2(x, y));
                        }
                    }
                    else if (layer.__identifier.StartsWith(MOSSY))
                    {

                        //var coordId = gridBasedX + gridBasedY * gridBasedWidth;
                        // Get "grid-based" coordinate of the tileId 
                        //var gridTileX = tileId - atlasGridBaseWidth * Std.int(tileId / atlasGridBaseWidth);

                        // Get the atlas pixel coordinate 
                        //var pixelTileX = padding + gridTileX * (gridSize + spacing);
                        //Same goes for the Y coordinates:

                        // Get "grid-based" coordinate of the tileId 
                        //var gridTileY = Std.int(tileId / atlasGridBaseWidth)

                        // Get the atlas pixel coordinate 
                        //var pixelTileY = padding + gridTileY * (gridSize + spacing);

                        foreach (LayerInstance.GridTile gridTile in layer.gridTiles)
                        {
                            int tileId = gridTile.t;
                            int atlasGridBaseWidth = 16;
                            int padding = 0;
                            int spacing = 0;
                            int gridSize = 16;

                            int gridTileX = tileId - atlasGridBaseWidth * (int)Math.Floor((decimal)(tileId / atlasGridBaseWidth));
                            int pixelTileX = padding + gridTileX * (gridSize + spacing);

                            int gridTileY = (int)Math.Floor((decimal)tileId / atlasGridBaseWidth);
                            var pixelTileY = padding + gridTileY * (gridSize + spacing);

                            new Entity(RootContainer.Instance.ColliderLayer, null, new Vector2(gridTile.px[0], gridTile.px[1]), SpriteUtil.CreateRectangle(Config.GRID, SpriteUtil.GetRandomColor()));

                            Logger.Log(JsonSerializer.Serialize(gridTile));

                            Logger.Log("gridTileX: " + gridTileX);
                            Logger.Log("pixelTileX: " + pixelTileX);
                            Logger.Log("gridTileY: " + gridTileY);
                            Logger.Log("pixelTileY: " + pixelTileY);
                        }

                    }
                }
            }

        }

        public Defs defs { get; set; }

        public string jsonVersion { get; set; }

        public int worldGridWidth { get; set; }
        public int worldGridHeight { get; set; }

        public IList<Level> levels { get; set; }

        public class Level
        {
            public string identifier { get; set; }
            public IList<LayerInstance> layerInstances { get; set; }
        }

        public class LayerInstance
        {
            public List<IntGrid> intGrid { get; set; }

            public List<GridTile> gridTiles { get; set; }

            public string __identifier { get; set; }

            public int __cWid { get; set; }

            public int __tilesetDefUid;

            public string __tilesetRelPath;

            public class IntGrid
            {
                public int coordId { get; set; }
                public int v { get; set; }
            }

            public class GridTile
            {
                public List<int> px { get; set; }

                public List<int> src { get; set; }

                public int f { get; set; }

                public int t { get; set; }

                public List<int> d { get; set; }
            }
        }

        public class Defs
        {

            public List<LDTKLayer> layers { get; set; }

            public List<Tileset> tilesets { get; set; }
            public class LDTKLayer
            {
                public string __type { get; set; }
                public string identifier { get; set; }
                public string type { get; set; }
                public int uid { get; set; }
                public int gridSize { get; set; }
                public float displayOpacity { get; set; }
                public float pxOffsetX { get; set; }
                public float pxOffsetY { get; set; }
                //public Dictionary<string, string> intGridValues { get; set; }
                public string autoTilesetDefUid { get; set; }
                public List<string> autoRuleGroups { get; set; }
                public string autoSourceLayerDefUid { get; set; }
                public int? tilesetDefUid { get; set; }
                public float tilePivotX { get; set; }
                public float tilePivotY { get; set; }
            }

            public class Tileset
            {
                public string identifier { get; set; }
                public int uid { get; set; }

                public string relPath { get; set; }
                public int pxWid { get; set; }
                public int pxHei { get; set; }
                public int tileGridSize { get; set; }
                public float spacing { get; set; }
                public float padding { get; set; }
            }
        }*/
    }
}

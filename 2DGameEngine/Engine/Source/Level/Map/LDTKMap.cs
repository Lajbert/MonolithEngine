using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Engine.Source.Level.Collision;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.GridCollision;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using MonolithEngine.Engine.Source.Scene;

namespace MonolithEngine.Source.Level
{
    public class LDTKMap
    {

        private readonly string CONTENT = "Content/";
        private readonly string EXTENSION_DOT = ".";
        private readonly string BACKGROUND = "Background";
        private readonly string FOREGROUND = "Foreground";
        private readonly string PARALLAX = "Parallax";
        private readonly string COLLIDERS = "Colliders";
        private readonly string ENTITIES = "Entities";
        private readonly string COIN = "Coin";

        //string path = "SpriteSheets/MagicCliffsEnvironment/";
        //Dictionary<string, Texture2D> spriteSheets = new Dictionary<string, Texture2D>();

        Dictionary<string, Texture2D> tilesets = new Dictionary<string, Texture2D>();

        public HashSet<EntityInstance> entities = new HashSet<EntityInstance>();

        private TileGroup tileGroup;

        private float scrollSpeedModifier = 0f;

        //private Vector2 pivot = new Vector2(-Config.GRID / 2, 0);
        private Vector2 pivot = Vector2.Zero;

        public LDTKMap(AbstractScene scene, LDTKJson json)
        {
            //Globals.Camera.LevelGridCountH = 256;
            //Globals.Camera.LevelGridCountW = 256;

            foreach (TilesetDefinition tileset in json.Defs.Tilesets) {
                string path = GetMonoGameContentName(tileset.RelPath);
                tilesets.Add(path, TextureCache.GetTexture(path));
            }

            foreach (Engine.Source.Level.Level level in json.Levels)
            {
                Array.Reverse(level.LayerInstances);
                foreach (LayerInstance layerInstance in level.LayerInstances)
                {

                    foreach (EntityInstance entity in layerInstance.EntityInstances)
                    {
                        entities.Add(entity);
                    }

                    string layerName = layerInstance.Identifier;
                    Layer currentLayer = null;
                    Texture2D tileSet = null;
                    if (layerName.StartsWith(COLLIDERS))
                    {
                        currentLayer = scene.LayerManager.EntityLayer;
                        tileSet = null;
                        //continue;
                    } else if (layerName.StartsWith(BACKGROUND))
                    {
                        //currentLayer = RootContainer.Instance.BackgroundLayer;
                        currentLayer = scene.LayerManager.CreateBackgroundLayer(int.Parse(layerName[layerName.Length - 1] + ""));
                        tileSet = tilesets[GetMonoGameContentName(layerInstance.TilesetRelPath)];
                        tileGroup = new TileGroup();
                    }
                    else if (layerName.StartsWith(PARALLAX))
                    {
                        //currentLayer = RootContainer.Instance.BackgroundLayer;
                        scrollSpeedModifier += 0.1f;
                        currentLayer = scene.LayerManager.CreateParallaxLayer(int.Parse(layerName[layerName.Length - 1] + ""), scrollSpeedModifier, true);
                        tileSet = tilesets[GetMonoGameContentName(layerInstance.TilesetRelPath)];
                        tileGroup = new TileGroup();
                    }
                    else if (layerName.StartsWith(FOREGROUND))
                    {
                        //currentLayer = RootContainer.Instance.BackgroundLayer;
                        currentLayer = scene.LayerManager.CreateForegroundLayer(int.Parse(layerName[layerName.Length - 1] + ""));
                        tileSet = tilesets[GetMonoGameContentName(layerInstance.TilesetRelPath)];
                        tileGroup = new TileGroup();
                    }
                 
                    if (layerInstance.Identifier.StartsWith(COLLIDERS))
                    {
                        currentLayer = null;
                        //public Dictionary<string, dynamic>[] IntGrid { get; set; }
                        foreach (IntGridValueInstance grid in layerInstance.IntGrid )
                        {
                            int y = (int)Math.Floor((decimal)grid.CoordId / layerInstance.CWid);
                            int x = (int)(grid.CoordId - y * layerInstance.CWid);
                            StaticCollider e = new StaticCollider((new Vector2(x, y)));
                            switch (grid.V)
                            {
                                case 0:
                                    e.AddTag("Collider");
                                    break;
                                case 1:
                                    e.AddTag("SlideWall");
                                    break;
                                case 2:
                                    //e.AddTag("Platform");
                                    break;
                                case 3:
                                    //e.AddTag("Ladder");
                                    //e.BlocksMovement = false;
                                    break;
                                case 4:
                                    e.AddTag("Platform");
                                    e.AddBlockedDirection(Direction.WEST);
                                    break;
                                case 5:
                                    e.AddTag("Platform");
                                    e.AddBlockedDirection(Direction.EAST);
                                    break;
                                case 6:
                                    e.AddTag("Platform");
                                    e.AddBlockedDirection(Direction.NORTH);
                                    break;
                                case 7:
                                    e.AddTag("Platform");
                                    e.AddBlockedDirection(Direction.SOUTH);
                                    break;
                            }
                        }

                    } else
                    {
                        foreach (TileInstance tile in layerInstance.GridTiles)
                        {
                            //Logger.Log("Tile: " + tile.);
                            //if (layerInstance.Identifier.StartsWith(PARALLAX)) { currentLayer = null;  continue; }
                            
                            long tileId = tile.T;
                            int atlasGridBaseWidth = (int)layerInstance.GridSize;
                            int padding = 0;
                            int spacing = 0;
                            int gridSize = Config.GRID;

                            int gridTileX = (int)tileId - atlasGridBaseWidth * (int)Math.Floor((decimal)(tileId / atlasGridBaseWidth));
                            int pixelTileX = padding + gridTileX * (gridSize + spacing);

                            int gridTileY = (int)Math.Floor((decimal)tileId / atlasGridBaseWidth);
                            var pixelTileY = padding + gridTileY * (gridSize + spacing);

                            /*Entity e = new Entity(currentLayer, null, new Vector2(tile.Px[0], tile.Px[1]), tileSet);
                            e.SourceRectangle = new Rectangle((int)tile.Src[0], (int)tile.Src[1], gridSize, gridSize);
                            e.Pivot = pivot;*/

                            Rectangle rect = new Rectangle((int)tile.Src[0], (int)tile.Src[1], gridSize, gridSize);
                            Vector2 pos = new Vector2(tile.Px[0], tile.Px[1]);
                            Color[] data = new Color[gridSize * gridSize];
                            //tileSet.GetData<Color>(data);
                            tileSet.GetData(0, rect, data, 0, data.Length);

                            if (tile.F != 0)
                            {
                                Texture2D flipped = TextureUtil.CreateRectangle(gridSize, Color.Black);
                                flipped.SetData(data);
                                if (tile.F == 1)
                                {
                                    flipped = TextureUtil.FlipTexture(flipped, false, true);
                                } 
                                else if (tile.F == 2)
                                {
                                    flipped = TextureUtil.FlipTexture(flipped, true, false);
                                }
                                else
                                {
                                    flipped = TextureUtil.FlipTexture(flipped, true, true);
                                }
                                
                                flipped.GetData(data);
                            }
                            //public void GetData<T>(int level, int arraySlice, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct;
                            
                            tileGroup.AddColorData(data, pos);
                            //e.Visible = false;
                            //e.Active = false;
                            //e.Pivot = new Vector2(gridSize / 2, gridSize / 2);

                        }
                        if (currentLayer != null)
                        {
                            Entity tile = new Entity(currentLayer, null, new Vector2(0, 0));
                            tile.SetSprite(tileGroup.GetTexture());
                            tile.GetComponent<Sprite>().DrawOffset = pivot;
                            //tile.Pivot = pivot;
                        }
                    }
                }
            }
        }

        private string GetMonoGameContentName(string fullpath)
        {
            string path = fullpath.Substring(fullpath.IndexOf(CONTENT) + CONTENT.Length);
            return path.Substring(0, path.LastIndexOf(EXTENSION_DOT));
        }
    }
}

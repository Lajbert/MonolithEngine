using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// A sample implementation of LDtk level editor software.
    /// By following the naming conventions in the software, the parser
    /// will be able to automatically parse and handle foreground,
    /// background and parallax layers.
    /// </summary>
    public class LDTKMap
    {

        private readonly string PREFIX = "../";
        private readonly string EXTENSION_DOT = ".";
        private readonly string BACKGROUND = "Background";
        private readonly string FOREGROUND = "Foreground";
        private readonly string PARALLAX = "Parallax";
        private readonly string COLLIDERS = "Colliders";

        private TileGroup tileGroup;

        private TileGroup mergedBackgroundTileGroup;
        private Layer mergedBackgroundLayer;

        private TileGroup mergedForegroundTileGroup;
        private Layer mergedForegroundLayer;

        private Vector2 pivot = Vector2.Zero;

        private LDTKJson world;

        public LDTKMap(LDTKJson json)
        {
            world = json;

            foreach (TilesetDefinition tileset in world.Defs.Tilesets) {
                string path = GetMonoGameContentName(tileset.RelPath);
                Assets.LoadTexture(path, path);
            }
        }

        public List<string> GetLevelNames()
        {
            List<string> result = new List<string>();
            foreach (Level level in world.Levels)
            {
                result.Add(level.Identifier);
            }
            result.Sort();
            return result;
        }

        public Vector2 GetLevelSize(string levelID)
        {
            List<string> result = new List<string>();
            foreach (Level level in world.Levels)
            {
                if (level.Identifier.Equals(levelID))
                {
                    return new Vector2(level.PxWid, level.PxHei);
                }
            }

            throw new Exception("Level not found.");
        }

        private static int CompareLayerInstances(LayerInstance li1, LayerInstance li2)
        {
            return li1.Identifier.CompareTo(li2.Identifier);
        }

        public HashSet<EntityInstance> ParseLevel(AbstractScene scene)
        {
            Logger.Debug("Parsing level...");
            HashSet<EntityInstance> entities = new HashSet<EntityInstance>();
            mergedBackgroundTileGroup = new TileGroup();
            mergedForegroundTileGroup = new TileGroup();
            mergedBackgroundLayer = scene.LayerManager.CreateBackgroundLayer();
            mergedForegroundLayer = scene.LayerManager.CreateForegroundLayer();

            foreach (Level level in world.Levels)
            {
                if (!level.Identifier.Equals(scene.GetName()))
                {
                    continue;
                }
                Logger.Debug("Parsing level: " + level.Identifier);
                float scrollSpeedModifier = 0f;

                Array.Sort(level.LayerInstances, CompareLayerInstances);

                foreach (LayerInstance layerInstance in level.LayerInstances)
                {

                    Logger.Debug("Parsing layer: " + layerInstance.Identifier);

                    foreach (EntityInstance entity in layerInstance.EntityInstances)
                    {
                        entities.Add(entity);
                    }

                    string layerName = layerInstance.Identifier;
                    Layer currentLayer = null;
                    Texture2D tileSet = null;
                    Color[] imageData;

                    if (layerName.StartsWith(COLLIDERS) && layerInstance.GridTiles.Length > 0)
                    {
                        currentLayer = scene.LayerManager.EntityLayer;
                        tileSet = null;
                    }
                    else if (layerName.StartsWith(BACKGROUND) && layerInstance.GridTiles.Length > 0)
                    {
                        currentLayer = scene.LayerManager.CreateBackgroundLayer(int.Parse(layerName[layerName.Length - 1] + ""));
                        tileSet = Assets.GetTexture(GetMonoGameContentName(layerInstance.TilesetRelPath));
                    }
                    else if (layerName.StartsWith(PARALLAX) && layerInstance.GridTiles.Length > 0)
                    {
                        scrollSpeedModifier += 0.1f;
                        currentLayer = scene.LayerManager.CreateParallaxLayer(int.Parse(layerName[layerName.Length - 1] + ""), scrollSpeedModifier, true);
                        tileSet = Assets.GetTexture(GetMonoGameContentName(layerInstance.TilesetRelPath));
                        tileGroup = new TileGroup();
                    }
                    else if (layerName.StartsWith(FOREGROUND) && layerInstance.GridTiles.Length > 0)
                    {
                        tileSet = Assets.GetTexture(GetMonoGameContentName(layerInstance.TilesetRelPath));
                    }
                    else
                    {
                        continue;
                    }

                    Logger.Debug("Loading grid tiles...");

                    imageData = new Color[tileSet.Width * tileSet.Height];
                    tileSet.GetData(0, new Rectangle(0, 0, tileSet.Width, tileSet.Height), imageData, 0, imageData.Length);

                    foreach (TileInstance tile in layerInstance.GridTiles)
                    {
                        TileGroup currentTileGroup;

                        if (layerName.StartsWith(BACKGROUND))
                        {
                            currentTileGroup = mergedBackgroundTileGroup;
                        }
                        else if (layerName.StartsWith(FOREGROUND))
                        {
                            currentTileGroup = mergedForegroundTileGroup;
                        }
                        else
                        {
                            currentTileGroup = tileGroup;
                        }

                        int gridSize = Config.GRID;
                        /*long tileId = tile.T;
                        int atlasGridBaseWidth = (int)layerInstance.GridSize;
                        int padding = 0;
                        int spacing = 0;

                        int gridTileX = (int)tileId - atlasGridBaseWidth * (int)Math.Floor((decimal)(tileId / atlasGridBaseWidth));
                        int pixelTileX = padding + gridTileX * (gridSize + spacing);

                        int gridTileY = (int)Math.Floor((decimal)tileId / atlasGridBaseWidth);
                        var pixelTileY = padding + gridTileY * (gridSize + spacing);*/

                        Rectangle rect = new Rectangle((int)tile.Src[0], (int)tile.Src[1], gridSize, gridSize);
                        Vector2 pos = new Vector2(tile.Px[0], tile.Px[1]);

                        Color[] data = new Color[gridSize * gridSize];
                        //tileSet.GetData(0, rect, data, 0, data.Length);

                        int i = 0;
                        for (int y = rect.Y; y < rect.Y + gridSize; y++)
                        {
                            for (int x = rect.X; x < rect.X + gridSize; x++)
                            {

                                int idx1D = y * tileSet.Width + x;
                                data[i++] = imageData[idx1D];
                            }
                        }

                        if (tile.F != 0)
                        {
                            Texture2D flipped = AssetUtil.CreateRectangle(gridSize, Color.Black);
                            flipped.SetData(data);
                            if (tile.F == 1)
                            {
                                flipped = AssetUtil.FlipTexture(flipped, false, true);
                            }
                            else if (tile.F == 2)
                            {
                                flipped = AssetUtil.FlipTexture(flipped, true, false);
                            }
                            else
                            {
                                flipped = AssetUtil.FlipTexture(flipped, true, true);
                            }

                            flipped.GetData(data);
                        }

                        currentTileGroup.AddColorData(data, pos);
                    }
                    if (currentLayer != null && !layerName.StartsWith(BACKGROUND) && !layerName.StartsWith(FOREGROUND))
                    {
                        Entity tile = new Entity(currentLayer, null, new Vector2(0, 0));
                        tile.SetSprite(tileGroup.GetTexture());
                        tile.GetComponent<Sprite>().DrawOffset = pivot;
                    }
                }
                if (!mergedBackgroundTileGroup.IsEmpty())
                {
                    Logger.Debug("Starting texture merging...");
                    Logger.Debug("Merging background layers into one texture: " + mergedBackgroundTileGroup.GetTexture().Bounds);
                    Entity mergedBG = new Entity(mergedBackgroundLayer, null, new Vector2(0, 0));
                    mergedBG.SetSprite(mergedBackgroundTileGroup.GetTexture());
                    mergedBG.GetComponent<Sprite>().DrawOffset = pivot;
                    mergedBG.Active = false;
                }

                if (!mergedForegroundTileGroup.IsEmpty())
                {
                    Logger.Debug("Merging foreground layers into one texture: " + mergedForegroundTileGroup.GetTexture().Bounds);
                    Entity mergedFG = new Entity(mergedForegroundLayer, null, new Vector2(0, 0));
                    mergedFG.SetSprite(mergedForegroundTileGroup.GetTexture());
                    mergedFG.GetComponent<Sprite>().DrawOffset = pivot;
                    mergedFG.Active = false;
                }

            }
            return entities;
        }

        public Dictionary<Vector2, int> GetIntGrid(AbstractScene scene, string gridName)
        {
            Dictionary<Vector2, int> result = new Dictionary<Vector2, int>();
            foreach (Level level in world.Levels)
            {
                if (!level.Identifier.Equals(scene.GetName()))
                {
                    continue;
                }

                foreach (LayerInstance layerInstance in level.LayerInstances)
                {
                    if (layerInstance.Identifier.Equals(gridName))
                    {
                        for (int i = 0; i < layerInstance.IntGridCsv.Length; i++)
                        {
                            int grid = (int)layerInstance.IntGridCsv[i];
                            if (grid == 0)
                            {
                                continue;
                            }
                            int y = (int)Math.Floor((decimal)i / layerInstance.CWid);
                            int x = (int)(i - y * layerInstance.CWid);
                            result.Add(new Vector2(x, y), grid);
                        }
                    }
                }
            }
            return result;
        }

        public Texture2D GetLayerAsTexture(string levelName, string layerName)
        {
            TileGroup result = new TileGroup();
            foreach (Level level in world.Levels)
            {
                if (level.Identifier != levelName)
                {
                    continue;
                }
                foreach (LayerInstance layerInstance in level.LayerInstances)
                {
                    Texture2D tileSet = null;
                    if (layerName ==layerInstance.Identifier && layerInstance.GridTiles.Length > 0)
                    {
                        tileSet = Assets.GetTexture(GetMonoGameContentName(layerInstance.TilesetRelPath));

                        foreach (TileInstance tile in layerInstance.GridTiles)
                        {
                            long tileId = tile.T;
                            int atlasGridBaseWidth = (int)layerInstance.GridSize;
                            int padding = 0;
                            int spacing = 0;
                            int gridSize = Config.GRID;

                            int gridTileX = (int)tileId - atlasGridBaseWidth * (int)Math.Floor((decimal)(tileId / atlasGridBaseWidth));
                            int pixelTileX = padding + gridTileX * (gridSize + spacing);

                            int gridTileY = (int)Math.Floor((decimal)tileId / atlasGridBaseWidth);
                            var pixelTileY = padding + gridTileY * (gridSize + spacing);

                            Rectangle rect = new Rectangle((int)tile.Src[0], (int)tile.Src[1], gridSize, gridSize);
                            Vector2 pos = new Vector2(tile.Px[0], tile.Px[1]);
                            Color[] data = new Color[gridSize * gridSize];
                            tileSet.GetData(0, rect, data, 0, data.Length);

                            if (tile.F != 0)
                            {
                                Texture2D flipped = AssetUtil.CreateRectangle(gridSize, Color.Black);
                                flipped.SetData(data);
                                if (tile.F == 1)
                                {
                                    flipped = AssetUtil.FlipTexture(flipped, false, true);
                                }
                                else if (tile.F == 2)
                                {
                                    flipped = AssetUtil.FlipTexture(flipped, true, false);
                                }
                                else
                                {
                                    flipped = AssetUtil.FlipTexture(flipped, true, true);
                                }

                                flipped.GetData(data);
                            }
                            result.AddColorData(data, pos);
                        }
                        return result.GetTexture();
                    }
                }
            }
            return null;
        }

        private string GetMonoGameContentName(string fullpath)
        {
            string path = fullpath.Substring(fullpath.IndexOf(PREFIX) + PREFIX.Length);
            return path.Substring(0, path.LastIndexOf(EXTENSION_DOT));
        }
    }
}

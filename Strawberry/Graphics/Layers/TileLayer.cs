/*
 * Strawberry Game Engine
 * File: TileLayer.cs
 * Author: Koosha Aabedini Nassab
 *
 * Tile-based layer rendering for tilemaps. Supports grid-indexed and exact-rect tile pushes,
 * plus loading tilemaps exported by TileMap Studio (https://tilemapstudio.app/).
 */

using Strawberry.Common;
using Strawberry.Core;
using Strawberry.Math;
using System.Text.Json;

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// A layer that renders a tilemap from a single texture.
    /// Tiles are stored persistently (not cleared every frame) and can be pushed either
    /// by grid index (column, row) or by an exact texture source rectangle.
    /// Supports loading the JSON format exported by TileMap Studio.
    /// </summary>
    public class TileLayer : Layer
    {
        const int DefaultMaxBatchCount = 8192;

        readonly int maxBatchCount;
        readonly List<TileEntry> tiles = new List<TileEntry>();
        readonly List<SpriteQuad> quadList = new List<SpriteQuad>();

        Geometry<VertexPositionTexColor> geometry;
        VertexPositionTexColor[] vertices;
        uint[] indices;

        BasicShader defaultShader;
        string activeBlendName = "Default";

        /// <summary>
        /// Internal representation of a single placed tile.
        /// </summary>
        private struct TileEntry
        {
            public float TexLeft, TexTop, TexRight, TexBottom;
            public float WorldX, WorldY;
            public float DestW, DestH;
            public Color Color;
        }

        /// <summary>
        /// Gets the currently active shader used for tile rendering.
        /// </summary>
        public BasicShader ActiveShader { get; private set; }

        /// <summary>
        /// Gets the graphics context for the current scene.
        /// </summary>
        public IGraphicsContext GraphicsContext => Scene.GameContext.GraphicsContext;

        /// <summary>
        /// Gets or sets the number of draw calls performed during the most recent render pass.
        /// </summary>
        public int DrawCalls { get; set; }

        /// <summary>
        /// The single texture from which all tiles are sampled.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// The size of a single tile in pixels, used when pushing tiles by grid index.
        /// For example, <c>(16, 16)</c> means each tile is 16px wide and 16px tall in the texture.
        /// This is set automatically when loading a TileMap Studio JSON file.
        /// </summary>
        public Vector2 GridSize { get; set; } = new Vector2(32f, 32f);

        /// <summary>
        /// The color tint applied to every tile by default. Default is white (no tint).
        /// Individual tiles pushed with a custom color override this.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Global scale applied to every tile's destination size. Default is (1, 1).
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// Global offset applied to every tile's world position. Useful for scrolling the
        /// entire tilemap (e.g. for parallax or camera offset). Default is (0, 0).
        /// </summary>
        public Vector2 Offset { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets the number of tiles currently stored in this layer.
        /// </summary>
        public int TileCount => tiles.Count;

        public TileLayer() : this(DefaultMaxBatchCount) { }

        public TileLayer(int maxBatchCount)
        {
            this.maxBatchCount = maxBatchCount;
        }

        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);

            vertices = new VertexPositionTexColor[maxBatchCount * 4];
            indices = BuildQuadIndices(maxBatchCount);

            geometry = GraphicsContext.CreateGeometry<VertexPositionTexColor>(
                vertices, indices, GeometryType.Dynamic, GeometryType.Static);

            defaultShader = new BasicShader(GraphicsContext, VertexElementContainer.VertexPositionTexColor);
            ActiveShader = defaultShader;
        }

        private static uint[] BuildQuadIndices(int quadCount)
        {
            var result = new uint[quadCount * 6];
            int writeIdx = 0;

            for (uint vertIdx = 0; vertIdx < quadCount * 4; vertIdx += 4)
            {
                result[writeIdx++] = vertIdx;
                result[writeIdx++] = vertIdx + 1;
                result[writeIdx++] = vertIdx + 2;
                result[writeIdx++] = vertIdx + 2;
                result[writeIdx++] = vertIdx + 3;
                result[writeIdx++] = vertIdx;
            }

            return result;
        }

        #region State Management

        public void SetShader(BasicShader shader)
        {
            ActiveShader = shader;
        }

        public void ResetShader()
        {
            ActiveShader = defaultShader;
        }

        public void SetBlendMode(string name)
        {
            activeBlendName = name;
        }

        #endregion

        #region Push (Grid-Indexed)

        /// <summary>
        /// Pushes a tile from the texture at tile index <paramref name="tileCol"/>, <paramref name="tileRow"/>
        /// to the world position <paramref name="worldPosition"/>. The tile's source rectangle is computed
        /// as <c>(tileCol * GridSize.X, tileRow * GridSize.Y, (tileCol+1) * GridSize.X, (tileRow+1) * GridSize.Y)</c>.
        /// For example, with a 32×32 grid, tile (2, 1) samples from (64, 32, 96, 64).
        /// </summary>
        /// <param name="tileCol">The column index of the tile in the texture (0-based).</param>
        /// <param name="tileRow">The row index of the tile in the texture (0-based).</param>
        /// <param name="worldPosition">The world-space position (top-left corner) where the tile will be drawn.</param>
        public void Push(int tileCol, int tileRow, Vector2 worldPosition)
        {
            float left = tileCol * GridSize.X;
            float top = tileRow * GridSize.Y;
            float right = left + GridSize.X;
            float bottom = top + GridSize.Y;

            PushExact(left, top, right, bottom, worldPosition, GridSize, Color);
        }

        /// <summary>
        /// Pushes a tile from the texture at tile index <paramref name="tileCol"/>, <paramref name="tileRow"/>
        /// to the grid cell <paramref name="cellX"/>, <paramref name="cellY"/> in world space.
        /// The destination world position is computed as <c>(cellX * GridSize.X, cellY * GridSize.Y)</c>,
        /// so the tile snaps to the layer's grid. The tile's source rectangle is also computed from
        /// <paramref name="tileCol"/> and <paramref name="tileRow"/> using <see cref="GridSize"/>.
        /// </summary>
        public void Push(int tileCol, int tileRow, int cellX, int cellY)
        {
            Vector2 worldPosition = new Vector2(cellX * GridSize.X, cellY * GridSize.Y);
            Push(tileCol, tileRow, worldPosition);
        }

        /// <summary>
        /// Pushes a tile from the texture at tile index <paramref name="tileCol"/>, <paramref name="tileRow"/>
        /// to the world position <paramref name="worldPosition"/>, with a custom destination size and color.
        /// </summary>
        public void Push(int tileCol, int tileRow, Vector2 worldPosition, Vector2 destinationSize, Color color)
        {
            float left = tileCol * GridSize.X;
            float top = tileRow * GridSize.Y;
            float right = left + GridSize.X;
            float bottom = top + GridSize.Y;

            PushExact(left, top, right, bottom, worldPosition, destinationSize, color);
        }

        #endregion

        #region Push (Exact Texture Rectangle)

        /// <summary>
        /// Pushes a tile using an exact texture source rectangle specified by
        /// <paramref name="texLeft"/>, <paramref name="texTop"/>, <paramref name="texRight"/>,
        /// <paramref name="texBottom"/> (in pixels). The tile is drawn at <paramref name="worldPosition"/>
        /// with the size <paramref name="destinationSize"/> (in world units).
        /// </summary>
        public void PushExact(float texLeft, float texTop, float texRight, float texBottom,
                              Vector2 worldPosition, Vector2 destinationSize)
        {
            PushExact(texLeft, texTop, texRight, texBottom, worldPosition, destinationSize, Color);
        }

        /// <summary>
        /// Pushes a tile using an exact texture source rectangle specified by
        /// <paramref name="texLeft"/>, <paramref name="texTop"/>, <paramref name="texRight"/>,
        /// <paramref name="texBottom"/> (in pixels), with a custom color tint.
        /// </summary>
        public void PushExact(float texLeft, float texTop, float texRight, float texBottom,
                              Vector2 worldPosition, Vector2 destinationSize, Color color)
        {
            tiles.Add(new TileEntry
            {
                TexLeft = texLeft,
                TexTop = texTop,
                TexRight = texRight,
                TexBottom = texBottom,
                WorldX = worldPosition.X,
                WorldY = worldPosition.Y,
                DestW = destinationSize.X,
                DestH = destinationSize.Y,
                Color = color
            });
        }

        /// <summary>
        /// Pushes a tile using an exact texture source rectangle, with the destination size
        /// automatically set to <c>(texRight - texLeft, texBottom - texTop)</c> (i.e. 1:1 pixel mapping).
        /// </summary>
        public void PushExact(float texLeft, float texTop, float texRight, float texBottom,
                              Vector2 worldPosition)
        {
            Vector2 destinationSize = new Vector2(texRight - texLeft, texBottom - texTop);
            PushExact(texLeft, texTop, texRight, texBottom, worldPosition, destinationSize, Color);
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Pushes a rectangular block of tiles from the texture to a rectangular block of cells in world space.
        /// Tile (tileColStart + i, tileRowStart + j) is drawn at world cell (cellXStart + i, cellYStart + j).
        /// </summary>
        public void PushBlock(int tileColStart, int tileRowStart,
                              int cellXStart, int cellYStart,
                              int cellsWide, int cellsHigh)
        {
            for (int j = 0; j < cellsHigh; j++)
                for (int i = 0; i < cellsWide; i++)
                    Push(tileColStart + i, tileRowStart + j, cellXStart + i, cellYStart + j);
        }

        /// <summary>
        /// Pushes the same tile (by index) to a rectangular block of cells in world space.
        /// </summary>
        public void PushFill(int tileCol, int tileRow,
                             int cellXStart, int cellYStart,
                             int cellsWide, int cellsHigh)
        {
            for (int j = 0; j < cellsHigh; j++)
                for (int i = 0; i < cellsWide; i++)
                    Push(tileCol, tileRow, cellXStart + i, cellYStart + j);
        }

        /// <summary>
        /// Removes all tiles from this layer.
        /// </summary>
        public void Clear()
        {
            tiles.Clear();
        }

        #endregion

        #region TileMap Studio Import

        /// <summary>
        /// Loads a single layer from a TileMap Studio JSON file.
        /// The <paramref name="tileSize"/> from the JSON is used to set <see cref="GridSize"/>.
        /// The <paramref name="tilesets"/> section is completely ignored — the user provides
        /// their own <paramref name="texture"/>. Only the layer whose <paramref name="layerName"/>
        /// matches is loaded; all other layers are skipped.
        /// </summary>
        /// <param name="jsonPath">Path to the .json file exported by TileMap Studio.</param>
        /// <param name="layerName">The name of the layer to load (matches the layer's "name" field in the JSON).</param>
        /// <param name="texture">The texture to use for all tiles. The tilesets' embedded data is ignored.</param>
        /// <returns>The number of tiles loaded. Returns 0 if the layer was not found or was empty.</returns>
        public int LoadFromTileMapStudio(string jsonPath, string layerName, Texture texture)
        {
            string json = System.IO.File.ReadAllText(jsonPath);
            return LoadFromTileMapStudioJson(json, layerName, texture);
        }

        /// <summary>
        /// Loads a single layer from a TileMap Studio JSON string.
        /// See <see cref="LoadFromTileMapStudio"/> for details.
        /// </summary>
        /// <param name="jsonText">The JSON text exported by TileMap Studio.</param>
        /// <param name="layerName">The name of the layer to load.</param>
        /// <param name="texture">The texture to use for all tiles.</param>
        /// <returns>The number of tiles loaded.</returns>
        public int LoadFromTileMapStudioJson(string jsonText, string layerName, Texture texture)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonText))
            {
                JsonElement root = doc.RootElement;

                // Read tileSize from the JSON and use it as GridSize.
                // TileMap Studio exports this either at the top level (older v3.x format)
                // or nested under "metadata" (newer v1.2 format). We check both.
                if (root.TryGetProperty("tileSize", out JsonElement tileSizeEl) && tileSizeEl.ValueKind == JsonValueKind.Number)
                {
                    int tileSize = tileSizeEl.GetInt32();
                    GridSize = new Vector2(tileSize, tileSize);
                }
                else if (root.TryGetProperty("metadata", out JsonElement metaEl)
                         && metaEl.TryGetProperty("tileSize", out JsonElement metaTileSizeEl)
                         && metaTileSizeEl.ValueKind == JsonValueKind.Number)
                {
                    int tileSize = metaTileSizeEl.GetInt32();
                    GridSize = new Vector2(tileSize, tileSize);
                }

                // Find the layer with the matching name.
                if (!root.TryGetProperty("layers", out JsonElement layersEl))
                    return 0;

                foreach (JsonElement layerEl in layersEl.EnumerateArray())
                {
                    if (!layerEl.TryGetProperty("name", out JsonElement nameEl))
                        continue;

                    if (nameEl.GetString() != layerName)
                        continue;

                    // Skip invisible layers.
                    if (layerEl.TryGetProperty("visible", out JsonElement visibleEl) && !visibleEl.GetBoolean())
                        return 0;

                    // Read opacity (0-100) and convert to a color multiplier.
                    byte alpha = 255;
                    if (layerEl.TryGetProperty("opacity", out JsonElement opacityEl))
                    {
                        int opacity = opacityEl.GetInt32();
                        opacity = System.Math.Clamp(opacity, 0, 100);
                        alpha = (byte)(opacity * 255 / 100);
                    }
                    Color layerColor = new Color(Color.R, Color.G, Color.B, alpha);

                    Texture = texture;

                    // Iterate the 2D data array: data[row][col].
                    if (!layerEl.TryGetProperty("data", out JsonElement dataEl))
                        return 0;

                    // Precompute tileset columns for Number-format decoding.
                    // We derive this from the texture dimensions and GridSize rather than
                    // reading the tilesets section (which we are instructed to ignore).
                    int tilesetCols = 1;
                    if (GridSize.X > 0)
                        tilesetCols = System.Math.Max(1, texture.Width / (int)GridSize.X);

                    int loaded = 0;
                    int rowIndex = 0;
                    foreach (JsonElement rowEl in dataEl.EnumerateArray())
                    {
                        int colIndex = 0;
                        foreach (JsonElement cellEl in rowEl.EnumerateArray())
                        {
                            int? tileXY = ReadCellTile(cellEl, tilesetCols);
                            if (tileXY.HasValue)
                            {
                                int tileX = tileXY.Value & 0xFFFF;
                                int tileY = (tileXY.Value >> 16) & 0xFFFF;

                                // Compute the world position from the cell's grid position.
                                Vector2 worldPos = new Vector2(
                                    colIndex * GridSize.X,
                                    rowIndex * GridSize.Y);

                                // Push the tile using grid indices. The Push method computes
                                // the source rectangle from tileX/tileY and GridSize.
                                Push(tileX, tileY, worldPos);

                                // Override the color with the layer opacity.
                                // Since Push already added the tile with the default color,
                                // we update the last entry's color.
                                if (alpha != 255)
                                {
                                    var entry = tiles[tiles.Count - 1];
                                    entry.Color = layerColor;
                                    tiles[tiles.Count - 1] = entry;
                                }

                                loaded++;
                            }
                            colIndex++;
                        }
                        rowIndex++;
                    }

                    return loaded;
                }

                // Layer not found.
                return 0;
            }
        }

        /// <summary>
        /// Reads a tile's (tileX, tileY) from a cell element, supporting all three
        /// TileMap Studio cell formats:
        /// <list type="bullet">
        /// <item><c>null</c> → empty cell (returns null).</item>
        /// <item>Object <c>{"tileX": 1, "tileY": 10, "tilesetId": ...}</c> → reads tileX/tileY directly.</item>
        /// <item>Number <c>151</c> → linear tile ID. 0 = empty; 1+ decoded as
        ///   <c>tileX = (id - 1) % cols</c>, <c>tileY = (id - 1) / cols</c>.</item>
        /// <item>Array <c>[tileX, tileY, tilesetId?]</c> → reads first two elements.</item>
        /// </list>
        /// Returns null for empty cells. Otherwise packs tileX (low 16 bits) and tileY (high 16 bits)
        /// into a single int to avoid allocating a tuple.
        /// </summary>
        private static int? ReadCellTile(JsonElement cellEl, int tilesetCols)
        {
            if (cellEl.ValueKind == JsonValueKind.Null)
                return null;

            if (cellEl.ValueKind == JsonValueKind.Object)
            {
                int tileX = 0, tileY = 0;
                bool found = false;

                if (cellEl.TryGetProperty("tileX", out JsonElement txEl))
                {
                    tileX = txEl.GetInt32();
                    found = true;
                }
                if (cellEl.TryGetProperty("tileY", out JsonElement tyEl))
                {
                    tileY = tyEl.GetInt32();
                    found = true;
                }

                if (!found)
                    return null;

                return (tileY << 16) | (tileX & 0xFFFF);
            }

            if (cellEl.ValueKind == JsonValueKind.Number)
            {
                // Linear tile ID format (Tiled-style: 0 = empty, 1+ = tile).
                long id = cellEl.GetInt64();
                if (id <= 0)
                    return null;

                int cols = System.Math.Max(1, tilesetCols);
                int tileX = (int)((id - 1) % cols);
                int tileY = (int)((id - 1) / cols);

                return (tileY << 16) | (tileX & 0xFFFF);
            }

            if (cellEl.ValueKind == JsonValueKind.Array)
            {
                // Compact array format: [tileX, tileY, tilesetId?]
                var arr = cellEl.EnumerateArray().ToList();
                if (arr.Count < 2)
                    return null;

                int tileX = arr[0].GetInt32();
                int tileY = arr[1].GetInt32();

                return (tileY << 16) | (tileX & 0xFFFF);
            }

            // Unknown format — treat as empty.
            return null;
        }

        #endregion

        #region Rendering

        public override void Render()
        {
            if (!Enabled || !Viewports.Contains(GraphicsContext.ActiveViewport.Name))
                return;

            if (tiles.Count == 0)
                return;

            // Build the quad list from stored tiles.
            quadList.Clear();
            BuildQuads();

            if (quadList.Count == 0)
                return;

            DrawCalls = 0;

            SpriteQuad temp = null;
            int quadCount = 0;
            int vertexIndex = 0;

            foreach (SpriteQuad spr in quadList)
            {
                if (temp == null)
                {
                    temp = spr;
                    ActivateBatchState(spr);
                }
                else if (temp != spr)
                {
                    if (quadCount > 0)
                        FlushBatch(ref quadCount, ref vertexIndex);

                    temp = spr;
                    ActivateBatchState(spr);
                }

                AppendQuadVertices(spr, ref vertexIndex);
                quadCount++;

                if (quadCount >= maxBatchCount)
                    FlushBatch(ref quadCount, ref vertexIndex);
            }

            if (quadCount > 0)
                FlushBatch(ref quadCount, ref vertexIndex);

            quadList.Clear();
            GraphicsContext.ActivateBlendMode("Default");
        }

        private void BuildQuads()
        {
            if (Texture == null)
                return;

            float texW = Texture.Width;
            float texH = Texture.Height;
            float scaleX = Scale.X;
            float scaleY = Scale.Y;
            float offsetX = Offset.X;
            float offsetY = Offset.Y;

            foreach (TileEntry t in tiles)
            {
                float x = Round(t.WorldX + offsetX);
                float y = Round(t.WorldY + offsetY);
                float w = Round(t.DestW * scaleX);
                float h = Round(t.DestH * scaleY);

                float u = t.TexLeft / texW;
                float v = t.TexTop / texH;
                float uvW = (t.TexRight - t.TexLeft) / texW;
                float uvH = (t.TexBottom - t.TexTop) / texH;

                var corners = new[]
                {
                    new Vector2(x, y),
                    new Vector2(x + w, y),
                    new Vector2(x + w, y + h),
                    new Vector2(x, y + h)
                };

                var uvs = new[]
                {
                    new Vector2(u, v),
                    new Vector2(u + uvW, v),
                    new Vector2(u + uvW, v + uvH),
                    new Vector2(u, v + uvH)
                };

                quadList.Add(new SpriteQuad(
                    Texture,
                    new Vector4(corners[0], uvs[0]),
                    new Vector4(corners[1], uvs[1]),
                    new Vector4(corners[2], uvs[2]),
                    new Vector4(corners[3], uvs[3]),
                    t.Color, ActiveShader, activeBlendName));
            }
        }

        private void ActivateBatchState(SpriteQuad spr)
        {
            spr.Shader.Activate();
            spr.Shader.Projection = CreateProjectionMatrix();
            spr.Shader.SetTexture(spr.Texture);
            GraphicsContext.ActivateBlendMode(spr.BlendName);
        }

        private Matrix4 CreateProjectionMatrix()
        {
            var vp = GraphicsContext.ActiveViewport;
            return Matrix4.CreateOrthographic(
                vp.ScenePos.X,
                vp.SceneSize.X + vp.ScenePos.X,
                vp.SceneSize.Y + vp.ScenePos.Y,
                vp.ScenePos.Y,
                0, 1);
        }

        private void FlushBatch(ref int quadCount, ref int vertexIndex)
        {
            geometry.UpdateVB(vertices);
            geometry.Render();
            DrawCalls++;

            Array.Clear(vertices, 0, vertices.Length);
            quadCount = 0;
            vertexIndex = 0;
        }

        private void AppendQuadVertices(SpriteQuad spr, ref int vertexIndex)
        {
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV1, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV2, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV3, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV4, spr.Color);
        }

        private static float Round(float value) => (float)System.Math.Round(value);

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tile
{
    public static Texture2D[] originalTiles;
    public static Texture2D[] expandedTiles;

    public static Texture2D PNGAtlas;
    public static string PNGFilepath = "/u4/shapes.png";
    public static string tileApple2Filepath1 = "/u4/SHP0.B";
    public static string tileApple2Filepath2 = "/u4/SHP1.B";
    public static string tileEGAFilepath = "/u4/SHAPES.EGA";
    public static string tileCGAFilepath = "/u4/SHAPES.CGA";

    public static Texture2D combinedExpandedTexture;
    public static Material combinedExpandedMaterial;
    public static int textureExpandedAtlasPowerOf2;

    public static Texture2D combinedLinearTexture;
    public static Material combinedLinearMaterial;

    // NOTE certain shaders used for things like sprites and unlit textures do not
    // do well with edges and leave ghosts of the nearby textures from the texture atlas
    // to solve this issue I need to create at least a one pixel mirror border around the
    // tile, this function creates a larger tile texture and adds this border around the tile placed in the center.
    // Special care must be made when combining meshes with textures like this and the Combine()
    // function has been updated to handle this situation and update the uv's. Given that some
    // platforms may require textures be certain integer multiples of 2 this function will allow
    // a larger than one pixel border around the tile.
    public const int TILE_BORDER_SIZE = 1;
    public static int expandedTileWidth;
    public static int expandedTileHeight;
    public static int originalTileWidth;
    public static int originalTileHeight;

    public static void ExpandTiles()
    {
        // allocate some textures pointers
        expandedTiles = new Texture2D[256];
        expandedTileWidth = originalTiles[0].width + 2 * TILE_BORDER_SIZE;
        expandedTileHeight = originalTiles[0].height + 2 * TILE_BORDER_SIZE;
        originalTileWidth = originalTiles[0].width;
        originalTileHeight = originalTiles[0].height;

        // go through all the original tiles
        for (int i = 0; i < 256; i++)
        {
            // create a new slightly larger texture with boarder for this tile
            Texture2D currentTile = originalTiles[i];
            Texture2D newTile = new Texture2D(expandedTileWidth, expandedTileHeight, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            newTile.filterMode = FilterMode.Point;

            // go through all the pixels in the source texture
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; width++)
                {
                    // copy the pixles into the middle
                    newTile.SetPixel(width + TILE_BORDER_SIZE, height + TILE_BORDER_SIZE, currentTile.GetPixel(width, height));
                }
            }

            // mirror the pixles on either side
            for (int height = 0; height < currentTile.height; height++)
            {
                // left side
                newTile.SetPixel(TILE_BORDER_SIZE - 1, height + TILE_BORDER_SIZE, currentTile.GetPixel(0, height));
                // right side
                newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, height + TILE_BORDER_SIZE, currentTile.GetPixel(currentTile.width - 1, height));
            }

            // mirror the pixles on top and bottom
            for (int width = 0; width < currentTile.width; width++)
            {
                // left side
                newTile.SetPixel(width + TILE_BORDER_SIZE, TILE_BORDER_SIZE - 1, currentTile.GetPixel(width, 0));
                // right side
                newTile.SetPixel(width + TILE_BORDER_SIZE, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(width, currentTile.height - 1));
            }

            // now the four corners
            newTile.SetPixel(TILE_BORDER_SIZE - 1, TILE_BORDER_SIZE - 1, currentTile.GetPixel(0, 0));
            newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, TILE_BORDER_SIZE - 1, currentTile.GetPixel(currentTile.width - 1, 0));
            newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(currentTile.width - 1, currentTile.height - 1));
            newTile.SetPixel(TILE_BORDER_SIZE - 1, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(0, currentTile.height - 1));

            // apply all the previous SetPixel() calls to the texture
            newTile.Apply();

            // save the new expanded texture
            expandedTiles[i] = newTile;
        }
    }

    public static void AnimateFlags()
    {
        //animate flags
        // TODO there is probably a better way to do this than mucking around
        // with the 2d texture pixels like the original game did but it will work for now.
        const int flagSpeed = 25;

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 4;
            const int swap_y2 = 5;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.TOWN;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 10;
            const int swap_x2 = 12;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.CASTLE;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;
            Texture2D tex = originalTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;
            Texture2D tex = originalTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6 + TILE_BORDER_SIZE;
            const int swap_x2 = 8 + TILE_BORDER_SIZE;
            const int swap_y1 = 2 + TILE_BORDER_SIZE;
            const int swap_y2 = 3 + TILE_BORDER_SIZE;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;
            Texture2D tex = expandedTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7 + TILE_BORDER_SIZE;
            const int swap_x2 = 9 + TILE_BORDER_SIZE;
            const int swap_y1 = 2 + TILE_BORDER_SIZE;
            const int swap_y2 = 3 + TILE_BORDER_SIZE;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;
            Texture2D tex = expandedTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }

        //if (Random.Range(0, 100) <= flagSpeed)
        {
            const int x1 = 3;
            const int x2 = 9;
            const int y1 = 9;
            const int y2 = 16;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.COOKING_FIRE;

            int offset_x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth + TILE_BORDER_SIZE;
            int offset_y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight + TILE_BORDER_SIZE;


            Color alpha = new Color(0, 0, 0, 0);
            //Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK];

            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    Color color = combinedExpandedTexture.GetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y);
                    if ((color == Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]) || (color == alpha))
                    {
                        if (Random.Range(0, 100) <= 50)
                        {
                            combinedExpandedTexture.SetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]);
                        }
                        else
                        {
                            combinedExpandedTexture.SetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y, alpha);
                        }
                    }
                }
            }
            combinedExpandedTexture.Apply();
        }
    }

    // TODO fix horse tile also
    public static void FixMageTile3()
    {
        // adjust the pixels on mage tile #3
        Texture2D currentTile = originalTiles[(int)U4_Decompiled_AVATAR.TILE.MAGE_NPC3];

        // go through all the pixels in the source texture and shift them one pixel
        for (int height = 0; height < currentTile.height; height++)
        {
            for (int width = currentTile.width - 1; width > 0; width--)
            {
                currentTile.SetPixel(width, height, currentTile.GetPixel((width - 1 + currentTile.width) % currentTile.width, height));
            }
        }

        // apply all the previous SetPixel() calls to the texture
        currentTile.Apply();
    }
    public static void CreateLinearTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        // extend as needed
        else if (tilesTextures.Length > 256)
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full size of texture atlas
        sizeW = tilesTextures.Length * originalSizeW;

        // creare a new combined texture big enough to hold all the textures
        combinedLinearTexture = new Texture2D(sizeW, originalSizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedLinearTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        int index = 0;
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            texture = (Texture2D)tilesTextures[i];
            if (texture)
            {
                int x = index * originalSizeW;
                int y = 0;

                combinedLinearTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());
                index++;
            }
        }
        // apply all the pixles we copied in the loop above
        combinedLinearTexture.Apply();

        // create a material based on this texture atlas
        combinedLinearMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedLinearMaterial.mainTexture = combinedLinearTexture;
    }

    public static Texture2D combinedTexture;
    public static Material combinedMaterial;
    public static Hashtable textureAtlasHashTable = new Hashtable();
    public static int textureAtlasPowerOf2;
    public static void CreateSquareTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        else if (tilesTextures.Length == 1)
        {
            textureAtlasPowerOf2 = 1;
        }
        else if (tilesTextures.Length <= 4)
        {
            textureAtlasPowerOf2 = 2;
        }
        else if (tilesTextures.Length <= 16)
        {
            textureAtlasPowerOf2 = 4;
        }
        else if (tilesTextures.Length <= 64)
        {
            textureAtlasPowerOf2 = 8;
        }
        else if (tilesTextures.Length <= 256)
        {
            textureAtlasPowerOf2 = 16;
        }
        // extend as needed
        else
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full size of texture atlas
        sizeW = textureAtlasPowerOf2 * originalSizeW;
        sizeH = textureAtlasPowerOf2 * originalSizeH;

        // creare a new combined texture big enough to hold all the textures
        combinedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        int index = 0;
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            texture = (Texture2D)tilesTextures[i];
            if (texture && !textureAtlasHashTable.ContainsKey(texture))
            {
                int x = (index % textureAtlasPowerOf2) * originalSizeW;
                int y = (index / textureAtlasPowerOf2) * originalSizeH;

                combinedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());

                x = index % textureAtlasPowerOf2;
                y = index / textureAtlasPowerOf2;
                textureAtlasHashTable.Add(texture, new Vector2(x, y));
                index++;
            }
        }
        // apply all the pixles we copied in the loop above
        combinedTexture.Apply();

        // create a material based on this texture atlas
        combinedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedMaterial.mainTexture = combinedTexture;
    }
    public static void CreateExpandedTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        else if (tilesTextures.Length == 1)
        {
            textureExpandedAtlasPowerOf2 = 1;
        }
        else if (tilesTextures.Length <= 4)
        {
            textureExpandedAtlasPowerOf2 = 2;
        }
        else if (tilesTextures.Length <= 16)
        {
            textureExpandedAtlasPowerOf2 = 4;
        }
        else if (tilesTextures.Length <= 64)
        {
            textureExpandedAtlasPowerOf2 = 8;
        }
        else if (tilesTextures.Length <= 256)
        {
            textureExpandedAtlasPowerOf2 = 16;
        }
        // extend as needed
        else
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full square size of texture atlas
        sizeW = textureExpandedAtlasPowerOf2 * originalSizeW;
        sizeH = textureExpandedAtlasPowerOf2 * originalSizeH;

        // creare a new combined texture big enough to hold all the textures
        combinedExpandedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedExpandedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            Texture2D texture = tilesTextures[i];
            if (texture)
            {
                int x = (i % textureExpandedAtlasPowerOf2) * originalSizeW;
                int y = (i / textureExpandedAtlasPowerOf2) * originalSizeH;

                combinedExpandedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());
            }
        }
        // apply all the pixles we copied in the loop above
        combinedExpandedTexture.Apply();

        // create a material based on this texture atlas
        combinedExpandedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedExpandedMaterial.mainTexture = combinedExpandedTexture;
    }

    public static void LoadTilesPNG()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + PNGFilepath))
        {
            Debug.Log("Could not find PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + PNGFilepath);

        // allocate something to start with so the loadImage can resize it to file the actual file
        PNGAtlas = new Texture2D(32, 32 * 256, TextureFormat.RGBA32, false);

        if (!PNGAtlas.LoadImage(fileData))
        {
            Debug.Log("Could not load PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        originalTileWidth = PNGAtlas.width;
        originalTileHeight = PNGAtlas.height / 256;
        originalTiles = new Texture2D[256];

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(originalTileWidth, originalTileHeight, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            int y = (255 - tile) * originalTileHeight;

            currentTile.SetPixels(0, 0, originalTileWidth, originalTileHeight, PNGAtlas.GetPixels(0, y, originalTileWidth, originalTileHeight));
            currentTile.Apply();
        }
    }

    public static void LoadTilesApple2()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + tileApple2Filepath1))
        {
            Debug.Log("Could not find Apple2 tiles file " + Application.persistentDataPath + tileApple2Filepath1);
            return;
        }

        // read the file
        byte[] fileData1 = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileApple2Filepath1);

        if (fileData1.Length != 4 * 1024)
        {
            Debug.Log("Apple2 Tiles file incorrect length " + fileData1.Length);
            return;
        }

        if (!System.IO.File.Exists(Application.persistentDataPath + tileApple2Filepath2))
        {
            Debug.Log("Could not find Apple2 tiles file " + Application.persistentDataPath + tileApple2Filepath2);
            return;
        }

        // read the file
        byte[] fileData2 = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileApple2Filepath2);

        if (fileData2.Length != 4 * 1024)
        {
            Debug.Log("Apple2 Tiles file incorrect length " + fileData2.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(14, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the Apple2 color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width - 2; /* width incremented below */ )
                {
                    bool highBitSet;
                    bool previousPixel;
                    bool pixel;
                    bool nextPixel;
                    Color color;

                    int y = currentTile.height - height - 1;

                    // go to the next byte in the file
                    int index = height * 256 + tile;

                    // get the first file pixel block
                    int pixelBlock = fileData1[index];

                    // go through the first half of the tile from the first file
                    highBitSet = (pixelBlock & 0x80) != 0;

                    // assume no tiling on the side of the tile
                    previousPixel = false;
                    pixel = (pixelBlock & 0x01) != 0;
                    nextPixel = (pixelBlock & 0x02) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x04) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x08) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x10) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x20) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x40) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    // next pixel is in the other file
                    pixelBlock = fileData2[index];
                    nextPixel = (pixelBlock & 0x01) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);

                    // do the second half of the tile from the other file
                    highBitSet = (pixelBlock & 0x80) != 0;

                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x02) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x04) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x08) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x10) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x20) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x40) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    // assume no tiling on the side of the tile
                    nextPixel = false;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();
        }
    }

    public static void LoadTilesEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        if (!System.IO.File.Exists(Application.persistentDataPath + tileEGAFilepath))
        {
            Debug.Log("Could not find EGA tiles file " + Application.persistentDataPath + tileEGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileEGAFilepath);

        if (fileData.Length != 32 * 1024)
        {
            Debug.Log("EGA tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[index] >> 4;
                    Color color = Palette.EGAColorPalette[colorIndex];

                    // check if these are people/creatures/ladders/anhk and use black as alpha channel 61
                    if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.ANKH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_UP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_DOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.FOREST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.COOKING_FIRE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.SHRINE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.ALTAR ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_LEFT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_TOP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRUSH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE2 ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_SMALL && tile <= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PARTY && tile <= (int)U4_Decompiled_AVATAR.TILE.SHEPHERD2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.GUARD && tile <= (int)U4_Decompiled_AVATAR.TILE.LORD_BRITISH2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.SERPENT && tile <= (int)U4_Decompiled_AVATAR.TILE.WATER_SPOUT2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.BAT && tile <= (int)U4_Decompiled_AVATAR.TILE.TROLL4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.INSECTS && tile <= (int)U4_Decompiled_AVATAR.TILE.INSECTS4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PHANTOM && tile <= (int)U4_Decompiled_AVATAR.TILE.MAGE_NPC4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.LAVA_LIZARD && tile <= (int)U4_Decompiled_AVATAR.TILE.ZORN4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.HYDRA && tile <= (int)U4_Decompiled_AVATAR.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BROWN) && tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_EAST))
                    {
                        if ((width == 13) && (height == 4))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_WEST))
                    {
                        if ((width == 3) && (height == 5))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the grass speckels don't show when we use the tile as a billboard
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.TOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these squid tiles and make black and blue into alpha
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.SQUID || tile == (int)U4_Decompiled_AVATAR.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // ships
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        ((tile >= (int)U4_Decompiled_AVATAR.TILE.SHIP_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.SHIP_SOUTH) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PIRATE_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make energy fields are transparent
                    else if (tile >= (int)U4_Decompiled_AVATAR.TILE.POISON_FIELD && tile <= (int)U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                    {
                        if (colorIndex == (int)Palette.EGA_COLOR.BLACK)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else just copy all the pixels with no modifications for now
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[index] & 0xf;
                    color = Palette.EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    // check if these are people/creatures/ladders/anhk and use black as alpha channel
                    if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.ANKH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_UP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_DOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.FOREST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.COOKING_FIRE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.SHRINE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.ALTAR ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_LEFT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_TOP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRUSH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE2 ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_SMALL && tile <= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PARTY && tile <= (int)U4_Decompiled_AVATAR.TILE.SHEPHERD2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.GUARD && tile <= (int)U4_Decompiled_AVATAR.TILE.LORD_BRITISH2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.SERPENT && tile <= (int)U4_Decompiled_AVATAR.TILE.WATER_SPOUT2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.BAT && tile <= (int)U4_Decompiled_AVATAR.TILE.TROLL4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.INSECTS && tile <= (int)U4_Decompiled_AVATAR.TILE.INSECTS4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PHANTOM && tile <= (int)U4_Decompiled_AVATAR.TILE.MAGE_NPC4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.LAVA_LIZARD && tile <= (int)U4_Decompiled_AVATAR.TILE.ZORN4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.HYDRA && tile <= (int)U4_Decompiled_AVATAR.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BROWN) && tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the green grass speckels don't show when we use the tile standing upright
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.TOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent TODO make black inside portal transparent also
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these tiles and make black and blue into alpha
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.SQUID || tile == (int)U4_Decompiled_AVATAR.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_EAST))
                    {
                        if ((width == 13) && (height == 4))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_WEST))
                    {
                        if ((width == 3) && (height == 5))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // ships
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        ((tile >= (int)U4_Decompiled_AVATAR.TILE.SHIP_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.SHIP_SOUTH) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PIRATE_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= (int)U4_Decompiled_AVATAR.TILE.POISON_FIELD && tile <= (int)U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                    {
                        if (colorIndex == (int)Palette.EGA_COLOR.BLACK)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else has no alpha channel defined yet
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // go to the next byte in the file
                    index++;
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();
        }
    }

    public static void LoadTilesCGA()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + tileCGAFilepath))
        {
            Debug.Log("Could not find CGA tiles file " + Application.persistentDataPath + tileCGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileCGAFilepath);

        if (fileData.Length != 16 * 1024)
        {
            Debug.Log("CGA Tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height += 2)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    int colorIndex;

                    colorIndex = (fileData[index + 0x20] & 0xC0) >> 6;
                    Color color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0xC0) >> 6;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x30) >> 4;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x30) >> 4;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x0C) >> 2;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x0C) >> 2;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x03) >> 0;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x03) >> 0;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    // go to the next byte in the file
                    index++;
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();

            // skip ahead
            index += 0x20;
        }
    }
}

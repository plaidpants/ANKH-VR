using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Font myFont;
    public Font myTransparentFont;

    // mainTerrain holds the terrain, animatedTerrrain, billboardTerrrain
    public GameObject mainTerrain;
    public GameObject terrain;
    public GameObject animatedTerrrain;
    public GameObject billboardTerrrain;

    public GameObject InputPanel;
    public GameObject MainMainLoop;
    public GameObject Keyboard;
    public GameObject KeyboardUpper;
    public GameObject KeyboardLower;
    public GameObject GameText;
    public GameObject Picture;
    public GameObject TalkChoice;
    public GameObject TalkContinue;
    public GameObject TalkMF;

    // reference to game engine
    public U4_Decompiled_TITLE u4_TITLE;

    GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];

    public struct tHeader
    {//size 0x11
        /*+00*/
        public ushort magic;//0x1234
        /*+02*/
        public ushort xsize, ysize;//320,200
        /*+06*/
        public byte __06;
        public byte __07;
        public byte __08;
        public byte __09;
        /*+0a*/
        public byte bitsinf;//2
        /*+0b*/
        public byte emark;//0xff
        /*+0c*/
        public char evideo;//'A'
        /*+0d*/
        public byte edesc;//[1 == edata is pallet]
        /*+0e*/
        public byte __0e;
        /*+0f*/
        public ushort esize;
    };

    public struct tBlock
    {
        /*+00*/
        public ushort PBSIZE;
        /*+02*/
        public ushort size;
        /*+05*/
        public byte MBYTE;
    };

    void ClearTexture(Texture2D texture, Color color)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    void LoadAVATARPicFile(string file, Texture2D texture)
    {
        int fileIndex = 0;

        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        // check if the file is at least the size of the header plus the length
        if (picFileData.Length < 0x11 + 0x02)
        {
            Debug.Log("Picture file incorrect length " + picFileData.Length);
            return;
        }

        tHeader header = new tHeader();

        header.magic = System.BitConverter.ToUInt16(picFileData, fileIndex); //0x1234
        fileIndex += 2;
        header.xsize = System.BitConverter.ToUInt16(picFileData, fileIndex); //320
        fileIndex += 2;
        header.ysize = System.BitConverter.ToUInt16(picFileData, fileIndex); //200
        fileIndex += 2;
        header.__06 = picFileData[fileIndex++];
        header.__07 = picFileData[fileIndex++];
        header.__08 = picFileData[fileIndex++];
        header.__09 = picFileData[fileIndex++];
        header.bitsinf = picFileData[fileIndex++];  //2
        header.emark = picFileData[fileIndex++]; //0xff 
        header.evideo = (char)picFileData[fileIndex++]; //'A'
        header.edesc = picFileData[fileIndex++]; //[1 == edata is pallet]
        header.__0e = picFileData[fileIndex++];
        header.esize = System.BitConverter.ToUInt16(picFileData, fileIndex);
        fileIndex += 2;

        // check header
        if (header.magic != 0x1234 ||
            //header.xsize != 320 || // vertical pixels
            //header.ysize != 200 || // horizonal pixels
            header.bitsinf != 2 || // bits per pixel
            header.emark != 0xff ||
            header.evideo != 'A' ||
            header.edesc != 1)
        {
            return;
        }

        // allocate destination for unpacked data
        byte[] dest = new byte[header.xsize * header.ysize / (8 / header.bitsinf) /* 0x3e80 or 16000*/];

        byte[] eData = new byte[header.esize];

        for (int i = 0; i < header.esize; i ++)
        {
            eData[i] = picFileData[fileIndex++];
        }

        // destination index
        int pdest = 0;

        // read in the number of blocks to process
        ushort numblks = System.BitConverter.ToUInt16(picFileData, fileIndex);
        fileIndex += 2;

        for (int i = 0; i < numblks; i ++) 
        {
			tBlock BLOCK = new tBlock();
			int len;

            // read in the block info
            BLOCK.PBSIZE = System.BitConverter.ToUInt16(picFileData, fileIndex);
            fileIndex += 2;
            BLOCK.size = System.BitConverter.ToUInt16(picFileData, fileIndex);
            fileIndex += 2;
            /*+05*/
            BLOCK.MBYTE = picFileData[fileIndex++];

            int work = fileIndex;
            int p = work;
			int block_end = work + BLOCK.PBSIZE - 5 /* size of tBlock */;
            fileIndex += (BLOCK.PBSIZE - 5 /* size of tBlock */);
            do 
            {
				len = 1;
				byte byt = picFileData[p++];
				if (byt == BLOCK.MBYTE) 
                {
					len = picFileData[p++];
					if(len == 0) 
                    {
                        len = System.BitConverter.ToUInt16(picFileData, p);
                        p += 2;

                    }
                    byt = picFileData[p++];
				} 
                while (len-- != 0)
                {
                    dest[pdest++] = byt;
                }
			} while (p < block_end);
		}

        // reset destination pointer
        pdest = 0;

        // transfer to texture
        // The end game sequence relies on images overlapping each other so we
        // will only copy if the pixel is not black
        for (int i = 0; i < header.ysize; i++)
        {
            for (int j = 0; j < header.xsize; j += 4)
            {
                byte byt = dest[pdest++];
                Color color = Palette.CGAColorPalette[(byt & 0xc0) >> 6];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 0, i, color);
                }
                color = Palette.CGAColorPalette[(byt & 0x30) >> 4];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 1, i, color);
                }
                color = Palette.CGAColorPalette[(byt & 0x0c) >> 2];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 2, i, color);
                }
                color = Palette.CGAColorPalette[(byt & 0x03) >> 0];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 3, i, color);
                }
            }
        }
        texture.Apply();
    }

    void LoadAVATAREGAFile(string file, Texture2D texture)
    {
        // check if the file exists
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        // check if the file at least has some size
        if (picFileData.Length == 0)
        {
            Debug.Log("Picture file incorrect length " + picFileData.Length);
            return;
        }

        // allocate destination for unpacked data, 2 pixels per byte
        byte[] dest = new byte[320 * 200 / 2];

        // reset source and destination indexes
        int destinationIndex = 0;
        int fileIndex = 0;

        // simple rle using magic code
        while (fileIndex < picFileData.Length)
        {
            // magic code is 2
            const byte MAGIC_CODE = 0x02;

            // read in the block info
            byte data = picFileData[fileIndex++];

            // check if data is the magic code
            if (data == MAGIC_CODE)
            {
                // get the length of the rle
                byte length = picFileData[fileIndex++];

                // get the data byte to use
                data = picFileData[fileIndex++];

                // fill destination with a runo of length of data 
                while (length-- != 0)
                {
                    // copy the data
                    dest[destinationIndex++] = data;
                }
            }
            else
            {
                // just copy the data to the destination
                dest[destinationIndex++] = data;
            }
        }

        // reset destination pointer
        destinationIndex = 0;

        // transfer to texture
        // The end game sequence relies on images overlapping each other so we
        // will only copy if the pixel is not black
        for (int y = 0; y < 200; y++)
        {
            for (int x = 0; x < 320; x += 4)
            {
                ushort word = System.BitConverter.ToUInt16(dest, destinationIndex);

                destinationIndex += 2;

                Color color = Palette.EGAColorPalette[(word & 0x000F) >> 0];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 1, texture.height - 1 - y, color);
                }
                color = Palette.EGAColorPalette[(word & 0x00F0) >> 4];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 0, texture.height - 1 - y, color);
                }
                color = Palette.EGAColorPalette[(word & 0x0F00) >> 8];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 3, texture.height - 1 - y, color);
                }
                color = Palette.EGAColorPalette[(word & 0xF000) >> 12];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 2, texture.height - 1 - y, color);
                }
            }
        }

        // apply pixels set above to texture
        texture.Apply();
    }

    Texture2D LoadTITLEPicPictureFile(string file, Texture2D texture)
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return null;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        lzw l = new lzw();

        long s = l.GetDecompressedSize(picFileData, picFileData.Length);
        byte[] uncompressedFileData = new byte[s];
        l.Decompress(picFileData, uncompressedFileData, picFileData.Length);

        return PIC_To_Texture2D(uncompressedFileData, texture);
    }

    byte[] LoadTITLEEGAPictureFile(string file)
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return null;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        lzw l = new lzw();

        long s = l.GetDecompressedSize(picFileData, picFileData.Length);
        byte[] dest = new byte[s];
        l.Decompress(picFileData, dest, picFileData.Length);

        return dest;
    }

    byte[] LoadTITLEEGAPictureFile2(string file)
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return null;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        lzw l = new lzw();

        long s = l.GetDecompressedSize(picFileData, picFileData.Length);
        byte[] dest = new byte[s];
        l.Decompress(picFileData, dest, picFileData.Length);

        return dest;
    }

    // TODO get this from the exe or create my own
    ushort[] RANDOM = {
                    0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,
                    0x0000,0x0001,0xC004,0x0010,0x0080,0x0104,0x0400,0x0401,
                    0x0100,0xC3C0,0x1000,0xC410,0x1004,0x0411,0x0110,0x03C4,
                    0x1080,0xC481,0x0110,0xC104,0x1010,0xD101,0x1084,0x13C1,
                    0x1010,0x1304,0xC484,0x3CF0,0x0F3C,0x0F3F,0x3C84,0x3C3F,
                    0x10F1,0x13FC,0xCFC1,0x3CF1,0xCFCF,0x3CFF,0xD13F,0x3FF1,
                    0xD3F1,0xF3FC,0x3C8F,0xCF8F,0x3F3F,0xFFCF,0x13FC,0xCFFF,
                    0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF
                };
    public Texture2D PIC_To_Texture2D(
        byte [] raw,
        Texture2D texture)
    {
        int x, y;

        for (y = 0; y < texture.height; y++)
        {
            int src;

            // Note: there is a 7 byte header on these files, not sure what it contains
            // Note: this is interlaced
            src = (y % 2) * 0x2000 + (y / 2) * (texture.width / 4) + 7;
            for (x = 0; x < texture.width; x += 8)
            {
                ushort word;

                word = System.BitConverter.ToUInt16(raw, src);

                src += 2;

                // two bits per pixel
                texture.SetPixel(x + 0, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x00c0) >> 6]);
                texture.SetPixel(x + 1, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0030) >> 4]);
                texture.SetPixel(x + 2, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x000c) >> 2]);
                texture.SetPixel(x + 3, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0003) >> 0]);
                texture.SetPixel(x + 4, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0xc000) >> 14]);
                texture.SetPixel(x + 5, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x3000) >> 12]);
                texture.SetPixel(x + 6, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0c00) >> 10]);
                texture.SetPixel(x + 7, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0300) >> 8]);
            }
        }

        texture.Apply();

        return texture;
    }

    public void EGA_To_Texture2D(
        byte[] raw,
        Texture2D texture)
    {
        int x, y;

        for (y = 0; y < texture.height; y++)
        {
            int src;

            src = y * texture.width / 2;
            for (x = 0; x < texture.width; x += 4)
            {
                ushort word;

                word = System.BitConverter.ToUInt16(raw, src);

                src += 2;

                // 4 bits per pixel, these pixels are swapped
                Color color = Palette.EGAColorPalette[(word & 0x000F) >> 0];
                texture.SetPixel(x + 1, texture.height - 1 - y, color);
                color = Palette.EGAColorPalette[(word & 0x00F0) >> 4];
                texture.SetPixel(x + 0, texture.height - 1 - y, color);
                color = Palette.EGAColorPalette[(word & 0x0F00) >> 8];
                texture.SetPixel(x + 3, texture.height - 1 - y, color);
                color = Palette.EGAColorPalette[(word & 0xF000) >> 12];
                texture.SetPixel(x + 2, texture.height - 1 - y, color);
            }
        }

        texture.Apply();
    }

    public void CopyTexture2D(
        byte[] raw,
        int src_x,
        int src_y,
        int dst_x,
        int dst_y,
        int width,
        int height,
        int randomStuff,
        Texture2D destination)
    {
        int x, y;

        for (y = src_y; y < height + src_y; y++)
        {
            int src;

            src = (y * 320 / 2) + (src_x / 2);
            for (x = src_x; x < width + src_x; x += 4)
            {
                ushort word;

                word = System.BitConverter.ToUInt16(raw, src);

                src += 2;

                if (randomStuff != -1)
                {
                    if (word == 0)
                    {
                        continue;
                    }

                    word &= RANDOM[(randomStuff & 0xff) + (Random.Range(0, 7))];
                }

                // 4 bits per pixel, these pixels are swapped
                Color color = Palette.EGAColorPalette[(word & 0x000F) >> 0];
                destination.SetPixel(dst_x + x - src_x + 1, destination.height - (dst_y + y - src_y), color);
                color = Palette.EGAColorPalette[(word & 0x00F0) >> 4];
                destination.SetPixel(dst_x + x - src_x + 0, destination.height - (dst_y + y - src_y), color);
                color = Palette.EGAColorPalette[(word & 0x0F00) >> 8];
                destination.SetPixel(dst_x + x - src_x + 3, destination.height - (dst_y + y - src_y), color);
                color = Palette.EGAColorPalette[(word & 0xF000) >> 12];
                destination.SetPixel(dst_x + x - src_x + 2, destination.height - (dst_y + y - src_y), color);
            }
        }

        // we will do this at the end of the update call instead
        //destination.Apply();
    }

    bool CheckTileForOpacity(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        return (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_WALL
                    || tileIndex == U4_Decompiled_AVATAR.TILE.LARGE_ROCKS
                    || tileIndex == U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL);
    }

    bool CheckShortTileForOpacity(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        return (CheckTileForOpacity(tileIndex) ||
                    ((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE)));
    }


    public void CreateMap(GameObject mapGameObject, U4_Decompiled_AVATAR.TILE[,] map, bool lookAtCamera = true)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;
        bool useExpandedTile;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in terrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in animatedTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // this takes about 1/2 second for the 64x64 outside grid.
        // go through the map tiles and create game objects for each
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location;
                Vector3 rotation;
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DIAGONAL_WATER_ARCHITECTURE1)
                {
                    mapTile = Primitive.CreateWedge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // trees we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.RUINS) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ALTAR) ||
                   // (tileIndex == U4_Decompiled.TILE.CHEST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.PARTY) || // the shrine map uses a fixed party tile instead of putting the party characters into the map
                    (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad); 
                    mapTile = Primitive.CreateQuad();
                    mapTile.name = tileIndex.ToString();
                    mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y + 0.001f, 0.0f); // move it just a bit into the back
                    // need to move it here first and rotate it into place before we can get the results of LookAt()
                    mapTile.transform.localPosition = location;
                    //mapTile.transform.localEulerAngles = new Vector3(-180.0f, -90.0f, 90.0f);
                    rotation = new Vector3(-90.0f, 180.0f, 180.0f);

                    if (lookAtCamera)
                    {
                        Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                        mapTile.transform.LookAt(look.transform);
                        //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                        rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                        rotation.x = rotation.x - 180.0f;
                    }

                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE)
                {
                    mapTile = Primitive.CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_TOP)
                {
                    mapTile = Primitive.CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = Primitive.CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.DOOR) || (tileIndex == U4_Decompiled_AVATAR.TILE.LOCKED_DOOR))
                {
                    mapTile = Primitive.CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = Primitive.CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_MAST)
                {
                    mapTile = Primitive.CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_WHEEL)
                {
                    mapTile = Primitive.CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CHEST)
                {
                    mapTile = Primitive.CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_LEFT)
                {
                    mapTile = Primitive.CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT)
                {
                    mapTile = Primitive.CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE)
                {
                    mapTile = Primitive.CreateCastleCenter();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = Primitive.CreateQuad();
                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
                    {
                        mapTile.transform.SetParent(animatedTerrrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        // since we animate the texture using uv we cannot use the expanded tiles and need to use the original ones
                        useExpandedTile = false;
                    }
                    else
                    {
                        mapTile.transform.SetParent(terrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        useExpandedTile = true;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                if (useExpandedTile)
                {
                    renderer.material.mainTexture = Tile.expandedTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                    renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                }
                else
                {
                    renderer.material.mainTexture = Tile.originalTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                    renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
                }
            }
        }

        // this takes about 150ms for the 64x64 outside grid.
        Combine.Combine1(terrainGameObject);
        Combine.Combine2(animatedTerrrainGameObject);
        Combine.Combine1(billboardTerrrainGameObject); // combine separately from terrain above as we need to point these towards the player

        // add our little water animator script
        // adding a script component in the editor is a significant performance hit, avoid adding if already present

        if (animatedTerrrainGameObject.GetComponent<Animate1>() == null)
        {
            animatedTerrrainGameObject.AddComponent<Animate1>();
        }

        // Position the settlement in place
        mapGameObject.transform.position = new Vector3(-5, 0, 7);

        // rotate settlement into place
        mapGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    public void CreateMapSubset(GameObject mapGameObject, U4_Decompiled_AVATAR.TILE[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make dungeon entrance into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // tress we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    mapTile = Primitive.CreateQuad();

                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    // need to move it here first and rotate it into place before we can get the results of LookAt()
                    mapTile.transform.localPosition = location;
                    mapTile.transform.localEulerAngles = new Vector3(-180.0f, -90.0f, 90.0f);
                    Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                    look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                    mapTile.transform.LookAt(look.transform);
                    //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                    rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                    rotation.x = rotation.x - 180.0f;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = Primitive.CreateQuad();

                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
                    {
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        // since we animate the texture using uv we cannot use the expanded tiles and need to use the original ones
                    }
                    else
                    {
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                Renderer renderer = mapTile.GetComponent<MeshRenderer>();

                renderer.material.mainTexture = Tile.originalTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // stash the object mesh, transform & texture information
                entireMapGameObjects[x, y] = mapTile;
            }
        }
    }

    public GameObject CreateMapTileObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled_AVATAR.TILE tileIndex, ref U4_Decompiled_AVATAR.TILE[,] map, int x, int y, bool allWalls)
    {
        GameObject mapTile;
        Vector3 location = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        bool useExpandedTile;
        bool useLinearTile;

        // solid object, brick, rocks etc. make into cubes
        if (CheckTileForOpacity(tileIndex))
        {
            if (allWalls == false)
            {
                U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
            }
            else
            {
                mapTile = Primitive.CreatePartialCube();
            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // Letters, make into short cubes
        else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
            || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
        {
            if (allWalls == false)
            {
                U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
            }
            else
            {
                mapTile = Primitive.CreatePartialCube();

            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make mountains into pyramids
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
        {
            mapTile = Primitive.CreatePyramid(1.0f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountatins to show their best side
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make dungeon entrace into pyramid, rotate so it faces the right direction
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
        {
            mapTile = Primitive.CreatePyramid(0.2f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make brush and hills into short pyramids
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
        {
            mapTile = Primitive.CreatePyramid(0.15f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make rocks into little bigger short pyramids since you cannot walk over them
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
        {
            mapTile = Primitive.CreatePyramid(0.25f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // tress we need to stand upright and face the camera
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.RUINS) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.SHRINE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
        {
            mapTile = Primitive.CreateQuad();
            mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            // put this in a resonable rotation, combine3() will do the actual lookat rotaion just before displaying
            rotation = new Vector3(-90.0f, -90.0f, 90.0f);

            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE)
        {
            mapTile = Primitive.CreateBridge();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_TOP)
        {
            mapTile = Primitive.CreateBridgeUpper();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM)
        {
            mapTile = Primitive.CreateBridgeLower();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.DOOR) || (tileIndex == U4_Decompiled_AVATAR.TILE.LOCKED_DOOR))
        {
            mapTile = Primitive.CreateDoor();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR_COLUMN)
        {
            mapTile = Primitive.CreatePillar();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_MAST)
        {
            mapTile = Primitive.CreateMast();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_WHEEL)
        {
            mapTile = Primitive.CreateWheel();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CHEST)
        {
            mapTile = Primitive.CreateChest();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_LEFT)
        {
            mapTile = Primitive.CreateCastleLeft();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT)
        {
            mapTile = Primitive.CreateCastleRight();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE)
        {
            mapTile = Primitive.CreateCastleCenter();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // all other terrain tiles are flat
        else
        {
            mapTile = Primitive.CreateQuad();

            // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
            // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
            if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
            {
                mapTile.transform.SetParent(animatedTerrrainGameObject.transform);
                location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                rotation = Vector3.zero;
                // since we animate the texture using uv we cannot use the expanded tiles and need to use the linear ones
                useExpandedTile = false;
                useLinearTile = true;
            }
            else
            {
                mapTile.transform.SetParent(terrainGameObject.transform);
                location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                rotation = Vector3.zero;
                useExpandedTile = true;
                useLinearTile = false;
            }
        }

        mapTile.transform.localEulerAngles = rotation;
        mapTile.transform.localPosition = location;

        // all terrain is static, used by combine below to merge meshes
        mapTile.isStatic = true;

        // set the shader
        Renderer renderer = mapTile.GetComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

        // set the tile and texture offset and scale

        if (useExpandedTile)
        {
            renderer.material = Tile.combinedExpandedMaterial;
            renderer.material.mainTexture = Tile.combinedExpandedTexture;
            renderer.material.mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
            renderer.material.mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);

        }
        else if (useLinearTile)
        {
            renderer.material = Tile.combinedLinearMaterial;
            renderer.material.mainTexture = Tile.combinedLinearTexture;
            renderer.material.mainTextureOffset = new Vector2((float)((int)tileIndex * Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
            renderer.material.mainTextureScale = new Vector2((float)Tile.originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
        }
        else
        {
            renderer.material.mainTexture = Tile.originalTiles[(int)tileIndex];
            renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
            renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
        }

        Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
        Vector2[] uv = new Vector2[mesh.uv.Length];
        Vector2 textureAtlasOffset;

        textureAtlasOffset = new Vector2((int)tileIndex % Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileWidth, (int)tileIndex / Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileHeight);
        for (int u = 0; u < mesh.uv.Length; u++)
        {
            Vector2 mainTextureOffset;
            Vector2 mainTextureScale;

            if (useExpandedTile)
            {
                mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);
            }
            else if (useLinearTile)
            {
                mainTextureOffset = new Vector2((float)((int)tileIndex * Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                mainTextureScale = new Vector2((float)Tile.originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
            }
            else
            {
                mainTextureOffset = new Vector2(0.0f, 0.0f);
                mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
            uv[u] += (textureAtlasOffset + mainTextureOffset);
        }
        mesh.uv = uv;

        renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
        renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

        // disable these as we don't need them in the actual game only for mesh combine
        mapTile.SetActive(false);

        return mapTile;
    }

    GameObject[] allMapTilesGameObjects = null;

    GameObject GetCachedTileGameObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled_AVATAR.TILE tileIndex, ref U4_Decompiled_AVATAR.TILE[,] map, int x, int y, bool allWalls)
    {
        if (allMapTilesGameObjects == null)
        {
            allMapTilesGameObjects = new GameObject[(int)U4_Decompiled_AVATAR.TILE.MAX];
            for (int i = 0; i < (int)U4_Decompiled_AVATAR.TILE.MAX; i++)
            {
                allMapTilesGameObjects[i] = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, (U4_Decompiled_AVATAR.TILE)i, ref map, 0, 0, true);
            }
        }

        return allMapTilesGameObjects[(int)tileIndex];
    }


    public void CreateMapSubsetPass2(GameObject mapGameObject, ref U4_Decompiled_AVATAR.TILE[,] map, ref GameObject[,] mapGameObjects, bool allWalls = false)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in terrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in animatedTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // go through the map tiles and create game objects for each
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }

                // create the gameObject tile
                //GameObject mapTile = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, tileIndex, ref map, x, y, allWalls);
                GameObject mapTile = GetCachedTileGameObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, tileIndex, ref map, x, y, allWalls);

                // stash the game object in the array
                mapGameObjects[x, y] = mapTile;
            }
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
 
    }

    // cast one ray
    void Cast_Ray(ref U4_Decompiled_AVATAR.TILE[,] map, 
        int diff_x,
        int diff_y,
        int pos_x,
        int pos_y, 
        ref U4_Decompiled_AVATAR.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled_AVATAR.TILE wrapTile)
    {
        U4_Decompiled_AVATAR.TILE temp_tile;

        // are we outside the destination raycast map area, stop here
        if (pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1) || pos_x - offset_x < 0 || pos_y - offset_y < 0)
        {
            return;
        }

        // has the tile already been copied, if so stop here
        if (raycastMap[pos_x - offset_x, pos_y - offset_y] != U4_Decompiled_AVATAR.TILE.BLANK)
        {
            return;
        }

        // check if we should wrap the source map or if we should fill
        // any tile outside of the map area with a specific tile such as GRASS
        // are we outside the source map?
        if ((wrapTile != U4_Decompiled_AVATAR.TILE.BLANK) && (pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1) || pos_x < 0 || pos_y < 0))
        {
            temp_tile = wrapTile;
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
        }
        else
        {
            // get the tile and copy it to the raycast map
            temp_tile = map[(pos_x + map.GetLength(0)) % map.GetLength(0), (pos_y + map.GetLength(1)) % map.GetLength(1)];
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
        }

        // check the tile for opaque tiles
        if ((temp_tile == U4_Decompiled_AVATAR.TILE.FOREST) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.MOUNTAINS) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.BLANK) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.BRICK_WALL))
        {
            return;
        }

        // continue the ray cast recursively
        pos_x = (pos_x + diff_x);
        pos_y = (pos_y + diff_y);
        Cast_Ray(ref map, diff_x, diff_y, pos_x, pos_y, ref raycastMap, offset_x, offset_y, wrapTile);
        
        if ((diff_x & diff_y) != 0)
        {
            Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                pos_x, 
                (pos_y - diff_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                (pos_x - diff_x), 
                pos_y, 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
        else
        {
            Cast_Ray(ref map, 
                (((diff_x == 0) ? 1 : 0) * diff_y + diff_x), 
                (diff_y - ((diff_y == 0) ? 1 : 0) * diff_x), 
                (diff_y + pos_x), 
                (pos_y - diff_x), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            Cast_Ray(ref map, 
                (diff_x - ((diff_x == 0) ? 1 : 0) * diff_y), 
                (((diff_y == 0) ? 1 : 0) * diff_x + diff_y), 
                (pos_x - diff_y), 
                (diff_x + pos_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
    }

    // visible area (raycast)
    void raycast(ref U4_Decompiled_AVATAR.TILE[,] map, int pos_x, int pos_y, ref U4_Decompiled_AVATAR.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled_AVATAR.TILE wrapTile)
    {
        if (pos_x < 0 || pos_y < 0 || pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1))
        {
            Debug.Log("start position is outside of source map ( " + pos_x + ", " + pos_y + ")");
            return;
        }

        if (pos_x - offset_x < 0 || pos_y - offset_y < 0 || pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1))
        {
            Debug.Log("offset does not contain the starting position given the dimensions of the destination raycast map " 
                + "position ( " + pos_x + ", " + pos_y + ")" 
                + " offset (" + offset_x + ", " + offset_y + ")" 
                + " dimensions (" + raycastMap.GetLength(0) + ", " + raycastMap.GetLength(1) + ")");
            return;
        }

        // set all visible tiles in the destination raycast map to blank to start
        for (int y = 0; y < raycastMap.GetLength(1); y++)
        {
            for (int x = 0; x < raycastMap.GetLength(0); x++)
            {
                raycastMap[x, y] = U4_Decompiled_AVATAR.TILE.BLANK;
            }
        }

        // copy the starting position as it is alway visible given the map offset
        U4_Decompiled_AVATAR.TILE currentTile = map[pos_x, pos_y];
        raycastMap[pos_x - offset_x, pos_y - offset_y] = currentTile;

        // cast out recusively from the starting position
        Cast_Ray(ref map, 0, -1, pos_x, (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP
        Cast_Ray(ref map, 0, 1, pos_x, (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN
        Cast_Ray(ref map, -1, 0, (pos_x - 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray LEFT
        Cast_Ray(ref map, 1, 0, (pos_x + 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray RIGHT
        Cast_Ray(ref map, 1, 1, (pos_x + 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the RIGHT
        Cast_Ray(ref map, 1, -1, (pos_x + 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the RIGHT
        Cast_Ray(ref map, -1, 1, (pos_x - 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the LEFT
        Cast_Ray(ref map, -1, -1, (pos_x - 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the LEFT
    }

    // changes in these require redrawing the map
    int lastRaycastPlayer_posx = -1;
    int lastRaycastPlayer_posy = -1;
    int lastRaycastPlayer_f_1dc = -1;
    bool last_door_timer = false;

    public Image picture;
    public Texture2D pictureTexture;
    public Hashtable pictureTextureAtlas = new Hashtable();
    public Hashtable pictureRawAtlas = new Hashtable();
    public string lastPictureFilename;
    public int lastPictureDest;

    public enum PICTURE
    {
        /* The use a special format for the AVATAR.EXE both PIC and EGA files are different than TITLE.EXE */
        RUNE_0 = 0, // I
        RUNE_1 = 1, // N
        RUNE_2 = 2, // F
        RUNE_3 = 3, // T
        RUNE_4 = 4, // Y
        RUNE_5 = 5, // infinity symbol
        START = 6, // main screen layout
        KEY7 = 7, // keyhole
        STONCRCL = 8, // end game, return to stone circle
        TRUTH = 9, // these are combined and displayed in the end game
        LOVE = 10,
        COURAGE = 11,
        HONESTY = 12,
        COMPASSN = 13,
        VALOR = 14,
        JUSTICE = 15,
        SACRIFIC = 16,
        HONOR = 17,
        SPIRIT = 18,
        HUMILITY = 19,
        MAX = 20
    };

    public enum PICTURE2
    {
        /* The use a special format for the TITLE.EXE both PIC and EGA files are different than AVATAR.EXE */
        OUTSIDE = 0, // outside circus
        PORTAL = 1, // stone circle for portal
        TREE = 2, // outside tree
        INSIDE = 3, // inside circus
        WAGON = 4, // gypsy wagon
        GYPSY = 5, // gypsy fortune teller
        ABACUS = 6, // abacus and beads
        HONCOM = 7, // honor and compassion tarot cards
        VALJUS = 8, // valor and justice tarot cards
        SACHONOR = 9, // sacrafice and honor tarot cards
        SPIRHUM = 10, // spirit and humility tarot cards
        TITLE = 11, // title screen
        ANIMATE = 12, // large animated character frames from the title screen
        MAX = 13
    };

    private void Start()
    {
        // this object needs to move around so it needs to be above the other which are based on the whole world map
        mainTerrain = new GameObject("Main Terrain");

        // create game object under us to hold these sub categories of things
        terrain = new GameObject("terrain");
        terrain.transform.SetParent(mainTerrain.transform);
        terrain.transform.localPosition = Vector3.zero;
        terrain.transform.localRotation = Quaternion.identity;
        animatedTerrrain = new GameObject("water");
        animatedTerrrain.transform.SetParent(mainTerrain.transform);
        animatedTerrrain.transform.localPosition = Vector3.zero;
        animatedTerrrain.transform.localRotation = Quaternion.identity;
        billboardTerrrain = new GameObject("billboard");
        billboardTerrrain.transform.SetParent(mainTerrain.transform);
        billboardTerrrain.transform.localPosition = Vector3.zero;
        billboardTerrrain.transform.localRotation = Quaternion.identity;

        // initialize the palette and load the tiles
        Palette.InitializeEGAPalette();
        Palette.InitializeCGAPalette();
        Palette.InitializeApple2Palette();
        Tile.LoadTilesEGA();
        //Tile.LoadTilesCGA();
        //Tile.LoadTilesApple2();
        //Tile.LoadTilesPNG();

        // fix a tile
        Tile.FixMageTile3();

        // expand the tiles
        Tile.ExpandTiles();

        // create texture atlas
        Tile.CreateLinearTextureAtlas(ref Tile.originalTiles);
        Tile.CreateSquareTextureAtlas(ref Tile.originalTiles);
        Tile.CreateExpandedTextureAtlas(ref Tile.expandedTiles);

        // get the font
        GameFont.LoadCharSetEGA();
        //LoadCharSetCGA();
        GameFont.ImportFontFromTexture(myFont, myTransparentFont, GameFont.fontAtlas, GameFont.fontTransparentAtlas);

        // set all the text objects to myFont in the input panel
        Text[] text = InputPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set again just the button text objects in the input panel to myTransparentFont
        Button[] buttons = InputPanel.GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            Text[] texts = b.GetComponentsInChildren<Text>(true);
            foreach (Text t in texts)
            {
                t.font = myTransparentFont;
            }
        }


        // get a reference to the game engine
        u4_TITLE = FindObjectOfType<U4_Decompiled_TITLE>();


        // allocate the onscreen texture
        pictureTexture = new Texture2D(320, 200);
        pictureTexture.filterMode = FilterMode.Point;
        ClearTexture(pictureTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // set the onscreen texture to the sprite
        picture.sprite = Sprite.Create(pictureTexture, new Rect(0, 0, pictureTexture.width, pictureTexture.height), new Vector2(0.5f, 0.5f));
        picture.color = Color.white;

        // everything I need it now loaded, start the game engine thread
        u4_TITLE.StartThread();
    }



    // Update is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f;

    // used for a flag animation timer
    float flagTimer = 0.0f;
    float flagTimerExpired = 0.0f;
    public float flagTimerPeriod = 0.10f;

    GameObject hiddenWorldMapGameObject;
    U4_Decompiled_TITLE.INPUT_MODE lastInputMode = 0;

    // Update is called once per frame
    void Update()
    {
        // update the timer
        flagTimer += Time.deltaTime;

        // only update periodically
        if (flagTimer > flagTimerExpired)
        {
            // reset the expired timer
            flagTimer -= flagTimerExpired;
            flagTimerExpired = flagTimerPeriod;
            if (Tile.textureExpandedAtlasPowerOf2 != 0)
            {
                Tile.AnimateFlags();
            }
        }

        // update the timer
        timer += Time.deltaTime;

        // only update periodically
        if (timer > timerExpired)
        {
            // reset the expired timer
            timer -= timerExpired;
            timerExpired = timerPeriod;

            if (u4_TITLE.gameText != null && GameText != null)
            {
                GameText.GetComponentInChildren<UnityEngine.UI.Text>().text = u4_TITLE.gameText;
            }

            if (lastInputMode != u4_TITLE.inputMode)
            {
                if (lastInputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME)
                {
                    // clear the text after a name is entered
                    u4_TITLE.gameText = "";
                }

                lastInputMode = u4_TITLE.inputMode;

                if (u4_TITLE.inputMode != U4_Decompiled_TITLE.INPUT_MODE.LAUNCH_GAME)
                {
                    InputPanel.SetActive(true);
                }
                else
                {
                    InputPanel.SetActive(false);
                    Application.LoadLevel(1);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU)
                {
                    u4_TITLE.gameText = "In another world, in a time to come.\n \nOptions:\n";
                    GameText.GetComponentInChildren<Text>().alignment = TextAnchor.LowerCenter;

                    RectTransform rt = GameText.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);

                    u4_TITLE.specialEffectAudioSource.Stop();

                    MainMainLoop.SetActive(true);
                }
                else
                {
                    MainMainLoop.SetActive(false);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME)
                {
                    u4_TITLE.gameText = "By what name shalt thou be known in this world and time? \n \n              ";
                    GameText.GetComponentInChildren<Text>().alignment = TextAnchor.LowerLeft;

                    RectTransform rt = GameText.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40);

                    Keyboard.SetActive(true);
                    KeyboardUpper.SetActive(true);
                    KeyboardLower.SetActive(false);
                }
                else
                {
                    Keyboard.SetActive(false);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE) ||
                       (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME) ||
                       (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE) ||
                       (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU))
                {
                    GameText.SetActive(true);
                }
                else
                {
                    GameText.SetActive(false);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE)||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE))
                {
                    RectTransform rt = GameText.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 94);

                    rt = Picture.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 155);
                }
                else
                {
                    RectTransform rt = Picture.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 99);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MALE_OR_FEMALE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_NO_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME))
                {
                    Picture.SetActive(true);
                }
                else
                {
                    Picture.SetActive(false);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE)
                {
                    TalkChoice.SetActive(true);
                }
                else
                {
                    TalkChoice.SetActive(false);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MALE_OR_FEMALE)
                {
                    TalkMF.SetActive(true);
                }
                else
                {
                    TalkMF.SetActive(false);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE))
                {
                    TalkContinue.SetActive(true);
                }
                else
                {
                    TalkContinue.SetActive(false);
                }
            }

            while (u4_TITLE.screenCopyFrameQueue.Count != 0)
            {
                U4_Decompiled_TITLE.ScreenCopyFrame screenCopyFrame = u4_TITLE.screenCopyFrameQueue.Dequeue();

                Texture2D sourceTexture = (Texture2D)pictureTextureAtlas[screenCopyFrame.p];
                byte[] raw = (byte[])pictureRawAtlas[screenCopyFrame.p];

                if (sourceTexture != null && raw != null)
                {
                    if (screenCopyFrame.random_stuff == -1)
                    {
                        Graphics.CopyTexture(
                            sourceTexture,
                            0,
                            0,
                            screenCopyFrame.src_x_in_char * 8,
                            sourceTexture.height - screenCopyFrame.src_y - screenCopyFrame.height,
                            screenCopyFrame.width_in_char * 8,
                            screenCopyFrame.height,
                            pictureTexture,
                            0,
                            0,
                            screenCopyFrame.dst_x_in_char * 8,
                            pictureTexture.height - screenCopyFrame.dst_y - screenCopyFrame.height);
                    }
                    else
                    {
                        CopyTexture2D(
                            raw,
                            screenCopyFrame.src_x_in_char * 8,
                            screenCopyFrame.src_y,
                            screenCopyFrame.dst_x_in_char * 8,
                            screenCopyFrame.dst_y,
                            screenCopyFrame.width_in_char * 8,
                            screenCopyFrame.height,
                            screenCopyFrame.random_stuff,
                            pictureTexture);
                    }
                }
            }

            while (u4_TITLE.loadPictureQueue.Count != 0)
            {
                U4_Decompiled_TITLE.LoadPicture loadPicture = u4_TITLE.loadPictureQueue.Dequeue();
                if (loadPicture.filename.Length > 0)
                {
                    lastPictureFilename = loadPicture.filename;
                    lastPictureDest = loadPicture.dest;

                    if (!pictureTextureAtlas.ContainsKey(lastPictureDest))
                    {
                        // create new texture
                        Texture2D addPictureTexture = new Texture2D(320, 200);

                        byte[] destRaw = LoadTITLEEGAPictureFile(loadPicture.filename.ToLower().Replace(".pic", ".ega"));
                        EGA_To_Texture2D(destRaw, addPictureTexture);

                        pictureTextureAtlas.Add(lastPictureDest, addPictureTexture);
                        pictureRawAtlas.Add(lastPictureDest, destRaw);
                    }
                    else
                    {
                        // update texture with new picture from file
                        byte[] destRaw = LoadTITLEEGAPictureFile(loadPicture.filename.ToLower().Replace(".pic", ".ega"));
                        EGA_To_Texture2D(destRaw, (Texture2D)pictureTextureAtlas[lastPictureDest]);
                        pictureRawAtlas[loadPicture.dest] = destRaw;
                    }
                }
            }

            if (u4_TITLE.screenDotQueue.Count > 0)
            {
                while (u4_TITLE.screenDotQueue.Count != 0)
                {
                    U4_Decompiled_TITLE.ScreenDot screenDot = u4_TITLE.screenDotQueue.Dequeue();

                    // convert back to EGA colors because the game engine is running with different palette
                    if (screenDot.color == 3)
                    {
                        pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BRIGHT_CYAN]);
                    }
                    else if (screenDot.color == 2)
                    {
                        pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]);
                    }
                    else
                    {
                        pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.CGAColorPalette[screenDot.color]);
                    }
                }
            }
            pictureTexture.Apply(); // TODO: try to do this only once per frame at the end to speed things up

            // TODO this is slower than other methods need to switch
            if (u4_TITLE.mapChanged)
            {
                u4_TITLE.mapChanged = false;
                CreateMap(gameObject, u4_TITLE.map);
            }
        }
    }

    // The font is setup so if the high bit is set it will use the inverse highlighted text
    // this function will set the high bit on all the characters in a string so when displayed with the font
    // it will be highlighted
    public string highlight(string s)
    {
        string temp = "";
        for (int j = 0; j < s.Length; j++)
        {
            char c = s[j];

            if (c == '\n')
            {
                temp += '\n';
            }
            else if (c == ' ')
            {
                temp += (char)(0x12 + 0x80);
            }
            else
            {
                temp += (char)(s[j] + 0x80);
            }
        }

        return temp;
    }
}

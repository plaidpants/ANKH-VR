using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Picture
{
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

    // TODO get this from the exe or create my own
    public static ushort[] RANDOM = {
                    0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,
                    0x0000,0x0001,0xC004,0x0010,0x0080,0x0104,0x0400,0x0401,
                    0x0100,0xC3C0,0x1000,0xC410,0x1004,0x0411,0x0110,0x03C4,
                    0x1080,0xC481,0x0110,0xC104,0x1010,0xD101,0x1084,0x13C1,
                    0x1010,0x1304,0xC484,0x3CF0,0x0F3C,0x0F3F,0x3C84,0x3C3F,
                    0x10F1,0x13FC,0xCFC1,0x3CF1,0xCFCF,0x3CFF,0xD13F,0x3FF1,
                    0xD3F1,0xF3FC,0x3C8F,0xCF8F,0x3F3F,0xFFCF,0x13FC,0xCFFF,
                    0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF
                };

    public static void ClearTexture(Texture2D texture, Color color)
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

    public static void LoadAVATARPicFile(string file, Texture2D texture)
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

        for (int i = 0; i < header.esize; i++)
        {
            eData[i] = picFileData[fileIndex++];
        }

        // destination index
        int pdest = 0;

        // read in the number of blocks to process
        ushort numblks = System.BitConverter.ToUInt16(picFileData, fileIndex);
        fileIndex += 2;

        for (int i = 0; i < numblks; i++)
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
                    if (len == 0)
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

    public static void LoadAVATAREGAFile(string file, Texture2D texture)
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

    public static Texture2D LoadTITLEPicPictureFile(string file, Texture2D texture)
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

        return PIC_To_Texture2D(uncompressedFileData, -1, texture);
    }

    public static Texture2D LoadTITLEEGAPictureFile(string file, Texture2D texture)
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

        return EGA_To_Texture2D(dest, -1, texture);
    }

    public static Texture2D PIC_To_Texture2D(
        byte[] raw,
        int randomStuff,
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

                if (randomStuff != -1)
                {
                    if (word == 0)
                    {
                        continue;
                    }

                    word &= RANDOM[(randomStuff & 0xff) + (Random.Range(0, 7))];
                }

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

    public static Texture2D EGA_To_Texture2D(
        byte[] raw,
        int randomStuff,
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

                if (randomStuff != -1)
                {
                    if (word != 0)
                    {
                        continue;
                    }

                    word &= RANDOM[(randomStuff & 0xff) + (Random.Range(0, 7))];
                }

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

        return texture;
    }

    public static byte[] LoadTITLEEGAPictureFile(string file)
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

    public static byte[] LoadTITLEEGAPictureFile2(string file)
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

    public static Texture2D PIC_To_Texture2D(
        byte[] raw,
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

    public static void EGA_To_Texture2D(
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

    public static void CopyTexture2D(
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
}

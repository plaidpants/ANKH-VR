using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    // input panel game objects, need to set from the unity editor
    public GameObject InputPanel;
    public GameObject MainMainLoop;
    public GameObject Keyboard;
    public GameObject KeyboardUpper;
    public GameObject KeyboardLower;
    public GameObject GameText;
    public GameObject PictureGameObject;
    public GameObject TalkChoice;
    public GameObject TalkContinue;
    public GameObject TalkMF;

    // reference to game engine, needs to be set from the unity editor
    public U4_Decompiled_TITLE u4_TITLE;

    // dynamically created fonts for buttons and text
    public Font myFont;
    public Font myTransparentFont;

    // input panel UI picture image object and texture object
    public Image picture;
    public Texture2D pictureTexture;

    // used to create a sort of database of picture textures as the game engine loads and manipulates/copies portions of them
    public Hashtable pictureTextureAtlas = new Hashtable();
    public Hashtable pictureRawAtlas = new Hashtable();

    // Used to limit Update processing as it is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f; // adjust as needed from the unity editor, should be less than or equal the game engine timer or we may miss things

    // Used for a flag animation timer
    float flagTimer = 0.0f;
    float flagTimerExpired = 0.0f;
    public float flagTimerPeriod = 0.10f; // adjust as needed from the unity editor

    // Used to store the last input mode for some input state handling
    U4_Decompiled_TITLE.INPUT_MODE lastInputMode = 0;

    private void Start()
    {
        // initialize the palette 
        Palette.InitializeEGAPalette();
        Palette.InitializeCGAPalette();
        Palette.InitializeApple2Palette();

        // load the tiles
        Tile.LoadTilesEGA();
        //Tile.LoadTilesCGA();
        //Tile.LoadTilesApple2();
        //Tile.LoadTilesPNG();

        // fix a tile
        Tile.FixMageTile3();

        // expand the tiles
        Tile.ExpandTiles();

        // create texture atlas database
        Tile.CreateLinearTextureAtlas(ref Tile.originalTiles);
        Tile.CreateSquareTextureAtlas(ref Tile.originalTiles);
        Tile.CreateExpandedTextureAtlas(ref Tile.expandedTiles);

        // load the font
        GameFont.LoadCharSetEGA();
        //GameFont.LoadCharSetCGA();

        // setup the fonts
        GameFont.ImportFontFromTexture(myFont, myTransparentFont, GameFont.fontAtlas, GameFont.fontTransparentAtlas);

        // set all the text objects to myFont in the input panel
        Text[] text = InputPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set them again but just the button text objects in the input panel to myTransparentFont so highlighting works
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

        // we want pixels
        pictureTexture.filterMode = FilterMode.Point;

        // clear the texture as new textures are not always white as unity will reuse old textures
        Picture.ClearTexture(pictureTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // set the onscreen texture to the sprite
        picture.sprite = Sprite.Create(pictureTexture, new Rect(0, 0, pictureTexture.width, pictureTexture.height), new Vector2(0.5f, 0.5f));
        // set the texture to full color with no color tint
        picture.color = Color.white;

        // everything I need it now loaded, start the game engine thread
        u4_TITLE.StartThread();
    }

    // used to update the input panel state to mantch the game engine current input mode
    void UpdateInputPanelState()
    {
        // check if the game engine is initialized and the Game Text object is set
        if (u4_TITLE != null && u4_TITLE.gameText != null && GameText != null)
        {
            // copy the game engine text to the game text object for display
            GameText.GetComponentInChildren<UnityEngine.UI.Text>().text = u4_TITLE.gameText;
        }

        // check if the input mode has changed
        if (lastInputMode != u4_TITLE.inputMode)
        {
            // if the last input mode was name entry we need to clear the text so the name entry text does not interfer with the gypsy text
            if (lastInputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME)
            {
                // clear the text after a name is entered
                u4_TITLE.gameText = "";
            }

            // update the last input mode
            lastInputMode = u4_TITLE.inputMode;

            // deactive the entire input panel when ready to launch the game
            if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.LAUNCH_GAME)
            {
                InputPanel.SetActive(false);
            }
            else
            {
                InputPanel.SetActive(true);
            }

            // check for main menu input
            if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU)
            {
                // use precanned text when on the main menu
                u4_TITLE.gameText = "In another world, in a time to come.\n \nOptions:\n";
                GameText.GetComponentInChildren<Text>().alignment = TextAnchor.LowerCenter;

                // adjust the size of the text panel to fit
                RectTransform rt = GameText.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);

                // stop the noise sound effect if it is playing
                u4_TITLE.specialEffectAudioSource.Stop();

                // enable the main menu input panel
                MainMainLoop.SetActive(true);
            }
            else
            {
                MainMainLoop.SetActive(false);
            }

            // check for name input mode
            if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME)
            {
                // use precanned text
                u4_TITLE.gameText = "By what name shalt thou be known in this world and time? \n \n              ";
                GameText.GetComponentInChildren<Text>().alignment = TextAnchor.LowerLeft;

                // adjust the size of the text panel to fit
                RectTransform rt = GameText.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40);

                // enable and initialize the keyboard input, this is handled completely by the unity XR after this using OnClick actions
                Keyboard.SetActive(true);
                KeyboardUpper.SetActive(true);
                KeyboardLower.SetActive(false);
            }
            else
            {
                Keyboard.SetActive(false);
            }

            // enable the game text in these modes
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

            // adjust the game text size in these mode so the text and story images are fully displayed
            if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE))
            {
                RectTransform rt = GameText.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 94);

                rt = PictureGameObject.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 155);
            }
            else
            {
                RectTransform rt = PictureGameObject.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 99);
            }

            // display the picture in these modes
            if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MALE_OR_FEMALE) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_NO_CONTINUE) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_CONTINUE) ||
                    (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME))
            {
                PictureGameObject.SetActive(true);
            }
            else
            {
                PictureGameObject.SetActive(false);
            }

            // check if we are in the A & B input mode
            if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE)
            {
                TalkChoice.SetActive(true);
            }
            else
            {
                TalkChoice.SetActive(false);
            }

            // check if we are in the M & F input mode
            if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MALE_OR_FEMALE)
            {
                TalkMF.SetActive(true);
            }
            else
            {
                TalkMF.SetActive(false);
            }

            // enable the continue mode input in these modes
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
    }

    // This processes screen copy requests from the game engine
    void ProcessScreenCopyQueue()
    {
        // process all items in the queue
        while (u4_TITLE.screenCopyFrameQueue.Count != 0)
        {
            // get an item from the queue
            U4_Decompiled_TITLE.ScreenCopyFrame screenCopyFrame = u4_TITLE.screenCopyFrameQueue.Dequeue();

            // get the raw and texture from the database using the pointer reference as an index
            Texture2D sourceTexture = (Texture2D)pictureTextureAtlas[screenCopyFrame.p];
            byte[] raw = (byte[])pictureRawAtlas[screenCopyFrame.p];

            // check if they are valid
            if (sourceTexture != null && raw != null)
            {
                // if not doing fancy random stuff just use the unity texture copy method, this can happen all the the GPU for speed
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
                // we are doing fancy random stuff for the title screen Ultima logo reveal so we need to do our own thing
                else
                {
                    // use out own texture copy method that can do the random stuff
                    // NOTE: we delay tha texture apply until later since we may have additional changes to the texture
                    Picture.CopyTexture2D(
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
    }

    // This processes picture loading requests from the game engine
    void ProcessScreenLoadQueue()
    {
        // process all items in the queue
        while (u4_TITLE.loadPictureQueue.Count != 0)
        {
            // get an item from the queue
            U4_Decompiled_TITLE.LoadPicture loadPicture = u4_TITLE.loadPictureQueue.Dequeue();

            // make sure it's a valid filename
            if (loadPicture.filename.Length > 0)
            {
                // check if we have already a picture loaded into this destination
                if (!pictureTextureAtlas.ContainsKey(loadPicture.dest))
                {
                    // create new texture if a new destination
                    Texture2D addPictureTexture = new Texture2D(320, 200);

                    // load the picture raw 
                    byte[] destRaw = Picture.LoadTITLEEGAPictureFile(loadPicture.filename.ToLower().Replace(".pic", ".ega"));

                    // convert the raw into a texture
                    Picture.EGA_To_Texture2D(destRaw, addPictureTexture);

                    // add this raw and texture into our database
                    pictureTextureAtlas.Add(loadPicture.dest, addPictureTexture);
                    pictureRawAtlas.Add(loadPicture.dest, destRaw);
                }
                // we have this destination in our database
                else
                {
                    // update existing texture and raw at that destination point index with new picture from file
                    byte[] destRaw = Picture.LoadTITLEEGAPictureFile(loadPicture.filename.ToLower().Replace(".pic", ".ega"));

                    // convert the raw into a texture and store in the database
                    Picture.EGA_To_Texture2D(destRaw, (Texture2D)pictureTextureAtlas[loadPicture.dest]);

                    // update the raw in the database
                    pictureRawAtlas[loadPicture.dest] = destRaw;
                }
            }
        }
    }

    // This is used to draw the Lord British signature on the title screen
    void ProcessScreenDotQueue()
    {
        // process all items in the queue
        while (u4_TITLE.screenDotQueue.Count != 0)
        {
            // get an item from the queue
            U4_Decompiled_TITLE.ScreenDot screenDot = u4_TITLE.screenDotQueue.Dequeue();

            // manually convert back to EGA colors and draw the dot because the game engine is running with different CGA palette
            if (screenDot.color == 3)
            {
                pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BRIGHT_CYAN]);
            }
            else if (screenDot.color == 2)
            {
                pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]);
            }
            // just in case have a fall through catch all
            else
            {
                pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.CGAColorPalette[screenDot.color]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update the flag timer
        flagTimer += Time.deltaTime;

        // only update periodically
        if (flagTimer > flagTimerExpired)
        {
            // reset the expired timer
            flagTimer -= flagTimerExpired;
            flagTimerExpired = flagTimerPeriod;

            // check if the tiles are loaded, TODO probably need a better way to do this
            if (Tile.textureExpandedAtlasPowerOf2 != 0)
            {
                // animate the flag tiles
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

            // handle all input panel updates
            UpdateInputPanelState();
            ProcessScreenCopyQueue();
            ProcessScreenLoadQueue();
            ProcessScreenDotQueue();

            // apply any picture updates from above, do this only once per frame at the end to speed things up
            pictureTexture.Apply(); 

            // TODO this is slower than other methods as it always recreates the map from scratch every frame
            if (u4_TITLE.mapChanged)
            {
                u4_TITLE.mapChanged = false;
                Map.CreateMap(gameObject, u4_TITLE.map);
            }

            // check if we have reached the end of the title input modes
            if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.LAUNCH_GAME)
            {
                // launch the game
                Application.LoadLevel(1);
            }
        }
    }
}

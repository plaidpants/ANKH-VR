#define USE_UNITY_DLL_FUNCTION

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS;
using Meta.WitAi.TTS.Data;
using System.Linq;

public class U4_Decompiled_TITLE : MonoBehaviour
{
    private Thread trd;

    public AudioSource specialEffectAudioSource;
    public string gameText;
    public bool started_playing_sound_effect = false;

    public char lastKeyboardHit;

    public TTSSpeaker speaker;
    public string lordBritishVoiceName;
    public string gypsyVoiceName;
    public string ticketTakerVoiceName;

    public struct ScreenCopyFrame
    {
        public int width_in_char/*bp04*/;
        public int height/*bp06*/;
        public int src_x_in_char/*bp08*/;
        public int src_y/*bp0a*/;
        public int p/*bp0e:bp0c*/;
        public int dst_y/*bp10*/;
        public int random_stuff /*bp12*/;
        public int dst_x_in_char/*bp14*/;
    };

    public Queue<ScreenCopyFrame> screenCopyFrameQueue = new Queue<ScreenCopyFrame>();

    public struct ScreenDot
    {
        public int x;
        public int y;
        public int color;
    };

    public Queue<ScreenDot> screenDotQueue = new Queue<ScreenDot>();

    public struct LoadPicture
    {
        public int dest;
        public string filename;
    };

    public Queue<LoadPicture> loadPictureQueue = new Queue<LoadPicture>();

    public INPUT_MODE inputMode;

    public enum INPUT_MODE
    {
        NONE = 0,
        MALE_OR_FEMALE = 1,
        NAME = 2,
        A_OR_B_CHOICE = 3,
        MAIN_MENU = 4,

        // general input
        DELAY_CONTINUE = 5,
        DELAY_NO_CONTINUE = 7,

        // drive letter for PCs, not really useful here
        DRIVE_LETTER = 6,

        LAUNCH_GAME = 8,

        DELAY_TEXT_CONTINUE = 9,
    }

#if !USE_UNITY_DLL_FUNCTION
    static System.IntPtr nativeLibraryPtr2;

    delegate void main();
    delegate void main_keyboardHit(char key);
    delegate void main_CurMap(byte[] buffer, int length);
    delegate int main_SoundFlag();
    delegate void main_SetDataPath(byte[] buffer, int length);
    delegate int main_sound_effect();
    delegate void main_sound_effect_done();
    delegate INPUT_MODE main_input_mode();
    delegate int main_GetPicture(byte[] buffer, int length);
    delegate int main_Text(byte[] buffer, int length);
    delegate int main_screen_copy_frame(int[] buffer, int length);
    delegate int main_dot(int[] buffer, int length);
    delegate int main_D_3683(byte[] buffer, int length);
    delegate int main_D_6976(byte[] buffer, int length);
    delegate int main_D_3A24(byte[] buffer, int length);
#endif

    void Awake()
    {
        //Debug.Log("Load songs");
        LoadSongs();

#if !USE_UNITY_DLL_FUNCTION
        //Debug.Log("Patch TITLE.EXE to TITLE.DLL");
        // create a DLL file from the original DOS TITLE.EXE file by patching it
        var sourceFile = new FileInfo(Application.persistentDataPath + "/u4/TITLE.EXE");
        var patchFile = new FileInfo(Application.persistentDataPath + "/u4/TITLE.bps");
#if PLATFORM_ANDROID && !UNITY_EDITOR
        var targetFile = new FileInfo(Application.persistentDataPath + "/u4/TITLE.so");
#else
        var targetFile = new FileInfo(Application.persistentDataPath + "/u4/TITLE.DLL");
#endif

        DecoderBSP.ApplyPatch(sourceFile, patchFile, targetFile);

        //Debug.Log("Load TITLE.DLL");
        // now attempt to load this DLL
        if (U4_Decompiled_TITLE.nativeLibraryPtr2 != System.IntPtr.Zero)
        {
            return;
        }

#if PLATFORM_ANDROID && !UNITY_EDITOR
        nativeLibraryPtr2 = Native.dlopen(Application.persistentDataPath + "/u4/TITLE", (int)Native.PosixDlopenFlags.Now);
#else
        nativeLibraryPtr2 = Native.LoadLibrary(Application.persistentDataPath + "/u4/TITLE.DLL");
#endif
        if (nativeLibraryPtr2 == System.IntPtr.Zero)
        {
            Debug.LogError("Failed to load native library");
        }
#endif

        // Set the data path for the DLL before we start the thread,
        // cstring are hard so we will just send the string buffer and a length.
        string path = Application.persistentDataPath + "/u4/";
        for (int i = 0; i < path.Length; i++)
        {
            bytebuffer[i] = (byte)path[i];
        }
        bytebuffer[path.Length] = 0;
#if USE_UNITY_DLL_FUNCTION
        main_SetDataPath(bytebuffer, path.Length);
#else
        Native.Invoke<main_SetDataPath>(U4_Decompiled_TITLE.nativeLibraryPtr2, bytebuffer, path.Length); 
#endif
    }

#if USE_UNITY_DLL_FUNCTION
#if PLATFORM_ANDROID && !UNITY_EDITOR
    // interface to the game engine
    [DllImport("libTITLE")]
    public static extern void main();
    [DllImport("libTITLE")]
    public static extern void main_keyboardHit(char key);
    [DllImport("libTITLE")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_SoundFlag();
    [DllImport("libTITLE")]
    public static extern void main_SetDataPath(byte[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_sound_effect();    
    [DllImport("libTITLE")]
    public static extern void main_sound_effect_done();
    [DllImport("libTITLE")]
    public static extern INPUT_MODE main_input_mode();
    [DllImport("libTITLE")]
    public static extern int main_GetPicture(byte[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_Text(byte[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_screen_copy_frame(int[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_dot(int[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_D_3683(byte[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_D_6976(byte[] buffer, int length);
    [DllImport("libTITLE")]
    public static extern int main_D_3A24(byte[] buffer, int length);
#else
    // interface to the game engine
    [DllImport("TITLE")]
    public static extern void main();
    [DllImport("TITLE")]
    public static extern void main_keyboardHit(char key);
    [DllImport("TITLE")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_SoundFlag();
    [DllImport("TITLE")]
    public static extern void main_SetDataPath(byte[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_sound_effect();
    [DllImport("TITLE")]
    public static extern void main_sound_effect_done();
    [DllImport("TITLE")]
    public static extern INPUT_MODE main_input_mode();
    [DllImport("TITLE")]
    public static extern int main_GetPicture(byte[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_Text(byte[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_screen_copy_frame(int[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_dot(int[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_D_3683(byte[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_D_6976(byte[] buffer, int length);
    [DllImport("TITLE")]
    public static extern int main_D_3A24(byte[] buffer, int length);
#endif
#endif

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f; // the game operates on a 300ms Sleep() so we want to update things faster than that

    // buffer used to read stuff from the game engine
    byte[] bytebuffer = new byte[10000];
    int[] intbuffer = new int[500];

    // Separate thread to run the game, we could attempt to make the data gathering function thread safe but for now this will do
    private void ThreadTask()
    {
        // start the DLL main thread
#if USE_UNITY_DLL_FUNCTION
        main();
#else
        Native.Invoke<main>(nativeLibraryPtr2);
#endif
    }


    // Start is called before the first frame update
    void Start()
    {
        //StartThread();
    }

   public void StartThread()
    {
        // don't start it twice
        if (trd == null)
        {
            // start a thread with the DLL main task
            trd = new Thread(new ThreadStart(this.ThreadTask));
            trd.IsBackground = true;
            trd.Start();
        }
    }

    IEnumerator SayWordCoroutine(string word)
    {
        string upper = word.ToUpper();

        for (int i = 0; i < upper.Length; i++)
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)upper[i]);
#else
            Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, (char)upper[i]);
#endif
            lastKeyboardHit = upper[i];
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.05f);
        }

#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_RETURN);
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, (char)KEYS.VK_RETURN);
#endif
        lastKeyboardHit = (char)KEYS.VK_RETURN;
    }

    public void CommandSayName()
    {
        string word = "Name\n";

        StartCoroutine(SayWordCoroutine(word));
    }

    public static string clickedButtonName = "";

    public void CommandSayButtonName()
    {
        StartCoroutine(SayWordCoroutine(clickedButtonName));
    }

    public void CommandSayWord(string word)
    {
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandA()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, 'A');
#endif
        lastKeyboardHit = 'A';
    }
    public void CommandB()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, 'B');
#endif
        lastKeyboardHit = 'B';
    }
    public void CommandM()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('M');
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, 'M');
#endif
        lastKeyboardHit = 'M';
    }
    public void CommandSayCharacter(char character)
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit(character);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, character);
#endif
        lastKeyboardHit = character;
    }

    public void CommandF()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('F');
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, 'F');
#endif
        lastKeyboardHit = 'F';
    }

    public void CommandContinue()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_RETURN);
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, (char)KEYS.VK_RETURN);
#endif
        lastKeyboardHit = (char)KEYS.VK_RETURN;
    }

    public void CommandBackspace()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_BACK);
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, (char)KEYS.VK_BACK);
#endif
        lastKeyboardHit = (char)KEYS.VK_BACK;
    }

    void OnApplicationQuit()
    {
        // CAUTION: this will cleanly exit the DLL in most cases, however is some areas such as combat this will not work and you will
        // need to kill the unity editor process to be able to restart. The abort function will cause the Unity editor to exit which
        // is not desired. Using the exit() function in the DLL will also exit the unity editor which is also not desired. This
        // was the best solution I could come up with for the moment. In the final app all threads will be exiting when quiting so
        // it is not an issue there, only when using the Unity editor do we have trouble. If Unity would load and unload the DLL at runtime
        // this would be a better solution but Unity 3D unfortantly loads any project DLLs at editor launch and keeps them loaded until you
        // quit the editor.

/* this doesn't work well
try
{
    trd.Abort();
}
catch
{ 
    Debug.Log("error");
}
*/

// signal to the game engine thread to exit all forever loops and return
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_ESCAPE);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_ESCAPE);
#endif

        lastKeyboardHit = (char)KEYS.VK_ESCAPE;

#if !USE_UNITY_DLL_FUNCTION
        // It is now safe to unload the DLL
        if (trd != null)
        {
#if UNITY_EDITOR
            // wait for the game engine thread to complete/return
            while (trd.IsAlive == true)
            {
                ;
            }
#endif

            // It is now safe to unload the DLL
            if (nativeLibraryPtr2 != System.IntPtr.Zero)
            {
                //Debug.Log("Unload AVATAR.DLL");
#if PLATFORM_ANDROID && !UNITY_EDITOR
                Debug.Log(Native.dlclose(nativeLibraryPtr2) == 0
                    ? "Native library successfully unloaded."
                    : "Native library could not be unloaded.");
#else
                Debug.Log(Native.FreeLibrary(nativeLibraryPtr2)
                    ? "Native library successfully unloaded."
                    : "Native library could not be unloaded.");
#endif
            }
        }
#endif
    }

    public enum KEYS
    {
        VK_LEFT = 0x25,
        VK_UP = 0x26,
        VK_RIGHT = 0x27,
        VK_DOWN = 0x28,
        VK_ESCAPE = 0x1B,
        VK_SPACE = 0x20,
        VK_RETURN = 0x0D,
        VK_BACK = 0x08,
        VK_END = 0x23,
        VK_HOME = 0x24
    };

    public System.Text.ASCIIEncoding enc;

    AudioClip[] music = new AudioClip[(int)MUSIC.MAX];
    MUSIC currentMusic;

    // order played in the original intro, from http://www.applevault.com/ultima/
    public enum MUSIC
    {
        TOWNS = 0,
        SHOPPING = 1,
        DUNGEON = 2,
        CASTLES = 3,
        RULEBRIT = 4,
        WANDERER = 5, // OUTSIDE
        COMBAT = 6,
        SHRINES = 7,
        FANFARE = 8,
        MAX = 9
    }

    public AudioClip[] soundEffects = new AudioClip[(int)SOUND_EFFECT.MAX];

    public enum SOUND_EFFECT
    {
        BLOCKED = 0, 
        STARTUP = 1,
        MAX = 2
    };

    public void LoadSongs()
    {
        // TODO: need to combine this with the other one
        for( int i = 0; i < (int)MUSIC.MAX; i++)
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".MP3"))
            {
                StartCoroutine(LoadSongCoroutine(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".MP3", (MUSIC)i));
            }
            else if (System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".OGG"))
            {
                StartCoroutine(LoadSongCoroutine(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".OGG", (MUSIC)i));
            }
        }
    }

    IEnumerator LoadSongCoroutine(string path, MUSIC index)
    {
        string url = string.Format("file://{0}", path);
        //Debug.Log("Load #" + (int)index + " " + url);
        WWW www = new WWW(url);
        yield return www;
        // note the updated interface does not seem to work with local files so don't bother updating until Unity fixes this
        //Debug.Log("Loaded #" + (int)index + " " + url);
        music[(int)index] = www.GetAudioClip(false, false);
    }


    AudioClip CreateBlockedSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;

        float frequency = 104f;

        sampleCount += (float)8 * sampleRate / frequency;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.5f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("blocked", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
     * draw_ultima_logo:
	lda #$ff     ;mode random
	sta blit_mode
	lda #$00
	sta blit_progress
@next_progress:
	lda #$05
	sta src_column
	sta dst_column
	lda #$22
	sta src_row
	sta dst_row
	bit hw_KEYBOARD
	bpl :+
	lda #$38
	sta blit_progress
:	ldx #$1e     ;width of logo, in bytes
	ldy #$2d     ;height of logo, in pixels
	jsr blit_image
	lda blit_progress
	clc
	adc #$01
	sta blit_progress
	cmp #$39
	bcc @next_progress
	rts

blit_image:
	stx blit_width
	sty num_rows
@do_row:
	lda blit_width
	sta num_columns
	ldy src_row
	lda bmplineaddr_lo,y
	sta ptr1_bitmap
	lda bmplineaddr_hi,y
	clc
	adc #$5a     ;ptr1 = BGND
	sta ptr1_bitmap + 1
	ldy dst_row
	lda bmplineaddr_lo,y
	sta ptr2_bitmap
	lda bmplineaddr_hi,y
	sta ptr2_bitmap + 1
	ldy dst_column
@do_column:
	lda (ptr1_bitmap),y
	and #$7f
	beq @next_column
	bit blit_mode
	beq @write_byte
	pha
	lda mask_rand1
	adc #$1d
	tax
	adc mask_rand2
	sta mask_rand1
	stx mask_rand2
	pha
	and #$07
	sta rand_8
	pla
	clc
	adc blit_progress
	bcc :+
	bit hw_SPEAKER
:	lda blit_progress
	clc
	adc rand_8
	tax
	lda blit_mask_table,x
	sta blit_mask
	pla
	and blit_mask
	ora #$80
@write_byte:
	sta (ptr2_bitmap),y
@next_column:
	iny
	dec num_columns
	bne @do_column
	inc src_row
	inc dst_row
	dec num_rows
	bne @do_row
	rts

mask_rand1:
	.byte $35
mask_rand2:
	.byte $9b
blit_width:
	.byte $dc
blit_mode:
	.byte $20
blit_progress:
	.byte $24
rand_8:
	.byte $31
blit_mask:
	.byte $45
blit_mask_table:
	.byte $00,$00,$00,$00,$00,$00,$00,$00
	.byte $00,$01,$02,$04,$08,$12,$20,$21
	.byte $10,$18,$40,$24,$42,$25,$14,$1a
	.byte $48,$29,$14,$52,$54,$55,$4a,$59
	.byte $45,$5a,$2a,$6c,$36,$37,$6a,$67
	.byte $4d,$5e,$39,$6d,$3b,$6f,$57,$7d
	.byte $5d,$7e,$6b,$7b,$77,$ff,$5f,$3f
	.byte $7f,$7f,$7f,$7f,$7f,$7f,$7f,$7f
     * 
     */

    /*
        max1 = 108f;
        min1 = 17f;

        max2 = 5000f;
        min2 = 445f;

        max3 = 7000f;
        min3 = 700f;
     */

    AudioClip CreateStartupSound(float length)
    {
        float sampleRate = 44100;

        int channels = 2;
        int cycles = 0;
        int count = 0;
        float frequencyMin;
        float frequencyMax;
        float frequency;
        float[] data;
        float phase = 0;
        float sampleCount;

        sampleCount = length * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        frequencyMin = 16.995f;
        frequencyMax = 99.936f;

        frequency = Random.Range(frequencyMin, frequencyMax);

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.5f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }

            // switch to a new frequency after so many cycles
            if (count > cycles)
            {
                count = 0;

                frequencyMin = 6E-09f * i * i + 0.0004f * i + 10f;
                frequencyMax = 3E-08f * i * i + 0.001f * i + 100f;

                //frequencyMin = -9E-09f * i * i + 0.0052f * i + 16.995f;
                //frequencyMax = -1E-07f * i * i + 0.0641f * i + 99.936f;

                frequency = Random.Range(frequencyMin, frequencyMax);
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("player hit", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    // Update is called once per frame
    void Update()
    {
        int buffer_index;

        timer += Time.deltaTime;

        // send some keyboard codes down to the engine,
        // Unity keydown is only active for a single frame so it cannot be in the timer check if
  
        if (Input.GetKeyDown(KeyCode.End))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_END);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_END);
#endif
            lastKeyboardHit = (char)KEYS.VK_END;
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_HOME);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_HOME);
#endif
            lastKeyboardHit = (char)KEYS.VK_HOME;
        }
        //else if (Input.GetKeyDown(KeyCode.PageUp))
        //{
        //    main_keyboardHit((char)KEYS.VK_PGUP);
        //}
        //else if (Input.GetKeyDown(KeyCode.PageDown))
        //{
        //    main_keyboardHit((char)KEYS.VK_PGDN);
        //}
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_RETURN);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_RETURN);
#endif
            lastKeyboardHit = (char)KEYS.VK_RETURN;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_ESCAPE);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_ESCAPE);
#endif
            lastKeyboardHit = (char)KEYS.VK_ESCAPE;
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_RETURN);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_RETURN);
#endif
            lastKeyboardHit = (char)KEYS.VK_RETURN;
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_BACK);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_BACK);
#endif
            lastKeyboardHit = (char)KEYS.VK_BACK;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_SPACE);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)KEYS.VK_SPACE);
#endif
            lastKeyboardHit = (char)KEYS.VK_SPACE;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'A');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'A');
#endif
            lastKeyboardHit = 'A';
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'B');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'B');
#endif
            lastKeyboardHit = 'B';
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'C');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'C');
#endif
            lastKeyboardHit = 'C';
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'D');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'D');
#endif
            lastKeyboardHit = 'D';
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'E');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'E');
#endif
            lastKeyboardHit = 'E';
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'F');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'F');
#endif
            lastKeyboardHit = 'F';
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'G');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'G');
#endif
            lastKeyboardHit = 'G';
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'H');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'H');
#endif
            lastKeyboardHit = 'H';
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'I');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'I');
#endif
            lastKeyboardHit = 'I';
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'J');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'J');
#endif
            lastKeyboardHit = 'J';
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'K');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'K');
#endif
            lastKeyboardHit = 'K';
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'L');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'L');
#endif
            lastKeyboardHit = 'L';
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'M');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'M');
#endif
            lastKeyboardHit = 'M';
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'N');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'N');
#endif
            lastKeyboardHit = 'N';
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'O');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'O');
#endif
            lastKeyboardHit = 'O';
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'P');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'P');
#endif
            lastKeyboardHit = 'P';
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Q');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'Q');
#endif
            lastKeyboardHit = 'Q';
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'R');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'R');
#endif
            lastKeyboardHit = 'R';
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'S');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'S');
#endif
            lastKeyboardHit = 'S';
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'T');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'T');
#endif
            lastKeyboardHit = 'T';
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'U');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'U');
#endif
            lastKeyboardHit = 'U';
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'V');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'V');
#endif
            lastKeyboardHit = 'V';
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'W');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'W');
#endif
            lastKeyboardHit = 'W';
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'X');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'X');
#endif
            lastKeyboardHit = 'X';
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Y');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'Y');
#endif
            lastKeyboardHit = 'Y';
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Z');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'Z');
#endif
            lastKeyboardHit = 'Z';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'0');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'0');
#endif
            lastKeyboardHit = '0';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'1');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'1');
#endif
            lastKeyboardHit = '1';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'2');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'2');
#endif
            lastKeyboardHit = '2';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'3');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'3');
#endif
            lastKeyboardHit = '3';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'4');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'4');
#endif
            lastKeyboardHit = '4';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'5');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'5');
#endif
            lastKeyboardHit = '5';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'6');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'6');
#endif
            lastKeyboardHit = '6';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'7');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'7');
#endif
            lastKeyboardHit = '7';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'8');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'8');
#endif
            lastKeyboardHit = '8';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'9');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr2, (char)'9');
#endif
            lastKeyboardHit = '9';
        }

        // check if we just finished playing a sound effect
        if (started_playing_sound_effect == true && specialEffectAudioSource.isPlaying == false)
        {
            // we just finished playing a sound effect
            started_playing_sound_effect = false;

            // let the game engine we finished playing the sound effect
#if USE_UNITY_DLL_FUNCTION
            main_sound_effect_done();
#else
            Native.Invoke<main_sound_effect_done>(U4_Decompiled_TITLE.nativeLibraryPtr2);
#endif
        }

        // TODO move this out of the game engine interface
        // TODO programatically create all the sounds instead of relying on sampled sounds
        // TODO cache sounds already created
        // TODO pre-create basic sfx at startup
        // TODO get source information of sound to better position it for 3D sound, monster, moongate, player, etc. e.g. currently moongate plays at old moongate position not at new position

        // are we currently already playing a sound effect
        if (started_playing_sound_effect == false)
        {
            // get if any sound effects are active from the game engine
#if USE_UNITY_DLL_FUNCTION
            int sound = main_sound_effect();
#else
            int sound = Native.Invoke<int, main_sound_effect>(U4_Decompiled_TITLE.nativeLibraryPtr2);
#endif
            // see if the sound effect from the game engine is valid
            if (sound != -1)
            {
                if (sound == (int)SOUND_EFFECT.BLOCKED)
                {
                    AudioClip clip = CreateBlockedSound();
                    specialEffectAudioSource.PlayOneShot(clip);
                }
                else if (sound == (int)SOUND_EFFECT.STARTUP)
                {
                    AudioClip clip = CreateStartupSound(4f);
                    specialEffectAudioSource.PlayOneShot(clip);
                }
                else
                {
                    specialEffectAudioSource.PlayOneShot(soundEffects[sound]);
                }

                started_playing_sound_effect = true;
            }
        }

        // only get data from the game engine periodically
        if (timer > timerExpired)
        {
            // update the timer
            timer = timer - timerExpired;
            timerExpired = timerPeriod;

            // read the input mode
#if USE_UNITY_DLL_FUNCTION
            inputMode = main_input_mode();
#else
            inputMode = Native.Invoke<INPUT_MODE, main_input_mode>(U4_Decompiled_TITLE.nativeLibraryPtr2);
#endif

            AudioSource musicSource = Camera.main.GetComponent<AudioSource>();
            if (musicSource != null)
            {
                if (musicSource.isPlaying == false)
                {
                    // get the next music clip
                    musicSource.clip = music[(int)currentMusic];

                    // start the music selection
                    musicSource.Play();

                    // get ready for next music
                    currentMusic = (MUSIC)((int)(currentMusic + 1) % (int)MUSIC.MAX);
                }
            }

            // create an ASCII encoder if needed for text processing
            if (enc == null)
            {
                enc = new System.Text.ASCIIEncoding();
            }

            // read the circular buffer from the game engine
#if USE_UNITY_DLL_FUNCTION
            int buffer_size = main_screen_copy_frame(intbuffer, intbuffer.Length);
#else
            int buffer_size = Native.Invoke<int, main_screen_copy_frame>(U4_Decompiled_TITLE.nativeLibraryPtr2, intbuffer, intbuffer.Length);
#endif
            for (int i = 0; i < buffer_size; /* INCREMENTED BELOW */)
            {
                ScreenCopyFrame screenCopyFrame = new ScreenCopyFrame();

                screenCopyFrame.width_in_char = intbuffer[i++];
                screenCopyFrame.height = intbuffer[i++];
                screenCopyFrame.src_x_in_char = intbuffer[i++];
                screenCopyFrame.src_y = intbuffer[i++];
                screenCopyFrame.p = intbuffer[i++];
                screenCopyFrame.dst_y = intbuffer[i++];
                screenCopyFrame.random_stuff = intbuffer[i++];
                screenCopyFrame.dst_x_in_char = intbuffer[i++];

                if ((screenCopyFrame.random_stuff != -1) && (playStartupSoundOnlyOnce == false))
                {
                    if (started_playing_sound_effect == false)
                    {
                        playStartupSoundOnlyOnce = true;

                        AudioClip startupSound = CreateStartupSound(4f);
                        specialEffectAudioSource.PlayOneShot(startupSound);

                        started_playing_sound_effect = true;
                    }
                }

                if ((screenCopyFrame.random_stuff == -1) && (playStartupSoundOnlyOnce == true))
                {
                    specialEffectAudioSource.Stop();
                }

                screenCopyFrameQueue.Enqueue(screenCopyFrame);
            }

            // read the circular buffer from the game engine
#if USE_UNITY_DLL_FUNCTION
            int dot_available = main_dot(intbuffer, intbuffer.Length);
#else
            int dot_available = Native.Invoke<int, main_dot>(U4_Decompiled_TITLE.nativeLibraryPtr2, intbuffer, intbuffer.Length);
#endif
            if (dot_available != 0)
            {
                ScreenDot dot = new ScreenDot();

                dot.x = intbuffer[0];
                dot.y = intbuffer[1];
                dot.color = intbuffer[2];

                screenDotQueue.Enqueue(dot);
            }

// read the circular text buffer from the game engine
#if USE_UNITY_DLL_FUNCTION
            int text_size = main_Text(bytebuffer, bytebuffer.Length);
#else
            int text_size = Native.Invoke<int, main_Text>(U4_Decompiled_TITLE.nativeLibraryPtr2, bytebuffer, bytebuffer.Length);
#endif
            // check if we have any new text to add
            if (text_size != 0)
            {
                // remove the animated whirlpool from the text last character if we have some new text
                if (gameText.Length > 0)
                {
                    gameText = gameText.Remove(gameText.Length - 1);
                }

                // convert the buffer into a string
                string engineText = enc.GetString(bytebuffer, 0, text_size);

                if (engineText.Contains("Welcome"))
                {
                    speaker.VoiceID = ticketTakerVoiceName;
                    speaker.Speak("Welcome friend! Enter in peace and find your path.");
                }
                if (engineText.Contains("You may approach"))
                {
                    speaker.VoiceID = gypsyVoiceName;
                    speaker.Speak("You may approach, O seeker.");
                }
                if (engineText.Contains("We have been waiting"))
                {
                    speaker.VoiceID = gypsyVoiceName;
                    speaker.Speak("We have been waiting such a long time, but at last you have come. Sit here and I shall read the path of your future.");
                }
                if (engineText.Contains("casting"))
                {
                    speaker.VoiceID = gypsyVoiceName;
                    speaker.Speak("Let us begin the casting.");
                }
                if (engineText.Contains("Consider this:"))
                {
                    speaker.VoiceID = gypsyVoiceName;
                    speaker.Speak("Consider this:");
                }
                if (inputMode == INPUT_MODE.A_OR_B_CHOICE)
                {
                    speaker.VoiceID = gypsyVoiceName;

                    string[] sentences = engineText.Split(new char[] { '.', '!', '?' });
                    foreach (string sentence in sentences)
                    {
                        // Clean up the question text before speaking it
                        string adjusted = sentence.Replace("A)", " ");
                        adjusted = adjusted.Replace("B)", " ");
                        adjusted = adjusted.Replace('\n', ' ');
                        adjusted = adjusted.Replace('\r', ' ');
                        adjusted = adjusted.Replace("thee", "<phoneme ph=\"ði\" alphabet=\"ipa\">thee</phoneme>", System.StringComparison.CurrentCultureIgnoreCase); // fix the pronounciation of Thee

                        // Need to queue this text as it comes in as pieces and will interrupt itself otherwise
                        speaker.SpeakQueued(adjusted);
                    }
                }
                if (engineText.Contains("So be it!"))
                {
                    speaker.VoiceID = gypsyVoiceName;
                    speaker.Speak("So be it! Thy path is chosen!");
                }
                if (engineText.Contains("sovereign"))
                {
                    speaker.VoiceID = lordBritishVoiceName;
                    speaker.Speak("Seek the counsel of thy sovereign.");
                }

                // add the ACSII encoded text to the display text plus read the whirlpool character
                gameText = gameText + engineText + (char)(0x1f - ((int)(Time.time * 3) % 4));

                int i;

                // remove any backspace characters
                for (i = 1; i < gameText.Length; i++)
                {
                    // check for a backspace
                    if (gameText[i] == (char)8)
                    {
                        gameText = gameText.Remove(i - 1, 2);
                    }
                }

                // remove all but the last 20 lines of text from the text buffer
                int newline_count = 0;
                const int MAX_NEWLINES = 20;
                for (i = gameText.Length - 1; (i > 0) && (newline_count < MAX_NEWLINES); i--)
                {
                    // check for a newline
                    if (gameText[i] == '\n')
                    {
                        // count the newlines
                        newline_count++;
                    }
                }

                // if we have enough lines cut the string 
                if (newline_count == MAX_NEWLINES)
                {
                    gameText = gameText.Substring(i + 2);
                }
            }

            // animate the spining whirlpool character by removing and adding to the end of the text, the update whirlpool character
            if (gameText.Length > 0)
            {
                gameText = gameText.Remove(gameText.Length - 1) + (char)(0x1f - ((int)(Time.time * 3) % 4));
            }
            else
            {
                gameText = "" + (char)(0x1f  - ((int)(Time.time * 3) % 4));
            }

#if USE_UNITY_DLL_FUNCTION
            int pictureFilenameDest = main_GetPicture(bytebuffer, bytebuffer.Length);
#else
            int pictureFilenameDest = Native.Invoke<int, main_GetPicture>(nativeLibraryPtr2, bytebuffer, bytebuffer.Length);
#endif
            if (pictureFilenameDest != 0)
            {
                LoadPicture loadPicture = new LoadPicture();

                int len;
                for (len = 0; len < 255; len++)
                {
                    if (bytebuffer[len] == 0)
                    {
                        break;
                    }
                }

                loadPicture.dest = pictureFilenameDest;
                loadPicture.filename = enc.GetString(bytebuffer, 0, len);

                loadPictureQueue.Enqueue(loadPicture);
            }
        }

#if USE_UNITY_DLL_FUNCTION
        main_D_3683(bytebuffer, bytebuffer.Length);
#else
        Native.Invoke<main_D_3683>(nativeLibraryPtr2, bytebuffer, bytebuffer.Length);
#endif
        buffer_index = 0;
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 19; x++)
            {
                initialMap[x, y] = (Tile.TILE)bytebuffer[buffer_index++];
            }
        }

#if USE_UNITY_DLL_FUNCTION
        int ret = main_D_3A24(bytebuffer, bytebuffer.Length);
#else
        int ret = Native.Invoke<int, main_D_3A24>(nativeLibraryPtr2, bytebuffer, bytebuffer.Length);
#endif
        bool pendingMapChanged = false;

        if (ret != 0)
        {
            buffer_index = 0;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 19; x++)
                {
                    Tile.TILE tile = (Tile.TILE)bytebuffer[buffer_index++];
                    if (tile != map[x, y])
                    {
                        map[x, y] = tile;
                        pendingMapChanged = true;
                    }
                }
            }
        }

        mapChanged = pendingMapChanged;

#if USE_UNITY_DLL_FUNCTION
        main_D_6976(bytebuffer, bytebuffer.Length);
#else
        Native.Invoke<main_D_6976>(nativeLibraryPtr2, bytebuffer, bytebuffer.Length);
#endif
    }

    bool playStartupSoundOnlyOnce = false;
    public Tile.TILE[,] map = new Tile.TILE[19, 5];
    public bool mapChanged = false;
    public Tile.TILE[,] initialMap = new Tile.TILE[19, 5];
}

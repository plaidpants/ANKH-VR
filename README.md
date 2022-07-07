# ANKH VR
<img src="https://user-images.githubusercontent.com/8979271/177441885-38f6dcca-48a4-4bd2-8031-42c6c518364e.png" width=50% height=50%>

This project runs the original Ultima 4 in VR using the Unity3D game engine. Currently it will run on the Windows PC with an Oculus Rift or Oculus Quest using a link connection but other systems may be supported in the future including potentially running natively on the Oculus Quest. It is functionally complete but there are bugs and minor stuff and improvements to make but I would consider it Beta at this point.

Instructions:

Aquire or purchase a free copy of Ultima 4 from gog.com or other source.

https://www.gog.com/en/game/ultima_4

Install the game on your Windows PC in a folder.

e.g. [GOGLibrary]\Ultima 4 - Quest of the Avatar

Download, rename and place in the ultima 4 install folder the original mockingboard music or other suitable music in mp3 or ogg format.
  
http://www.applevault.com/ultima/

castles.mp3<br />
combat.mp3<br />
dungeon.mp3<br />
fanfare.mp3<br />
rulebrit.mp3<br />
shopping.mp3<br />
shrines.mp3<br />
towns.mp3<br />
wanderer.mp3<br />

or 

https://bitbucket.org/mcmagi/ultima-exodus/downloads/u4-voyd-music-12.zip

Move any game save files you would like to use to the root of the ultima 4 install directory.

MONSTERS.SAV<br />
OUTMONST.SAV<br />
PARTY.SAV<br />

Download the appropriate AVATAR.bps & TITLE.bps patch file for your system from the link below and copy them to the ultima 4 install directory
  
https://github.com/plaidpants/u4-decompiled/releases

Copy the contents of the entire ultima 4 install folder to the following location on your system and then run the ANKH-VR.exe or ANKH-VR.apk (apk not yet available)

Windows PC
  
%APPDATA%\\..\LocalLow\SwivelChairGames\ANKH-VR\u4

  %APPDATA%\\..\LocalLow\SwivelChairGames\ANKH-VR\u4\AVATAR.EXE <- you should have this file at this directory location along with all the other files
  
Oculus Quest (not yet available)
                                                                  
sdcard\Android\data\com.SwivelChairGames.ANKHVR\files\u4
                                                                  
  sdcard\Android\data\com.SwivelChairGames.ANKHVR\files\u4\AVATAR.EXE <- you should have this file at this directory location along with all the other files
                                                                 
This project does not contain any copyrighted game assets or code or text or graphics or fonts or any other intellectual property related to Ultima 4 it relies completely on the files from the original game. This project is not licensed by Origin Systems or Electronic Arts.

![OculusScreenshot1656893311](https://user-images.githubusercontent.com/8979271/177064319-6e233842-107a-498e-929a-6e0cb1e17b65.jpeg)
![OculusScreenshot1656893278](https://user-images.githubusercontent.com/8979271/177064320-18d15813-419d-4daa-863d-d9d1fcf30693.jpeg)
![OculusScreenshot1656894520](https://user-images.githubusercontent.com/8979271/177064324-3a8c583d-f32f-4a5a-88bc-aa18bda29694.jpeg)
![OculusScreenshot1656889680](https://user-images.githubusercontent.com/8979271/177064323-ea84ed0e-3043-4a62-96cf-55fa67598b1b.jpeg)
![OculusScreenshot1657072821](https://user-images.githubusercontent.com/8979271/177451840-c08e55aa-4602-4d8c-a2c4-476e75e67204.jpeg)
![OculusScreenshot1656893632](https://user-images.githubusercontent.com/8979271/177064316-fb6cd808-dde5-4dd8-a53f-146108d803cb.jpeg)
![OculusScreenshot1657072842](https://user-images.githubusercontent.com/8979271/177451701-2f484d69-53af-4281-9016-0544a5177490.jpeg)
![OculusScreenshot1656893808](https://user-images.githubusercontent.com/8979271/177064315-be7f3d3d-d46d-4309-91c3-fa963fcc5979.jpeg)
![OculusScreenshot1656893966](https://user-images.githubusercontent.com/8979271/177064314-64c98e2b-58d2-48f8-8277-8bf2e2963762.jpeg)
![OculusScreenshot1656894019](https://user-images.githubusercontent.com/8979271/177064331-606f73b1-1809-4909-bdfa-b49b5ed6c15f.jpeg)
![OculusScreenshot1657072748](https://user-images.githubusercontent.com/8979271/177451783-b57f93b6-e3f0-40cc-8cc1-5265e3034a2c.jpeg)
![OculusScreenshot1656894104](https://user-images.githubusercontent.com/8979271/177064329-9d30fa02-7516-497a-a554-69456d10be02.jpeg)
![OculusScreenshot1656894287](https://user-images.githubusercontent.com/8979271/177064326-e9cbf252-e53f-45bc-b423-f270f7e88e13.jpeg)
![OculusScreenshot1656894238](https://user-images.githubusercontent.com/8979271/177064328-5a5076e5-4736-4dee-8e09-1c0896211302.jpeg)
![OculusScreenshot1649382760](https://user-images.githubusercontent.com/8979271/163682643-d788f259-0705-452e-8a63-127ce4d9c24f.jpeg)
![OculusScreenshot1649382903](https://user-images.githubusercontent.com/8979271/163682648-7b30c985-67fb-4127-9519-49daafd5cb17.jpeg)
![OculusScreenshot1650589506](https://user-images.githubusercontent.com/8979271/164576777-a6845f7d-7187-40f8-b3e1-670046022fa1.jpeg)

This project make use of the following:<br />
"Effects Pro: Simple Image Effects" from the Unity Asset Store for the magic inverse screen effect, MIT license.<br />
https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/effects-pro-simple-image-effects-191099<br />
Combine textures and meshes for tile map mesh and texture generation, MIT license.<br />
https://forum.unity.com/threads/combine-textures-and-meshes-reduce-draw-calls.117155/<br />
ScreenShakeVR for the tremor effect, License: You can freely use/modify the code.<br />
from here https://www.zulubo.com/gamedev/2019/1/5/vr-screen-shake-the-art-of-not-throwing-up<br />
Zulubo Productions zach.tsiakalis.brown@gmail.com<br />
Wireframe Shader for the highlighted box around the party member in combat, License information http://www.shaderslab.com/index.html free to use without any credits.<br />
http://www.shaderslab.com/demo-94---wireframe-without-diagonal.html<br />
Unlit-AlphaTest shader modified for lighting effect in the dungeon, MIT license<br />
Unity built-in shader source<br />
https://unity3d.com/get-unity/download/archive<br />
Native dll invocation helper by Francis R. Griffiths-Keam to dynamicaly load the patched DLL at startup, license: free to use this code as you please.<br />
http://runningdimensions.com/blog/?p=5<br />
BSP file format used to patch AVATAR.EXE and TITLE.EXE to AVATAR.DLL and TITLE.DLL at startup<br />
https://www.romhacking.net/documents/746/<br />
https://raw.githubusercontent.com/TheAnsarya/bps-patch/master/bps-patch/Decoder.cs (was completely rewritten from scratch for mono for unity)<br />
Flips, free file patching tool, used to create AVATAR.BPS and TITLE.BPS from the modified u4-decompiled binaries and the original AVATAR.EXE and TITLE.EXE<br />
https://github.com/Alcaro/Flips<br />
https://www.smwcentral.net/?p=section&a=details&id=11474<br />
lzw algorithm and file format used by ultima 4<br />
https://ultima4.ultimacodex.com/ultima-4-encodedecode/ (was completely rewritten from scratch in mono for unity)<br />
u4-decompiled, used to create a modified binary DLL instead of the original ultima 4 EXE so we can have unity link and run the original game engine, this project also uses a patch file system to create the DLL from the original EXE at startup so no original intellectual property from utlima 4 must be included in the release. there is quite a bit of magic going on under the hood to be able to make this work. I would like to do the same on the Oculus Quest natively but this may not be possible with the operating system running on the Oculus Quest.<br />
https://github.com/plaidpants/u4-decompiled/<br />
https://github.com/ergonomy-joe/u4-decompiled/<br />
ScatterMyStars for the stars and sky, MIT License<br />
https://github.com/ReCogMission/FirstTutorials<br />

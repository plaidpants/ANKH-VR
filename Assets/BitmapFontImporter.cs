// Originally From https://github.com/BenoitFreslon/BitmapFontImporter/
// Apache License Version 2.0
// modified to remove XML requirement and just use a imported fixed size texture 

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

public static class BitmapFontImporter
{
	
	[MenuItem("Assets/Generate Bitmap Font")]
	public static void GenerateFont ()
	{
		UnityEngine.Object selected = Selection.activeObject;
		string rootPath = Path.GetDirectoryName (AssetDatabase.GetAssetPath (selected));
		
		Texture2D texture = AssetDatabase.LoadAssetAtPath (rootPath + "/" + selected.name + ".png", typeof(Texture2D)) as Texture2D;
		if (!texture)
			throw new UnityException ("Texture2d asset doesn't exist for " + selected.name);
		
		string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension (selected.name);
		
		Work (selected, exportPath, texture);
	}
	
	
	private static void Work (UnityEngine.Object import, string exportPath, Texture2D texture)
	{
		if (!import)
			throw new UnityException (import.name + "is not a valid font-xml file");
		
		
		//XmlDocument xml = new XmlDocument ();
		//xml.LoadXml (import.text);
		
		//XmlNode info = xml.GetElementsByTagName ("info") [0];
        //XmlNode common = xml.GetElementsByTagName("common")[0];
		//XmlNodeList chars = xml.GetElementsByTagName ("chars") [0].ChildNodes;
		
		float texW = texture.width;
		float texH = texture.height;
		
		CharacterInfo[] charInfos = new CharacterInfo[128];
		Rect r;
		
		for (int i=0; i< charInfos.Length; i++) {
				CharacterInfo charInfo = new CharacterInfo ();
			
				charInfo.index = i;
				charInfo.advance = 8;
			
				r = new Rect ();
				r.x = (i % 16) * (8 / texW);
				r.y = (i / 16) * (8 / texH);
				r.width = 8 / texW;
				r.height = 8 / texH;
				r.y = 1f - r.y - r.height;
				charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
				charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
				charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
				charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);
			
			
				r = new Rect ();
				r.x = 0;
				r.y = 0;
				r.width = 8;
				r.height = 8;
				r.y = -r.y;
				r.height = -r.height;
				charInfo.minX = (int)r.xMin;
				charInfo.maxX = (int)r.xMax;
				charInfo.minY = (int)r.yMax;
				charInfo.maxY = (int)r.yMin;
			
				charInfos [i] = charInfo;
		}
		
		// Create material
		Shader shader = Shader.Find ("UI/Default");
		Material material = new Material (shader);
		material.mainTexture = texture;
		AssetDatabase.CreateAsset (material, exportPath + ".mat");
		
		// Create font
		Font font = new Font ();
		font.material = material;
		font.name = "font";
		font.characterInfo = charInfos;

        SerializedObject mFont = new SerializedObject(font);
        mFont.FindProperty("m_FontSize").floatValue = 10.0f;
		mFont.FindProperty("m_LineSpacing").floatValue = 8.0f;

		/* Don't work yet
        int kerningsCount = int.Parse(kernings.Attributes.GetNamedItem("count").InnerText);
        if (kerningsCount > 0)
        {
            SerializedProperty kerningsProp = mFont.FindProperty("m_KerningValues");
            for (int i = 0; i < kerningsCount; i++)
            {
                kerningsProp.InsertArrayElementAtIndex(i);

                XmlNode kerning = kernings.ChildNodes[i];

                SerializedProperty kern = kerningsProp.GetArrayElementAtIndex(i);

                kern.FindPropertyRelative("second").floatValue = float.Parse(kerning.Attributes.GetNamedItem("amount").InnerText); ;
            }
        }*/


		mFont.ApplyModifiedProperties();

		AssetDatabase.CreateAsset (font, exportPath + ".fontsettings");
	}
	
	private static float ToFloat (XmlNode node, string name)
	{
		return float.Parse (node.Attributes.GetNamedItem (name).InnerText);
	}
}
#endif
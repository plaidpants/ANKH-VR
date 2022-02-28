// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "3D Tilemap System/Legacy Unlit Animated"
{
    Properties
    {
        _TilemapTexture ("Tilemap Texture3D", 3D) = "black" {}
        _TilesetTexture ("Tileset Texture3D", 3D) = "black" {}
        _LayerArrayTexture ("Layer Array Texture1D", 2D) = "black" {}
		_WaterAnimationSpeed("Water Animation Speed", Vector) = (0,0,0,0) //EDIT: Add speed here
		_LavaAnimationSpeed("Lava Animation Speed", Vector) = (0,0,0,0) //EDIT: Add speed here
	}
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

			#include "UnityCG.cginc"

            uniform sampler2D _LayerArrayTexture;
            uniform sampler3D _TilemapTexture;
            uniform sampler3D _TilesetTexture;
            uniform int2      _GridMapSize;
            uniform int       _LayersCount;
            uniform int       _TilesCount;
			fixed4 _WaterAnimationSpeed;
			fixed4 _LavaAnimationSpeed;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex     : SV_POSITION;
                float2 tilemap_uv : TEXCOORD0;
                float2 tileset_uv : TEXCOORD1;
            };
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.tilemap_uv = v.texcoord.xy;
                o.tileset_uv = v.texcoord.xy * _GridMapSize.xy;
                return o;
            }
            
            fixed4 frag (v2f IN) : COLOR
            {
                float2 layerData = float2(0, 0);
                float  tilemap = 0;
                fixed4 tileset = fixed4(0, 0, 0, 0);

                fixed4 outputColor = tex3D(_TilesetTexture, float3(IN.tileset_uv, 0));
                float  index = 0;
                float previousLayerCount = 0;
                
                for (int i = 0; i < _LayersCount; i++)
                {
                    // Get layer data
                    // layerData.x = The number of tiles in that layer
                    // layerData.y = The alpha intensity of that layer
                    layerData = tex2D(_LayerArrayTexture, float2(1.0 / (_LayersCount - 1.0) * i, 0)).rg;

                    // Get the tilemap data
                    // Each pixel value of the 3d tilemap texture stores the slice number that the 3d tileset texture should render
                    tilemap = tex3D(_TilemapTexture, float3(IN.tilemap_uv, 1.0 / (_LayersCount - 1) * i)).r * 255;
                    
                    // Compute the right slice index for current layer
                    index = tilemap + previousLayerCount * 255;
                    previousLayerCount += layerData.x;
					
					float tmq = (_Time.y) * 16;
					if (index < 3) 
						tileset = tex3D(_TilesetTexture, float3(IN.tileset_uv + _WaterAnimationSpeed * tmq, 1.0 / (_TilesCount - 1) * index));
					else if ((index == 76) || (index == 71) || (index == 70) || (index == 69) || (index == 68))
						tileset = tex3D(_TilesetTexture, float3(IN.tileset_uv + _LavaAnimationSpeed * tmq, 1.0 / (_TilesCount - 1) * index));
					else
						tileset = tex3D(_TilesetTexture, float3(IN.tileset_uv, 1.0 / (_TilesCount - 1) * index));

                    outputColor = lerp(outputColor, tileset, layerData.y * tileset.a);
                }
                
                return outputColor;
            }
            
            ENDCG
        }
    }
}
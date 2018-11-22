Shader "Custom/NewWater" {
	Properties {
		_Tint("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapDepth ("Normal Map Depth", Range(-1.0 ,40.0)) = 1
        _NoiseTex("Extra Wave Noise", 2D) = "white" {}
        _Speed("Wave Speed", Range(0,5)) = 0.5
        _Amount("Wave Amount", Range(0,5)) = 0.5
        _Height("Wave Height", Range(0,1)) = 0.5
        _Foam("Foamline Thickness", Range(0,5)) = 0.5
		_FoamColor("Foamline colour", Color) = (1, 1, 1, .5)
        _Transparency("Transparency", Range(0.0,1)) = 0.25
        _DistortStrength("- Distortion Strength", Range(0,5)) = 1

        //Diffuse
        [Toggle(USE_DIFFUSE)] _UseDiffuse("Diffuse ON/OFF", Float) = 0
        _DiffuseThresh ("Diffuse Threshold", Range(0,1)) = 0.5

        //Specular
        [Toggle(USE_SPECULAR)] _UseSpecular("Specular ON/OFF", Float) = 0
        _Shininess ("Shininess", Range(0,10)) = 1
        _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)

        //Reflection
        [Toggle(USE_REFLECTION)] _UseReflection("Reflection ON/OFF", Float) = 0
        _ReflectionAmount ("Reflection Amount", Range(0,10)) = 1

        //Refraction
        [Toggle(USE_REFRACTION)] _UseRefraction("Refraction ON/OFF", Float) = 0
        _Cube("Reflection Map", Cube) = "" {}
        _RefractiveIndex("Refractive Index", Range(-10,10)) = 1.5
	}
	SubShader {

        GrabPass{"_BackgroundTexture"}

        Pass
        {
            Tags{"Queue" = "Transparent" "RenderType"="Transparent"}
            CGPROGRAM 
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SPECULAR
            #pragma shader_feature USE_REFRACTION
            #pragma shader_feature USE_REFLECTION
            #pragma shader_feature USE_DIFFUSE
            
           
            #include "UnityCG.cginc"

            uniform float4 _LightColor0;

            uniform float4 _SpecColor;
            uniform float _NormalMapDepth, _DiffuseThresh, _Transparency, _Shininess,  _DistortStrength;

            uniform samplerCUBE _Cube;
            uniform float _ReflectionAmount, _RefractiveIndex;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 scrPos : TEXCOORD1;//
                float4 posWorld : TEXCOORD2;	
                float3 tangentWorld : TEXCOORD4;
                float3 biNormalWorld : TEXCOORD5;
                float3 viewDir : TEXCOORD7;
                float3 lightPos : TEXCOORD8;
                float4 grabPos : TEXCOORD9;
            };
 
            float4 _Tint;
			float4 _FoamColor;
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            sampler2D _MainTex, _NormalMap, _NoiseTex, _BackgroundTexture;//
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _NormalMap_ST;
            float _Speed, _Amount, _Height, _Foam;//
           
            v2f vert (appdata v) {
                v2f o;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject; 
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw,0,0));
                v.vertex.y += (sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height); //movement

                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal                
                o.normal += v.vertex;
                o.lightPos = normalize(_WorldSpaceLightPos0.xyz);
                o.viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
                o.tangentWorld = normalize( mul( unity_ObjectToWorld, v.tangent ).xyz );
                o.biNormalWorld = normalize( cross( o.normal, o.tangentWorld ) * v.tangent.w );                

                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.scrPos = ComputeScreenPos(o.vertex); // grab position on screen
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                float distort = tex2Dlod(_NoiseTex, float4(v.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw,0,0)).yz * _DistortStrength;
                o.grabPos.x -= _DistortStrength/3;
                o.grabPos.xyz += distort;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
           
            fixed4 frag (v2f input) : SV_Target {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, input.uv);
                float4 texN = tex2D(_NormalMap, input.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
                half4 col = _Tint * albedo; // texture times tint;

                //unpackNormal function
                float3 localCoords = float3( 2.0 * texN.ag - float2( 1.0, 1.0 ), 0.0 );
                localCoords.z = _NormalMapDepth;

                //normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    input.tangentWorld,
                    input.biNormalWorld,
                    input.normal
                );
                float3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ) );
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
                float3 lightDirection = _WorldSpaceLightPos0.xyz - input.posWorld.xyz * _WorldSpaceLightPos0.w;

                #ifdef USE_DIFFUSE
				    float4 diffuse = max( dot(normalDirection, normalize( (lightDirection - input.posWorld.xyz) )), 0.0 ) * _DiffuseThresh;
                    col *= diffuse;
                #endif

                #ifdef USE_SPECULAR
                    //float3 specularReflection = _SpecColor.rgb * pow(max(0.0, dot(normalize(reflect(-lightDirection, input.normal)), normalize(input.viewDir))), _Shininess);
                    float3 vert2LightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
                    float oneOverDistance = 1.0 / length(vert2LightSource);
                    float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w);
                    float3 specularReflection;
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-input.lightPos, input.normal), viewDirection)), _Shininess);
                    col *= float4(specularReflection, 1.0);
                #endif

                #ifdef USE_REFLECTION
                    float3 reflectedDir = reflect( normalize(input.viewDir), input.normal);
                    col += texCUBE(_Cube, reflectedDir)  * _ReflectionAmount;
                #endif

                #ifdef USE_REFRACTION
                    float3 refractedDir = refract(normalize(input.viewDir), input.normal, 1.0 / _RefractiveIndex);
                    col *= texCUBE(_Cube, refractedDir);
                #endif

                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.scrPos))); // depth
                half4 foamLine = 1 - saturate(_Foam * (depth - input.scrPos.w)); // foam line by comparing depth and screenposition
                col += foamLine; // add the foam line and tint to the texture				

                col *= _FoamColor;       
                col.a = _Transparency;
                
                return tex2Dproj(_BackgroundTexture, input.grabPos);
            }
            ENDCG
        }

        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SPECULAR
            #pragma shader_feature USE_REFRACTION
            #pragma shader_feature USE_REFLECTION
            #pragma shader_feature USE_DIFFUSE
            
           
            #include "UnityCG.cginc"

            uniform float4 _LightColor0;

            uniform float4 _SpecColor;
            uniform float _NormalMapDepth, _DiffuseThresh, _Transparency, _Shininess;

            uniform samplerCUBE _Cube;
            uniform float _ReflectionAmount, _RefractiveIndex;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 scrPos : TEXCOORD1;//
                float4 posWorld : TEXCOORD2;	
                float3 tangentWorld : TEXCOORD4;
                float3 biNormalWorld : TEXCOORD5;
                float3 viewDir : TEXCOORD7;
                float3 lightPos : TEXCOORD8;
            };
 
            float4 _Tint;
			float4 _FoamColor;
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            sampler2D _MainTex,_NormalMap, _NoiseTex;//
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _NormalMap_ST;
            float _Speed, _Amount, _Height, _Foam;//
           
            v2f vert (appdata v) {
                v2f o;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject; 

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw,0,0));
                v.vertex.y += (sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height); //movement

                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal                
                o.normal += v.vertex;
                o.lightPos = normalize(_WorldSpaceLightPos0.xyz);
                o.viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
                o.tangentWorld = normalize( mul( unity_ObjectToWorld, v.tangent ).xyz );
                o.biNormalWorld = normalize( cross( o.normal, o.tangentWorld ) * v.tangent.w );                

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex); // grab position on screen
                UNITY_TRANSFER_FOG(o,o.vertex);
               
                return o;
            }
           
            fixed4 frag (v2f input) : SV_Target {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, input.uv);
                float4 texN = tex2D(_NormalMap, input.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
                half4 col = _Tint * albedo; // texture times tint;

                //unpackNormal function
                float3 localCoords = float3( 2.0 * texN.ag - float2( 1.0, 1.0 ), 0.0 );
                localCoords.z = _NormalMapDepth;

                //normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    input.tangentWorld,
                    input.biNormalWorld,
                    input.normal
                );
                float3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ) );
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
                float3 lightDirection = _WorldSpaceLightPos0.xyz - input.posWorld.xyz * _WorldSpaceLightPos0.w;

                #ifdef USE_DIFFUSE
				    float4 diffuse = max( dot(normalDirection, normalize( (lightDirection - input.posWorld.xyz) )), 0.0 ) * _DiffuseThresh;
                    col *= diffuse;
                #endif

                #ifdef USE_SPECULAR
                    //float3 specularReflection = _SpecColor.rgb * pow(max(0.0, dot(normalize(reflect(-lightDirection, input.normal)), normalize(input.viewDir))), _Shininess);
                    float3 vert2LightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
                    float oneOverDistance = 1.0 / length(vert2LightSource);
                    float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w);
                    float3 specularReflection;
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-input.lightPos, input.normal), viewDirection)), _Shininess);
                    col *= float4(specularReflection, 1.0);
                #endif

                #ifdef USE_REFLECTION
                    float3 reflectedDir = reflect( normalize(input.viewDir), input.normal);
                    col += texCUBE(_Cube, reflectedDir) * _ReflectionAmount;
                #endif

                #ifdef USE_REFRACTION
                    float3 refractedDir = refract(normalize(input.viewDir), input.normal, 1.0 / _RefractiveIndex);
                    col *= texCUBE(_Cube, refractedDir);
                #endif

                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.scrPos))); // depth
                half4 foamLine = 1 - saturate(_Foam * (depth - input.scrPos.w)); // foam line by comparing depth and screenposition
                col += foamLine; // add the foam line and tint to the texture				

                col *= _FoamColor;       
                col.a = _Transparency;
 
                return col;
            }
            ENDCG
		}

        Pass {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SPECULAR
            #pragma shader_feature USE_REFRACTION
            #pragma shader_feature USE_REFLECTION
            #pragma shader_feature USE_DIFFUSE
            
            #include "UnityCG.cginc"

            uniform float4 _LightColor0;

            uniform float4 _SpecColor;
            uniform float _NormalMapDepth, _DiffuseThresh, _Transparency, _Shininess;

            uniform samplerCUBE _Cube;
            uniform float _ReflectionAmount, _RefractiveIndex;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 scrPos : TEXCOORD1;//
                float4 posWorld : TEXCOORD2;	
                float3 tangentWorld : TEXCOORD4;
                float3 biNormalWorld : TEXCOORD5;
                float3 viewDir : TEXCOORD7;
                float3 lightPos : TEXCOORD8;
            };
 
            float4 _Tint;
			float4 _FoamColor;
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            sampler2D _MainTex,_NormalMap, _NoiseTex;//
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _NormalMap_ST;
            float _Speed, _Amount, _Height, _Foam;//
           
            v2f vert (appdata v) {
                v2f o;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw,0,0));
                v.vertex.y += (sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height); //movement
                
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal                
                o.normal += v.vertex;
                o.lightPos = normalize(_WorldSpaceLightPos0.xyz);
                o.viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
                o.tangentWorld = normalize( mul( unity_ObjectToWorld, v.tangent ).xyz );
                o.biNormalWorld = normalize( cross( o.normal, o.tangentWorld ) * v.tangent.w );                

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex); // grab position on screen
                UNITY_TRANSFER_FOG(o,o.vertex);
               
                return o;
            }
           
            fixed4 frag (v2f input) : SV_Target {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, input.uv);
                float4 texN = tex2D(_NormalMap, input.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
                half4 col = _Tint * albedo; // texture times tint;

                //unpackNormal function
                float3 localCoords = float3( 2.0 * texN.ag - float2( 1.0, 1.0 ), 0.0 );
                localCoords.z = _NormalMapDepth;

                //normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    input.tangentWorld,
                    input.biNormalWorld,
                    input.normal
                );
                float3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ) );
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
                float3 lightDirection = _WorldSpaceLightPos0.xyz - input.posWorld.xyz * _WorldSpaceLightPos0.w;
                
                #ifdef USE_DIFFUSE
				    float4 diffuse = max( dot(normalDirection, normalize( (lightDirection - input.posWorld.xyz) )), 0.0 ) * _DiffuseThresh;
                    col *= diffuse;
                #endif

                #ifdef USE_SPECULAR
                    //float3 specularReflection = _SpecColor.rgb * pow(max(0.0, dot(normalize(reflect(-lightDirection, input.normal)), normalize(input.viewDir))), _Shininess);
                    float3 vert2LightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
                    float oneOverDistance = 1.0 / length(vert2LightSource);
                    float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w);
                    float3 specularReflection;
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-input.lightPos, input.normal), viewDirection)), _Shininess);
                    col *= float4(specularReflection, 1.0);
                #endif

                #ifdef USE_REFLECTION
                    float3 reflectedDir = reflect( normalize(input.viewDir), input.normal);
                    col += texCUBE(_Cube, reflectedDir) * _ReflectionAmount;
                #endif

                #ifdef USE_REFRACTION
                    float3 refractedDir = refract(normalize(input.viewDir), input.normal, 1.0 / _RefractiveIndex);
                    col *= texCUBE(_Cube, refractedDir);
                #endif

                

                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.scrPos))); // depth
                half4 foamLine = 1 - saturate(_Foam * (depth - input.scrPos.w)); // foam line by comparing depth and screenposition
                col += foamLine; // add the foam line and tint to the texture				

                col *= _FoamColor;       
                col.a = _Transparency;
 
                return col;
            }
            ENDCG
        }

        Pass {
            Tags {"LightMode"="ForwardAdd"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SPECULAR
            #pragma shader_feature USE_REFRACTION
            #pragma shader_feature USE_REFLECTION
            #pragma shader_feature USE_DIFFUSE
            
           
            #include "UnityCG.cginc"

            uniform float4 _LightColor0;

            uniform float4 _SpecColor;
            uniform float _NormalMapDepth, _DiffuseThresh, _Transparency, _Shininess;

            uniform samplerCUBE _Cube;
            uniform float _ReflectionAmount, _RefractiveIndex;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 scrPos : TEXCOORD1;//
                float4 posWorld : TEXCOORD2;	
                float3 tangentWorld : TEXCOORD4;
                float3 biNormalWorld : TEXCOORD5;
                float3 viewDir : TEXCOORD7;
                float3 lightPos : TEXCOORD8;
            };
 
            float4 _Tint;
			float4 _FoamColor;
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            sampler2D _MainTex,_NormalMap, _NoiseTex;//
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _NormalMap_ST;
            float _Speed, _Amount, _Height, _Foam;//
           
            v2f vert (appdata v) {
                v2f o;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject; 

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw,0,0));
                v.vertex.y += (sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height); //movement
                
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal                
                o.normal += v.vertex;
                o.lightPos = normalize(_WorldSpaceLightPos0.xyz);
                o.viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
                o.tangentWorld = normalize( mul( unity_ObjectToWorld, v.tangent ).xyz );
                o.biNormalWorld = normalize( cross( o.normal, o.tangentWorld ) * v.tangent.w );                

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex); // grab position on screen
                UNITY_TRANSFER_FOG(o,o.vertex);
               
                return o;
            }
           
            fixed4 frag (v2f input) : SV_Target {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, input.uv);
                float4 texN = tex2D(_NormalMap, input.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
                half4 col = _Tint * albedo; // texture times tint;

                //unpackNormal function
                float3 localCoords = float3( 2.0 * texN.ag - float2( 1.0, 1.0 ), 0.0 );
                localCoords.z = _NormalMapDepth;

                //normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    input.tangentWorld,
                    input.biNormalWorld,
                    input.normal
                );
                float3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ) );
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
                float3 lightDirection = _WorldSpaceLightPos0.xyz - input.posWorld.xyz * _WorldSpaceLightPos0.w;

                #ifdef USE_DIFFUSE
				    float4 diffuse = max( dot(normalDirection, normalize( (lightDirection - input.posWorld.xyz) )), 0.0 ) * _DiffuseThresh;
                    col *= diffuse;
                #endif

                #ifdef USE_SPECULAR
                    //float3 specularReflection = _SpecColor.rgb * pow(max(0.0, dot(normalize(reflect(-lightDirection, input.normal)), normalize(input.viewDir))), _Shininess);
                    float3 vert2LightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
                    float oneOverDistance = 1.0 / length(vert2LightSource);
                    float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w);
                    float3 specularReflection;
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-input.lightPos, input.normal), viewDirection)), _Shininess);
                    col *= float4(specularReflection, 1.0);
                #endif

                #ifdef USE_REFLECTION
                    float3 reflectedDir = reflect( normalize(input.viewDir), input.normal);
                    col += texCUBE(_Cube, reflectedDir) * _ReflectionAmount;
                #endif

                #ifdef USE_REFRACTION
                    float3 refractedDir = refract(normalize(input.viewDir), input.normal, 1.0 / _RefractiveIndex);
                    col *= texCUBE(_Cube, refractedDir);
                #endif


                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.scrPos))); // depth
                half4 foamLine = 1 - saturate(_Foam * (depth - input.scrPos.w)); // foam line by comparing depth and screenposition
                col += foamLine; // add the foam line and tint to the texture				

                col *= _FoamColor;       
                col.a = _Transparency;
 
                return col;
            }

            ENDCG
        }

        Pass {
            Tags {"LightMode"="ForwardAdd"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SPECULAR
            #pragma shader_feature USE_REFRACTION
            #pragma shader_feature USE_REFLECTION
            #pragma shader_feature USE_DIFFUSE
            
           
            #include "UnityCG.cginc"

            uniform float4 _LightColor0;

            uniform float4 _SpecColor;
            uniform float _NormalMapDepth, _DiffuseThresh, _Transparency, _Shininess;

            uniform samplerCUBE _Cube;
            uniform float _ReflectionAmount, _RefractiveIndex;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 scrPos : TEXCOORD1;//
                float4 posWorld : TEXCOORD2;	
                float3 tangentWorld : TEXCOORD4;
                float3 biNormalWorld : TEXCOORD5;
                float3 viewDir : TEXCOORD7;
                float3 lightPos : TEXCOORD8;
            };
 
            float4 _Tint;
			float4 _FoamColor;
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            sampler2D _MainTex,_NormalMap, _NoiseTex;//
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _NormalMap_ST;
            float _Speed, _Amount, _Height, _Foam;//
           
            v2f vert (appdata v) {
                v2f o;

                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject; 
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw,0,0));
                v.vertex.y += (sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height); //movement
                
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal                
                o.normal += v.vertex;
                o.lightPos = normalize(_WorldSpaceLightPos0.xyz);
                o.viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
                o.tangentWorld = normalize( mul( unity_ObjectToWorld, v.tangent ).xyz );
                o.biNormalWorld = normalize( cross( o.normal, o.tangentWorld ) * v.tangent.w );                

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex); // grab position on screen
                UNITY_TRANSFER_FOG(o,o.vertex);
               
                return o;
            }
           
            fixed4 frag (v2f input) : SV_Target {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, input.uv);
                float4 texN = tex2D(_NormalMap, input.uv.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
                half4 col = _Tint * albedo; // texture times tint;

                //unpackNormal function
                float3 localCoords = float3( 2.0 * texN.ag - float2( 1.0, 1.0 ), 0.0 );
                localCoords.z = _NormalMapDepth;

                //normal transpose matrix
                float3x3 local2WorldTranspose = float3x3(
                    input.tangentWorld,
                    input.biNormalWorld,
                    input.normal
                );
                float3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ) );
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
                float3 lightDirection = _WorldSpaceLightPos0.xyz - input.posWorld.xyz * _WorldSpaceLightPos0.w;

                #ifdef USE_DIFFUSE
				    float4 diffuse = max( dot(normalDirection, normalize( (lightDirection - input.posWorld.xyz) )), 0.0 ) * _DiffuseThresh;
                    col *= diffuse;
                #endif

                #ifdef USE_SPECULAR
                    //float3 specularReflection = _SpecColor.rgb * pow(max(0.0, dot(normalize(reflect(-lightDirection, input.normal)), normalize(input.viewDir))), _Shininess);
                    float3 vert2LightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
                    float oneOverDistance = 1.0 / length(vert2LightSource);
                    float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w);
                    float3 specularReflection;
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-input.lightPos, input.normal), viewDirection)), _Shininess);
                    col *= float4(specularReflection, 1.0);
                #endif

                #ifdef USE_REFLECTION
                    float3 reflectedDir = reflect( normalize(input.viewDir), input.normal);
                    col += texCUBE(_Cube, reflectedDir) * _ReflectionAmount;
                #endif

                #ifdef USE_REFRACTION
                    float3 refractedDir = refract(normalize(input.viewDir), input.normal, 1.0 / _RefractiveIndex);
                    col *= texCUBE(_Cube, refractedDir);
                #endif


                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.scrPos))); // depth
                half4 foamLine = 1 - saturate(_Foam * (depth - input.scrPos.w)); // foam line by comparing depth and screenposition
                col += foamLine; // add the foam line and tint to the texture				

                col *= _FoamColor;       
                col.a = _Transparency;
 
                return col;
            }

            ENDCG
        }
	}
}

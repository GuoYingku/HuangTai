Shader "MyShader/Phong"
{
    Properties
    {
        _DiffuseColor("漫反射颜色",color)=(1,1,1,1)
        _SpecularColor("高光颜色",color)=(1,1,1,1)
        _Gloss("高光范围",Range(0,256))=30
        [Toggle]_halfLambert("开启半兰伯特",int)=0
    }
    SubShader
    {
        Pass
        {
            Tags{"LightMode"="ForwardBase"}
            
            CGPROGRAM
 
            #pragma vertex vert 
            #pragma fragment frag 
            #include "UnityCg.cginc"
            #include "Lighting.cginc"
 
            float _Gloss;
           float4 _SpecularColor;
           float4  _DiffuseColor;
            bool _halfLambert;
 
            struct vertexInput 
            {
                float4 vertex:POSITION;
                float3 normal:NORMAL;
            };
 
            struct vertexOutput
            {
                float4 pos:SV_POSITION;
                fixed3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD2;
            };
 
            vertexOutput vert(vertexInput v)
            {
                vertexOutput o=(vertexOutput)0;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.worldNormal=UnityObjectToWorldNormal(v.normal);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
                return o;
            }
            float4 frag(vertexOutput i):SV_TARGET
            {
               fixed3 worldNormal=normalize(i.worldNormal);
               fixed3 worldLightDir=normalize(_WorldSpaceLightPos0.xyz);
 
               float lambert=saturate(dot(worldLightDir,worldNormal));
               float halflambert=dot(worldLightDir,worldNormal)*0.5+0.5;
               fixed3 viewDir=normalize(_WorldSpaceCameraPos.xyz-i.worldPos);
               fixed3 reflectDir=normalize(reflect(-worldLightDir,worldNormal));
 
               fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;
               fixed3 diffuseTemp=_DiffuseColor.rgb*_LightColor0.rgb;
               fixed3 specular=_SpecularColor.rgb*_LightColor0.rgb*pow(saturate(dot(viewDir,reflectDir)),_Gloss);
               
              if(_halfLambert)
               return float4(diffuseTemp*halflambert+specular+ambient,1.0);
               else
               return float4(diffuseTemp*lambert+specular+ambient,1.0);
 
            }
            ENDCG
        }
    }
}
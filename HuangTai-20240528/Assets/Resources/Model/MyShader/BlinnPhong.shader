Shader "MyShader/BlinnPhong"
{
  Properties
  {
    _DiffuseColor("漫反射颜色",color)=(1,1,1,1)
    _SpecluarColor("高光反射颜色",color)=(1,1,1,1)
    _Gloss("高光反射范围",Range(0,256))=30
    [Toggle]_halfLambert("开启半兰伯特",float)=0
  }
  SubShader
  {
    Pass
    {
        Tags{"LightMode"="ForwardBase"}
 
        CGPROGRAM
 
        #pragma vertex vert 
        #pragma fragment frag
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
 
        fixed4 _DiffuseColor;
        fixed4 _SpecluarColor;
        float _Gloss;
        bool _halfLambert;
 
        struct vertexInput
        {
            float4 vertex:POSITION;
            fixed3 normal:NORMAL;
        };
 
        struct vertexOutput
        {
            float4 pos:SV_POSITION;
            fixed3 worldNormal:TEXCOORD0;
            fixed3 worldPos:TEXCOORD1;
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
            fixed3 worldNormal = normalize(i.worldNormal);
            fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
            fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz-i.worldPos.xyz);
            fixed3 hDir=normalize(worldLightDir+viewDir);
 
            float lambert=saturate(dot(worldLightDir,worldNormal));
            float halflambert=dot(worldLightDir,worldNormal)*0.5+0.5;
 
            fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT;
            fixed3 specular = _SpecluarColor.rgb*_LightColor0.rgb*pow(saturate(dot(worldNormal,hDir)),_Gloss);
            fixed3 diffuseTemp = _DiffuseColor.rgb*_LightColor0.rgb;
 
            if(_halfLambert)
            return float4(diffuseTemp*halflambert+ambient+specular,1.0);
            else
            return float4(diffuseTemp*lambert+ambient+specular,1.0);
 
        }
 
        ENDCG
    }
  }
}
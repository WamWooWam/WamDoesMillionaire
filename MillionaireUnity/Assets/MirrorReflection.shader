//Shader(mirror reflection Shader)
Shader "Mirror Reflection" 
{ 
	// Some Properties
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_ReflectionTex ("Reflection", 2D) = "white" { TexGen ObjectLinear }
		_ReflectionColor("ReflectionColor",Color) = (0.3,0.3,0.3,0.3)
	}

	Subshader 
	{ 
		Pass 
		{
			SetTexture[_MainTex] { combine texture }

			SetTexture[_ReflectionTex] 
			{ 
				matrix [_ProjMatrix] 
				combine texture * previous 
			}

			SetTexture[_MainTex]
			{
				ConstantColor[_ReflectionColor]
				combine constant * previous DOUBLE
			}
		}
	}

	Subshader 
	{
		Pass 
		{
			SetTexture [_MainTex] { combine texture }
		}
	}

}

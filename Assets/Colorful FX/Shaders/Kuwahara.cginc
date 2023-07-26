// Colorful FX - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

#ifndef RADIUS
#define RADIUS 3
#endif

#define H3Z half3(0.0, 0.0, 0.0)

sampler2D _MainTex;
half2 _PSize;

inline void loop(int j0, int j1, int i0, int i1, half2 uv, out half3 m, out half3 s)
{
	for (int j = j0; j <= j1; j++)
	{
		for (int i = i0; i <= i1; i++)
		{
			half3 c = tex2Dlod(_MainTex, half4(uv + half2(i, j) * _PSize, 0.0, 0.0)).rgb;
			m += c;
			s += c * c;
		}
	}
}

half4 frag(v2f_img i) : SV_Target
{
	half3 m[4] = { H3Z, H3Z, H3Z, H3Z };
	half3 s[4] = { H3Z, H3Z, H3Z, H3Z };
	
	loop(-RADIUS,      0, -RADIUS,      0, i.uv, m[0], s[0]);
	loop(-RADIUS,      0,       0, RADIUS, i.uv, m[1], s[1]);
	loop(      0, RADIUS,       0, RADIUS, i.uv, m[2], s[2]);
	loop(      0, RADIUS, -RADIUS,      0, i.uv, m[3], s[3]);
	
	half n = half((RADIUS + 1) * (RADIUS + 1));
	half minSigma2 = 1e+2;
	half3 color = H3Z;

	for (int k = 0; k < 4; k++)
	{
		m[k] /= n;
		s[k] = abs(s[k] / n - m[k] * m[k]);

		half sigma2 = s[k].r + s[k].g + s[k].b;

		if (sigma2 < minSigma2)
		{
			minSigma2 = sigma2;
			color = m[k];
		}
	}

	return half4(color, 1.0);
}

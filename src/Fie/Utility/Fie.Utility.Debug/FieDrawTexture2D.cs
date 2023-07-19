using UnityEngine;

namespace Fie.Utility.Debug
{
	public class FieDrawTexture2D
	{
		private Texture2D m_tex;

		private int m_width;

		private int m_height;

		private Color[] m_linesColor;

		public void Begin(Texture2D tex)
		{
			m_tex = tex;
			m_width = m_tex.width;
			m_height = m_tex.height;
			if (m_linesColor == null || m_linesColor.Length != m_width)
			{
				m_linesColor = null;
				m_linesColor = new Color[m_width];
			}
		}

		public void End()
		{
			m_tex.Apply();
		}

		public void Clear(Color col)
		{
			for (int i = 0; i < m_width; i++)
			{
				m_linesColor[i] = col;
			}
			for (int j = 0; j < m_height; j++)
			{
				m_tex.SetPixels(0, j, m_width, 1, m_linesColor);
			}
		}

		public void SetPixel(int x, int y, Color col)
		{
			if (x >= 0 && x < m_width && y >= 0 && y < m_height)
			{
				m_tex.SetPixel(x, y, col);
			}
		}

		private void DrawLineH(int x1, int y1, int x2, Color col)
		{
			if (y1 >= 0 && y1 < m_height)
			{
				if (x1 > x2)
				{
					int num = x1;
					x1 = x2;
					x2 = num;
				}
				if (x2 > 0 && x1 < m_width)
				{
					if (x1 < 0)
					{
						x1 = 0;
					}
					if (x2 >= m_width)
					{
						x2 = m_width - 1;
					}
					for (int i = 0; i <= x2 - x1; i++)
					{
						m_linesColor[i] = col;
					}
					m_tex.SetPixels(x1, y1, x2 - x1 + 1, 1, m_linesColor);
				}
			}
		}

		private void DrawLineV(int x1, int y1, int y2, Color col)
		{
			if (x1 >= 0 && x1 < m_width)
			{
				if (y1 > y2)
				{
					int num = y1;
					y1 = y2;
					y2 = num;
				}
				if (y2 > 0 && y1 < m_height)
				{
					if (y1 < 0)
					{
						y1 = 0;
					}
					if (y2 >= m_height)
					{
						y2 = m_height - 1;
					}
					for (int i = y1; i <= y2; i++)
					{
						m_tex.SetPixel(x1, i, col);
					}
				}
			}
		}

		public void DrawLine(int x1, int y1, int x2, int y2, Color col)
		{
			if (x1 == x2)
			{
				DrawLineV(x1, y1, y2, col);
			}
			else if (y1 == y2)
			{
				DrawLineH(x1, y1, x2, col);
			}
			else if ((x1 >= 0 || x2 >= 0) && (x1 < m_width || x2 < m_width) && (y1 >= 0 || y2 >= 0) && (y1 < m_height || y2 < m_height))
			{
				if (x1 < 0 && x2 >= 0)
				{
					y1 = (y2 - y1) * -x1 / (x2 - x1) + y1;
					x1 = 0;
				}
				else if (x2 < 0 && x1 >= 0)
				{
					y2 = (y2 - y1) * -x1 / (x2 - x1) + y1;
					x2 = 0;
				}
				if (x1 >= m_width && x2 < m_width)
				{
					y1 = (y2 - y1) * (m_width - x1) / (x2 - x1) + y1;
					x1 = m_width - 1;
				}
				else if (x2 >= m_width && x1 < m_width)
				{
					y2 = (y2 - y1) * (m_width - x1) / (x2 - x1) + y1;
					x2 = m_width - 1;
				}
				if (y1 < 0 && y2 >= 0)
				{
					x1 = (x2 - x1) * -y1 / (y2 - y1) + x1;
					y1 = 0;
				}
				else if (y2 < 0 && y1 >= 0)
				{
					x2 = (x2 - x1) * -y1 / (y2 - y1) + x1;
					y2 = 0;
				}
				if (y1 >= m_height && y2 < m_height)
				{
					x1 = (x2 - x1) * (m_height - y1) / (y2 - y1) + x1;
					y1 = m_height - 1;
				}
				else if (y2 >= m_height && y1 < m_height)
				{
					x2 = (x2 - x1) * (m_height - y1) / (y2 - y1) + x1;
					y2 = m_height - 1;
				}
				if (x1 < 0)
				{
					x1 = 0;
				}
				if (x2 < 0)
				{
					x2 = 0;
				}
				if (x1 >= m_width)
				{
					x1 = m_width - 1;
				}
				if (x2 >= m_width)
				{
					x2 = m_width - 1;
				}
				if (y1 < 0)
				{
					y1 = 0;
				}
				if (y2 < 0)
				{
					y2 = 0;
				}
				if (y1 >= m_height)
				{
					y1 = m_height - 1;
				}
				if (y2 >= m_height)
				{
					y2 = m_height - 1;
				}
				int num = Mathf.Abs(x2 - x1);
				int num2 = Mathf.Abs(y2 - y1);
				int num3 = 1;
				int num4 = 1;
				if (x1 > x2)
				{
					num3 = -1;
				}
				if (y1 > y2)
				{
					num4 = -1;
				}
				int num5 = 0;
				if (num > num2)
				{
					int num6 = y1;
					for (int i = x1; i != x2; i += num3)
					{
						num5 += num2;
						if (num5 > num)
						{
							num5 -= num;
							num6 += num4;
						}
						m_tex.SetPixel(i, num6, col);
					}
				}
				else
				{
					int num7 = x1;
					for (int j = y1; j != y2; j += num4)
					{
						num5 += num;
						if (num5 > num2)
						{
							num5 -= num2;
							num7 += num3;
						}
						m_tex.SetPixel(num7, j, col);
					}
				}
				m_tex.SetPixel(x2, y2, col);
			}
		}

		public void DrawRectangle(int x1, int y1, int x2, int y2, Color col)
		{
			DrawLineH(x1, y1, x2, col);
			DrawLineH(x1, y2, x2, col);
			DrawLineV(x1, y1, y2, col);
			DrawLineV(x2, y1, y2, col);
		}

		public void DrawRectangleFill(int x1, int y1, int x2, int y2, Color col)
		{
			if (x1 > x2)
			{
				int num = x1;
				x1 = x2;
				x2 = num;
			}
			if (y1 > y2)
			{
				int num2 = y1;
				y1 = y2;
				y2 = num2;
			}
			if ((x1 >= 0 || x2 >= 0) && (x1 < m_width || x2 < m_width) && (y1 >= 0 || y2 >= 0) && (y1 < m_height || y2 < m_height))
			{
				if (x1 < 0)
				{
					x1 = 0;
				}
				if (x2 >= m_width)
				{
					x2 = m_width - 1;
				}
				if (y1 < 0)
				{
					y1 = 0;
				}
				if (y2 >= m_height - 1)
				{
					y2 = m_height - 1;
				}
				int num3 = x2 - x1 + 1;
				for (int i = 0; i < num3; i++)
				{
					m_linesColor[i] = col;
				}
				for (int j = y1; j <= y2; j++)
				{
					m_tex.SetPixels(x1, j, num3, 1, m_linesColor);
				}
			}
		}
	}
}

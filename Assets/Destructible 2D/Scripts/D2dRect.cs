﻿using UnityEngine;

namespace Destructible2D
{
	[System.Serializable]
	public struct D2dRect
	{
		public int MinX;
		
		public int MaxX;
		
		public int MinY;
		
		public int MaxY;
		
		public bool IsSet
		{
			get
			{
				return MinX != MaxX && MinY != MaxY;
			}
		}
		
		public int SizeX
		{
			get
			{
				return MaxX - MinX;
			}
		}
		
		public int SizeY
		{
			get
			{
				return MaxY - MinY;
			}
		}

		public int Area
		{
			get
			{
				return SizeX * SizeY;
			}
		}

		public static D2dRect CreateFromMinSize(int minX, int minY, int sizeX, int sizeY)
		{
			var o = default(D2dRect);

			if (sizeX > 0 && sizeY > 0)
			{
				o.MinX = minX;
				o.MaxX = minX + sizeX;
				o.MinY = minY;
				o.MaxY = minY + sizeY;
			}

			return o;
		}

		public static D2dRect CalculateOverlap(D2dRect a, D2dRect b)
		{
			b.MinX = Mathf.Clamp(b.MinX, a.MinX, a.MaxX);
			b.MaxX = Mathf.Clamp(b.MaxX, a.MinX, a.MaxX);
			b.MinY = Mathf.Clamp(b.MinY, a.MinY, a.MaxY);
			b.MaxY = Mathf.Clamp(b.MaxY, a.MinY, a.MaxY);
			
			return b;
		}

		public static D2dRect CalculateCombined(D2dRect a, D2dRect b)
		{
			b.MinX = Mathf.Min(b.MinX, a.MinX, a.MaxX);
			b.MaxX = Mathf.Max(b.MaxX, a.MinX, a.MaxX);
			b.MinY = Mathf.Min(b.MinX, a.MinY, a.MaxY);
			b.MaxY = Mathf.Max(b.MaxX, a.MinY, a.MaxY);
			
			return b;
		}
		
		public D2dRect(int newMinX, int newMaxX, int newMinY, int newMaxY)
		{
			if (newMinX < newMaxX && newMinY < newMaxY)
			{
				MinX = newMinX;
				MaxX = newMaxX;
				MinY = newMinY;
				MaxY = newMaxY;
			}
			else
			{
				MinX = 0;
				MaxX = 0;
				MinY = 0;
				MaxY = 0;
			}
		}

		public void Set(int newMinX, int newMaxX, int newMinY, int newMaxY)
		{
			if (newMinX < newMaxX && newMinY < newMaxY)
			{
				MinX = newMinX;
				MaxX = newMaxX;
				MinY = newMinY;
				MaxY = newMaxY;
			}
			else
			{
				MinX = 0;
				MaxX = 0;
				MinY = 0;
				MaxY = 0;
			}
		}
		
		public void Add(int newX, int newY)
		{
			Add(newX, newX + 1, newY, newY + 1);
		}
		
		public void Add(D2dRect rect)
		{
			Add(rect.MinX, rect.MaxX, rect.MinY, rect.MaxY);
		}
		
		public void Add(int newMinX, int newMaxX, int newMinY, int newMaxY)
		{
			if (SizeX == 0)
			{
				MinX = newMinX;
				MaxX = newMaxX;
			}
			else
			{
				if (newMinX < MinX) MinX = newMinX;
				if (newMaxX > MaxX) MaxX = newMaxX;
			}
			
			if (SizeY == 0)
			{
				MinY = newMinY;
				MaxY = newMaxY;
			}
			else
			{
				if (newMinY < MinY) MinY = newMinY;
				if (newMaxY > MaxY) MaxY = newMaxY;
			}
		}
		
		public void Clear()
		{
			MinX = 0;
			MaxX = 0;
			MinY = 0;
			MaxY = 0;
		}

		public void Expand(int size)
		{
			if (IsSet == true)
			{
				MinX -= size;
				MaxX += size;
				MinY -= size;
				MaxY += size;
			}
		}

		public override string ToString()
		{
			return string.Format("(MinX={0}, MaxX={1}, MinY={2}, MaxY={3})", MinX, MaxX, MinY, MaxY);
		}
	}
}
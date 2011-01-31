using System;
using System.Collections.Generic;

namespace NetLib
{
	public abstract class GameObj
	{
		public float UID;//how is this handled?
		public float xPos;
		public float yPos;
		public float xVel;
		public float yVel;
		public int netsize;
		
		public GameObj ()
		{
		}
		
		public abstract List<byte[]> Export();
	}
	
	public class Circle : GameObj
	{
		public float rad;
		
		public Circle(float xP, float yP, float xV, float yV, float r)
		{
			//this is the minimum number of values needed to re-create this class.
			//do we need to hard-code this?
			netsize = 6;
			
			xPos=xP;
			yPos=yP;
			xVel=xV;
			yVel=yV;
			rad=r;
		}
		
		public override List<byte[]> Export()
		{
			List<byte[]> result = new List<byte[]>();
			result.Add(BitConverter.GetBytes(xPos));
			result.Add(BitConverter.GetBytes(yPos));
			result.Add(BitConverter.GetBytes(xVel));
			result.Add(BitConverter.GetBytes(yVel));
			result.Add(BitConverter.GetBytes(rad));
			return result;
		}

	}
	
	public class Player : Circle
	{
		public Player(float xP, float yP, float xV, float yV, float r):base(xP,yP,xV,yV,r)
		{

		}
		
		public const UInt32 netObjType = 0;
	}
	
	public class Enemy : Circle
	{

		public Enemy(float xP, float yP, float xV, float yV, float r):base(xP,yP,xV,yV,r)
		{

		}			
		
		public const UInt32 netObjType = 1;
	}
}


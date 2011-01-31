using System;
namespace NetLib
{
	public abstract class GameObj
	{
		public int xPos;
		public int yPos;
		public int xVel;
		public int yVel;
		
		public GameObj ()
		{
		}
	}
	
	public class Circle : GameObj
	{
		public int rad;
	}
	
	public class Player : Circle
	{
		public Player(int xP, int yP, int xV, int yV, int r)
		{
			xPos=xP;
			yPos=yP;
			xVel=xV;
			yVel=yV;
			rad=r;
		}
		
		public const UInt32 netObjType = 0;
	}
	
	public class Enemy : Circle
	{
		public Enemy(int xP, int yP, int xV, int yV, int r)
		{
			xPos=xP;
			yPos=yP;
			xVel=xV;
			yVel=yV;
			rad=r;
		}
		
		public const UInt32 netObjType = 1;
	}
}


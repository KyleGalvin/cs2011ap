using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public enum Action
{
    Create = 0x60000000,
    Delete = 0x10000000,
    Update = 0x20000000,
    Request = 0x30000000,
    Describe = 0x40000000,
    Text = 0x50000000,
    Identify = 0x70000000
}

public enum Type
{
    Player = 0x09000000,
    AI = 0x01000000,
    Building = 0x03000000,
    Bullet = 0x04000000,
    Explosion = 0x05000000,
    Powerup = 0x06000000,
    Text = 0x07000000,
    Connection = 0x08000000,
    Move = 0x0A000000
}

/// <summary>
/// Can interpret the command and data associated with a packet.
/// </summary>
public class PackageInterpreter
{
    #region Constructors (1)

    public PackageInterpreter()
    {
    }

    #endregion Constructors

    #region Methods (5)

    // Public Methods (4) 

    /// <summary>
    /// Encodes the comm.
    /// </summary>
    /// <param name="a">A.</param>
    /// <param name="t">The t.</param>
    /// <param name="Comm">The comm.</param>
    /// <returns></returns>
    /// Lobby Communication Protocols
    public List<byte[]> encodeComm(Action a, Type t, String Comm)
    {
        Console.WriteLine("Encoding data:" + a.ToString() + " " + t.ToString() + Comm);
        List<byte[]> result = new List<byte[]>();

        //how many 32 bit network numbers do we need to contain the string?
        UInt32 length = (UInt32)Comm.Length / 4;
        if (Comm.Length % 4 != 0)
        {
            length++;
        }

        //add packet header
        result.Add(BitConverter.GetBytes(((length) << 16) ^ ((UInt32)a) ^ ((UInt32)t)));
        Console.WriteLine(BitConverter.ToString(result[0]));

        char[] carray = Comm.ToCharArray();

        UInt32 segment = 0;
        int SegmentLen = 0;
        //add packet body
        for (int i = 0; i < carray.Length / 4; i++)
        {
            if (SegmentLen == 4)
            {
                SegmentLen = 0;
                continue;
            }

            segment = segment ^ ((UInt32)carray[SegmentLen] << (4 * (SegmentLen - 1)));
            SegmentLen++;

            Console.WriteLine("Segment:" + segment.ToString());
            result.Add(BitConverter.GetBytes(segment));
            segment = 0;
        }

        Console.WriteLine("end");
        return result;
    }

    /// <summary>
    /// Encodes the objs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a">A.</param>
    /// <param name="t">The t.</param>
    /// <param name="objs">The objs.</param>
    /// <returns></returns>
    /// Game State Communication Protocols
    public List<byte[]> encodeObjs<T>(Action a, Type t, List<T> objs)
    {

        List<byte[]> result = new List<byte[]>();
        if (a == Action.Identify)
        {
            AP.Player tempPlayer = (AP.Player)(object)objs[0];
            UInt32 header = (UInt32)a ^ (UInt32)t ^ (1 << 16);
            result.Add(BitConverter.GetBytes(header));
            result.Add(BitConverter.GetBytes(tempPlayer.playerId));
        }
        else
        {
            UInt32 count = (UInt32)objs.Count;
            count = count << 16;
            UInt32 header = (UInt32)a ^ (UInt32)t ^ count;
            //Console.WriteLine("HEADER: ACTION: " + (Action)a + " TYPE: " + (Type)t + " COUNT: " + count );
            result.Add(BitConverter.GetBytes(header));

            foreach (T obj in objs)
            {
                result.AddRange(serialize<T>(t, obj));
            }
        }
        return result;
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <returns></returns>
    public UInt32 GetCount(UInt32 header)
    {
        return (header & 0x00FF0000) >> 16;
    }

    //Interpreter seems best suited to place TypeSize information
    /// <summary>
    /// Gets the size of the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public UInt32 GetTypeSize(Type type)
    {

        switch (type)//these values need to be sorted out when the protocol is more sound
        {
            case Type.Player:
                return 0x6;
            case Type.AI:
                return 0x5;
            case Type.Building:
                return 0x5;
            case Type.Bullet:
                return 0x5;
            case Type.Powerup:
                return 0x5;
            case Type.Connection:
                return 0x1;
            case Type.Move:
                return 0x1;
            default:
                return 0x0;
        }
    }
    // Private Methods (1) 

    /// <summary>
    /// Serializes the specified t.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t">The t.</param>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    /// turns a list of objects into a serialized network stream
    private List<byte[]> serialize<T>(Type t, T obj)
    {
        List<byte[]> result = new List<byte[]>();
        Console.WriteLine("Serializing object to send over network...");

        //each type of object has a different composure.
        //here we define the structure of all possible types
        switch (t)
        {
            case Type.AI:
                AP.Enemy e = (AP.Enemy)(object)obj;
                Console.WriteLine("AI-- UID:{0} xPos:{1} yPos:{2} xVel:{3} yVel:{4}", e.UID, e.xPos, e.yPos, e.xVel, e.yVel);
                result.Add(BitConverter.GetBytes(e.UID));
                result.Add(BitConverter.GetBytes(e.xPos));
                result.Add(BitConverter.GetBytes(e.yPos));
                result.Add(BitConverter.GetBytes(e.xVel));
                result.Add(BitConverter.GetBytes(e.yVel));
                break;
            case Type.Bullet:
                if (NetManager.myRole == "server")
                {
                    AP.Bullet b = (AP.Bullet)(object)obj;
                    Console.WriteLine("Bullet-- UID:{0} xPos:{1} yPos:{2} xVel:{3} yVel:{4}", b.UID, b.xPos, b.yPos,b.velocity.X,b.velocity.Y);
                    result.Add(BitConverter.GetBytes((UInt32)b.UID | (UInt32)b.playerID << 30));
                    result.Add(BitConverter.GetBytes(b.xPos));
                    result.Add(BitConverter.GetBytes(b.yPos));
                    result.Add(BitConverter.GetBytes(b.velocity.X));
                    result.Add(BitConverter.GetBytes(b.velocity.Y));
                }
                else
                {
                    AP.Bullet b = (AP.Bullet)(object)obj;
                    Console.WriteLine("Bullet-- UID:{0} xPos:{1} yPos:{2} MouseX:{3} MouseY:{4}", b.UID, b.xPos, b.yPos, b.velocity.X, b.velocity.Y);
                    result.Add(BitConverter.GetBytes((UInt32)b.UID | (UInt32)b.playerID << 30));
                    result.Add(BitConverter.GetBytes(b.xPos));
                    result.Add(BitConverter.GetBytes(b.yPos));
                    result.Add(BitConverter.GetBytes(b.velocity.X));
                    result.Add(BitConverter.GetBytes(b.velocity.Y));
                }
                break;
            case Type.Player:
                AP.Player p = (AP.Player)(object)obj;
                Console.WriteLine("Player-- UID:{0} xPos:{1} yPos:{2} xVel:{3} yVel:{4} health:{5}", p.playerId, p.xPos, p.yPos, p.xVel, p.yVel, p.health);
                result.Add(BitConverter.GetBytes(p.playerId));
                result.Add(BitConverter.GetBytes(p.xPos));
                result.Add(BitConverter.GetBytes(p.yPos));
                result.Add(BitConverter.GetBytes(p.xVel));
                result.Add(BitConverter.GetBytes(p.yVel));
                result.Add(BitConverter.GetBytes(p.health));
                break;
            case Type.Powerup:
                
                AP.Crate c = (AP.Crate)(object)obj;
                Console.WriteLine("Powerup-- UID:{0} PlayerID:{1} xPos:{2} yPos:{3} Type:{4}", c.UID, c.enemyID, c.xPos, c.yPos, c.crateType);
                result.Add(BitConverter.GetBytes(c.UID));
                result.Add(BitConverter.GetBytes(c.enemyID));
                result.Add(BitConverter.GetBytes(c.xPos));
                result.Add(BitConverter.GetBytes(c.yPos));
                result.Add(BitConverter.GetBytes(c.crateType));
                break;
            case Type.Move:
                result.Add(BitConverter.GetBytes((int)(object)obj));
                break;
            default:
                break;
        }
        Console.WriteLine();
        return result;
    }

    #endregion Methods
}



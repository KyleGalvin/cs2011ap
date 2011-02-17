using System;
using AP;
using System.Windows.Forms;

namespace NetLib
{
    public class LobbyPackWorker : PackWorker
    {
        public ListBox list;
        public LobbyPackWorker(ref ListBox list)
        {
            this.list = list;
        }

        public override void HandleDescribe(NetPackage pack)
        {
            if (pack.typeofobj == (UInt32)Type.Building)
            {
                //create game in lobby. pack.body contains game name
                list.Items.Add("dafdf");
            }
            else if (pack.typeofobj == (UInt32)Type.Player)
            {
                //Add player to connections. pack.body contains playername
            }
        }

        public override void HandleRequest(NetPackage pack)
        {
            Console.WriteLine("HERE");
            //join game. pack.body contains game name
            list.Items.Add("PlayerName");
        }

        public override void HandleCreate(NetPackage pack)
        {
            //start game. no body
        }
    }

}
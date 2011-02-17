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

        public void HandleDescribe(NetPackage pack)
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

        public void HandleRequest(NetPackage pack)
        {
            //join game. pack.body contains game name
            list.Items.Add("PlayerName");
        }

        public void HandleCreate(NetPackage pack)
        {
            //start game. no body
        }
    }

}
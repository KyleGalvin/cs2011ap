using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace NetLib
{
	
	class MainClass
	{	
		public static void Main (string[] args)
		{		
			int DefaultPort = 9999;
			
			while(true){
				Console.WriteLine("[c]reate Lobby, [j]oin Lobby, [q]uit");
				string input = Console.ReadLine();

				if (input[0]=='c'){
					LobbyManager myLobby = new LobbyManager(DefaultPort);
					break;
				}else if(input[0]=='j'){
					ClientManager myClient = new ClientManager(DefaultPort);
				}else if(input[0]=='q'){
						break;
				}else{
				}
			}
		}
	}		
}		


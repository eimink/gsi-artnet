using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSGSI;
using CSGSI.Nodes;
using System.Diagnostics;
using ArtNet.Sockets;
using ArtNet.Packets;
using System.Net;

namespace gsi_artnet
{
    class Program
    {
        public static Config config;
        public static ArtNetSocket artNet;
        public static GameStateListener gsl = new GameStateListener(3000);
        public static byte[] dmxChannelData = new byte[9];

        static void Main(string[] args)
        {
            Console.WriteLine("Starting gsi-artnet...");
            if (config == null)
            {
                config = new Config();
                config.Load();
                Console.WriteLine("Config loaded.");
            }
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            InitializeArtNet();
            if (!gsl.Start())
            {
                if (artNet != null)
                {
                    artNet.Close();
                }
                Console.WriteLine("Could not initialize ArtNet! Exiting...");
                Environment.Exit(0);
            }
            Console.WriteLine("Listening for CS:GO GSI data...");
        }

        static void InitializeArtNet()
        {
            Console.WriteLine("Initializing ArtNet.");
            artNet = new ArtNetSocket();
            artNet.EnableBroadcast = true;
            Debug.WriteLine(artNet.BroadcastAddress);
            artNet.Open(IPAddress.Parse(config.ArtNetIP), IPAddress.Parse(config.ArtNetMask));
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            config.Save();
            if (artNet != null)
            {
                artNet.Close();
            }
            gsl.Stop();
        }

        static void OnNewGameState(GameState gs)
        {
            {
                if (gs.Bomb.State == BombState.Planted) dmxChannelData[0] = 255;
                else dmxChannelData[0] = 0;
                if (gs.Bomb.State == BombState.Defusing) dmxChannelData[1] = 255;
                else dmxChannelData[1] = 0;
                if (gs.Bomb.State == BombState.Defused) dmxChannelData[2] = 255;
                else dmxChannelData[2] = 0;
                if (gs.Round.Bomb == BombState.Exploded) dmxChannelData[3] = 255;
                else dmxChannelData[3] = 0;
                if (gs.Round.Phase == RoundPhase.Over)
                {
                    if (gs.Round.WinTeam == RoundWinTeam.CT)
                        dmxChannelData[4] = 255;
                    else
                        dmxChannelData[5] = 255;
                }
                else
                {
                    dmxChannelData[4] = 0;
                    dmxChannelData[5] = 0;
                }
                if (gs.Map.Phase == MapPhase.Live) dmxChannelData[6] = 255;
                else dmxChannelData[6] = 0;
                if (gs.Map.Phase == MapPhase.Intermission) dmxChannelData[7] = 255;
                else dmxChannelData[7] = 0;
                if (gs.Map.Phase == MapPhase.GameOver) dmxChannelData[8] = 255;
                else dmxChannelData[8] = 0;
            }
            SendPacketOverArtNet();
        }

        public static void SendPacketOverArtNet()
        {
            Console.WriteLine("Data: " + BitConverter.ToString(dmxChannelData));
            if (artNet.IsBound)
            {
                ArtNetDmxPacket packet = new ArtNetDmxPacket();
                packet.Universe = config.ArtNetUniverse;
                packet.DmxData = new byte[dmxChannelData.Length];
                Buffer.BlockCopy(dmxChannelData, 0, packet.DmxData, 0, dmxChannelData.Length);
                artNet.Send(packet);
            }
        }
    }
}

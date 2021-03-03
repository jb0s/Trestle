﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking
{
    public class Client
    {
        private readonly Queue<byte[]> _commands = new();
        private readonly AutoResetEvent _resume = new(false);
        internal bool EncryptionEnabled = false;
        public ClientState State = ClientState.Handshaking;
        public Player Player;
        public TcpClient TcpClient;
        public TrestleThreadPool ThreadPool;
        internal int Protocol = 0;
        internal int ClientIdentifier = -1;
        public bool Kicked = false;
        public bool SetCompressionSend = false;
        private long lastPing = 0;
        
        internal byte[] SharedKey { get; set; }
        internal ICryptoTransform Encrypter { get; set; }
        internal ICryptoTransform Decrypter { get; set; }
        internal string ConnectionId { get; set; }
        internal string Username { get; set; }
        public int TeleportTicket { get; set; }

        public Client(TcpClient client)
        {
            TcpClient = client;

            if (client != null)
            {
                ThreadPool = new TrestleThreadPool();
                ThreadPool.LaunchThread(ThreadRun);

                var bytes = new byte[8];
                Globals.Random.NextBytes(bytes);
                ConnectionId = Encoding.ASCII.GetString(bytes).Replace("-", "");
            }
        }
        
        private void ThreadRun()
        {
            while (_resume.WaitOne())
            {
                byte[] command;
                lock (_commands)
                {
                    command = _commands.Dequeue();
                }
                SendData(command);
            }
        }
        
        public void AddToQueue(byte[] data, bool queue = false)
        {
            if (TcpClient != null)
            {
                if (queue)
                {
                    lock (_commands)
                    {
                        _commands.Enqueue(data);
                    }
                    _resume.Set();
                }
                else
                {
                    SendData(data);
                }
            }
        }
        
        public void SendPacket(Packet packet)
        {
            if (TcpClient != null)
            {
                if (packet.GetType().GetCustomAttribute<ClientBoundAttribute>(true) == null)
                    throw new Exception("Packet is not ClientBound");

                packet.Client = this;
                var data = packet.SerializePacket();
                data.FlushData();
            }
        }
        
        public void SendData(byte[] data)
        {
            if (TcpClient != null)
            {
                try
                {
                    if (Encrypter != null)
                    {
                        var toEncrypt = data;
                        data = new byte[toEncrypt.Length];
                        Encrypter.TransformBlock(toEncrypt, 0, toEncrypt.Length, data, 0);

                        var a = TcpClient.GetStream();
                        a.Write(data, 0, data.Length);
                        a.Flush();
                    }
                    else
                        TcpClient.Client.Send(data);
                    //Globals.ClientManager.CleanErrors(this);
                }
                catch(Exception ex)
                {
                    TcpClient.Client.Close();
                }
            }
        }

        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
    }
}
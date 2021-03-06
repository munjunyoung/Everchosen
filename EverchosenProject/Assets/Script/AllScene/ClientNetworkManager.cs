﻿using System.Net.Sockets;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using LitJson;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

//2.0만사용가능 3.5이후로는 호환이 안됨 유니티





namespace Client
{

    static class ClientNetworkManager 
    {
        public static Socket ClientSocket = null;
        private static Socket _serverSocket = null;
        private static readonly byte[] Buffer = new byte[1024];

        // Save device unique id.
        public static string ClientDeviceId;

        public static bool Connected = false;
        public static MatchingPacket PacketData; // 유니티에서 사용할 데이터를 담을변수
        public static ProfileData ProfileData=null;
        
        public static string ReceiveMsg = null; //유니티쪽에서 사용할 메시지를 담을 변수
        public static void ConnectToServer(string hostName, int hostPort)
        {
           
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //연결성공
                ClientSocket.BeginConnect(hostName, hostPort, ConnectCallback, ClientSocket);
            
            }
            catch (Exception e)
            {
                Debug.Log(e);
                //연결실패
               Debug.Log("연결에 실패했습니다.");

            }
        }
        

        //beginReceive 콜백함수
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndConnect(ar);
                _serverSocket = tempSocket;

                if (_serverSocket.Connected == true)
                {
                    Debug.Log("연결됨");
                }
                
                Receive();
                Connected = true;

            }
            catch (Exception e)
            {
                Debug.Log(e);
               Debug.Log("연결에 실패했습니다.2");
            }

        }
        
        //매칭된시간 알려주고 3초뒤에시작한다 . 
        //지금이 19분인데 19분 30초에 매칭시작 33초에 게임시작해 패킷을 보낸다음에 
        //데이터 전송
        public static void Send(string msg, object data)
        {
            try
            {
                if (ClientSocket.Connected)
                {
                    var setData = data;
                    Packet sample = new Packet
                    {
                        MsgName = msg,
                        Data = JsonConvert.SerializeObject(setData)
                    };

                   // Debug.Log();
                    var json = JsonConvert.SerializeObject(sample);
                    var sendData = Encoding.UTF8.GetBytes(json);
                    ClientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, SendCallBack, ClientSocket);
                    
                }
                else
                {
                    //소켓이 연결되어있지않음
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("전송에러");
            }
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            string message = (string)ar.AsyncState; //완료메시지 같은거 보내기..
        }
        

#region receive
        public static void Receive()
        {
            Array.Clear(Buffer, 0, Buffer.Length);
            try
            {
                _serverSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallBack, _serverSocket);
            }
            catch (Exception e)
            {
               Debug.Log("Receive Exeption : "+e);
            }
        }
        
        //수신콜백함수
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                //var tempSocket = (Socket)ar.AsyncState;
               // int readSize = tempSocket.EndReceive(ar);//버퍼 사이즈 받아옴
                if (Buffer[0] == 0)
                {
                    Debug.Log("버퍼에 아무것도 안들어왓음");
                    return;
                }
                var receiveJson = new UTF8Encoding().GetString(Buffer);
                
                var receiveData = JsonConvert.DeserializeObject<Packet>(receiveJson);
                ReceiveMsg = receiveData.MsgName;
                Debug.Log("test : " + receiveData.Data);

                switch (ReceiveMsg)
                {
                    case "OnSucceedLogin":
                        ProfileData = JsonConvert.DeserializeObject<ProfileData>(receiveData.Data);
                        break;
                    case "OnSucceedMatching":
                        PacketData = JsonConvert.DeserializeObject<MatchingPacket>(receiveData.Data);
                        break;
                    case "InGame":
                        break;
                }
                if (_serverSocket.Connected == true)
                {
                    Receive();
                }
                    
            }
            catch (SocketException e)
            {
                Debug.Log("Socket error : " + e);
                //데이저 수신 에러
            }
            catch (Exception e)
            {
                Debug.Log("exeption 에러 : "+e);
            }

            var matchingPakcet = new MatchingPacket("id","tribe",0,1);

            var packetwrapper = new Packet()
            {
                MsgName = "MatchingPacket",
                Data = JsonConvert.SerializeObject(matchingPakcet)
            };
        }
        
        public static void SocketClose()
        {
            if (!ClientSocket.Connected) return;

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }
    }
#endregion
    
    public class Packet
    {
        public string MsgName { get; set; }
        public string Data { get; set; }
    }

    public class ProfileData
    {
        public string NickName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
    }

    public class MatchingPacket
    {
        public string Id { get; set; }
        public string Tribe { get; set;}
        public int Spell { get; set; }
        public int TeamColor { get; set;}
      

        public MatchingPacket(string id, string tribe, int spell, int teamColor)
        {
            this.Id = id;
            this.Tribe = tribe;
            this.Spell = spell;
            this.TeamColor = teamColor;

        }
    }

}

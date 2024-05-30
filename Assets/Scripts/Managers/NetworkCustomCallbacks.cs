using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.AI;
using UnityEngine.UIElements;

public partial class NetworkManager : Manager
{
    // byte     : 0 ~ 255   : 1 byte
    // short    : 0 ~ 65535 : 2 byte
    // int      : -21억 ~21억 : 4 byte
    // long long : -922경 ~ 922경 : 8 byte
    public enum MessageType : byte
    {
        Error, LoadComplete, Walk, Spawn, Teleport, LookAt, Attack
    }

    public struct LoadComplete_Message
    {
        public int current, max; // 지금까지 로드한 개수, 로드해야할 최대 개수
    };

    public struct Spawn_Message
    {
        public float pos_x, pos_y, pos_z;
    }

    public struct Walk_Message
    {
        public float pos_x, pos_y, pos_z;
    }
    public struct Teleport_Message
    {
        public float pos_x, pos_y, pos_z;
    }
    public struct LookAt_Message
    {
        public float pos_x, pos_y, pos_z;
    }

    public struct Attack_Message
    {
        public float pos_x, pos_y, pos_z, rot_x, rot_y, rot_z, scale_x, scale_y, scale_z, duration, damage;
    }

    public static byte[] CreateMessage<T>(MessageType messageType, T container) where T : struct
    {
        // 메시지를 만들 때 최종 바이트 배열의 크기
        // 메시지 타입의 크기를 보고 
        // 그 위에 보낼 정보 크기를 더한다.
        int messageLength = sizeof(MessageType);
        byte[] message = container.ToByteArray();
        messageLength += message.Length;

        // Type : [보내기]
        // message : [h][i]
        // result : [보내기][h][i]
        byte[] result = new byte[messageLength];
        System.Array.Copy(message, 0, result, sizeof(MessageType), message.Length);
        // 일단은 messageType이 1byte라 아래와 같이 가능
        result[0] = (byte)messageType;
        return result;

    }

    public static void SendMessage<T>(MessageType mType, T container) where T: struct
    {
        Backend.Match.SendDataToInGameRoom(CreateMessage(mType, container));
    }

    public static MessageType ReadMessage(byte[] message, out object result, out System.Type type)
    {
        MessageType mType = (MessageType)message[0];

        // 메시지 타입 뒤에 _Message를 붙여 구조체 찾기
        string typeString = $"NetworkManager+{mType}_Message";
        type = System.Type.GetType(typeString);
        //type = typeof(LaodComplete_Message);
        if (type == null)
        {
            UIManager.ClaimError("오류", $"알 수 없는 메시지 타입 : {mType}", "확인", null);
            result = null;
            return MessageType.Error;
        }

        // System.Activator : 대상을 타입을 토대로 새로운 인스턴스를 만들어 줄 수 있다.
        result = System.Activator.CreateInstance(type);

        // 앞 쪽에 메시지 타입 받은것은 뺀다.
        byte[] realMessage = new byte[message.Length - sizeof(MessageType)];

        // 메시지의 앞 부분을 날려버리고 나머지만 저장
        System.Array.Copy(message, sizeof(MessageType), realMessage, 0, realMessage.Length);

        result = realMessage.ToStruct(type);

        return mType;
    }

    void RegistrationCustomCallbacks()
    {
        Backend.Match.OnMatchRelay = (args) =>
        {
            switch(ReadMessage(args.BinaryUserData, out object message, out System.Type messageType))
            {
                case MessageType.LoadComplete:
                    LoadComplete_Message info = (LoadComplete_Message)message;
                    if(info.current == info.max)
                    {
                        UIManager.ClaimError("로드 완료", $"{args.From.NickName} 플레이어 데이터 로드 완료", "확인", null);
                        if(inGameUserInfoDictionary.TryGetValue(args.From.SessionId, out PlayerInfo player))
                        {
                            player.isLoaded = true;
                        }
                    }
                    else
                    {
                        Debug.Log($"{args.From.NickName} 플레이어 데이터 로드중 ({info.current}/{info.max})");
                    }
                    break;
                case MessageType.Spawn:
                    Spawn_Message spawnInfo = (Spawn_Message)message;
                    GetUser(args.From.SessionId).controller.Spawn(spawnInfo.pos_x, spawnInfo.pos_y, spawnInfo.pos_z);
                    break;
                case MessageType.Walk:
                    Walk_Message walkInfo = (Walk_Message)message;
                    GetUser(args.From.SessionId).controller.Walk(walkInfo.pos_x, walkInfo.pos_y, walkInfo.pos_z);
                    break;
                case MessageType.Teleport:
                    Teleport_Message teleportInfo = (Teleport_Message)message;
                    GetUser(args.From.SessionId).controller.Teleport(teleportInfo.pos_x, teleportInfo.pos_y, teleportInfo.pos_z);
                    break;
                case MessageType.LookAt:
                    LookAt_Message lookAtInfo = (LookAt_Message)message;
                    GetUser(args.From.SessionId).controller.LookAt(lookAtInfo.pos_x, lookAtInfo.pos_y, lookAtInfo.pos_z);
                    break;
                case MessageType.Attack:
                    Attack_Message attackInfo = (Attack_Message)message;
                    Vector3 wantPos = GetUser(args.From.SessionId).controller.transform.position;
                    GetUser(args.From.SessionId).controller.Attack(ref attackInfo);
                    break;
                case MessageType.Error:
                default:
                    UIManager.ClaimError("오류", $"알 수 없는 메시지 타입 : {args.BinaryUserData[0]}", "확인", null);
                    break;
            }
        };
    }
}

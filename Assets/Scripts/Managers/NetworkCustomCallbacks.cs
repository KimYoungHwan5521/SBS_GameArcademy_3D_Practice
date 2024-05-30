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
    // int      : -21�� ~21�� : 4 byte
    // long long : -922�� ~ 922�� : 8 byte
    public enum MessageType : byte
    {
        Error, LoadComplete, Walk, Spawn, Teleport, LookAt, Attack
    }

    public struct LoadComplete_Message
    {
        public int current, max; // ���ݱ��� �ε��� ����, �ε��ؾ��� �ִ� ����
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
        // �޽����� ���� �� ���� ����Ʈ �迭�� ũ��
        // �޽��� Ÿ���� ũ�⸦ ���� 
        // �� ���� ���� ���� ũ�⸦ ���Ѵ�.
        int messageLength = sizeof(MessageType);
        byte[] message = container.ToByteArray();
        messageLength += message.Length;

        // Type : [������]
        // message : [h][i]
        // result : [������][h][i]
        byte[] result = new byte[messageLength];
        System.Array.Copy(message, 0, result, sizeof(MessageType), message.Length);
        // �ϴ��� messageType�� 1byte�� �Ʒ��� ���� ����
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

        // �޽��� Ÿ�� �ڿ� _Message�� �ٿ� ����ü ã��
        string typeString = $"NetworkManager+{mType}_Message";
        type = System.Type.GetType(typeString);
        //type = typeof(LaodComplete_Message);
        if (type == null)
        {
            UIManager.ClaimError("����", $"�� �� ���� �޽��� Ÿ�� : {mType}", "Ȯ��", null);
            result = null;
            return MessageType.Error;
        }

        // System.Activator : ����� Ÿ���� ���� ���ο� �ν��Ͻ��� ����� �� �� �ִ�.
        result = System.Activator.CreateInstance(type);

        // �� �ʿ� �޽��� Ÿ�� �������� ����.
        byte[] realMessage = new byte[message.Length - sizeof(MessageType)];

        // �޽����� �� �κ��� ���������� �������� ����
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
                        UIManager.ClaimError("�ε� �Ϸ�", $"{args.From.NickName} �÷��̾� ������ �ε� �Ϸ�", "Ȯ��", null);
                        if(inGameUserInfoDictionary.TryGetValue(args.From.SessionId, out PlayerInfo player))
                        {
                            player.isLoaded = true;
                        }
                    }
                    else
                    {
                        Debug.Log($"{args.From.NickName} �÷��̾� ������ �ε��� ({info.current}/{info.max})");
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
                    UIManager.ClaimError("����", $"�� �� ���� �޽��� Ÿ�� : {args.BinaryUserData[0]}", "Ȯ��", null);
                    break;
            }
        };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.AI;

public partial class NetworkManager : Manager
{
    // byte     : 0 ~ 255   : 1 byte
    // short    : 0 ~ 65535 : 2 byte
    // int      : -21�� ~21�� : 4 byte
    // long long : -922�� ~ 922�� : 8 byte
    public enum MessageType : byte
    {
        Error, LoadComplete, Move, Spawn
    }

    public struct LoadComplete_Message
    {
        public int current, max; // ���ݱ��� �ε��� ����, �ε��ؾ��� �ִ� ����
    };

    public struct Move_Message
    {
        public float pos_x, pos_y, pos_z;
    }

    public struct Spawn_Message
    {
        public float pos_x, pos_y, pos_z;
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
                    Vector3 spawnPos = new Vector3(spawnInfo.pos_x, spawnInfo.pos_y, spawnInfo.pos_z);
                    if (GetUser(args.From.SessionId).testObject != null)
                    {
                        GetUser(args.From.SessionId).testObject.transform.position = spawnPos;

                    }
                    else
                    {
                        GameObject inst = PoolManager.Instanciate(ResourceEnum.Prefab.Player, spawnPos);
                        // �ش��ϴ� �������� �־��
                        GetUser(args.From.SessionId).testObject = inst;

                    }
                    break;
                case MessageType.Move:
                    var agent = GetUser(args.From.SessionId).testObject.GetComponent<NavMeshAgent>();
                    agent.SetDestination(new Vector3(((Move_Message)message).pos_x, ((Move_Message)message).pos_y, ((Move_Message)message).pos_z));
                    break;
                case MessageType.Error:
                default:
                    UIManager.ClaimError("����", $"�� �� ���� �޽��� Ÿ�� : {args.BinaryUserData[0]}", "Ȯ��", null);
                    break;
            }
        };
    }
}

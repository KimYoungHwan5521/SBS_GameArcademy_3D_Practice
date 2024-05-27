using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public partial class NetworkManager : Manager
{
    // byte     : 0 ~ 255   : 1 byte
    // short    : 0 ~ 65535 : 2 byte
    // int      : -21�� ~21�� : 4 byte
    // long long : -922�� ~ 922�� : 8 byte
    public enum MessageType : byte
    {
        LoadComplete
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

    void RegistrationCustomCallbacks()
    {

        Backend.Match.OnMatchRelay = (args) =>
        {
            UIManager.ClaimError("�޽���", args.BinaryUserData.ToString_UTF8(), "Ȯ��", null);
        };
    }
}

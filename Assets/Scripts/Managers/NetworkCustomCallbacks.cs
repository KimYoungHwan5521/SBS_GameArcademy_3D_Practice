using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public partial class NetworkManager : Manager
{
    // byte     : 0 ~ 255   : 1 byte
    // short    : 0 ~ 65535 : 2 byte
    // int      : -21억 ~21억 : 4 byte
    // long long : -922경 ~ 922경 : 8 byte
    public enum MessageType : byte
    {
        LoadComplete
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

    void RegistrationCustomCallbacks()
    {

        Backend.Match.OnMatchRelay = (args) =>
        {
            UIManager.ClaimError("메시지", args.BinaryUserData.ToString_UTF8(), "확인", null);
        };
    }
}

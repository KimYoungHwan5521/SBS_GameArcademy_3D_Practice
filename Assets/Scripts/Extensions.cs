using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// encoding
using System.Text;
using System.Runtime.InteropServices;
using System;

public static class Extensions
{
    // 싱글턴
    // "먼저 온" 사람 승 -> 뒤에 온 사람 파괴
    // "무엇이든지" : 어떤 자료형이든 들어올 수 있어야 한다.
    // int용, float용, monobehaviour용...
    // 어차피 똑같은 역할을 하는 함수라면 자료형만 바꾸면 된다. : <Type>이라고 명시
    // GetComponent<T>()
    //                                                                제일 먼저온 놈을 저장해 논다.
    public static WantType MakeSingleton<WantType>(this WantType target, ref WantType location, bool destroyObject = true)
    {
        // 제일 먼저 왔으면 : 저장
        if(location == null)
        {
            location = target;
        }
        // 누가 이미 와있다 : 파괴
        else
        {
            //                    타겟을 오브젝트인 것처럼 취급
            //                    오브젝트 아니면 -> null 반환
            Component targetComponent = target as Component;

            if(targetComponent)
            {
                GameObject.Destroy(destroyObject ? targetComponent.gameObject : targetComponent);

            }
            else
            {
                UnityEngine.Object targetObject = target as UnityEngine.Object;
                if (targetObject)
                {
                    GameObject.Destroy(targetObject);
                }
            }
        }
        return location;
    }

    public static string GetFileName(this string path)
    {
        if (path == null || path.Length == 0) return "";
        string newPath = "";
        bool findSpace = false;
        for(int i=0; i<path.Length; i++)
        {
            if (path[i] != ' ')
            {
                if(findSpace)
                {
                    newPath += char.ToUpper(path[i]);
                    findSpace= false;
                }
                else
                {
                    newPath += path[i];

                }

            }
            else
            {
                    findSpace= true;

            }
        }

        // string[] splited = newPath.Split("/");
        // return splited[splited.Length - 1];

        // return newPath.Substring(path.LastIndexOf("/") + 1);

        return newPath[(path.LastIndexOf("/") + 1)..];

    }

    public static byte[] ToByteArray_UTF8(this string target)
    {
        return Encoding.UTF8.GetBytes(target);
    }

    public static string ToString_UTF8(this byte[] target)
    {
        return Encoding.UTF8.GetString(target);
    }

    public static byte[] ToByteArray<T>(this T target) where T : struct
    {
        // C# 내장함수를 강제로 끄집어 내
        int size = Marshal.SizeOf(typeof(T));
        byte[] result = new byte[size];

        // 메모리중 하나를 카리키는 포인터 : 여기다가 저장
        // struct를 byte로 바꾸기 위해 저장할 공간을 할당
        IntPtr ptr = Marshal.AllocHGlobal(size);
        // 원본 구조체는 어디에 있나 찾아 ptr에 넣기
        Marshal.StructureToPtr(target, ptr, false);
        // 복사해서 배열에 넣기
        Marshal.Copy(ptr, result, 0, size);
        // 메모리 해제
        Marshal.FreeHGlobal(ptr);
        return result;
    }

    public static T ToStruct<T>(this byte[] target) where T : struct
    {
        // 스트럭트로 바꿔서 형변환 해주는 함수
        return (T)target.ToStruct(typeof(T));

    }

    public static object ToStruct(this byte[] target, System.Type type)
    {
        int size = Marshal.SizeOf(type);
        // 예외처리
        if (size != target.Length)
        {
            throw new InvalidCastException("잘못된 바이트 변환 시도");
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(target, 0, ptr, size);
        object result = Marshal.PtrToStructure(ptr, type);
        Marshal.FreeHGlobal(ptr);
        return result;

    }


}

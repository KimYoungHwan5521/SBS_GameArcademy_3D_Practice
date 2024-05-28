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
    // �̱���
    // "���� ��" ��� �� -> �ڿ� �� ��� �ı�
    // "�����̵���" : � �ڷ����̵� ���� �� �־�� �Ѵ�.
    // int��, float��, monobehaviour��...
    // ������ �Ȱ��� ������ �ϴ� �Լ���� �ڷ����� �ٲٸ� �ȴ�. : <Type>�̶�� ���
    // GetComponent<T>()
    //                                                                ���� ������ ���� ������ ���.
    public static WantType MakeSingleton<WantType>(this WantType target, ref WantType location, bool destroyObject = true)
    {
        // ���� ���� ������ : ����
        if(location == null)
        {
            location = target;
        }
        // ���� �̹� ���ִ� : �ı�
        else
        {
            //                    Ÿ���� ������Ʈ�� ��ó�� ���
            //                    ������Ʈ �ƴϸ� -> null ��ȯ
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
        // C# �����Լ��� ������ ������ ��
        int size = Marshal.SizeOf(typeof(T));
        byte[] result = new byte[size];

        // �޸��� �ϳ��� ī��Ű�� ������ : ����ٰ� ����
        // struct�� byte�� �ٲٱ� ���� ������ ������ �Ҵ�
        IntPtr ptr = Marshal.AllocHGlobal(size);
        // ���� ����ü�� ��� �ֳ� ã�� ptr�� �ֱ�
        Marshal.StructureToPtr(target, ptr, false);
        // �����ؼ� �迭�� �ֱ�
        Marshal.Copy(ptr, result, 0, size);
        // �޸� ����
        Marshal.FreeHGlobal(ptr);
        return result;
    }

    public static T ToStruct<T>(this byte[] target) where T : struct
    {
        // ��Ʈ��Ʈ�� �ٲ㼭 ����ȯ ���ִ� �Լ�
        return (T)target.ToStruct(typeof(T));

    }

    public static object ToStruct(this byte[] target, System.Type type)
    {
        int size = Marshal.SizeOf(type);
        // ����ó��
        if (size != target.Length)
        {
            throw new InvalidCastException("�߸��� ����Ʈ ��ȯ �õ�");
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(target, 0, ptr, size);
        object result = Marshal.PtrToStructure(ptr, type);
        Marshal.FreeHGlobal(ptr);
        return result;

    }


}

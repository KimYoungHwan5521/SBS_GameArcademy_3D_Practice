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

    // ���� ȸ��
    public static Vector3 GetHorizontalRotate(this Vector3 target, float angle)
    {
        // ������ ȸ�� ����
        // ���� ��ȯ ���    ��cosA -sinA����x��
        //                  ��sinA  cosA����z��
        // ��cosA    0   -sinA����x��
        // ��0       1       0����y��
        // ��sinA    0    cosA����z��
        /// x' = xCosA - zSinA
        /// z' = xSinA + zCosA
        float theta = angle * Mathf.Deg2Rad;
        return new Vector3(
            target.x * Mathf.Cos(theta) - target.z * Mathf.Sin(theta),
            target.y,
            target.x * Mathf.Sin(theta) + target.z * Mathf.Cos(theta)
        );
    }

    public static Vector3 GetVerticalRotate(this Vector3 target, float angle)
    {
        float theta = angle * Mathf.Deg2Rad;

        float horizontalLength = Mathf.Sqrt(target.x * target.x + target.z * target.z);
        return new Vector3
        (
        // ��cosA -sinA���� y ��
        // ��sinA  cosA����x,z��
        /// y' = y * CosA - Sqrt(x^2+z^2) * SinA
        /// (x' : z=0, z' : x=0)  = y * SinA + Sqrt(x^2+z^2) * CosA
            target.y * Mathf.Sin(theta) + target.x * Mathf.Cos(theta),
            target.y * Mathf.Cos(theta) - horizontalLength * Mathf.Sin(theta),
            target.y * Mathf.Sin(theta) + target.z * Mathf.Cos(theta)
        );

    }

    // ���򰢵��� ���� -> ���� ��ȯ
    public static Vector3 GetAngledVector(this float horizontalAngle)
    {
        float theta = horizontalAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));
    }

    public static Vector3 GetAngledVector(this float horizontalAngle, float verticalAngle)
    {
        float horizontalTheta = horizontalAngle * Mathf.Deg2Rad;
        float verticalTheta = verticalAngle * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Sin(horizontalTheta) * Mathf.Abs(Mathf.Cos(verticalTheta)),
            Mathf.Sin(horizontalTheta), 
            Mathf.Cos(horizontalTheta) * Mathf.Abs(Mathf.Cos(verticalTheta))
        );
    }

    public static float GetHorizontalAngle(this Vector3 vector)
    {
        // ���� : x, z
        // atan : �̹� ������� ������ ���� ���� ����
        return Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;
    }
    
    public static float GetVerticalAngle(this Vector3 vector)
    {
        // ���� : y x,z
        return Mathf.Atan2(Mathf.Sqrt(vector.z * vector.z + vector.x * vector.x), vector.y) * Mathf.Rad2Deg;
    }


}

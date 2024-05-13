using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : Manager
{
    OptionManager.OptionData loadedOptionData;
    public OptionManager.OptionData LoadedOptionData => loadedOptionData;

    public override IEnumerator Initiate()
    {
        Save<OptionManager.OptionData>($"{Application.persistentDataPath}/SaveData", $"{Application.persistentDataPath}/SaveData/OptionData.json", OptionManager.defaultOptionData);
        loadedOptionData = Load<OptionManager.OptionData>($"{Application.persistentDataPath}/SaveData/OptionData.json", ref OptionManager.defaultOptionData);

        yield return null;
    }

    // ���� �ڷ����̵� �ε��� �� �ִ� �Լ�
    // ���̺�� �ε带 ���׸� �Լ��� ������ֽð�, ���ϴ� ������ �־��ֱ�
    // ����/���е� ���� �ֱ�
    public bool Save<T>(string folderPath, string filePath, T data)
    {
        // 1. �����Ͱ� null ���� Ȯ��
        if (data == null) return false;
        // 2. �̹� �ִ��� Ȯ��
        // ������ �־�� ������ �ִ�.
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(filePath))
        {
            // 3. ������ ����
            // ���� �����ִ� ���¸� �޾ƿ°� -> �ٽ� �ݾƾߵ�
            File.Create(filePath).Close();
        }
        // 4. �����͸� JSON���� ��ȯ
        string jsonData = JsonUtility.ToJson(data);
        // 5. ��ȯ�� �����͸� ���Ͽ� �־���
        File.WriteAllText(filePath, jsonData);
        return true;
    }

    public T Load<T>(string filePath, ref T defaultData)
    {
        /*
         * ���� �˻��� �� ������ Ȯ�εǹǷ� ��������
        // 1. ������ �ִ��� Ȯ��
        if(!Directory.Exists(folderPath))
        {
            // 2. ������ ������ null���� �����ϰ� ����
            return default;
        }
         */
        // 3. ������ �ִ��� Ȯ��
        if(!File.Exists(filePath))
        {
            // 4. ������ ������ null���� �����ϰ� ����
            Debug.LogWarning($"Data Loading Fails : \'{filePath}\' Not Found");
            return defaultData;
        }
        // 5. �����͸� ������ data�� ����
        T data;
        // 6. ������ �����͸� ���� ������ ReadAllText
        string jsonData = File.ReadAllText(filePath);
        // 7. ���� json �����͸� data�� �Ľ�
        data = JsonUtility.FromJson<T>(jsonData);
        // 8. data�� ��ȯ
        return data;
    }

}

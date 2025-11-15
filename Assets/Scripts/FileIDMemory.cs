using UnityEngine;

public class FileIDMemory : MonoBehaviour
{
    private int fileID;

    public void SetFileID(int fileNum)
    {
        fileID = fileNum;
    }

    public int GetFileID()
    {
        return fileID;
    }
}

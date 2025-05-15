using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public static class ArchiveUtilities
{
#if UNITY_EDITOR
    [MenuItem("Debug/Check Archive Compression")]
    static public void CheckCompression()
    {
        string archivePath = EditorUtility.OpenFilePanel("Pick AssetBundle or other Unity Archive file", "", "");
        if (archivePath.Length == 0)
            return;

        Debug.Log($"Bundle {archivePath} has compression type {ReadBytes(archivePath)}");
    }
#endif

    private static string ReadBytes(string archivePath)
    {
        ;
        using (BinaryReader reader = new BinaryReader(File.Open(archivePath, FileMode.Open)))
        {
            var bundleHeaderData = ArchiveHeaderData.CreateFromStream(reader);
            
            Debug.Log("signature: -" + bundleHeaderData.Signature + "-");
            Debug.Log("version: -" + bundleHeaderData.Version + "-");
            Debug.Log("UnityWebBundleVersion: -" + bundleHeaderData.UnityWebBundleVersion + "-");
            Debug.Log("UnityWebMinimumRevision: -" + bundleHeaderData.UnityWebMinimumRevision + "-");
            Debug.Log("Size: -" + bundleHeaderData.Size + "-");
            Debug.Log("CompressedBlocksInfoSize: -" + bundleHeaderData.CompressedBlocksInfoSize + "-");
            Debug.Log("UncompressedBlocksInfoSize: -" + bundleHeaderData.UncompressedBlocksInfoSize + "-");
        }
        return "";
    }
}



public class ArchiveHeaderData
{
    public string Signature;
    public int Version;
    public string UnityWebBundleVersion;
    public string UnityWebMinimumRevision;
    public long Size;
    public int CompressedBlocksInfoSize;
    public int UncompressedBlocksInfoSize;
    public int Flags;
    public static ArchiveHeaderData CreateFromStream(BinaryReader reader)
    {
        ArchiveHeaderData h = new ArchiveHeaderData();
        h.Signature = SerializationUtility.ReadNullTerminatedString(reader);
        h.Version = SerializationUtility.ReadInt32BigEndian(reader);
        h.UnityWebBundleVersion = SerializationUtility.ReadNullTerminatedString(reader);
        h.UnityWebMinimumRevision = SerializationUtility.ReadNullTerminatedString(reader);
        h.Size = SerializationUtility.ReadInt64BigEndian(reader);
        h.CompressedBlocksInfoSize = SerializationUtility.ReadInt32BigEndian(reader);
        h.UncompressedBlocksInfoSize = SerializationUtility.ReadInt32BigEndian(reader);
        h.Flags = SerializationUtility.ReadInt32BigEndian(reader);
        return h;
    }
}

class SerializationUtility
{
    public static string ReadNullTerminatedString(BinaryReader r)
    {
        StringBuilder builder = new StringBuilder();
        while (true)
        {
            byte b = r.ReadByte();
            if (b == 0)
                break;
            builder.Append((char)b);
        }
        return builder.ToString();
    }
    public static int ReadInt32BigEndian(BinaryReader r)
    {
        byte[] b = r.ReadBytes(4);
        System.Array.Reverse(b);
        return System.BitConverter.ToInt32(b);
    }
    public static long ReadInt64BigEndian(BinaryReader r)
    {
        byte[] b = r.ReadBytes(8);
        System.Array.Reverse(b);
        return System.BitConverter.ToInt64(b);
    }
}
using System.IO;
using Unity.Content;
using Unity.IO.Archive;
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

    static public UnityEngine.CompressionType GetArchiveCompression(string archivePath)
    {
        var archiveHandle = ArchiveFileInterface.MountAsync(ContentNamespace.Default, archivePath, "temp:");
        archiveHandle.JobHandle.Complete();

        if (archiveHandle.Status == ArchiveStatus.Failed)
            throw new System.ArgumentException($"Failed to load {archivePath}");

        var compression = archiveHandle.Compression;
        archiveHandle.Unmount().Complete();
        return compression;
    }

    private static string ReadBytes(string archivePath)
    {
        ;
        using (BinaryReader reader = new BinaryReader(File.Open(archivePath, FileMode.Open)))
        {
            var signature = reader.ReadBytes(7);
            Debug.Log("signature: " + signature);
            
            var version = reader.ReadUInt32();
            Debug.Log("version: " + version);
            
            var webVersion = reader.ReadString();
            Debug.Log("webVersion: " + webVersion);
            
            var editorVersion = reader.ReadString();
            Debug.Log("editorVersion: " + editorVersion);
            
            var bundleSize = reader.ReadUInt64();
            Debug.Log("bundleSize: " + bundleSize);
            
            var compressedSize = reader.ReadUInt64();
            Debug.Log("compressedSize: " + compressedSize);
            
            var uncompressedSize = reader.ReadUInt64();
            Debug.Log("uncompressedSize: " + uncompressedSize);
        }
        return "";
    }
}
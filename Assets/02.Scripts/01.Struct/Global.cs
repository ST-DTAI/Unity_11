
using System.Collections.Generic;

public static class Global
{
    public static float UnityCorrectValue = 0.001f;

    public static string host = "192.168.0.4";
    public static string database = "clts_unity";
    public static string user = "clts";
    public static string password = "clts";
    public static string charset = "utf8";

    public enum DbLockType
    {
        CRSTATUS,
        DOORSTATE,
        YARDMAP,
        Count
    }
    public static readonly object[] dbLocks = new object[(int)DbLockType.Count]
    {
        new object(),
        new object(),
        new object() 
    };
    public static List<CrStatus> CrStatusList = new List<CrStatus>();
    public static Dictionary<string, int> DoorStateDict = new Dictionary<string, int>();
    public static List<YardMap> YardMapList = new List<YardMap>();
    public static List<float> DongSpacing = new List<float>();
}
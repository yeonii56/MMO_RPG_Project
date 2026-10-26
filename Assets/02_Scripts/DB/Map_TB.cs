using System.Collections.Generic;

public class Map_TB
{
    public Dictionary<int, MapList> MapList = new Dictionary<int, MapList>();
    public Dictionary<int, PotalList> PotalList = new Dictionary<int, PotalList>();
    public Dictionary<int, Monster> Monster = new Dictionary<int, Monster>();
}

public class MapList
{
    public int Idx { get; init; }
    public string Prefab { get; init; }
    public string MapData { get; init; }
    public string Scene { get; init; }
}

public class PotalList
{
    public int Idx { get; init; }
    public int PosX { get; init; }
    public int PosY { get; init; }
    public int MapIdx { get; init; }
    public int MovePotal { get; init; }
}

public class Monster
{
    public int Idx { get; init; }
    public string Prefab { get; init; }
    public int PosX { get; init; }
    public int PosY { get; init; }
    public int MapIdx { get; init; }
    public int Count { get; init; }
}

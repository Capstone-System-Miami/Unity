using System;

//Made By Antony

[Flags]
public enum ExitDirection
{
    None = 0,
    North = 1 << 0,
    South = 1 << 1,
    East = 1 << 2, 
    West = 1 << 3  
}


public enum IntersectionType
{
    None = 0,
    N = ExitDirection.North,                                                                    // 1
    S = ExitDirection.South,                                                                    // 2
    E = ExitDirection.East,                                                                     // 4
    W = ExitDirection.West,                                                                     // 8
    NS = ExitDirection.North | ExitDirection.South,                                             // 3
    NE = ExitDirection.North | ExitDirection.East,                                              // 5
    NW = ExitDirection.North | ExitDirection.West,                                              // 9
    SE = ExitDirection.South | ExitDirection.East,                                              // 6
    SW = ExitDirection.South | ExitDirection.West,                                              // 10
    EW = ExitDirection.East | ExitDirection.West,                                               // 12
    NSE = ExitDirection.North | ExitDirection.South | ExitDirection.East,                       // 7
    NSW = ExitDirection.North | ExitDirection.South | ExitDirection.West,                       // 11
    NEW = ExitDirection.North | ExitDirection.East | ExitDirection.West,                        // 13
    SEW = ExitDirection.South | ExitDirection.East | ExitDirection.West,                        // 14
    NSEW = ExitDirection.North | ExitDirection.South | ExitDirection.East | ExitDirection.West  // 15
    
}
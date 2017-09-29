using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Commander {

    public static MiniGameLegacy[] Minigames;

    public static Player[] Players;

	public const int VICTORY_COUNT = 2;
    
    static Commander()
    {
        Players = new Player[]{
            new Player("いちたろう",1),
            new Player("じろう",2),
        };

        Minigames = new MiniGameLegacy[]{
			new MiniGameLegacy("Love Triangle(終了しない不具合あり Alt+F4で終了)","MiniGames/LoveTriangle/LoveTriangle.exe"),
			new MiniGameLegacy("ネタざんまい","MiniGames/NetaZanmai/NetaZanmai.exe"),
			new MiniGameLegacy("ESCAPE SPY","MiniGames/ESCAPESPY/ESCAPESPY.exe"),
			new MiniGameLegacy("Beach Gun Dash","MiniGames/BeachGunDash/BeachGunDash.exe"),
        };
    }
}

[System.Obsolete()]
public class MiniGameLegacy
{
    public string name;
    public string path;

    public MiniGameLegacy(string name,string path)
    {
        this.name = name;
        this.path = path;
    }
}

public class Player
{
    public string name;
    public int characterId;
    public int point = 0;

    public Player(string name,int characterId)
    {
        this.name = name;
        this.characterId = characterId;
    }
}

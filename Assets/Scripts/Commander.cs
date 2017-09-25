using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Commander {

    public static MiniGame[] Minigames;

    public static Player[] Players;
    
    static Commander()
    {
        Players = new Player[]{
            new Player("いちたろう",1),
            new Player("じろう",2),
        };

        Minigames = new MiniGame[]{
            new MiniGame("Love Triangle(終了しない不具合あり Alt+F4で終了)","MiniGames/LoveTriangle/LoveTriangle.exe"),
            new MiniGame("ネタざんまい","MiniGames/NetaZanmai/NetaZanmai.exe"),
            new MiniGame("ESCAPE SPY","MiniGames/ESCAPESPY/ESCAPESPY.exe"),
            new MiniGame("Beach Gun Dash","MiniGames/BeachGunDash/BeachGunDash.exe"),
        };
    }
}

public class MiniGame
{
    public string name;
    public string path;

    public MiniGame(string name,string path)
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

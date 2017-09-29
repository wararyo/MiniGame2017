using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Commander {

    public static Player[] Players;

    public static List<MiniGame> PlayedMiniGames;

	public const int VICTORY_COUNT = 2;
    
    static Commander()
    {
        Players = new Player[]{
            new Player("いちたろう",1),
            new Player("じろう",2),
        };

        PlayedMiniGames = new List<MiniGame>();

    }

    public static void Initialize()
    {
        PlayedMiniGames.Clear();
        Players[0].point = 0;
        Players[1].point = 0;
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

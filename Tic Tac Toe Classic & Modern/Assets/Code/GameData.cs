public enum GameMode { Classic, Modern }
public enum OpponentType { VsAI, LocalTwoPlayers, LANNetwork }
public enum AIDifficulty { Easy, Medium, Hard }
public enum PlayerSymbol { X, O }

public static class GameData
{
    public static GameMode SelectedMode = GameMode.Classic;
    public static OpponentType SelectedOpponent = OpponentType.VsAI;
    public static AIDifficulty SelectedDifficulty = AIDifficulty.Easy;
    public static PlayerSymbol SelectedSymbol = PlayerSymbol.X;
    public static int TotalRounds = 1;
}
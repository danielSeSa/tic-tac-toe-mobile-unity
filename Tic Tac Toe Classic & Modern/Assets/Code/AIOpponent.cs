using UnityEngine;
using System.Collections.Generic;

public static class AIOpponent
{
    public static int GetBestMove(string[] board, string aiSymbol, string humanSymbol, AIDifficulty difficulty)
    {
        List<int> emptyCells = GetEmptyCells(board);
        if (emptyCells.Count == 0) return -1;

        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return GetRandomMove(emptyCells);

            case AIDifficulty.Medium:
                return GetMediumMove(board, emptyCells, aiSymbol, humanSymbol);

            case AIDifficulty.Hard:
                return GetHardMove(board, emptyCells, aiSymbol, humanSymbol);

            default:
                return GetRandomMove(emptyCells);
        }
    }

    private static int GetRandomMove(List<int> emptyCells)
    {
        int randomIndex = Random.Range(0, emptyCells.Count);
        return emptyCells[randomIndex];
    }

    private static int GetMediumMove(string[] board, List<int> emptyCells, string aiSymbol, string humanSymbol)
    {
        int winningMove = FindWinningMove(board, emptyCells, aiSymbol);
        if (winningMove != -1) return winningMove;

        int blockMove = FindWinningMove(board, emptyCells, humanSymbol);
        if (blockMove != -1) return blockMove;

        return GetRandomMove(emptyCells);
    }

    private static int GetHardMove(string[] board, List<int> emptyCells, string aiSymbol, string humanSymbol)
    {
        int winningMove = FindWinningMove(board, emptyCells, aiSymbol);
        if (winningMove != -1) return winningMove;

        int blockMove = FindWinningMove(board, emptyCells, humanSymbol);
        if (blockMove != -1) return blockMove;

        if (emptyCells.Contains(4)) return 4;

        List<int> corners = new List<int> { 0, 2, 6, 8 };
        List<int> availableCorners = corners.FindAll(c => emptyCells.Contains(c));
        if (availableCorners.Count > 0)
        {
            return availableCorners[Random.Range(0, availableCorners.Count)];
        }

        return GetRandomMove(emptyCells);
    }

    private static int FindWinningMove(string[] board, List<int> emptyCells, string symbol)
    {
        foreach (int cellIndex in emptyCells)
        {
            board[cellIndex] = symbol;
            if (CheckWinForSymbol(board, symbol))
            {
                board[cellIndex] = ""; 
                return cellIndex;
            }
            board[cellIndex] = ""; 
        }
        return -1;
    }

    private static bool CheckWinForSymbol(string[] board, string symbol)
    {
        int[][] winConditions = new int[][]
        {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8},
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8},
            new int[] {0, 4, 8}, new int[] {2, 4, 6}
        };

        foreach (var condition in winConditions)
        {
            if (board[condition[0]] == symbol &&
                board[condition[1]] == symbol &&
                board[condition[2]] == symbol)
            {
                return true;
            }
        }
        return false;
    }

    private static List<int> GetEmptyCells(string[] board)
    {
        List<int> empty = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i])) empty.Add(i);
        }
        return empty;
    }
}
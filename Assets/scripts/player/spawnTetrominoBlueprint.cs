using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class spawnTetrominoBlueprint : MonoBehaviour
{
    public abstract void spawnTetromino();

    
    protected abstract void spawnTetrominoInCorrectLocation(GameObject currentTetromino);

    protected void tetrominoSelection(GameObject[] arrayOfTetrominoes, Queue<GameObject> queueOfNextTetrominoes, char[] pieces)
    {
        System.Random rndNumb = new System.Random();
        int chosenTetromino = rndNumb.Next(0, pieces.Length);
        if (pieces[chosenTetromino] != '8')
        {
            enqueTetromino(pieces[chosenTetromino]);
        }
        else
        {
            yateShuffle(pieces);
            // recursive until the base case of not being an 8 occurs
            tetrominoSelection(arrayOfTetrominoes, queueOfNextTetrominoes ,pieces);
        }
    }

    protected char[] yateShuffle(char[] pieces)
    {
        // 1 = I, 2 = J, 3 = L, 4 = O, 5 = S, 6 = T, 7 = Z, 8 = reroll
        for (int i = 0; i < pieces.Length; i++)
        {
            // System.Random used instead of Unity's random
            System.Random rnd = new System.Random();
            int rndNumb = rnd.Next(i, pieces.Length - 1);
            char pieceToSwap = pieces[i];
            char pieceToSwap2 = pieces[rndNumb];
            pieces[i] = pieceToSwap2;
            pieces[rndNumb] = pieceToSwap;
        }
        return pieces;
    }

    protected abstract void enqueTetromino(char tetromino);
}
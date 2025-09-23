#include <stdio.h>
#include <stdbool.h>

#define N 9

bool isSafe(int board[N][N], int row, int col, int num);
bool solveSudoku(int board[N][N]);
void printBoard(int board[N][N]);

int main()
{
      int board[N][N] = {
        {5, 3, 0, 0, 7, 0, 0, 0, 0},
        {6, 0, 0, 1, 9, 5, 0, 0, 0},
        {0, 9, 8, 0, 0, 0, 0, 6, 0},
        {8, 0, 0, 0, 6, 0, 0, 0, 3},
        {4, 0, 0, 8, 0, 3, 0, 0, 1},
        {7, 0, 0, 0, 2, 0, 0, 0, 6},
        {0, 6, 0, 0, 0, 0, 2, 8, 0},
        {0, 0, 0, 4, 1, 9, 0, 0, 5},
        {0, 0, 0, 0, 8, 0, 0, 7, 9}
    };

    printf("Sudoku Puzzle (Initial):\n");
    printBoard(board);

    if(solveSudoku(board)){
        printf("\nSudoku Solution\n");
        printBoard(board);
    } else {
        printf("\nNo solution exists.\n");
    }


    return 0;
}

bool isSafe(int board[N][N], int row, int col, int num)
{
    for (int x = 0; x < N; x++)
    {
        if (board[row][x] == num)
        {
            return false;
        }
    }

    for (int x = 0; x < N; x++)
    {
        if (board[x][col] == num)
        {
            return false;
        }
    }

    int startRow = row - row % 3, startCol = col - col % 3;
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            if (board[i + startRow][j + startCol] == num)
            {
                return false;
            }
        }
    }
    return true;
}

bool solveSudoku(int board[N][N])
{
    int row, col;
    bool emptyCellFound = false;

    for (row = 0; row < N; row++)
    {
        for (col = 0; col < N; col++)
        {
            if (board[row][col] == 0)
            {
                emptyCellFound = true;
                break;
            }
        }
        if (emptyCellFound)
        {
            break;
        }
    }
    if (!emptyCellFound)
    {
        return true;
    }

    for (int num = 1; num <= N; num++)
    {
        if (isSafe(board, row, col, num))
        {
            board[row][col] = num;

            if (solveSudoku(board))
            {
                return true;
            }
            board[row][col] = 0;
        }
    }
    return false;
}

void printBoard(int board[N][N]) {
    for(int i = 0; i < N; i++) {
        for(int j = 0; j < N; j++) {
            printf("%d ", board[i][j]);
        }
        printf("\n");
    }
}
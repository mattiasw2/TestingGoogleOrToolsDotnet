namespace suduko
{
    using Google.OrTools.Sat;
    using System;

    class SudokuSolver
    {
        static void Main()
        {
            // Define a 9x9 Sudoku grid.
            int[,] grid = new int[,]
            {
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

            SolveSudoku(grid);
        }

        static void SolveSudoku(int[,] grid)
        {
            CpModel model = new CpModel();

            // Create variables for the Sudoku grid.
            IntVar[,] cells = new IntVar[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = model.NewIntVar(1, 9, $"cell_{i}_{j}");
                    if (grid[i, j] != 0)
                    {
                        model.Add(cells[i, j] == grid[i, j]); // Add known values as constraints.
                    }
                }
            }

            // Add constraints for rows, columns, and boxes.
            for (int i = 0; i < 9; i++)
            {
                model.AddAllDifferent(GetRow(cells, i));
                model.AddAllDifferent(GetColumn(cells, i));
                model.AddAllDifferent(GetBox(cells, i / 3, i % 3));
            }

            // Create and solve the model.
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            // Print the solution if found.
            if (status == CpSolverStatus.Feasible)
            {
                PrintSudoku(cells, solver);
            }
            else
            {
                Console.WriteLine("No solution found");
            }
        }

        static IntVar[] GetRow(IntVar[,] cells, int row)
        {
            IntVar[] result = new IntVar[9];
            for (int j = 0; j < 9; j++) result[j] = cells[row, j];
            return result;
        }

        static IntVar[] GetColumn(IntVar[,] cells, int col)
        {
            IntVar[] result = new IntVar[9];
            for (int i = 0; i < 9; i++) result[i] = cells[i, col];
            return result;
        }

        static IntVar[] GetBox(IntVar[,] cells, int boxRow, int boxCol)
        {
            IntVar[] result = new IntVar[9];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i * 3 + j] = cells[boxRow * 3 + i, boxCol * 3 + j];
                }
            }
            return result;
        }

        static void PrintSudoku(IntVar[,] cells, CpSolver solver)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(solver.Value(cells[i, j]) + " ");
                }
                Console.WriteLine();
            }
        }
    }

}
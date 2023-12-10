# Advent of Code 2023

Solutions to Advent of Code 2023 using C#.

## Setting up a new day

To set up a new day, run the following script with appropriate day:

```
./setupDay.sh X
```

Manually copy the puzzle input into the newly created `inputs/dayXX.txt` file.

The `GetTestCases()` method can be used to define examples given in the problem description, setting the expected part one or two result to null will cause that test to be skipped when running the relevant part.

## Running the solutions

To continuously watch and run the code for the given day:

```
dotnet watch build X
```

If no argument is provided then a default value is taken from within `Program.cs`, this is updated to the newly created day when the above setup script is used.

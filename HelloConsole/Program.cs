namespace HelloConsole;

class Program
{
    static void Main(string[] args)
    {
        /* dotnet run */
        HelloWorld();

        /* dotnet run Parameter1 */
        // PassingOneParameter(args);

        /* dotnet run Parameter1 Parameter2 */
        // PassingMoreThanOneParameter(args);

        /* dotnet run 42 */
        // ParsingInputParameters(args);

        /* dotnet run Parameter1 Parameter2 */
        // SwitchingInputParameters(args);

        /* dotnet run */
        // MissingInputParameter(args);

        /* dotnet run */
        // ConsoleProperties();

        /* dotnet run */
        // ReadLine();

        /* dotnet run */
        // ReadKeyTrue();

        /* dotnet run */
        // ReadKeyFalse();

        /* dotnet run */
        // Clear();

        /* dotnet run */
        // CancelKeyPress();
    }

    #region helper methods

    private static void HelloWorld()
    {
        Console.WriteLine("Hello, World!");
    }

    private static void PassingOneParameter(string[] args)
    {
        Console.WriteLine($"Hello, {args[0]}!");
    }

    private static void PassingMoreThanOneParameter(string[] args)
    {
        Console.WriteLine($"Hello, {args[0]} {args[1]}!");
    }

    private static void ParsingInputParameters(string[] args)
    {
        var value = args[0];
        Console.WriteLine($"The input value {value} is of type {value.GetType()}");
        var parsedValue = int.Parse(value);
        Console.WriteLine($"The parsed value {parsedValue} is of type {parsedValue.GetType()}");
    }

    private static void SwitchingInputParameters(string[] args)
    {
        Console.WriteLine($"Before switching => {args[0]} {args[1]}!");
        Console.WriteLine($"After switching => {args[1]} {args[0]}!");
    }

    private static void MissingInputParameter(string[] args)
    {
        Console.WriteLine($"Hello, {args[0]}!");
    }

    private static void ConsoleProperties()
    {
        // Performing a backup of the background and foreground colors
        var originalBackgroundColor = Console.BackgroundColor;
        var originalForegroundColor = Console.ForegroundColor;

        // Changing the background and foreground colors
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Setting the title of the terminal while the application is running
        Console.Title = "Chapter 03 Console Application";

        Console.WriteLine($"Hello, superior terminal user!");

        // Restoring the background and foreground colors to their original values
        Console.BackgroundColor = originalBackgroundColor;
        Console.ForegroundColor = originalForegroundColor;

        Console.WriteLine("Press any key to exit...");

        // Waiting for the user to press a key to end the program.
        // This is useful to see the altering of the terminal's title
        Console.ReadKey(true);
    }

    private static void ReadLine()
    {
        Console.WriteLine("Enter some text then hit ENTER, or simply hit ENTER to end the program.");

        string? line;
        while ((line = Console.ReadLine()) != string.Empty)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine("Goodbye!");
    }

    private static void ReadKeyTrue()
    {
        Console.WriteLine("Press any key or ESC to exit...");

        var keyPressed = Console.ReadKey(true).Key;

        while (keyPressed != ConsoleKey.Escape)
        {
            Console.WriteLine($"You pressed {keyPressed}");
            keyPressed = Console.ReadKey(true).Key;
        }
    }

    private static void ReadKeyFalse()
    {
        Console.WriteLine("Press any key or ESC to exit...");

        var keyPressed = Console.ReadKey(false).Key;

        while (keyPressed != ConsoleKey.Escape)
        {
            Console.WriteLine($"You pressed {keyPressed}");
            keyPressed = Console.ReadKey(false).Key;
        }
    }

    private static void Clear()
    {
        var lorem = "Here is some random text to display in the terminal that will be cleared...";
        Console.WriteLine(lorem);
        Console.WriteLine();

        Console.WriteLine("Press C to clear the screen before exiting the method, or any other key to exit without clearing the screen...");
        if (Console.ReadKey(true).Key == ConsoleKey.C)
        {
            Console.Clear();
        }
    }

    private static void CancelKeyPress()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // This will prevent the program from terminating immediately
            Console.WriteLine("CancelKeyPress event raised!\nPerforming cleanup...");
            // Performing cleanup operations (logging out of services, saving progress state, closing database connections, etc.)
            Environment.Exit(0); // This will terminate the program when cleanup is done
        };

        int counter = 1;
        while (true)
        {
            Console.WriteLine($"Printing line number {counter}");
            counter++;
            Task delayTask = Task.Run(async () => await Task.Delay(1000));
            delayTask.Wait();
        }
    }

    #endregion
}

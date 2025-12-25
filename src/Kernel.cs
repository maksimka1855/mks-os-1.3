using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace MKS_OS
{
    public class Kernel : Sys.Kernel
    {
        // ========== CONSTANTS AND VARIABLES ==========
        private readonly string VERSION = "1.3";
        private readonly string AUTHOR = "maksimka1855";
        private string currentDirectory = "C:\\";
        private string userName = "user";
        private bool isRunning = true;
        private string bootTime;

        // Interface colors
        private ConsoleColor[] themeColors = {
            ConsoleColor.Cyan,    // Primary
            ConsoleColor.Green,   // Success
            ConsoleColor.Yellow,  // Warning
            ConsoleColor.Red,     // Error
            ConsoleColor.Magenta  // Accent
        };

        // Simple in-memory file system
        private Dictionary<string, string> fileSystem = new Dictionary<string, string>();
        private Dictionary<string, string> directoryStructure = new Dictionary<string, string>();

        // Command history
        private List<string> commandHistory = new List<string>();

        // ========== MAIN KERNEL ==========
        protected override void BeforeRun()
        {
            try
            {
                bootTime = DateTime.Now.ToString("HH:mm:ss");
                InitializeFileSystem();
                ShowBootScreen();

                Print($"MKS-OS v{VERSION} successfully loaded!", themeColors[0]);
                Print("Type 'help' for command list\n", themeColors[1]);
            }
            catch (Exception ex)
            {
                Print($"Initialization error: {ex.Message}", themeColors[3]);
            }
        }

        protected override void Run()
        {
            try
            {
                while (isRunning)
                {
                    ShowPrompt();
                    string input = GetInput();

                    if (!string.IsNullOrEmpty(input))
                    {
                        ProcessCommand(input);
                    }
                }
            }
            catch (Exception ex)
            {
                Print($"\nCritical error: {ex.Message}", themeColors[3]);
                Print("Press any key to reboot...", themeColors[2]);
                Console.ReadKey();
                Sys.Power.Reboot();
            }
        }

        // ========== INITIALIZATION ==========
        private void InitializeFileSystem()
        {
            // Root folders
            directoryStructure["C:\\"] = "Root Directory";
            directoryStructure["C:\\System\\"] = "System Files";
            directoryStructure["C:\\Users\\"] = "User Profiles";
            directoryStructure["C:\\Programs\\"] = "Applications";
            directoryStructure["C:\\Documents\\"] = "User Documents";

            // System files
            fileSystem["C:\\boot.ini"] = "[boot loader]\ntimeout=30\ndefault=multi(0)disk(0)rdisk(0)partition(1)\\WINDOWS\n[operating systems]\nmulti(0)disk(0)rdisk(0)partition(1)\\WINDOWS=\"MKS-OS\" /fastdetect";
            fileSystem["C:\\config.sys"] = "DEVICE=C:\\System\\ANSI.SYS\nFILES=40\nBUFFERS=30\nLASTDRIVE=Z";
            fileSystem["C:\\autoexec.bat"] = "@echo off\necho Starting MKS-OS v" + VERSION + "\npath=C:\\System;C:\\Programs\nprompt=$P$G";
            fileSystem["C:\\readme.txt"] = "Welcome to MKS-OS!\n\nThis is a text-based operating system built on Cosmos User Kit.\n\nMain commands:\n- help: command list\n- dir: list files\n- type: show file contents\n- cls: clear screen\n- info: system information\n\nUse 'shutdown' or 'reboot' to exit.";
            fileSystem["C:\\System\\kernel.sys"] = "MKS-OS Kernel v" + VERSION + "\nMemory: 16MB\nProcessor: 386+ compatible\nStatus: Operational";
            fileSystem["C:\\Users\\" + userName + "\\profile.ini"] = "[User]\nName=" + userName + "\nLevel=User\nLastLogin=" + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

        private void ShowBootScreen()
        {
            Console.Clear();
            Console.ForegroundColor = themeColors[4];

            string[] bootLogo = {
                "┌─────────────────────────────────────────────────────┐",
                "│                                                     │",
                "│  ███╗   ███╗██╗  ██╗███████╗    ██████╗ ███████╗   │",
                "│  ████╗ ████║██║ ██╔╝██╔════╝    ██╔══██╗██╔════╝   │",
                "│  ██╔████╔██║█████╔╝ ███████╗    ██║  ██║███████╗   │",
                "│  ██║╚██╔╝██║██╔═██╗ ╚════██║    ██║  ██║╚════██║   │",
                "│  ██║ ╚═╝ ██║██║  ██╗███████║    ██████╔╝███████║   │",
                "│  ╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝    ╚═════╝ ╚══════╝   │",
                "│                                                     │",
                "│         Text Operating System v" + VERSION.PadRight(8) + "           │",
                "│         Built on Cosmos User Kit                    │",
                "│                                                     │",
                "└─────────────────────────────────────────────────────┘"
            };

            foreach (string line in bootLogo)
            {
                Console.WriteLine(line);
            }

            Console.ResetColor();
            Console.WriteLine("\n");

            // Simulate boot process
            Print("Initializing CPU...", themeColors[1]);
            Print("Checking memory...", themeColors[1]);
            Print("Initializing file system...", themeColors[1]);
            Print("Loading command shell...", themeColors[1]);
            Console.WriteLine();
        }

        // ========== HELPER METHODS ==========
        private string GetFileNameFromPath(string path)
        {
            int lastSlash = path.LastIndexOf('\\');
            if (lastSlash >= 0 && lastSlash < path.Length - 1)
            {
                return path.Substring(lastSlash + 1);
            }
            return path;
        }

        private string GetDirectoryFromPath(string path)
        {
            int lastSlash = path.LastIndexOf('\\');
            if (lastSlash > 0)
            {
                return path.Substring(0, lastSlash + 1);
            }
            return path;
        }

        private string[] GetPathParts(string path)
        {
            return path.Trim('\\').Split('\\');
        }

        private string GetParentDirectory(string path)
        {
            var parts = GetPathParts(path);
            if (parts.Length <= 1) return "C:\\";

            string result = "C:\\";
            for (int i = 0; i < parts.Length - 1; i++)
            {
                result += parts[i] + "\\";
            }
            return result;
        }

        // ========== CORE METHODS ==========
        private void ShowPrompt()
        {
            Console.ForegroundColor = themeColors[0];
            Console.Write($"{userName}@{currentDirectory}> ");
            Console.ResetColor();
        }

        private string GetInput()
        {
            string input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input))
            {
                commandHistory.Add(input);
            }

            return input?.Trim();
        }

        private void Print(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // ========== COMMAND SYSTEM ==========
        private void ProcessCommand(string input)
        {
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string command = parts.Length > 0 ? parts[0].ToLower() : "";
            string[] args = parts.Length > 1 ? GetArraySlice(parts, 1) : new string[0];

            switch (command)
            {
                case "help":
                    ShowHelp(args);
                    break;
                case "cls":
                case "clear":
                    Console.Clear();
                    break;
                case "echo":
                    Echo(args);
                    break;
                case "dir":
                case "ls":
                    ListDirectory(args);
                    break;
                case "cd":
                    ChangeDirectory(args);
                    break;
                case "type":
                case "cat":
                    ShowFile(args);
                    break;
                case "info":
                case "about":
                    SystemInfo();
                    break;
                case "time":
                    ShowTime();
                    break;
                case "date":
                    ShowDate();
                    break;
                case "calc":
                    Calculator(args);
                    break;
                case "edit":
                    EditFile(args);
                    break;
                case "mkdir":
                    CreateDirectory(args);
                    break;
                case "rm":
                case "del":
                    DeleteFile(args);
                    break;
                case "history":
                    ShowHistory();
                    break;
                case "theme":
                    ChangeTheme(args);
                    break;
                case "user":
                    ChangeUser(args);
                    break;
                case "shutdown":
                    Shutdown();
                    break;
                case "reboot":
                    Reboot();
                    break;
                case "exit":
                    isRunning = false;
                    Print("Ending session...", themeColors[1]);
                    break;
                case "ver":
                    Print($"MKS-OS version {VERSION}", themeColors[0]);
                    break;
                case "mem":
                    ShowMemoryInfo();
                    break;
                case "pwd":
                    Print($"Current directory: {currentDirectory}", themeColors[1]);
                    break;
                case "color":
                    ChangeColor(args);
                    break;
                case "copy":
                    CopyFile(args);
                    break;
                case "ren":
                case "rename":
                    RenameFile(args);
                    break;
                case "":
                    // Empty command - do nothing
                    break;
                default:
                    Print($"Command '{command}' not found. Type 'help' for help.", themeColors[2]);
                    break;
            }
        }

        private string[] GetArraySlice(string[] array, int startIndex)
        {
            if (startIndex >= array.Length) return new string[0];

            string[] result = new string[array.Length - startIndex];
            for (int i = startIndex; i < array.Length; i++)
            {
                result[i - startIndex] = array[i];
            }
            return result;
        }

        // ========== COMMAND IMPLEMENTATIONS ==========
        private void ShowHelp(string[] args)
        {
            Print("\n╔══════════════════════════════════════════════════════════╗", themeColors[0]);
            Print("║                     COMMAND HELP MENU                     ║", themeColors[0]);
            Print("╠══════════════════════════════════════════════════════════╣", themeColors[0]);
            Print("║  help              - Show this help                       ║", themeColors[1]);
            Print("║  cls / clear       - Clear screen                         ║", themeColors[1]);
            Print("║  echo <text>       - Print text                           ║", themeColors[1]);
            Print("║  dir / ls          - List files and folders               ║", themeColors[1]);
            Print("║  cd <path>         - Change directory                     ║", themeColors[1]);
            Print("║  type <file>       - Show file contents                   ║", themeColors[1]);
            Print("║  info / about      - System information                   ║", themeColors[1]);
            Print("║  time              - Current time                         ║", themeColors[1]);
            Print("║  date              - Current date                         ║", themeColors[1]);
            Print("║  calc <expr>       - Simple calculator                    ║", themeColors[1]);
            Print("║  edit <file>       - Edit file                            ║", themeColors[1]);
            Print("║  mkdir <name>      - Create directory                     ║", themeColors[1]);
            Print("║  rm / del <file>   - Delete file                          ║", themeColors[1]);
            Print("║  copy <src> <dst>  - Copy file                            ║", themeColors[1]);
            Print("║  ren <old> <new>   - Rename file                          ║", themeColors[1]);
            Print("║  history           - Command history                      ║", themeColors[1]);
            Print("║  theme <num>       - Change theme (0-4)                   ║", themeColors[1]);
            Print("║  color <color>     - Change text color                    ║", themeColors[1]);
            Print("║  user <name>       - Change user                          ║", themeColors[1]);
            Print("║  ver               - System version                       ║", themeColors[1]);
            Print("║  mem               - Memory information                   ║", themeColors[1]);
            Print("║  pwd               - Current directory                    ║", themeColors[1]);
            Print("║  shutdown          - Shutdown system                      ║", themeColors[1]);
            Print("║  reboot            - Reboot system                        ║", themeColors[1]);
            Print("║  exit              - Exit shell                           ║", themeColors[1]);
            Print("╚══════════════════════════════════════════════════════════╝", themeColors[0]);
            Print("");
        }

        private void Echo(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Usage: echo <text>", themeColors[2]);
                return;
            }

            string text = string.Join(" ", args);
            Print(text);
        }

        private void ListDirectory(string[] args)
        {
            string path = args.Length > 0 ? args[0] : currentDirectory;

            Print($"\nContents of {path}:", themeColors[0]);
            Print("Name                    Type       Size", themeColors[0]);
            Print("────────────────────────────────────────────────", themeColors[0]);

            // Show directories
            foreach (var dir in directoryStructure)
            {
                if (dir.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                {
                    string dirName = GetFileNameFromPath(dir.Key.TrimEnd('\\'));
                    if (string.IsNullOrEmpty(dirName)) dirName = ".";

                    Console.ForegroundColor = themeColors[4];
                    Console.Write(dirName.PadRight(25));
                    Console.ForegroundColor = themeColors[1];
                    Console.Write("<DIR>".PadRight(11));
                    Console.ForegroundColor = themeColors[2];
                    Console.WriteLine("-");
                    Console.ResetColor();
                }
            }

            // Show files
            foreach (var file in fileSystem)
            {
                if (file.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = GetFileNameFromPath(file.Key);
                    int fileSize = file.Value.Length;

                    Console.ForegroundColor = themeColors[1];
                    Console.Write(fileName.PadRight(25));
                    Console.ForegroundColor = themeColors[0];
                    Console.Write("FILE".PadRight(11));
                    Console.ForegroundColor = themeColors[3];
                    Console.WriteLine($"{fileSize} bytes");
                    Console.ResetColor();
                }
            }

            Print("────────────────────────────────────────────────", themeColors[0]);

            // Statistics
            int dirCount = 0;
            int fileCount = 0;
            long totalSize = 0;

            foreach (var dir in directoryStructure)
                if (dir.Key.StartsWith(path)) dirCount++;

            foreach (var file in fileSystem)
            {
                if (file.Key.StartsWith(path))
                {
                    fileCount++;
                    totalSize += file.Value.Length;
                }
            }

            Print($"Directories: {dirCount}, Files: {fileCount}, Total: {totalSize} bytes\n", themeColors[0]);
        }

        private void ChangeDirectory(string[] args)
        {
            if (args.Length == 0)
            {
                Print($"Current directory: {currentDirectory}", themeColors[1]);
                return;
            }

            string newPath = args[0];

            if (newPath == "..")
            {
                // Go up one level
                currentDirectory = GetParentDirectory(currentDirectory);
            }
            else if (newPath == ".")
            {
                // Stay in current directory
                return;
            }
            else if (newPath.StartsWith("C:\\"))
            {
                // Absolute path
                string testPath = newPath.EndsWith("\\") ? newPath : newPath + "\\";
                if (directoryStructure.ContainsKey(testPath))
                {
                    currentDirectory = testPath;
                }
                else
                {
                    Print($"Directory '{newPath}' not found", themeColors[3]);
                }
            }
            else
            {
                // Relative path
                string testPath = currentDirectory + newPath + "\\";
                if (directoryStructure.ContainsKey(testPath))
                {
                    currentDirectory = testPath;
                }
                else
                {
                    Print($"Directory '{newPath}' not found", themeColors[3]);
                }
            }

            Print($"Current directory: {currentDirectory}", themeColors[1]);
        }

        private void ShowFile(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Usage: type <filename>", themeColors[2]);
                return;
            }

            string fileName = args[0];
            string fullPath = fileName.Contains(":\\") ? fileName : currentDirectory + fileName;

            if (fileSystem.ContainsKey(fullPath))
            {
                Print($"\nContents of {fileName}:", themeColors[0]);
                Print("════════════════════════════════════════════", themeColors[0]);
                Print(fileSystem[fullPath], themeColors[1]);
                Print("════════════════════════════════════════════\n", themeColors[0]);
            }
            else
            {
                Print($"File '{fileName}' not found", themeColors[3]);
            }
        }

        private void SystemInfo()
        {
            Print("\n╔══════════════════════════════════════════════════════════╗", themeColors[0]);
            Print("║                    SYSTEM INFORMATION                     ║", themeColors[0]);
            Print("╠══════════════════════════════════════════════════════════╣", themeColors[0]);
            Print($"║  System:          MKS-OS v{VERSION}".PadRight(58) + "║", themeColors[1]);
            Print($"║  Kernel:          Cosmos User Kit".PadRight(58) + "║", themeColors[1]);
            Print($"║  Developer:       {AUTHOR}".PadRight(58) + "║", themeColors[1]);
            Print($"║  Current User:    {userName}".PadRight(58) + "║", themeColors[1]);
            Print($"║  Directory:       {currentDirectory}".PadRight(58) + "║", themeColors[1]);
            Print($"║  Boot Time:       {bootTime}".PadRight(58) + "║", themeColors[1]);
            Print("╠══════════════════════════════════════════════════════════╣", themeColors[0]);
            Print($"║  Total Files:     {fileSystem.Count}".PadRight(58) + "║", themeColors[2]);
            Print($"║  Total Dirs:      {directoryStructure.Count}".PadRight(58) + "║", themeColors[2]);
            Print($"║  Command History: {commandHistory.Count}".PadRight(58) + "║", themeColors[2]);
            Print($"║  Current Time:    {DateTime.Now:HH:mm:ss}".PadRight(58) + "║", themeColors[2]);
            Print("╚══════════════════════════════════════════════════════════╝", themeColors[0]);
            Print("");
        }

        private void ShowMemoryInfo()
        {
            Print("\nMemory Information:", themeColors[0]);
            Print("────────────────────────────────────────────────", themeColors[0]);
            Print($"File System: {fileSystem.Count} files, {directoryStructure.Count} directories", themeColors[1]);
            Print($"Command History: {commandHistory.Count} entries", themeColors[1]);

            // Calculate total file size
            long totalSize = 0;
            foreach (var file in fileSystem)
            {
                totalSize += file.Value.Length;
            }
            Print($"Total file size: {totalSize} bytes", themeColors[1]);

            // Simulated memory information
            Print("\nSystem Memory (simulated):", themeColors[2]);
            Print("  Available: 16 MB", themeColors[1]);
            Print("  Used: 2 MB", themeColors[1]);
            Print("  Free: 14 MB", themeColors[1]);
            Print("────────────────────────────────────────────────\n", themeColors[0]);
        }

        private void ShowTime()
        {
            Print($"Current time: {DateTime.Now:HH:mm:ss}", themeColors[1]);
        }

        private void ShowDate()
        {
            string[] months = {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };

            string[] days = {
                "Sunday", "Monday", "Tuesday", "Wednesday",
                "Thursday", "Friday", "Saturday"
            };

            DateTime now = DateTime.Now;
            string dayName = days[(int)now.DayOfWeek];
            string monthName = months[now.Month - 1];

            Print($"Today: {dayName}, {monthName} {now.Day}, {now.Year}", themeColors[1]);
        }

        private void Calculator(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Simple calculator. Examples:", themeColors[0]);
                Print("  calc 5 + 3", themeColors[2]);
                Print("  calc 10 * 2.5", themeColors[2]);
                Print("  calc 4+6/2", themeColors[2]);
                return;
            }

            string expression = string.Join(" ", args);

            try
            {
                // Simple calculator for basic operations
                expression = expression.Replace(" ", "");

                // Try to find operator
                char[] operators = { '+', '-', '*', '/' };
                int operatorIndex = -1;
                char op = ' ';

                for (int i = 0; i < expression.Length; i++)
                {
                    if (Array.IndexOf(operators, expression[i]) >= 0)
                    {
                        operatorIndex = i;
                        op = expression[i];
                        break;
                    }
                }

                if (operatorIndex > 0)
                {
                    string leftPart = expression.Substring(0, operatorIndex);
                    string rightPart = expression.Substring(operatorIndex + 1);

                    if (double.TryParse(leftPart, out double a) && double.TryParse(rightPart, out double b))
                    {
                        double result = 0;
                        string opSymbol = "";

                        switch (op)
                        {
                            case '+':
                                result = a + b;
                                opSymbol = "+";
                                break;
                            case '-':
                                result = a - b;
                                opSymbol = "-";
                                break;
                            case '*':
                                result = a * b;
                                opSymbol = "×";
                                break;
                            case '/':
                                if (b != 0)
                                {
                                    result = a / b;
                                    opSymbol = "÷";
                                }
                                else
                                {
                                    Print("Error: division by zero!", themeColors[3]);
                                    return;
                                }
                                break;
                        }

                        Print($"{a} {opSymbol} {b} = {result}", themeColors[1]);
                    }
                    else
                    {
                        Print("Invalid number format", themeColors[3]);
                    }
                }
                else if (double.TryParse(expression, out double singleNumber))
                {
                    Print($"Number: {singleNumber}", themeColors[1]);
                }
                else
                {
                    Print("Invalid expression", themeColors[3]);
                }
            }
            catch
            {
                Print("Calculation error", themeColors[3]);
            }
        }

        private void EditFile(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Usage: edit <filename>", themeColors[2]);
                return;
            }

            string fileName = args[0];
            string fullPath = fileName.Contains(":\\") ? fileName : currentDirectory + fileName;

            Print($"Editing file: {fileName}", themeColors[0]);
            Print("Enter content (type 'END' on separate line to finish):", themeColors[0]);
            Print("════════════════════════════════════════════", themeColors[0]);

            StringBuilder content = new StringBuilder();
            string line;

            while (true)
            {
                line = Console.ReadLine();
                if (line == "END") break;
                content.AppendLine(line);
            }

            fileSystem[fullPath] = content.ToString();

            Print("════════════════════════════════════════════", themeColors[0]);
            Print($"File '{fileName}' saved ({content.Length} bytes)", themeColors[1]);

            // Add directory if it doesn't exist
            string dirPath = GetDirectoryFromPath(fullPath);
            if (!directoryStructure.ContainsKey(dirPath))
            {
                directoryStructure[dirPath] = "User Directory";
            }
        }

        private void CreateDirectory(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Usage: mkdir <dirname>", themeColors[2]);
                return;
            }

            string dirName = args[0];
            string fullPath = currentDirectory + dirName + "\\";

            if (!directoryStructure.ContainsKey(fullPath))
            {
                directoryStructure[fullPath] = "User Created Directory";
                Print($"Directory '{dirName}' created", themeColors[1]);
            }
            else
            {
                Print($"Directory '{dirName}' already exists", themeColors[3]);
            }
        }

        private void DeleteFile(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Usage: del <filename>", themeColors[2]);
                return;
            }

            string fileName = args[0];
            string fullPath = fileName.Contains(":\\") ? fileName : currentDirectory + fileName;

            if (fileSystem.ContainsKey(fullPath))
            {
                fileSystem.Remove(fullPath);
                Print($"File '{fileName}' deleted", themeColors[1]);
            }
            else
            {
                Print($"File '{fileName}' not found", themeColors[3]);
            }
        }

        private void CopyFile(string[] args)
        {
            if (args.Length < 2)
            {
                Print("Usage: copy <source> <destination>", themeColors[2]);
                return;
            }

            string source = args[0];
            string destination = args[1];

            string sourcePath = source.Contains(":\\") ? source : currentDirectory + source;
            string destPath = destination.Contains(":\\") ? destination : currentDirectory + destination;

            if (fileSystem.ContainsKey(sourcePath))
            {
                fileSystem[destPath] = fileSystem[sourcePath];
                Print($"File copied from '{source}' to '{destination}'", themeColors[1]);
            }
            else
            {
                Print($"Source file '{source}' not found", themeColors[3]);
            }
        }

        private void RenameFile(string[] args)
        {
            if (args.Length < 2)
            {
                Print("Usage: rename <oldname> <newname>", themeColors[2]);
                return;
            }

            string oldName = args[0];
            string newName = args[1];

            string oldPath = oldName.Contains(":\\") ? oldName : currentDirectory + oldName;
            string newPath = newName.Contains(":\\") ? newName : currentDirectory + newName;

            if (fileSystem.ContainsKey(oldPath))
            {
                fileSystem[newPath] = fileSystem[oldPath];
                fileSystem.Remove(oldPath);
                Print($"File renamed from '{oldName}' to '{newName}'", themeColors[1]);
            }
            else
            {
                Print($"File '{oldName}' not found", themeColors[3]);
            }
        }

        private void ShowHistory()
        {
            if (commandHistory.Count == 0)
            {
                Print("Command history is empty", themeColors[2]);
                return;
            }

            Print("\nCommand History:", themeColors[0]);
            Print("════════════════════════════════════════════", themeColors[0]);

            for (int i = 0; i < commandHistory.Count; i++)
            {
                Console.ForegroundColor = themeColors[1];
                Console.Write($"{i + 1:000} ");
                Console.ForegroundColor = themeColors[0];
                Console.WriteLine(commandHistory[i]);
            }

            Print("════════════════════════════════════════════\n", themeColors[0]);
        }

        private void ChangeTheme(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Available themes:", themeColors[0]);
                for (int i = 0; i < themeColors.Length; i++)
                {
                    Console.ForegroundColor = themeColors[i];
                    Console.WriteLine($"  {i}: {themeColors[i].ToString()}");
                }
                Console.ResetColor();
                return;
            }

            if (int.TryParse(args[0], out int themeIndex) && themeIndex >= 0 && themeIndex < themeColors.Length)
            {
                // Create new theme by shifting colors
                ConsoleColor[] newTheme = new ConsoleColor[themeColors.Length];
                for (int i = 0; i < themeColors.Length; i++)
                {
                    newTheme[i] = themeColors[(i + themeIndex) % themeColors.Length];
                }
                themeColors = newTheme;

                Print($"Theme changed to {themeColors[0].ToString()}", themeColors[1]);
            }
            else
            {
                Print($"Invalid theme number. Use values 0-{themeColors.Length - 1}", themeColors[3]);
            }
        }

        private void ChangeColor(string[] args)
        {
            if (args.Length == 0)
            {
                Print("Available colors:", themeColors[0]);
                string[] colorNames = {
                    "Black", "DarkBlue", "DarkGreen", "DarkCyan", "DarkRed",
                    "DarkMagenta", "DarkYellow", "Gray", "DarkGray", "Blue",
                    "Green", "Cyan", "Red", "Magenta", "Yellow", "White"
                };

                for (int i = 0; i < 16; i++)
                {
                    Console.ForegroundColor = (ConsoleColor)i;
                    Console.WriteLine($"  {i}: {colorNames[i]}");
                }
                Console.ResetColor();
                return;
            }

            if (int.TryParse(args[0], out int colorIndex) && colorIndex >= 0 && colorIndex < 16)
            {
                Console.ForegroundColor = (ConsoleColor)colorIndex;
                Print("Text color changed successfully!", (ConsoleColor)colorIndex);
            }
            else
            {
                Print("Invalid color number. Use values 0-15", themeColors[3]);
            }
        }

        private void ChangeUser(string[] args)
        {
            if (args.Length == 0)
            {
                Print($"Current user: {userName}", themeColors[1]);
                return;
            }

            string newUser = args[0];
            userName = newUser;

            // Update profile
            string profilePath = $"C:\\Users\\{userName}\\profile.ini";
            fileSystem[profilePath] = $"[User]\nName={userName}\nLevel=User\nLastLogin={DateTime.Now:dd.MM.yyyy HH:mm}";

            // Add user directory
            string userDir = $"C:\\Users\\{userName}\\";
            if (!directoryStructure.ContainsKey(userDir))
            {
                directoryStructure[userDir] = $"Home directory of {userName}";
            }

            Print($"User changed to: {userName}", themeColors[1]);
        }

        private void Shutdown()
        {
            Print("Preparing to shutdown...", themeColors[2]);
            Print("Saving system settings...", themeColors[2]);
            Print("Stopping processes...", themeColors[2]);
            Print("Shutting down MKS-OS...", themeColors[2]);

            for (int i = 3; i > 0; i--)
            {
                Print($"System will shutdown in {i}...", themeColors[3]);
                // Simple delay
                for (int j = 0; j < 10000000; j++) { }
            }

            isRunning = false;
            Sys.Power.Shutdown();
        }

        private void Reboot()
        {
            Print("Rebooting system...", themeColors[2]);
            isRunning = false;
            Sys.Power.Reboot();
        }
    }
}
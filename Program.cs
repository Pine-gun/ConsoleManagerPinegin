using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace FileManager_Pinegin
{
    internal class Program
    {
        public static int WINDOW_WIDTH = 120;
        public static int WINDOW_HEIGTH = 40;
        public const string ROOT_DIRECTORY = "C:\\Users\\PineGun\\source";//корневая директория на начало работы
        public static string currentDirectory = "C:\\Users\\PineGun\\source";//текущая директория
        public static string allDataToOneString = "";
        public static string saveCommandsFile = "D:\\Pinegun\\ConsoleManagerPinegin\\SaveCommands.txt";//файл для записи ранее введенных команд
        public static string errorsFile = "D:\\Pinegun\\ConsoleManagerPinegin\\Errors.txt";//файл для записи ошибок
        private const string ACCESIBLE_COMMANDS = "help - вывод на экран списка доступных команд\r\ncd path_to_directory - перейти в указанную директорию\r\ncd .. - перейти на уровень выше\r\ncd - перейти в домашнюю директорию пользователя\r\ntree n_page path_to_directory - постраничный вывод списка файлов и директорий в текущей директории,\r\n\t\t\t\tгде n_page - номер страницы для вывода\r\nls -l path_to_directory_or_file - вывод информации о файле или директории\r\nmkdir directory_name - создать новую директорию\r\ntouch file_name - создать новый пустой файл\r\ncp source_file destination_path - скопировать файл или директорию\r\nmv source destination_path - переместить или переименовать файл или директорию\r\nrm file_name - удалить файл\r\nrm -r directory_name - удалить директорию и её содержимое\r\nrmdir directory_name - удалить пустую директорию\r\ncat n_page file_name - вывести содержимое файла на экран,где n_page - номер страницы для вывода\r\nred file_name - открыть файл в текстовом редакторе\r\necho “text” file_name - передать текст в указанный файл";

        static void Main(string[] args)
        {
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGTH);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGTH);

            DrawConsole(0, 0, WINDOW_WIDTH, 18);
            DrawConsole(0, 20, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 21);
            Console.Write("Для вывода списка команд введите \"help\"");
            File.WriteAllText(saveCommandsFile, "");
            UpdateConsole();
        }
        /// <summary>
        /// вызывает методы наполнения и прорисовки командной строки
        /// ловит ошибки и вызывает метод ErrorsToFile для их записи в файл 
        /// </summary>
        public static void UpdateConsole()
        {
            try
            {
                DrawInputCommandField(currentDirectory, 1, 31, WINDOW_WIDTH, 1);
                CommandInputProcess();
            }
            catch (Exception ex)
            {
                allDataToOneString = ex.Message;
                ErrorsToFile();
            }

        }
        /// <summary>
        /// метод вывода ошибок в окно вывода информации и записи их в файл Errors.txt
        /// </summary>
        public static void ErrorsToFile()
        {
            DrawConsole(0, 20, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 21);
            int symbolsAtLine = 118;// количество символов, помещающихся на одной строке консоли вывода
            int linesQuantity = (int)Math.Ceiling(allDataToOneString.Length / (double)symbolsAtLine);
            string[] messageStrings = new string[linesQuantity];
            for (int i = 0; i < linesQuantity; i++)
            {
                for (int j = 0 + symbolsAtLine*i; j < allDataToOneString.Length; j++)
                {
                    messageStrings[i] += allDataToOneString[j];
                    if (messageStrings[i].Length == symbolsAtLine)
                    {
                        break;
                    }
                }
            }
            for (int n = 0; n < messageStrings.Length; n++)
            {

                Console.SetCursorPosition(1, 21 + n);
                Console.Write(messageStrings[n]);
            }
            Console.SetCursorPosition(1, 33);
            List<string> errors = File.ReadAllLines(errorsFile).ToList();
            errors.Add(allDataToOneString.ToString());
            File.WriteAllLines(errorsFile, errors.ToArray());
            allDataToOneString ="";
            UpdateConsole();
        }
        /// <summary>
        /// метод наполнения командной строки и считывания команды, введенной пользователем
        /// включает в себя ограничения на выход из поля командной строки
        /// включает в себя сохранение введенных команд в файл SaveCommands.txt 
        /// и вывод их в командную строку клавишами курсора вверх вниз
        /// </summary>
        public static void CommandInputProcess()
        {
            StringBuilder command = new StringBuilder();
            List<string> saves = File.ReadAllLines(saveCommandsFile).ToList();

            int left = 0; int top = 0;
            GetCurrentCursorPosition(ref left, ref top);
            int counter = 0;
            char keyChar;
            while (true)
            {
                int currentLeft = 0; int currentTop = 0;
                GetCurrentCursorPosition(ref currentLeft, ref currentTop);
                ConsoleKeyInfo info = Console.ReadKey();
                ConsoleKey key = info.Key;
                keyChar = info.KeyChar;

                if (key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key == ConsoleKey.Backspace && Console.CursorLeft < left)
                {
                    Console.Write(">");
                }
                else if (key == ConsoleKey.Backspace)
                {
                    command.Remove(command.Length - 1, 1);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                }
                else if (currentLeft == WINDOW_WIDTH - 2)
                {
                    Console.SetCursorPosition(currentLeft, currentTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft, currentTop);

                }
                else if (key == ConsoleKey.UpArrow && saves.Count != 0)
                {
                    command.Clear();
                    if (counter < saves.Count-1)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 0;
                    }
                    command.Append(saves[counter]);
                    DrawInputCommandField(currentDirectory, 1, 31, WINDOW_WIDTH, 1);
                    Console.Write(command);
                }
                else if (key == ConsoleKey.DownArrow && saves.Count != 0)
                {
                    command.Clear();
                    command.Append(saves[counter]);
                    if (counter > 0)
                    {
                        counter--;
                    }
                    else
                    {
                        counter = saves.Count-1;
                    }
                    DrawInputCommandField(currentDirectory, 1, 31, WINDOW_WIDTH, 1);
                    Console.Write(command);
                }
                else
                {
                    command.Append(keyChar);
                }
            }

            saves.Add(command.ToString());
            File.WriteAllLines(saveCommandsFile, saves.ToArray());
            CommandParser(command);
        }
        /// <summary>
        /// метод разделения введенной пользователем команды на слова 
        /// и выбор варианта действий в зависимости от первого слова
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void CommandParser(StringBuilder command)
        {
            string[] splittedCommand = command.ToString().Split(" ");

            switch (splittedCommand[0])
            {
                case "help":
                    allDataToOneString = ACCESIBLE_COMMANDS;
                    DrawToConsole(1);
                    break;
                case "cd":
                    ChangeDirectory(command.ToString());
                    break;
                case "tree":
                    ShowTree(command.ToString());
                    break;
                case "mkdir":
                    MakeDir(command.ToString());
                    break;
                case "touch":
                    CreateFile(command.ToString());
                    break;
                case "rm":
                    DeleteDirectoryWithFilesOrFile(command.ToString());
                    break;
                case "rmdir":
                    DeleteDirectory(command.ToString());
                    break;
                case "cat":
                    GetFileContent(command.ToString());
                    DrawToConsole(int.Parse(splittedCommand[1]));
                    break;
                case "red":
                    FileRedactor(command.ToString());
                    break;
                case "echo":
                    TextToFile(command.ToString());
                    break;
                case "cp":
                    CopyFileOrDirectory(command.ToString());
                    break;
                case "ls":
                    ObjectInfo(command.ToString());
                    DrawToConsole(1);
                    break;
                case "mv":
                    MoveFileOrDirectory(command.ToString());
                    break;
            }
            UpdateConsole();
        }       

        /// <summary>
        /// метод вывода дерева каталогов на экран
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void ShowTree(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (Directory.Exists(splittedCommand[2]))
            {
                GetTree(new DirectoryInfo(splittedCommand[2]), "", true);
                DrawToConsole(int.Parse(splittedCommand[1]));
            }
            else
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Нет такой папки!");
            }
        }
        /// <summary>
        /// метод перемещения папок или файлов
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void MoveFileOrDirectory(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[1].Contains("\\"))
            {
                if (File.Exists(splittedCommand[1]))
                {
                    File.Move(splittedCommand[1], splittedCommand[2]);
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Файл успешно перемещен!");
                }
                else if (Directory.Exists(splittedCommand[1]))
                {
                    Directory.Move(splittedCommand[1], splittedCommand[2]);
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Папка успешно перемещена!");
                }
            }
            else
            {
                if (File.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    File.Move(currentDirectory + $"\\{splittedCommand[1]}", splittedCommand[2]);
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Файл успешно перемещен!");
                }
                else if (Directory.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    Directory.Move(currentDirectory + $"\\{splittedCommand[1]}", splittedCommand[2]);
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Папка успешно перемещена!");
                }
            }

        }
        /// <summary>
        /// метод записи информации о файле в строковую переменную allDataToOneString
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void ObjectInfo(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[1] == "-l")
            {
                if (splittedCommand[2].Contains("\\"))
                {
                    if (File.Exists(splittedCommand[2]))
                    {
                        FileInfo file = new FileInfo(splittedCommand[2]);
                        allDataToOneString +=$"{file.Name}\nВремя создания файла:{file.CreationTime}\nПоследнее обращение к файлу:{file.LastAccessTime}";
                    }
                    else if (Directory.Exists(splittedCommand[2]))
                    {
                        DirectoryInfo directory = new DirectoryInfo(splittedCommand[2]);
                        allDataToOneString +=$"{directory.Name}\nВремя создания папки:{directory.CreationTime}\nПоследнее обращение к папке:{directory.LastAccessTime}";
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки или файла!");
                    }
                }
                else
                {
                    if (File.Exists(currentDirectory + $"\\{splittedCommand[2]}"))
                    {
                        FileInfo file = new FileInfo(currentDirectory + $"\\{splittedCommand[2]}");
                        allDataToOneString +=$"{file.Name}\nВремя создания файла:{file.CreationTime}\nПоследнее обращение к файлу:{file.LastAccessTime}";
                    }
                    else if (Directory.Exists(currentDirectory + $"\\{splittedCommand[2]}"))
                    {
                        DirectoryInfo directory = new DirectoryInfo(currentDirectory + $"\\{splittedCommand[2]}");
                        allDataToOneString +=$"{directory.Name}\nВремя создания папки:{directory.CreationTime}\nПоследнее обращение к папке:{directory.LastAccessTime}";
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки или файла!");
                    }
                }
            }
            else
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Неправильный ввод команды!!!");
            }
        }
        /// <summary>
        /// метод копирования паок или файлов из одной директории в другую
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void CopyFileOrDirectory(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[1].Contains("\\"))
            {
                string[] splittedPath = splittedCommand[1].ToString().Split("\\");
                if (File.Exists(splittedCommand[1]))
                {
                    if (!Directory.Exists(splittedCommand[2]))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки назначения!");
                    }
                    else
                    {
                        File.WriteAllText(splittedCommand[2] + "\\" + splittedPath[splittedPath.Length-1], File.ReadAllText(splittedCommand[1]));
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Файл успешно скопирован!");
                    }
                }
                else if (Directory.Exists(splittedCommand[1]))
                {
                    if (!Directory.Exists(splittedCommand[2]))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки назначения!");
                    }
                    else
                    {
                        CopyDirectory(splittedCommand[1], splittedCommand[2]);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Папка успешно скопирована!");
                    }
                }

            }
            else
            {
                if (File.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    if (!Directory.Exists(splittedCommand[2]))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки назначения!");
                    }
                    else
                    {
                        File.WriteAllText(splittedCommand[2] + "\\" + splittedCommand[1], File.ReadAllText(currentDirectory + $"\\{splittedCommand[1]}"));
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Файл успешно скопирован!");
                    }
                }
                else if (Directory.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    if (!Directory.Exists(splittedCommand[2]))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки назначения!");
                    }
                    else
                    {
                        CopyDirectory(currentDirectory + $"\\{splittedCommand[1]}", splittedCommand[2]);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Папка успешно скопирована!");
                    }
                }
            }
        }
        /// <summary>
        /// рекурсивный метод копирования папки со всем содержимым из одной директории в другую
        /// </summary>
        /// <param name="FromDir">путь к директории, откуда копируют</param>
        /// <param name="ToDir">путьк директории, куда копируют</param>
        private static void CopyDirectory(string FromDir, string ToDir)
        {
            Directory.CreateDirectory(ToDir);
            string[] files = Directory.GetFiles(FromDir);
            for (int i = 0; i<files.Length; i++)
            {
                File.Copy(files[i], ToDir + "\\" + Path.GetFileName(files[i]));
            }
            string[] directories = Directory.GetDirectories(FromDir);
            for (int i = 0; i < directories.Length; i++)
            {
                CopyDirectory(directories[i], ToDir + "\\" + Path.GetFileName(directories[i]));
            }

        }
        /// <summary>
        /// метод добавления текста в файл
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void TextToFile(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (!String.IsNullOrEmpty(splittedCommand[1]) && splittedCommand[1][0] == '\"' && splittedCommand[1][splittedCommand[1].Length-1] == '\"')
            {
                if (splittedCommand.Length < 3)
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Вы не ввели название файла!");
                }
                else if (splittedCommand[2].Contains("\\"))
                {
                    if (!File.Exists(splittedCommand[2]))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такого файла!");
                    }
                    else
                    {
                        for (int i = 1; i < splittedCommand[1].Length-1; i++)
                        {
                            allDataToOneString += splittedCommand[1][i];
                        }

                        File.WriteAllText(splittedCommand[2], allDataToOneString);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Текст записан в файл!");
                        allDataToOneString = "";
                    }
                }
                else
                {
                    if (!File.Exists(currentDirectory + $"\\{splittedCommand[2]}"))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такого файла!");
                    }
                    else
                    {
                        for (int i = 1; i < splittedCommand[1].Length-1; i++)
                        {
                            allDataToOneString += splittedCommand[1][i];
                        }

                        File.WriteAllText(currentDirectory + $"\\{splittedCommand[2]}", allDataToOneString);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Текст записан в файл!");
                        allDataToOneString = "";
                    }
                }
            }

        }
        /// <summary>
        /// метод открытия файла в текстовом редакторе Notepad
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void FileRedactor(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand.Length < 2)
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Вы не ввели название файла!");
            }
            else if (splittedCommand[1].Contains("\\"))
            {
                if (!File.Exists(splittedCommand[1]))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такого файла!");
                }
                else
                {
                    Process.Start("notepad.exe", splittedCommand[1]);
                }
            }
            else
            {
                if (!File.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такого файла!");
                }
                else
                {
                    Process.Start("notepad.exe", currentDirectory + $"\\{splittedCommand[1]}");
                }
            }
        }
        /// <summary>
        /// метод записи содержимого файла в строковую переменную allDataToOneString
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void GetFileContent(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[2].Contains("\\"))
            {
                if (!File.Exists(splittedCommand[2]))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такого файла!");
                }
                else
                {
                    string[] content = File.ReadAllLines(splittedCommand[2]);
                    for (int i = 0; i < content.Length; i++)
                    {
                        allDataToOneString += content[i] + "\n";
                    }
                }
            }
            else
            {
                if (!File.Exists(currentDirectory + $"\\{splittedCommand[2]}"))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такого файла!");
                }
                else
                {
                    string[] content = File.ReadAllLines(currentDirectory + $"\\{splittedCommand[2]}");
                    for (int i = 0; i < content.Length; i++)
                    {
                        allDataToOneString += content[i] + "\n";
                    }
                }
            }

        }
        /// <summary>
        /// метод удаления пустой директории
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void DeleteDirectory(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[1].Contains("\\"))
            {
                DirectoryInfo directory = new DirectoryInfo(splittedCommand[1]);
                FileInfo[] files = directory.GetFiles();
                if (!Directory.Exists(splittedCommand[1]))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такой папки!");
                }
                else if (files.Length > 0)
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Папка не пустая!!!");
                }
                else
                {
                    string answer = " ";
                    while (answer != "y" && answer !="n")
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.WriteLine("Вы точно хотите удалить папку??? Да(y) или нет(n)??");
                        Console.SetCursorPosition(2, 22);
                        answer = Console.ReadLine();
                    }
                    if (answer ==  "y")
                    {
                        Directory.Delete(splittedCommand[1]);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Папка удалена!!!");
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        UpdateConsole();
                    }
                }
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(currentDirectory + $"\\{splittedCommand[1]}");
                FileInfo[] files = directory.GetFiles();
                if (!Directory.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такой папки!");
                }
                else if (files.Length > 0)
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Папка не пустая!!!");
                }
                else
                {
                    string answer = " ";
                    while (answer != "y" && answer !="n")
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.WriteLine("Вы точно хотите удалить папку и все ее содержимое??? Да(y) или нет(n)??");
                        Console.SetCursorPosition(2, 22);
                        answer = Console.ReadLine();
                    }
                    if (answer ==  "y")
                    {
                        Directory.Delete(currentDirectory + $"\\{splittedCommand[1]}");
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Папка удалена!!!");
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        UpdateConsole();
                    }
                }
            }
        }
        /// <summary>
        /// метод удаления файла или директории со всем содержимым (вызывает другой метод RecursiveDelete)
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        private static void DeleteDirectoryWithFilesOrFile(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[1] == "-r")
            {
                if (splittedCommand[2].Contains("\\"))
                {
                    if (!Directory.Exists(splittedCommand[2]))
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Нет такой папки!");
                    }
                    else
                    {
                        string answer = " ";
                        while (answer != "y" && answer !="n")
                        {
                            DrawConsole(0, 20, WINDOW_WIDTH, 8);
                            Console.SetCursorPosition(1, 21);
                            Console.WriteLine("Вы точно хотите удалить папку и все ее содержимое??? Да(y) или нет(n)??");
                            Console.SetCursorPosition(2, 22);
                            answer = Console.ReadLine();
                        }
                        if (answer ==  "y")
                        {
                            DirectoryInfo directory = new DirectoryInfo(splittedCommand[2]);
                            RecursiveDelete(directory);
                            DrawConsole(0, 20, WINDOW_WIDTH, 8);
                            Console.SetCursorPosition(1, 21);
                            Console.Write("Папка удалена!!!");
                        }
                        else
                        {
                            DrawConsole(0, 20, WINDOW_WIDTH, 8);
                            UpdateConsole();
                        }
                    }
                }
                else if (!Directory.Exists(currentDirectory + $"\\{splittedCommand[2]}"))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такой папки!");
                }
                else
                {
                    string answer = " ";
                    while (answer != "y" && answer !="n")
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.WriteLine("Вы точно хотите удалить папку и все ее содержимое??? Да(y) или нет(n)??");
                        Console.SetCursorPosition(2, 22);
                        answer = Console.ReadLine();
                    }
                    if (answer ==  "y")
                    {
                        DirectoryInfo directory = new DirectoryInfo(currentDirectory + $"\\{splittedCommand[2]}");
                        RecursiveDelete(directory);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Папка удалена!!!");
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        UpdateConsole();
                    }
                }
            }
            else if (splittedCommand[1].Contains("\\"))
            {
                if (!File.Exists(splittedCommand[1]))
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Нет такого файла!");
                }
                else
                {
                    string answer = " ";
                    while (answer != "y" && answer !="n")
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.WriteLine("Вы точно хотите удалить файл??? Да(y) или нет(n)??");
                        Console.SetCursorPosition(2, 22);
                        answer = Console.ReadLine();
                    }
                    if (answer ==  "y")
                    {
                        File.Delete(splittedCommand[1]);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.Write("Файл удален!!!");
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        UpdateConsole();
                    }

                }
            }
            else if (!File.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Нет такого файла!");
            }
            else
            {
                string answer = " ";
                while (answer != "y" && answer !="n")
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.WriteLine("Вы точно хотите удалить файл??? Да(y) или нет(n)??");
                    Console.SetCursorPosition(2, 22);
                    answer = Console.ReadLine();
                }
                if (answer ==  "y")
                {
                    File.Delete(currentDirectory + $"\\{splittedCommand[1]}");
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Файл удален!!!");
                }
                else
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    UpdateConsole();
                }
            }
        }
        /// <summary>
        /// метод рекурсивного удаления папки со всем содержимым
        /// </summary>
        /// <param name="baseDir">путь к директории, которую нужно удалитоь</param>
        private static void RecursiveDelete(DirectoryInfo baseDir)
        {
            DirectoryInfo[] subDirectories = baseDir.GetDirectories();
            if (subDirectories.Length > 0)
            {
                for (int i = 0; i<subDirectories.Length; i++)
                {
                    RecursiveDelete(subDirectories[i]);
                }
            }
            FileInfo[] files = baseDir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i].FullName);
            }
            Directory.Delete(baseDir.FullName);
        }
        /// <summary>
        /// метод перехода в другую директорию
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        public static void ChangeDirectory(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand.Length < 2)
            {
                currentDirectory = ROOT_DIRECTORY;
            }
            else
            {
                if (splittedCommand[1] == "..")
                {
                    string[] splittedDirectory = currentDirectory.Split("\\");
                    currentDirectory = "";
                    for (int i = 0; i < splittedDirectory.Length - 1; i++)
                    {
                        if (i == splittedDirectory.Length - 2)
                        {
                            currentDirectory += splittedDirectory[i];
                            break;
                        }
                        currentDirectory += splittedDirectory[i] + "\\";
                    }
                }
                else if (splittedCommand[1].Contains("\\") && Directory.Exists(splittedCommand[1]))
                {
                    currentDirectory = splittedCommand[1];
                }
                else if (Directory.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    currentDirectory += $"\\{splittedCommand[1]}";
                }
                else
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Папка не существует!");
                }
            }
        }
        /// <summary>
        /// метод считывания текущей позиции курсора
        /// </summary>
        /// <param name="left">расположение по горизонтали</param>
        /// <param name="top">расположение по вертикали</param>
        public static void GetCurrentCursorPosition(ref int left, ref int top)
        {
            left = Console.CursorLeft; top = Console.CursorTop;
        }
        /// <summary>
        /// метод прорисовки командной строки
        /// </summary>
        /// <param name="directory">текущая директория</param>
        /// <param name="x">координата по горизонтали</param>
        /// <param name="y">координата по вертикали</param>
        /// <param name="wigth">ширина окна</param>
        /// <param name="heigth">высота окна</param>
        public static void DrawInputCommandField(string directory, int x, int y, int wigth, int heigth)
        {
            DrawConsole(0, 30, wigth, heigth);
            Console.SetCursorPosition(x, y);
            Console.Write($"{directory}>");
        }
        /// <summary>
        /// метод прорисовки консольного окна
        /// </summary>
        /// <param name="x">координата по горизонтали</param>
        /// <param name="y">координата по вертикали</param>
        /// <param name="wigth">ширина окна</param>
        /// <param name="heigth">высота окна</param>
        public static void DrawConsole(int x, int y, int width, int heigth)
        {
            Console.SetCursorPosition(x, y);
            //Вывод шапки
            Console.Write("┌");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("─");
            }
            Console.Write("┐");

            //Вывод тела
            for (int i = 0; i < heigth; i++)
            {
                Console.Write("│");
                for (int j = 0; j < width - 2; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("│");
            }

            //Вывод подвала
            Console.Write("└");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("─");
            }
            Console.Write("┘");
        }
        /// <summary>
        /// метод записи иерархии директории в строковую переменную allDataToOneString
        /// </summary>
        /// <param name="directory">путь к искомой директории </param>
        /// <param name="indent">отступ</param>
        /// <param name="lastDirectory"> идентификатор конечной директории</param>
        public static void GetTree(DirectoryInfo directory, string indent, bool lastDirectory)
        {
            allDataToOneString += indent;
            if (lastDirectory)
            {
                allDataToOneString += "└──";
                indent += "   ";
            }
            else
            {
                allDataToOneString += "├──";
                indent += "│  ";
            }
            allDataToOneString += directory.Name + "\n";

            try
            {
                DirectoryInfo[] subDirectories = directory.GetDirectories();

                for (int i = 0; i < subDirectories.Length; i++)
                {
                    GetTree(subDirectories[i], indent, i == subDirectories.Length - 1);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// метод вывода на экран содержимого строковой переменной allDataToOneString
        /// </summary>
        /// <param name="page">номер страницы для вывода</param>
        public static void DrawToConsole(int page)
        {
            DrawConsole(0, 0, WINDOW_WIDTH, 18);

            string[] line = allDataToOneString.Split('\n');
            int linesAtPage = 18;
            int pagesQuantity = (int)Math.Ceiling(line.Length / (double)linesAtPage);

            string[,] pages = new string[pagesQuantity, linesAtPage];

            if (line.Length >= linesAtPage)
            {
                for (int i = 0; i < pages.GetLength(0); i++)
                {
                    int counter = 0;
                    for (int j = linesAtPage * i; j < linesAtPage * (i + 1); j++)
                    {
                        if (line[j] == "")
                        {
                            break;
                        }
                        pages[i, counter] = line[j];
                        counter++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < pages.GetLength(0); i++)
                {
                    int counter = 0;
                    for (int j = linesAtPage * i; j < line.Length; j++)
                    {
                        pages[i, counter] = line[j];
                        counter++;
                    }
                }
            }

            for (int i = 0; i < pages.GetLength(1); i++)
            {
                Console.SetCursorPosition(1, i + 1);
                Console.WriteLine(pages[page - 1, i]);
            }
            string currentPage = $"Страница: [{page} / {pages.GetLength(0)}]";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - currentPage.Length / 2, 19);
            Console.WriteLine(currentPage);
            allDataToOneString = "";
        }
        /// <summary>
        /// метод создания директории
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        public static void MakeDir(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand[1].Contains("\\") && (!Directory.Exists(splittedCommand[1])))
            {
                Directory.CreateDirectory(splittedCommand[1]);
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Папка создана!");
            }
            else if (splittedCommand[1].Contains("\\") && Directory.Exists(splittedCommand[1]))
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Папка уже существует!");
            }
            else if (Directory.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Папка уже существует!");
            }
            else
            {
                Directory.CreateDirectory(currentDirectory + $"\\{splittedCommand[1]}");
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Папка создана!");
            }
        }
        /// <summary>
        /// метод создания файла
        /// </summary>
        /// <param name="command">команда, введенная пользователем</param>
        public static void CreateFile(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");

            if (File.Exists(splittedCommand[1]))
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Файл уже существует!");
            }
            else if (splittedCommand[1].Contains("\\"))
            {
                string[] splittedElements = splittedCommand[1].ToString().Split("\\");
                string directoryOfFile = "";
                for (int i = 0; i < splittedElements.Length - 1; i++)
                {
                    if (i == splittedElements.Length - 2)
                    {
                        directoryOfFile += splittedElements[i];
                        break;
                    }
                    directoryOfFile += splittedElements[i] + "\\";
                }
                if (Directory.Exists(directoryOfFile) && splittedElements[splittedElements.Length - 1].Contains("."))
                {
                    File.Create(splittedCommand[1]);
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Файл создан!");
                }
                else
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Неверный путь или название файла!");
                }
            }
            else if (File.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Файл уже существует!");
            }
            else if (splittedCommand[1].Contains("."))
            {
                File.Create(currentDirectory + $"\\{splittedCommand[1]}");
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Файл создан!");
            }
            else
            {
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.Write("Неверный путь или название файла!");
            }
        }
    }
}
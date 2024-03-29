Название проекта: 	"Консольный файловый менеджер на языке программирования С#"

Описание:		Консольный файловый менеджер, предоставляющий базовые операции для
			управления файлами и каталогами, а также обеспечивающий удобный
			интерфейс для навигации и выполнения операций. 
			Функционал:
			● Перемещение между директориями.
			● Просмотр содержимого директории.
			● Просмотр информации о файлах и директориях.
			● Создание директорий и файлов.
			● Копирование и перемещение файлов.
			● Удаление файлов и директорий.
			● Просмотр содержимого файлов.
			● Редактирование файлов.			

Использование:		Команды: 
				help - вывод на экран списка доступных команд
				cd path_to_directory - перейти в указанную директорию
				cd .. - перейти на уровень выше
				cd - перейти в домашнюю директорию пользователя
				tree n_page path_to_directory - постраничный вывод списка файлов и директорий в текущей директории,
								где n_page - номер страницы для вывода
				ls -l path_to_directory_or_file - вывод информации о файле или директории
				mkdir directory_name - создать новую директорию
				touch file_name - создать новый пустой файл
				cp source_file destination_path - скопировать файл или директорию
				mv source destination_path - переместить или переименовать файл или директорию
				rm file_name - удалить файл
				rm -r directory_name - удалить директорию и её содержимое
				rmdir directory_name - удалить пустую директорию
				cat n_page file_name - вывести содержимое файла на экран,
							где n_page - номер страницы для вывода
				red file_name - открыть файл в текстовом редакторе
				echo “text” file_name - передать текст в указанный файл
			Команды поддерживают, как относительный, так и абсолютный путь к файлу

			Для быстрого ввода в командную строку, ранее введенных команд, можно использовать 
			клавиши управления курсором: вверх, вниз

			Посмотреть ошибки, полученные во время использования программы, можно в текстовом файле errors.txt, 
			расположенным в директории проекта.

Примеры использования:  Скриншоты использования программы хранятся в файле Examples.docx, расположенным в директории проекта.

Автор проекта:		Пинегин Денис Владимирович

Статус проекта:		Тестирование

Запуск:			Для использования менеджера на вашем персональном компьютере нужно 
			сначала изменить значения полей класса Programm. А именно:
			полю ROOT_DIRECTORY присвоить значение корневой директории.
 			Например:public const string ROOT_DIRECTORY = "C:\\Users\\PineGun\\source";
			полю currentDirectory сделать аналогичную процедуру.
			Предоставить правильный путь к расположению файлов SaveCommands.txt
			и Errors.txt. Оба эти файла находятся в папке с самим проектом ConsoleManagerPinegin.
			Нужно дополнить поля информацией с расположением директории проекта.
			Например: 
			public static string saveCommandsFile = "D:\\Pinegun\\ConsoleManagerPinegin\\SaveCommands.txt";
        		public static string errorsFile = "D:\\Pinegun\\ConsoleManagerPinegin\\Errors.txt";


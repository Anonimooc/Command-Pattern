using System.Text;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class InsertTextCommand : ICommand
{
    private TextEditor _textEditor;
    private string _textToInsert;

    public InsertTextCommand(TextEditor textEditor, string textToInsert)
    {
        _textEditor = textEditor;
        _textToInsert = textToInsert;
    }

    public void Execute()
    {
        _textEditor.InsertText(_textToInsert);
    }

    public void Undo()
    {
        _textEditor.DeleteText(_textToInsert.Length);
    }
}

public class DeleteTextCommand : ICommand
{
    private TextEditor _textEditor;
    private int _lengthToDelete;
    private string _deletedText;

    public DeleteTextCommand(TextEditor textEditor, int lengthToDelete)
    {
        _textEditor = textEditor;
        _lengthToDelete = lengthToDelete;
    }

    public void Execute()
    {
        _deletedText = _textEditor.DeleteText(_lengthToDelete);
    }

    public void Undo()
    {
        _textEditor.InsertText(_deletedText);
    }
}

public class TextEditor
{
    private StringBuilder _text = new StringBuilder();

    // Метод для вставки текста
    public void InsertText(string text)
    {
        _text.Append(text);
        Console.WriteLine($"Текст после вставки: {_text}");
    }

    // Метод для удаления текста
    public string DeleteText(int length)
    {
        if (length > _text.Length)
        {
            length = _text.Length;
        }

        string deletedText = _text.ToString(_text.Length - length, length);
        _text.Remove(_text.Length - length, length);
        Console.WriteLine($"Текст после удаления: {_text}");
        return deletedText;
    }

    // Метод для отображения текущего текста
    public string GetText()
    {
        return _text.ToString();
    }
}

public class CommandInvoker
{
    private Stack<ICommand> _commandHistory = new Stack<ICommand>();

    // Выполнение команды
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _commandHistory.Push(command);
    }

    // Отмена последней выполненной команды
    public void UndoLastCommand()
    {
        if (_commandHistory.Count > 0)
        {
            var command = _commandHistory.Pop();
            command.Undo();
        }
        else
        {
            Console.WriteLine("Нет команд для отмены.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Создаем экземпляр текстового редактора и инвокера
        TextEditor textEditor = new TextEditor();
        CommandInvoker invoker = new CommandInvoker();

        // Выполним несколько команд
        Console.WriteLine("Вводим текст 'Hello, World!':");
        ICommand insertCommand = new InsertTextCommand(textEditor, "Hello, World!");
        invoker.ExecuteCommand(insertCommand);

        Console.WriteLine("\nУдаляем 6 символов:");
        ICommand deleteCommand = new DeleteTextCommand(textEditor, 6);
        invoker.ExecuteCommand(deleteCommand);

        // Проверяем результат после выполнения команд
        Console.WriteLine($"\nТекущий текст: {textEditor.GetText()}");

        // Отменяем последнюю операцию (удаление текста)
        Console.WriteLine("\nОтменяем последнюю операцию (удаление текста):");
        invoker.UndoLastCommand();

        // Проверяем результат после отмены
        Console.WriteLine($"\nТекущий текст: {textEditor.GetText()}");

        // Отменяем последнюю операцию (вставку текста)
        Console.WriteLine("\nОтменяем последнюю операцию (вставку текста):");
        invoker.UndoLastCommand();

        // Проверяем результат после отмены
        Console.WriteLine($"\nТекущий текст: {textEditor.GetText()}");
    }
}

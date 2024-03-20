using Microsoft.VisualBasic.FileIO;

namespace KNN;

public class Command
{
    public string Name;
    public Type[] ArgumentTypes;
    public Delegate TargetMethod;

    public Command(string name, Type[] argTypes, Delegate targetMethod)
    {
        Name = name;
        ArgumentTypes = argTypes;
        TargetMethod = targetMethod;
    }

    public override string ToString()
    {
        return $"{Name} - Calls {TargetMethod}";
    }
}

public class Terminal
{
    public Dictionary<string, Command> Commands = new();

    public void AddCommand(Command command) => Commands.Add(command.Name, command);

    public Command? FindCommandByName(string name)
    {
        try
        {
            return Commands[name];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }
    
    public void StartLoop()
    {
        while (true)
        {
            var input = Console.ReadLine().Split(' ');
            
            
        }
    }
}
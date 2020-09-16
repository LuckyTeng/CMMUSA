namespace SoftJawCut
{
    public interface ICodeProvider
    {
        string Parse(MachineCommands machineCommands);
    }
}
using System.Collections.ObjectModel;

namespace SoftJawCut
{
    public enum MachineAction
    {
        Reset = 0,
        Rapid,
        Feed,
        Home,
        SetWCS,
        SpindleOn,
        SpindleOff,
        CoolantOn,
        CoolantOff,
        ToolChange,
        ToolHeightComp,
        OptionStop,
        ProgramStop,
        EndProgram,
        Absolute,
        Relative,
    }

    public interface IMachineCommand
    {
        MachineAction Action { get; }
    }

    public class ResetCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.Reset; } }
    }

    public class RapidCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.Rapid; } }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
    }

    public class FeedCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.Feed; } }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

        public double? F { get; set; }
    }

    public class ToolChangeCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.ToolChange; } }
        public int ToolNumber;
    }

    public class ToolHeightCompCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.ToolHeightComp; } }
        public int H;
        public double? Z;
        public int? D;
    }

    public class EndProgramCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.EndProgram; } }
    }

    public class HomeCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.Home; } }
    }

    public class SpindleOnCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.SpindleOn; } }
        public int RPM;
    }

    public class SpindleOffCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.SpindleOff; } }
    }

    public class CoolantOnCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.CoolantOn; } }
    }

    public class CoolantOffCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.CoolantOff; } }
    }

    public class WCSCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.SetWCS; } }

        /// <summary>
        /// 1 = G54 2 = G55
        /// </summary>
        public int Offset;
    }

    public class ProgramStopCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.ProgramStop; } }
    }

    public class OptionStopCommand : IMachineCommand
    {
        public MachineAction Action { get { return MachineAction.OptionStop; } }
    }

    /// <summary>
    /// A set of MachineCommand
    /// </summary>
    public class MachineCommands : Collection<IMachineCommand>
    {

    }
}

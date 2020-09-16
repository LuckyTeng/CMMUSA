using System.Text;

namespace SoftJawCut
{
    /// <summary>
    /// Convert MachineCommands to Fadal format 1 G-Code
    /// </summary>
    public class FadalGCodeProvider : ICodeProvider
    {
        public string Parse(MachineCommands machineCommands)
        {
            var sb = new StringBuilder();

            sb.AppendLine("%");
            sb.AppendLine("O00077(SOFTJAW CUT)");

            foreach (var mc in machineCommands)
            {
                switch (mc.Action)
                {
                    case MachineAction.Reset:
                        sb.AppendLine("G0 G20 G40 G49 G80 G90 G94 G17");
                        break;
                    case MachineAction.Rapid:
                        var rapid = (RapidCommand)mc;

                        sb.Append("G0");
                        if (rapid.X.HasValue)
                            sb.Append(string.Format("X{0:0.0###}", rapid.X));

                        if (rapid.Y.HasValue)
                            sb.Append(string.Format("Y{0:0.0###}", rapid.Y));

                        if (rapid.Z.HasValue)
                            sb.Append(string.Format("Z{0:0.0###}", rapid.Z));


                        sb.AppendLine();
                        break;
                    case MachineAction.Feed:
                        var feed = (FeedCommand)mc;

                        sb.Append("G1");
                        if (feed.X.HasValue)
                            sb.Append(string.Format("X{0:0.0###}", feed.X));

                        if (feed.Y.HasValue)
                            sb.Append(string.Format("Y{0:0.0###}", feed.Y));

                        if (feed.Z.HasValue)
                            sb.Append(string.Format("Z{0:0.0###}", feed.Z));

                        if (feed.F.HasValue)
                            sb.Append(string.Format("F{0:0.0###}", feed.F));

                        sb.AppendLine();
                        break;
                    case MachineAction.Home:
                        sb.AppendLine("G53 Z0."); // fadal no G90
                        break;
                    case MachineAction.SetWCS:
                        var wcsCommand = (WCSCommand)mc;

                        switch (wcsCommand.Offset)
                        {
                            case 1:
                                sb.AppendLine("E1");
                                break;
                            case 2:
                                sb.AppendLine("E2");
                                break;
                        }
                        break;
                    case MachineAction.SpindleOn:
                        var spindleOnCommand = (SpindleOnCommand)mc;
                        sb.AppendLine(string.Format("G0G90S{0:d}M3", spindleOnCommand.RPM));
                        break;
                    case MachineAction.SpindleOff:
                        sb.AppendLine("M5");
                        break;
                    case MachineAction.CoolantOn:
                        sb.AppendLine("M7");
                        break;
                    case MachineAction.CoolantOff:
                        sb.AppendLine("M9");
                        break;
                    case MachineAction.EndProgram:
                        sb.AppendLine("M0");
                        sb.AppendLine("M30"); // fadal M30 will goto x0 y0
                        break;
                    case MachineAction.ToolChange:
                        var toolChangeCommand = (ToolChangeCommand)mc;
                        sb.AppendLine(string.Format("T{0:d}M6", toolChangeCommand.ToolNumber));
                        break;
                    case MachineAction.ToolHeightComp:
                        var toolHeightComp = (ToolHeightCompCommand)mc;
                        sb.Append(string.Format("H{0:d}", toolHeightComp.H)); // fadal no use G43

                        if (toolHeightComp.Z.HasValue)
                            sb.Append(string.Format("Z{0:0.0###}", toolHeightComp.Z));

                        if (toolHeightComp.D.HasValue)
                            sb.Append(string.Format("D{0:d}", toolHeightComp.D));

                        sb.AppendLine();
                        break;
                    case MachineAction.OptionStop:
                        sb.AppendLine("M1");
                        break;
                    case MachineAction.ProgramStop:
                        sb.AppendLine("M0");
                        break;
                    default:
                        break;
                }
            }

            sb.Append("%");
            return sb.ToString();
        }
    }
}
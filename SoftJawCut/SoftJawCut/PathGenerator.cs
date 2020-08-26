using System;
using System.Collections.ObjectModel;
using System.Text;

namespace SoftJawCut
{
    /// <summary>
    /// Determines how we produce path.
    /// </summary>
    public class PathArgument
    {
        public int RoughDia;   // thouzandth inch
        public int FinishDia;  // thouzandth inch
        public int Deep;       // thouzandth inch
        public int UpSize;     // thouzandth inch
        public int DownSize;   // thouzandth inch
        public int CutLength;  // thouzandth inch
    }

    /// <summary>
    /// Build PathArgument
    /// </summary>
    public class PathArgumentBuilder
    {
        public static PathArgument Build(int roughDia, int finishDia, int deep, int upSize, int downSize, int cutLength)
        {
            var pa = new PathArgument();
            pa.RoughDia = roughDia;
            pa.FinishDia = finishDia;
            pa.Deep = deep;
            pa.UpSize = upSize;
            pa.DownSize = downSize;
            pa.CutLength = cutLength;

            return pa;
        }
    }

    /// <summary>
    /// Produce Path from PathArgument
    /// </summary>
    public class PathGenerator
    {
        int STEP_OVER_Y = 15;
        int STEP_OVER_Z = 250;
        int FINISH_TO_LEAVE = 8; // 8 thou
        double CLEARANCE_HEIGHT = 0.15;

        public MachineCommands Generator(PathArgument pathArgument)
        {
            var mcs = new MachineCommands();

            // first, we reset machine
            mcs.Add(new ResetCommand());

            // next, home z axis
            mcs.Add(new HomeCommand());

            // change tool 10 for rough;
            mcs.Add(new ToolChangeCommand { ToolNumber = 10 });

            // spindle on, coolant on
            mcs.Add(new SpindleOnCommand { RPM = getRPM(pathArgument) });
            mcs.Add(new CoolantOnCommand());

            // init pos
            mcs.Add(new WCSCommand { Offset = 1 });
            mcs.Add(new RapidCommand { X = -0.500, Y = getRoughCompY(pathArgument) }); ;

            mcs.Add(new ToolHeightCompCommand { H = 10, D = 10, Z = CLEARANCE_HEIGHT });
            // check tool height
            mcs.Add(new ProgramStopCommand());

            roughtPath(pathArgument, mcs, CutDir.Top);

            mcs.Add(new RapidCommand { Z = CLEARANCE_HEIGHT });

            mcs.Add(new WCSCommand { Offset = 2 });

            roughtPath(pathArgument, mcs, CutDir.Bottom);

            mcs.Add(new RapidCommand { Z = CLEARANCE_HEIGHT });

            // option stop before tool change
            mcs.Add(new OptionStopCommand());

            // change tool 9 for finish;
            mcs.Add(new ToolChangeCommand { ToolNumber = 9 });

            // spindle on, coolant on
            mcs.Add(new SpindleOnCommand { RPM = getFinishRPM(pathArgument) });
            mcs.Add(new CoolantOnCommand());

            // cal finish path
            mcs.Add(new WCSCommand { Offset = 1 });
            mcs.Add(new RapidCommand { X = -0.500, Y = getFinishCompY(pathArgument) }); ;

            mcs.Add(new ToolHeightCompCommand { H = 9, D = 9, Z = CLEARANCE_HEIGHT });
            finishPath(pathArgument, mcs, CutDir.Top);

            mcs.Add(new RapidCommand { Z = CLEARANCE_HEIGHT });

            mcs.Add(new WCSCommand { Offset = 2 });

            finishPath(pathArgument, mcs, CutDir.Bottom);

            // GOTO CLEANRANCE
            mcs.Add(new RapidCommand { Z = CLEARANCE_HEIGHT });

            // end program
            mcs.Add(new SpindleOffCommand());
            mcs.Add(new CoolantOffCommand());
            mcs.Add(new HomeCommand());
            mcs.Add(new ToolChangeCommand { ToolNumber = 8 });
            mcs.Add(new WCSCommand { Offset = 1 });
            mcs.Add(new RapidCommand { X = 0, Y = 5 });
            mcs.Add(new EndProgramCommand());
            return mcs;
        }

        enum CutDir
        {
            Top = 0,
            Bottom,
        }

        private void finishPath(PathArgument pathArgument, MachineCommands mcs, CutDir cd)
        {
            // finish loop y first, if not last y goto deep z directly(cuz rough already cut)
            double sign = cd == CutDir.Top ? 1.0 : -1.0;
            double initZ = 0.0;
            double targetY = sign * (cd == CutDir.Top ? pathArgument.UpSize : pathArgument.DownSize) / 1000.0;
            double initX = -0.500;
            double yStep = sign * (pathArgument.RoughDia - STEP_OVER_Y) / 1000.0; // step to go
            double targetZ = -pathArgument.Deep / 1000.0;
            int xDirection = 0; // 0 to right 1 to left

            // get abs y
            double initY = sign * getFinishCompY(pathArgument);

            // move to start pos
            mcs.Add(new RapidCommand { X = initX, Y = sign * getFinishCompY(pathArgument) });

            // y loop
            while (true)
            {
                double y = initY - (sign * getFinishCompY(pathArgument));
                if (cd == CutDir.Bottom)
                {
                    if (y <= targetY)
                        break;
                }
                else
                {
                    if (y >= targetY)
                        break;
                }

                initY += yStep;
                if (cd == CutDir.Bottom)
                    initY = Math.Max(initY - (sign * getFinishCompY(pathArgument)), targetY) + (sign * getFinishCompY(pathArgument));
                else
                    initY = Math.Min(initY - (sign * getFinishCompY(pathArgument)), targetY) + (sign * getFinishCompY(pathArgument));

                mcs.Add(new FeedCommand { F = getRoughFeed(pathArgument), Y = initY });

                y = initY - (sign * getFinishCompY(pathArgument));
                if ( y != targetY ) // not last Y
                {
                    // move to last Z
                    if (initZ != targetZ)
                    {
                        mcs.Add(new FeedCommand { F = 10.0, Z = targetZ });
                        initZ = targetZ;
                    }

                    if (xDirection == 0)
                    {
                        initX += pathArgument.CutLength / 1000.0;
                        xDirection = 1;
                    }

                    else
                    {
                        initX -= pathArgument.CutLength / 1000.0;
                        xDirection = 0;
                    }

                    mcs.Add(new FeedCommand { F = getFinishFeed(pathArgument), X = initX });
                }
                else
                {
                    initZ = 0.0; // cut from z = 0
                    mcs.Add(new RapidCommand { Z = CLEARANCE_HEIGHT });
                    while (true)
                    {
                        if (initZ <= targetZ) // reach target
                            break;

                        initZ += (-STEP_OVER_Z / 1000.0);
                        // boundary
                        initZ = Math.Max(initZ, targetZ);

                        mcs.Add(new FeedCommand { F = 10.0, Z = initZ });

                        if (xDirection == 0)
                        {
                            initX += pathArgument.CutLength / 1000.0;
                            xDirection = 1;
                        }

                        else
                        {
                            initX -= pathArgument.CutLength / 1000.0;
                            xDirection = 0;
                        }

                        mcs.Add(new FeedCommand { F = getFinishFeed(pathArgument), X = initX });
                    }
                }
                    
            }
            
        }

        /// <summary>
        /// Calculate rought path.
        /// </summary>
        private void roughtPath(PathArgument pathArgument, MachineCommands mcs, CutDir cd)
        {
            // calc rough path
            double sign = cd == CutDir.Top ? 1.0 : -1.0;
            double initZ = 0.0;
            double targetY = sign * ((cd == CutDir.Top ? pathArgument.UpSize : pathArgument.DownSize) - FINISH_TO_LEAVE) / 1000.0;
            double initX = -0.500;
            double yStep = sign * (pathArgument.RoughDia - STEP_OVER_Y) / 1000.0; // step to go
            double targetZ = -(pathArgument.Deep - FINISH_TO_LEAVE) / 1000.0;
            int xDirection = 0; // 0 to right 1 to left

            bool retractNeeded = false;

            // deep loop
            while (true)
            {
                if (initZ <= targetZ) // reach target
                    break;

                if (retractNeeded)
                    mcs.Add(new RapidCommand { Z = CLEARANCE_HEIGHT });

                // move to start pos
                mcs.Add(new RapidCommand { X = initX, Y = sign * getRoughCompY(pathArgument) });

                initZ += (-STEP_OVER_Z / 1000.0);
                // boundary
                initZ = Math.Max(initZ, targetZ);

                mcs.Add(new FeedCommand { F = 10.0, Z = initZ });

                // get abs y
                double initY = sign * getRoughCompY(pathArgument);

                // y loop
                while (true)
                {
                    double y = initY - (sign * getRoughCompY(pathArgument));
                    if (cd == CutDir.Bottom)
                    {
                        if (y <= targetY)
                            break;
                    }
                    else
                    {
                        if (y >= targetY)
                            break;
                    }
                    
                    retractNeeded = true;

                    initY += yStep;
                    if ( cd == CutDir.Bottom )
                        initY = Math.Max(initY - (sign * getRoughCompY(pathArgument)), targetY) + (sign * getRoughCompY(pathArgument));
                    else
                        initY = Math.Min(initY - (sign * getRoughCompY(pathArgument)), targetY) + (sign * getRoughCompY(pathArgument));

                    mcs.Add(new FeedCommand { F = getRoughFeed(pathArgument), Y = initY });

                    if (xDirection == 0)
                    {
                        initX += pathArgument.CutLength / 1000.0;
                        xDirection = 1;
                    }

                    else
                    {
                        initX -= pathArgument.CutLength / 1000.0;
                        xDirection = 0;
                    }

                    mcs.Add(new FeedCommand { F = getRoughFeed(pathArgument), X = initX });
                }
            }
        }

        private double getRoughCompY(PathArgument pathArgument)
        {
            return -(pathArgument.RoughDia - 200) / 2000.0;
        }

        private double getFinishCompY(PathArgument pathArgument)
        {
            return -(pathArgument.FinishDia - 200) / 2000.0;
        }

        private double getRoughFeed(PathArgument pathArgument)
        {
            if (pathArgument.RoughDia == 250)
                return 20.0;
            else if (pathArgument.RoughDia == 375)
                return 25.0;
            else if (pathArgument.RoughDia == 500)
                return 35.0;

            return 20.0;
        }

        private double getFinishFeed(PathArgument pathArgument)
        {
            if (pathArgument.FinishDia == 250)
                return 6.0;
            else if (pathArgument.RoughDia == 375)
                return 8.0;
            else if (pathArgument.RoughDia == 500)
                return 10.0;

            return 6.0;
        }

        private int getRPM(PathArgument pathArgument)
        {
            int dia = pathArgument.RoughDia;
            if (dia == 250)
                return 6500;
            else if (dia == 375)
                return 6500;
            else if (dia == 500)
                return 7000;

            return 6000;
        }

        private int getFinishRPM(PathArgument pathArgument)
        {
            int dia = pathArgument.FinishDia;
            if (dia == 250)
                return 6500;
            else if (dia == 375)
                return 6500;
            else if (dia == 500)
                return 7000;

            return 6000;
        }
    }

    /// <summary>
    /// Convert MachineCommands to Hass G-Code
    /// </summary>
    public class HassGCodeProvider
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
                        sb.AppendLine("G53 G90 Z0.");
                        break;
                    case MachineAction.SetWCS:
                        var wcsCommand = (WCSCommand)mc;

                        switch (wcsCommand.Offset)
                        {
                            case 1:
                                sb.AppendLine("G54");
                                break;
                            case 2:
                                sb.AppendLine("G55");
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
                        sb.AppendLine("M8");
                        break;
                    case MachineAction.CoolantOff:
                        sb.AppendLine("M9");
                        break;
                    case MachineAction.EndProgram:
                        sb.AppendLine("M30");
                        break;
                    case MachineAction.ToolChange:
                        var toolChangeCommand = (ToolChangeCommand)mc;
                        sb.AppendLine(string.Format("T{0:d}M6", toolChangeCommand.ToolNumber));
                        break;
                    case MachineAction.ToolHeightComp:
                        var toolHeightComp = (ToolHeightCompCommand)mc;
                        sb.Append(string.Format("G43H{0:d}", toolHeightComp.H));
                        
                        if ( toolHeightComp.Z.HasValue )
                            sb.Append(string.Format("Z{0:0.0###}", toolHeightComp.Z));

                        if ( toolHeightComp.D.HasValue )
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

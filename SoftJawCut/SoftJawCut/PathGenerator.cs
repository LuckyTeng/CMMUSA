using System;
using System.Collections.ObjectModel;

namespace SoftJawCut
{
    /// <summary>
    /// Determines how we produce path.
    /// </summary>
    public class PathArgument
    {
        public int RoughDia;   // thousandth inch
        public int FinishDia;  // thousandth inch
        public int Deep;       // thousandth inch
        public int UpSize;     // thousandth inch
        public int DownSize;   // thousandth inch
        public int CutLength;  // thousandth inch
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
            mcs.Add(new SpindleOnCommand { RPM = getRoughRPM(pathArgument) });
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
            double yStep = sign * (pathArgument.FinishDia - STEP_OVER_Y) / 1000.0; // step to go
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

                mcs.Add(new FeedCommand { F = getFinishFeed(pathArgument), Y = initY });

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

        /// <summary>
        /// Get Rough Compensation from Rought
        /// </summary>
        /// <param name="pathArgument"></param>
        /// <returns></returns>
        private double getRoughCompY(PathArgument pathArgument)
        {
            // always zero
            return 0;//-(pathArgument.RoughDia - pathArgument.RoughDia) / 2000.0;
        }

        private double getFinishCompY(PathArgument pathArgument)
        {
            return -(pathArgument.FinishDia - pathArgument.RoughDia) / 2000.0;
        }

        private double getRoughFeed(PathArgument pathArgument)
        {
            if (pathArgument.RoughDia == 250)
                return 30.0;
            else if (pathArgument.RoughDia == 375)
                return 25.0;
            else if (pathArgument.RoughDia == 500)
                return 36.0;

            return 20.0;
        }

        private double getFinishFeed(PathArgument pathArgument)
        {
            if (pathArgument.FinishDia == 250)
                return 40.0;
            else if (pathArgument.RoughDia == 375)
                return 8.0;
            else if (pathArgument.RoughDia == 500)
                return 36.0;

            return 6.0;
        }

        private int getRoughRPM(PathArgument pathArgument)
        {
            int dia = pathArgument.RoughDia;
            if (dia == 250)
                return 7500;
            else if (dia == 375)
                return 4000;
            else if (dia == 500)
                return 3000;

            return 6000;
        }

        private int getFinishRPM(PathArgument pathArgument)
        {
            int dia = pathArgument.FinishDia;
            if (dia == 250)
                return 6000;
            else if (dia == 375)
                return 4000;
            else if (dia == 500)
                return 3000;

            return 6000;
        }
    }
}

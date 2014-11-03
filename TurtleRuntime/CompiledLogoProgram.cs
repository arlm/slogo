using System;
using System.Collections.Generic;
using System.Text;

namespace TurtleRuntime
{
    public class CompiledLogoProgram
    {
        public Turtle _turtle = null;
        public DateTime _startTime;

        public void _initialize(Turtle t) 
        { 
            _turtle = t;
            _startTime = DateTime.Now;
        }

        public delegate int DebugStep(int line);
        public event DebugStep OnDebugStep;

        public void _debug_step(int line)
        {
            if (OnDebugStep != null)
            {
                if (OnDebugStep(line) == -1)
                    throw new TurtleStopException();
            }
                
        }
    }
}

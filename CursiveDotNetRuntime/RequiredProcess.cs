﻿using Cursive.Debugging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursive
{
    public class RequiredProcess : Process
    {
        public RequiredProcess(string name, string description, IReadOnlyCollection<ValueKey> inputs, IReadOnlyCollection<ValueKey> outputs, IReadOnlyCollection<string> returnPaths, string folder = null)
            : base(name, description, folder)
        {
            Inputs = inputs;
            Outputs = outputs;
            ReturnPaths = returnPaths;
        }
        
        internal UserProcess Implementation { get; set; }
        public override IReadOnlyCollection<string> ReturnPaths { get; }
        public override IReadOnlyCollection<ValueKey> Inputs { get; }
        public override IReadOnlyCollection<ValueKey> Outputs { get; }

        internal override async Task<Response> Run(ValueSet inputs, CallStack stack)
        {
            return await Implementation.Run(inputs, stack);
        }

        public async Task<Response> Run(ValueSet inputs)
        {
            var stack = new RunCallStack();
            return await Run(inputs, stack);
        }

        public async Task<Response> Debug(ValueSet inputs, Func<DebugStackFrame, Task> enteredStep)
        {
            var stack = new DebugCallStack(enteredStep);
            return await Run(inputs, stack);
        }
    }
}
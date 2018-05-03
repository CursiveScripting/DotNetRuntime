﻿using Cursive.Debugging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursive
{
    public abstract class Step
    {
        protected Step(string name)
        {
            Name = name;
        }

        public abstract Task<Step> Run(ValueSet variables, CallStack stack);

        public string Name { get; }
        protected internal ValueSet FixedInputs { get; } = new ValueSet();
        protected internal Dictionary<ValueKey, ValueKey> InputMapping { get; } = new Dictionary<ValueKey, ValueKey>();
        protected internal Dictionary<ValueKey, ValueKey> OutputMapping { get; } = new Dictionary<ValueKey, ValueKey>();

        internal void SetInputParameter(ValueKey parameter, object value)
        {
            FixedInputs[parameter] = value;
        }

        internal void MapInputParameter(ValueKey parameter, ValueKey source)
        {
            InputMapping[parameter] = source;
        }

        internal void MapOutputParameter(ValueKey parameter, ValueKey destination)
        {
            OutputMapping[parameter] = destination;
        }
    }
}
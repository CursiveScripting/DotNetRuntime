﻿using System.Collections.Generic;

namespace Cursive
{
    public class Model
    {
        Dictionary<string, object> elements = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                object o;
                if (elements.TryGetValue(key, out o))
                    return o;
                return null;
            }
            set
            {
                elements[key] = value;
            }
        }

        internal Model Clone()
        {
            Model other = new Model();
            foreach (var kvp in elements)
                other[kvp.Key] = kvp.Value;
            return other;
        }
    }
}
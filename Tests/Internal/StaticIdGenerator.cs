using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mjml.Net;

namespace Tests.Internal
{
    internal sealed class StaticIdGenerator : IIdGenerator
    {
        private readonly string[] values;
        private int position = -1;

        public StaticIdGenerator(params string[] values)
        {
            this.values = values;
        }

        public string Next()
        {
            position++;

            if (position == values.Length)
            {
                position = 0;
            }

            return values[position];
        }
    }
}

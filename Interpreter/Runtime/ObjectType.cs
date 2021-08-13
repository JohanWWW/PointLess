﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public enum ObjectType
    {
        Object,
        Method,
        MethodData,
        String,
        ArbitraryBitInteger,
        ArbitraryPrecisionDecimal,
        Boolean,
        Void,
        Array,
        Dictionary,
        UnsignedByte
    }
}

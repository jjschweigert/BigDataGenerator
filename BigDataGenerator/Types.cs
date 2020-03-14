using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataGenerator
{
    public static class Types
    {
        public enum HiveNumerics_T
        {
            TinyInteger,
            SmallInteger,
            Integer,
            BigInteger,
            Float,
            Double,
            Decimal
        }

        public enum HiveDateTimes_T
        {
            Timestamp,
            Date
        }

        public enum HiveStrings_T
        {
            String,
            VarChar,
            Char
        }

        public enum HiveMisc_T
        {
            Boolean
        }


    }
}

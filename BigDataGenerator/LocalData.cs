using System;
using System.Collections.Generic;
using System.Text;

namespace BigDataGenerator
{
    public class LocalData
    {
        public Dictionary<Types.HiveNumerics_T, Type> HiveNumeric_T_To_ApplicationType = new Dictionary<Types.HiveNumerics_T, Type>
        {
            { Types.HiveNumerics_T.TinyInteger, typeof(sbyte) },
            { Types.HiveNumerics_T.SmallInteger, typeof(short) },
            { Types.HiveNumerics_T.Integer, typeof(int) },
            { Types.HiveNumerics_T.BigInteger, typeof(long) },
            { Types.HiveNumerics_T.Float, typeof(float) },
            { Types.HiveNumerics_T.Double, typeof(double) },
            { Types.HiveNumerics_T.Decimal, typeof(decimal) }
        };

        public Dictionary<Types.HiveDateTimes_T, Type> HiveDateTime_T_To_ApplicationType = new Dictionary<Types.HiveDateTimes_T, Type>
        {
            { Types.HiveDateTimes_T.Timestamp, typeof(TimeSpan) },
            { Types.HiveDateTimes_T.Date, typeof(DateTime) }
        };

        public Dictionary<Types.HiveStrings_T, Type> HiveString_T_To_ApplicationType = new Dictionary<Types.HiveStrings_T, Type>
        {
            { Types.HiveStrings_T.String, typeof(string) },
            { Types.HiveStrings_T.VarChar, typeof(char[]) },
            { Types.HiveStrings_T.Char, typeof(char) }
        };

        public Dictionary<Types.HiveMisc_T, Type> HiveMisc_T_To_ApplicationType = new Dictionary<Types.HiveMisc_T, Type>
        {
            { Types.HiveMisc_T.Boolean, typeof(bool) }
        };
    }
}

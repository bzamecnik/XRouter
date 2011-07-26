﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectConfigurator.ValueValidators
{
    public class RangeValidatorAttribute : ValueValidatorAttribute
    {
        private static readonly string DefaultErrorDescription = "Value {0} is not within allowed range. Minimal values is {1}. Maximal value is {2}.";
        private static readonly string InvalidRangeTypeErrorDescription = "Invalid type of range values. Exptected {0} but is {1}.";
        private static readonly string InvalidValueTypeErrorDescription = "Value type {0} is invalid. Expected a numeric type.";

        private static readonly Type[] TypesConvertibleToDecimal = {
            typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64), 
            typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64),
            typeof(Decimal)
        };

        public object Min { get; private set; }

        public object Max { get; private set; }

        public string ErrorDescription { get; private set; }

        public RangeValidatorAttribute(int min, int max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(int min, int max, string errorDescription) 
            : this((object)min, (object)max, errorDescription)
        {
        }

        public RangeValidatorAttribute(long min, long max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(long min, long max, string errorDescription)
            : this((object)min, (object)max, errorDescription)
        {
        }

        public RangeValidatorAttribute(uint min, uint max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(uint min, uint max, string errorDescription)
            : this((object)min, (object)max, errorDescription)
        {
        }

        public RangeValidatorAttribute(ulong min, ulong max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(ulong min, ulong max, string errorDescription)
            : this((object)min, (object)max, errorDescription)
        {
        }

        public RangeValidatorAttribute(decimal min, decimal max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(decimal min, decimal max, string errorDescription)
            : this((object)min, (object)max, errorDescription)
        {
        }

        public RangeValidatorAttribute(float min, float max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(float min, float max, string errorDescription)
            : this((object)min, (object)max, errorDescription)
        {
        }

        public RangeValidatorAttribute(double min, double max)
            : this((object)min, (object)max, null)
        {
        }

        public RangeValidatorAttribute(double min, double max, string errorDescription)
            : this((object)min, (object)max, errorDescription)
        {
        }

        private RangeValidatorAttribute(object min, object max, string errorDescription)
        {
            Min = min;
            Max = max;
            ErrorDescription = errorDescription ?? DefaultErrorDescription;
        }

        public override bool IsValid(object value, out string errorDescription)
        {
            if (value == null) {
                errorDescription = string.Format(InvalidValueTypeErrorDescription, string.Empty);
                return false;
            }

            if ((value is float) || (value is double)) {
                double valueInDouble = Convert.ToDouble(value);
                if ((Min is float) || (Min is double)) {
                    double minInDouble = Convert.ToDouble(Min);
                    double maxInDouble = Convert.ToDouble(Max);
                    if ((valueInDouble >= minInDouble) && (valueInDouble <= maxInDouble)) {
                        errorDescription = null;
                        return true;
                    } else {
                        errorDescription = string.Format(ErrorDescription, value, Min, Max);
                        return false;
                    }
                } else {
                    errorDescription = string.Format(InvalidRangeTypeErrorDescription, value.GetType(), Min.GetType());
                    return false;
                }
            } else if (IsConvertibleToDecimal(value.GetType())) {
                decimal valueInDecimal = Convert.ToDecimal(value);
                if (IsConvertibleToDecimal(Min.GetType())) {
                    decimal minInDecimal = Convert.ToDecimal(Min);
                    decimal maxInDecimal = Convert.ToDecimal(Max);
                    if ((valueInDecimal >= minInDecimal) && (valueInDecimal <= maxInDecimal)) {
                        errorDescription = null;
                        return true;
                    } else {
                        errorDescription = string.Format(ErrorDescription, value, Min, Max);
                        return false;
                    }
                } else {
                    errorDescription = string.Format(InvalidRangeTypeErrorDescription, value.GetType(), Min.GetType());
                    return false;
                }
            } else {
                errorDescription = string.Format(InvalidValueTypeErrorDescription, value.GetType());
                return false;
            }
        }

        private bool IsConvertibleToDecimal(Type type)
        {
            return TypesConvertibleToDecimal.Contains(type);
        }
    }
}

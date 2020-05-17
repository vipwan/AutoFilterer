﻿using AutoFilterer.Abstractions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types
{
    [Obsolete("This type is obsolete. Use OperatorFilter<T> instead of this. Because this Range type is too heavy and performanceless. OperatorFilter has more dynamic options.")]
    public partial class Range<T> : IRange<T>, IRange, IEquatable<string>, IFormattable
        where T : struct, IComparable
    {
        public Range()
        {
        }

        public Range(string value)
        {
            var parsed = Parse<T>(value);
            this.Min = parsed.Min;
            this.Max = parsed.Max;
        }

        public Range(T? min, T? max) : this()
        {
            Min = min;
            Max = max;
        }

        public T? Min { get; set; }
        public T? Max { get; set; }

        IComparable IRange.Min => Min;
        IComparable IRange.Max => Max;

        public static implicit operator Range<T>(string val)
        {
            return Parse<T>(val);
        }

        public static Range<TParam> Parse<TParam>(string value) where TParam : struct, IComparable
        {
            var splitted = value.Split(' ');

            return new Range<TParam>(
                        splitted[0] == null || splitted[0] == "-" ? default(TParam) : (TParam)Convert.ChangeType(splitted[0], typeof(TParam)),
                        splitted[1] == null || splitted[1] == "-" ? default(TParam) : (TParam)Convert.ChangeType(splitted[1], typeof(TParam))
                        );
        }

        public static explicit operator string(Range<T> val)
        {
            return val.ToString();
        }

        public override string ToString()
        {
            return $"{this.Min?.ToString() ?? "-"} {this.Max?.ToString() ?? "-"}";
        }

        public Expression BuildExpression(Expression body, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            return GetRangeComparison();

            BinaryExpression GetRangeComparison()
            {
                BinaryExpression minExp = default, maxExp = default;

                if (Min != null)
                {
                    minExp = Expression.GreaterThanOrEqual(
                               Expression.Property(body, targetProperty.Name),
                               Expression.Constant(Min));
                    if (Max == null)
                        return minExp;
                }

                if (Max != null)
                {
                    maxExp = Expression.LessThanOrEqual(
                                Expression.Property(body, targetProperty.Name),
                                Expression.Constant(Max));
                    if (Min == null)
                        return maxExp;
                }

                return Expression.And(minExp, maxExp);
            }
        }

        public bool Equals(string other)
        {
            return this.ToString() == other;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString();
        }
    }
}

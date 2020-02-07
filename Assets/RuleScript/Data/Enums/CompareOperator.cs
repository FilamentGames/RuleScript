using System;
using BeauUtil;

namespace RuleScript.Data
{
    /// <summary>
    /// Comparison operator.
    /// </summary>
    public enum CompareOperator : byte
    {
        // binary
        LessThanOrEqualTo = 0,
        LessThan = 1,
        EqualTo = 2,
        NotEqualTo = 3,
        GreaterThan = 4,
        GreaterThanOrEqualTo = 5,

        // unary
        True = 8,
        False = 9,

        // string-specific
        Contains = 10,
        StartsWith = 11,
        EndsWith = 12,
        DoesNotContain = 13,
        DoesNotStartWith = 14,
        DoesNotEndWith = 15,
        IsEmpty = 16,
        IsNotEmpty = 17,
        Matches = 18,
        DoesNotMatch = 19
    }

    /// <summary>
    /// Extension methods for the CompareOperator operator.
    /// </summary>
    static public class ComparisonExtensions
    {
        #region Evaluation

        static public bool Evaluate(this CompareOperator inComparison, int inCheck, int inValue)
        {
            switch (inComparison)
            {
                case CompareOperator.LessThanOrEqualTo:
                    return inCheck <= inValue;
                case CompareOperator.LessThan:
                    return inCheck < inValue;
                case CompareOperator.EqualTo:
                    return inCheck == inValue;
                case CompareOperator.NotEqualTo:
                    return inCheck != inValue;
                case CompareOperator.GreaterThan:
                    return inCheck > inValue;
                case CompareOperator.GreaterThanOrEqualTo:
                    return inCheck >= inValue;

                case CompareOperator.True:
                    return inCheck > 0;
                case CompareOperator.False:
                    return inCheck <= 0;
                default:
                    throw InvalidComparisonException(inComparison);
            }
        }

        static public bool Evaluate(this CompareOperator inComparison, float inCheck, float inValue)
        {
            switch (inComparison)
            {
                case CompareOperator.LessThanOrEqualTo:
                    return inCheck <= inValue;
                case CompareOperator.LessThan:
                    return inCheck < inValue;
                case CompareOperator.EqualTo:
                    return inCheck == inValue;
                case CompareOperator.NotEqualTo:
                    return inCheck != inValue;
                case CompareOperator.GreaterThan:
                    return inCheck > inValue;
                case CompareOperator.GreaterThanOrEqualTo:
                    return inCheck >= inValue;

                case CompareOperator.True:
                    return inCheck > 0;
                case CompareOperator.False:
                    return inCheck <= 0;
                default:
                    throw InvalidComparisonException(inComparison);
            }
        }

        static public bool Evaluate(this CompareOperator inComparison, bool inCheck, bool inValue)
        {
            switch (inComparison)
            {
                case CompareOperator.EqualTo:
                    return inCheck == inValue;
                case CompareOperator.NotEqualTo:
                    return inCheck != inValue;

                case CompareOperator.True:
                    return inCheck;
                case CompareOperator.False:
                    return !inCheck;
                default:
                    throw InvalidComparisonException(inComparison);
            }
        }

        static public bool Evaluate(this CompareOperator inComparison, string inCheck, string inValue)
        {
            inCheck = inCheck ?? string.Empty;
            inValue = inValue ?? string.Empty;

            switch (inComparison)
            {
                case CompareOperator.EqualTo:
                    return inCheck == inValue;
                case CompareOperator.NotEqualTo:
                    return inCheck != inValue;

                case CompareOperator.True:
                    return !string.IsNullOrEmpty(inCheck);
                case CompareOperator.False:
                    return string.IsNullOrEmpty(inCheck);

                case CompareOperator.Contains:
                    return inCheck.Contains(inValue);
                case CompareOperator.StartsWith:
                    return inCheck.StartsWith(inValue);
                case CompareOperator.EndsWith:
                    return inCheck.EndsWith(inValue);
                case CompareOperator.DoesNotContain:
                    return !inCheck.Contains(inValue);
                case CompareOperator.DoesNotStartWith:
                    return !inCheck.StartsWith(inValue);
                case CompareOperator.DoesNotEndWith:
                    return !inCheck.EndsWith(inValue);
                case CompareOperator.IsEmpty:
                    return string.IsNullOrEmpty(inValue);
                case CompareOperator.IsNotEmpty:
                    return !string.IsNullOrEmpty(inValue);
                case CompareOperator.Matches:
                    return ScriptUtils.StringMatch(inCheck, inValue);
                case CompareOperator.DoesNotMatch:
                    return !ScriptUtils.StringMatch(inCheck, inValue);
                default:
                    throw InvalidComparisonException(inComparison);
            }
        }

        static public bool EvaluateComparable<T>(this CompareOperator inComparison, T inCheck, T inValue) where T : IComparable<T>
        {
            int compare = inCheck.CompareTo(inValue);
            return Evaluate(inComparison, compare, 0);
        }

        static public bool EvaluateEquatable<T>(this CompareOperator inComparison, T inCheck, T inValue) where T : IEquatable<T>
        {
            bool bEquals = inCheck.Equals(inValue);
            return Evaluate(inComparison, bEquals, true);
        }

        static public bool EvaluateReferenceEquals(this CompareOperator inComparison, object inCheck, object inValue)
        {
            bool bEquals = object.ReferenceEquals(inCheck, inValue);
            return Evaluate(inComparison, bEquals, true);
        }

        #endregion // Evaluation

        #region Properties

        static public string Symbol(this CompareOperator inComparison)
        {
            switch (inComparison)
            {
                case CompareOperator.LessThanOrEqualTo:
                    return "<=";
                case CompareOperator.LessThan:
                    return "<";
                case CompareOperator.EqualTo:
                    return "==";
                case CompareOperator.NotEqualTo:
                    return "!=";
                case CompareOperator.GreaterThan:
                    return ">";
                case CompareOperator.GreaterThanOrEqualTo:
                    return ">=";

                default:
                    return Name(inComparison);
            }
        }

        static public string Name(this CompareOperator inComparison)
        {
            switch (inComparison)
            {
                case CompareOperator.LessThanOrEqualTo:
                    return "Less Than or Equal To";
                case CompareOperator.LessThan:
                    return "Less Than";
                case CompareOperator.EqualTo:
                    return "Equal To";
                case CompareOperator.NotEqualTo:
                    return "Not Equal To";
                case CompareOperator.GreaterThan:
                    return "Greater Than";
                case CompareOperator.GreaterThanOrEqualTo:
                    return "Greater Than Or Equal To";

                case CompareOperator.True:
                    return "Is True";
                case CompareOperator.False:
                    return "Is False";

                case CompareOperator.Contains:
                    return "Contains";
                case CompareOperator.StartsWith:
                    return "Starts With";
                case CompareOperator.EndsWith:
                    return "Ends With";
                case CompareOperator.DoesNotContain:
                    return "Does Not Contain";
                case CompareOperator.DoesNotStartWith:
                    return "Does Not Start With";
                case CompareOperator.DoesNotEndWith:
                    return "Does Not End With";
                case CompareOperator.IsEmpty:
                    return "Is Empty";
                case CompareOperator.IsNotEmpty:
                    return "Is Not Empty";
                case CompareOperator.Matches:
                    return "Matches";
                case CompareOperator.DoesNotMatch:
                    return "Does Not Match";

                default:
                    throw new ArgumentException("Unknown comparison type " + inComparison, "inComparison");
            }
        }

        static public bool IsBinary(this CompareOperator inComparison)
        {
            switch (inComparison)
            {
                case CompareOperator.LessThan:
                case CompareOperator.LessThanOrEqualTo:
                case CompareOperator.EqualTo:
                case CompareOperator.NotEqualTo:
                case CompareOperator.GreaterThan:
                case CompareOperator.GreaterThanOrEqualTo:
                case CompareOperator.Contains:
                case CompareOperator.StartsWith:
                case CompareOperator.EndsWith:
                case CompareOperator.DoesNotContain:
                case CompareOperator.DoesNotStartWith:
                case CompareOperator.DoesNotEndWith:
                case CompareOperator.Matches:
                case CompareOperator.DoesNotMatch:
                    return true;

                default:
                    return false;
            }
        }

        static public bool IsUnary(this CompareOperator inComparison)
        {
            switch (inComparison)
            {
                case CompareOperator.True:
                case CompareOperator.False:
                case CompareOperator.IsEmpty:
                case CompareOperator.IsNotEmpty:
                    return true;

                default:
                    return false;
            }
        }

        #endregion // Properties

        static private ArgumentException InvalidComparisonException(CompareOperator inComparison)
        {
            return new ArgumentException("Invalid comparison type " + inComparison, "inComparison");
        }
    }
}
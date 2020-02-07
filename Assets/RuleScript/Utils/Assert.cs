#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEVELOPMENT
#endif // DEVELOPMENT

using System;
using System.Diagnostics;

namespace RuleScript
{
    static internal class Assert
    {
        private class AssertException : Exception
        {
            private const string ASSERT_PREFIX = "[Assert Failed]";

            public AssertException(string inMessage)
                : base(ASSERT_PREFIX + "\n" + inMessage)
            { }
        }

        [Conditional("DEVELOPMENT")]
        static public void True(bool inbCondition)
        {
            if (!inbCondition)
            {
                OnFailure("Value was false");
            }
        }

        [Conditional("DEVELOPMENT")]
        static public void True(bool inbCondition, string inMessage)
        {
            if (!inbCondition)
            {
                OnFailure(inMessage);
            }
        }

        [Conditional("DEVELOPMENT")]
        static public void True(bool inbCondition, string inMessage, params object[] inMessageParams)
        {
            if (!inbCondition)
            {
                OnFailure(String.Format(inMessage, inMessageParams));
            }
        }

        [Conditional("DEVELOPMENT")]
        static public void True(bool inbCondition, string inMessage, object inParam)
        {
            if (!inbCondition)
            {
                OnFailure(String.Format(inMessage, inParam));
            }
        }

        [Conditional("DEVELOPMENT")]
        static public void Fail()
        {
            OnFailure("Failure");
        }

        [Conditional("DEVELOPMENT")]
        static public void Fail(string inMessage)
        {
            OnFailure(inMessage);
        }

        [Conditional("DEVELOPMENT")]
        static public void Fail(string inMessage, params object[] inMessageParams)
        {
            OnFailure(String.Format(inMessage, inMessageParams));
        }

        [Conditional("DEVELOPMENT")]
        static public void Fail(string inMessage, object inParam)
        {
            OnFailure(String.Format(inMessage, inParam));
        }

        static private void OnFailure(string inMessage)
        {
            throw new AssertException(inMessage);
        }

    }
}
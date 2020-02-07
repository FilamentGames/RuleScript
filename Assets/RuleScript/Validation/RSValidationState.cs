using System.Collections.Generic;
using System.Text;

namespace RuleScript.Validation
{
    public sealed class RSValidationState
    {
        private class StackFrame
        {
            private readonly int m_Depth;
            private readonly string m_HeaderName;
            private readonly string m_Prefix;
            private readonly string m_NewlineReplace;
            private readonly StringBuilder m_StringBuilder;

            public int WarningCount;
            public int ErrorCount;

            public int IssueCount { get { return WarningCount + ErrorCount; } }

            public StackFrame(int inDepth, string inContext)
            {
                m_Depth = inDepth;
                m_HeaderName = inContext;
                m_Prefix = GetPrefix(inDepth);
                m_NewlineReplace = GetNewlineReplace(inDepth);
                m_StringBuilder = new StringBuilder();
            }

            public void Warn(string inWarning, params object[] inWarningArgs)
            {
                ++WarningCount;
                WriteLineStart();
                string warning = ReplaceNewlines(inWarning, inWarningArgs);
                warning = string.Format("<color=yellow>{0}</color>", warning);
                m_StringBuilder.Append(warning);
            }

            public void Error(string inErrorCount, params object[] inErrorArgs)
            {
                ++ErrorCount;
                WriteLineStart();
                string error = ReplaceNewlines(inErrorCount, inErrorArgs);
                error = string.Format("<color=red>{0}</color>", error);
                m_StringBuilder.Append(error);
            }

            public void MergeUp(StackFrame inParent)
            {
                inParent.WarningCount += WarningCount;
                inParent.ErrorCount += ErrorCount;

                int issueCount = WarningCount + ErrorCount;
                if (issueCount > 0)
                {
                    inParent.WriteLineStart();

                    if (ErrorCount > 0)
                    {
                        inParent.m_StringBuilder.AppendFormat("<color=red>Issues with {0} ({1} errors, {2} warnings)</color>", m_HeaderName, ErrorCount, WarningCount);
                    }
                    else
                    {
                        inParent.m_StringBuilder.AppendFormat("<color=yellow>Issues with {0} ({1} warnings)</color>", m_HeaderName, WarningCount);
                    }

                    inParent.m_StringBuilder.Append('\n');
                    inParent.m_StringBuilder.Append(m_StringBuilder);
                }
                else if (m_StringBuilder.Length > 0)
                {
                    inParent.WriteLineStart();

                    inParent.m_StringBuilder.AppendFormat("Info for {0}", m_HeaderName);
                    inParent.m_StringBuilder.Append('\n');
                    inParent.m_StringBuilder.Append(m_StringBuilder);
                }
            }

            private void WriteLineStart()
            {
                if (m_StringBuilder.Length > 0)
                    m_StringBuilder.Append('\n');
                m_StringBuilder.Append(m_Prefix);
            }

            private string ReplaceNewlines(string inFormat, params object[] inArgs)
            {
                string result = string.Format(inFormat, inArgs);
                result = result.Replace("\n", m_NewlineReplace);
                return result;
            }

            public override string ToString()
            {
                if (m_StringBuilder.Length > 0)
                    return m_StringBuilder.ToString();
                return string.Empty;
            }

            static private string GetPrefix(int inDepth)
            {
                if (inDepth <= 0)
                    return string.Empty;

                if (inDepth == 1)
                    return " - ";

                return new string(' ', (inDepth - 1) * 3) + " - ";
            }

            static private string GetNewlineReplace(int inDepth)
            {
                if (inDepth <= 0)
                    return string.Empty;

                return new string(' ', inDepth * 3);
            }
        }

        private readonly Stack<StackFrame> m_Stack = new Stack<StackFrame>();
        private StackFrame m_CurrentFrame;
        private string m_FinalOutput;

        internal RSValidationState(string inName)
        {
            m_CurrentFrame = new StackFrame(0, inName);
            m_Stack.Push(m_CurrentFrame);
        }

        public int IssueCount
        {
            get
            {
                if (m_CurrentFrame != null)
                    return m_CurrentFrame.IssueCount;

                return 0;
            }
        }

        public int WarningCount
        {
            get
            {
                if (m_CurrentFrame != null)
                    return m_CurrentFrame.WarningCount;

                return 0;
            }
        }

        public int ErrorCount
        {
            get
            {
                if (m_CurrentFrame != null)
                    return m_CurrentFrame.ErrorCount;

                return 0;
            }
        }

        public string Output
        {
            get
            {
                return m_FinalOutput ?? string.Empty;
            }
        }

        internal void PushContext(string inName, params object[] inArgs)
        {
            string contextName = string.Format(inName, inArgs);
            StackFrame newFrame = new StackFrame(m_Stack.Count, contextName);
            m_Stack.Push(newFrame);
            m_CurrentFrame = newFrame;
        }

        internal void PopContext()
        {
            m_Stack.Pop();
            StackFrame prevFrame = m_Stack.Peek();
            m_CurrentFrame.MergeUp(prevFrame);
            m_CurrentFrame = prevFrame;
        }

        internal void Finish()
        {
            m_FinalOutput = m_CurrentFrame.ToString();
        }

        internal void Warn(string inWarning, params object[] inArgs)
        {
            m_CurrentFrame.Warn(inWarning, inArgs);
        }

        internal void Error(string inError, params object[] inArgs)
        {
            m_CurrentFrame.Error(inError, inArgs);
        }
    }
}
namespace RuleScript.Runtime
{
    public interface IRSDebugLogger
    {
        void Log(string inLog);
        void Log(string inLogFormat, params object[] inArgs);

        void Warn(string inWarning);
        void Warn(string inWarningFormat, params object[] inArgs);

        void Error(string inError);
        void Error(string inErrorFormat, params object[] inArgs);
    }
}
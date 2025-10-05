namespace LogSystem
{
    public interface ILogSystem
    {
        void Error(string message);
        void Info(string message);
        void Warning(string message);
        void Write(string message, string level = "INFO");
    }
}
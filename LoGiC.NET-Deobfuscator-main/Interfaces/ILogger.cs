namespace LogicDeobfuscator.Interfaces
{
    public interface ILogger
    {
        public void Information(object msg);
        public void Success(object msg);
        public void Warning(object msg);
        public void Error(object msg);
    }
}
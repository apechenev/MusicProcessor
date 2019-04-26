namespace MusicProcessor.API
{
    interface IProgressHandler
    {
        void Reset();
        void SetMax(int value);
        void Report(int value);
    }
}

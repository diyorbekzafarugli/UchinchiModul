namespace Delegates;

public class DataProcessor
{
    private readonly ProgressReporter _progressReporter;
    public DataProcessor(ProgressReporter progressReporter)
    {
        _progressReporter = progressReporter;
    }

    public async Task ProcessData(int dataSize)
    {
        for (int i = 0; i < dataSize; i++)
        {
            await Task.Delay(50);

            if (i % 10 != 0) continue;

            var percentComplete = (int)((float)i / dataSize * 100);

            _progressReporter(percentComplete);
        }

        _progressReporter(100);
        Console.WriteLine("Processing complete");
    }
}

public delegate void ProgressReporter(int percentComplete);


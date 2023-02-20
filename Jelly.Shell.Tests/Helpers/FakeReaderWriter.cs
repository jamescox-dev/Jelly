namespace Jelly.Shell.Tests.Helpers;

using Jelly.Shell.Io;

public class FakeReaderWriter : IReader, IWriter, IHistoryManager
{
    readonly Queue<string> _queuedLines = new();

    public IList<IoOp> IoOps { get; set; } = new List<IoOp>();

    public IList<string> RecordedHistory { get; set; } = new List<string>();

    public void EnqueueInput(string line) => _queuedLines.Enqueue(line);

    public string ReadLine()
    {
        var line = _queuedLines.Dequeue();
        
        IoOps.Add(new ReadLineOp(line));
        
        return line;
    }

    public void WriteLine(string output) => IoOps.Add(new WriteLineOp(output));
    
    public void Write(string output) => IoOps.Add(new WriteOp(output));

    public bool IoOpsContains(params IoOp[] expected)
    {
        if (IoOps.Count >= expected.Length)
        {
            for (var i = 0; i < IoOps.Count - expected.Length + 1; ++i)
            {
                if (IoOps.Skip(i).Take(expected.Length).SequenceEqual(expected))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void VerifyIoOpsContains(params IoOp[] expected) => 
        IoOpsContains(expected).Should().BeTrue();
    
    public void VerifyIoOpsDoesNotContain(params IoOp[] expected) => 
        IoOpsContains(expected).Should().BeFalse();

    public void AddHistory(string command)
    {
        RecordedHistory.Add(command);
    }

    public void LoadHistory()
    {
        IoOps.Add(new LoadHistoryOp());
    }

    public void SaveHistory()
    {
        IoOps.Add(new SaveHistoryOp());
    }
}

public abstract record class IoOp(string Data) {}

public record class ReadLineOp(string Data) : IoOp(Data) {} 

public record class WriteLineOp(string Data) : IoOp(Data) {} 

public record class WriteOp(string Data) : IoOp(Data) {} 

public record class LoadHistoryOp() : IoOp("") {}

public record class SaveHistoryOp() : IoOp("") {}

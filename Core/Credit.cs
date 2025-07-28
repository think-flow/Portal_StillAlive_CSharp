using System.Diagnostics;
using System.Threading.Channels;
using PortalStillAlive.Data;

namespace PortalStillAlive.Core;

public static class Credit
{
    public static Task DrawAsync(ChannelWriter<OutputMsg> tx, Stage stage, CancellationToken token)
    {
        var taskCompletionSource = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            try
            {
                int i = 0;
                int creditX = 0;
                int length = CreditsData.Credits.Length;
                var creditList = new LinkedList<string>();
                creditList.AddFirst("");
                var stopwatch = Stopwatch.StartNew();

                foreach (char ch in CreditsData.Credits)
                {
                    token.ThrowIfCancellationRequested();

                    double duration = 174.0 / length * i;
                    i += 1;

                    if (ch == '\n')
                    {
                        creditX = 0;

                        creditList.AddLast("");
                        if (creditList.Count > stage.CreditsHeight)
                        {
                            // 删掉前面多余不用显示的行
                            for (int j = 0; j < creditList.Count - stage.CreditsHeight; j++)
                            {
                                creditList.RemoveFirst();
                            }
                        }

                        for (int y = 2; y < 2 + stage.CreditsHeight - creditList.Count; y++)
                        {
                            tx.Print((stage.CreditsPosX, y), new string(' ', stage.CreditsWidth));
                        }

                        string[] creditListArray = creditList.ToArray();
                        for (int k = 0; k < creditList.Count; k++)
                        {
                            int y = 2 + stage.CreditsHeight - creditList.Count + k;
                            int count = stage.CreditsWidth - creditListArray[k].Length;
                            tx.Print((stage.CreditsPosX, y), string.Format("{0}{1}", creditListArray[k], new string(' ', count)));
                        }
                    }
                    else
                    {
                        creditList.Last!.Value += ch;
                        tx.Print((stage.CreditsPosX + creditX, stage.CreditsHeight + 1), ch.ToString());

                        creditX++;
                    }

                    while (stopwatch.Elapsed.TotalSeconds < duration)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    }
                }

                taskCompletionSource.SetResult();
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
        })
        {
            Name = "credit",
            IsBackground = true
        };
        thread.Start();
        return taskCompletionSource.Task;
    }
}

using System.Diagnostics;
using System.Threading.Channels;
using PortalStillAlive.Data;

namespace PortalStillAlive.Core;

public static class Lyric
{
    public static Task DrawAsync(ChannelWriter<OutputMsg> tx, Stage stage, CancellationToken token)
    {
        var taskCompletionSource = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                int index = 0;
                int cursorX = 2;
                int cursorY = 2;
                var lyrics = LyricData.Lyrics;
                while (lyrics[index].Mode != 9)
                {
                    token.ThrowIfCancellationRequested();

                    var currentLyric = lyrics[index];
                    long pastTime = stopwatch.ElapsedMilliseconds / 10;

                    if (pastTime > currentLyric.Time)
                    {
                        int wordCount = 0;
                        double interval;

                        if (currentLyric.Mode <= 1 || currentLyric.Mode >= 5)
                        {
                            wordCount = currentLyric.Words.ToString()!.Length;
                        }

                        if (wordCount == 0)
                        {
                            wordCount = 1;
                        }

                        if (currentLyric.Interval < 0)
                        {
                            var nextLyric = lyrics[index + 1];
                            interval = (nextLyric.Time - currentLyric.Time) / 100d / wordCount;
                        }
                        else
                        {
                            interval = currentLyric.Interval / wordCount;
                        }

                        if (currentLyric.Mode == 0)
                        {
                            DrawLyrics(tx, currentLyric.Words.ToString()!, ref cursorX, ref cursorY, interval, true);
                        }
                        else if (currentLyric.Mode == 1)
                        {
                            DrawLyrics(tx, currentLyric.Words.ToString()!, ref cursorX, ref cursorY, interval, false);
                        }
                        else if (currentLyric.Mode == 2)
                        {
                            DrawArts((int) currentLyric.Words, tx, stage);
                        }
                        else if (currentLyric.Mode == 3)
                        {
                            ClearLyrics(tx, stage);
                            cursorX = 2;
                            cursorY = 2;
                        }
                        else if (currentLyric.Mode == 4)
                        {
                            if (stage.EnableSound)
                            {
                                Player.Play(Stage.SoundFilePath);
                            }
                        }
                        else if (currentLyric.Mode == 5)
                        {
                            //启动credit线程
                            _ = Credit.DrawAsync(tx, stage, token);
                        }

                        index++;
                    }

                    // 一句歌词结束后，需要保持光标仍然在lyric区域显示
                    tx.Print((cursorX, cursorY), "\0");
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }

                taskCompletionSource.TrySetResult();
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            finally
            {
                //标记通道已完成
                tx.Complete();
            }
        })
        {
            Name = "lyric",
            IsBackground = true
        };
        thread.Start();
        return taskCompletionSource.Task;
    }

    private static void DrawArts(int ch, ChannelWriter<OutputMsg> tx, Stage stage)
    {
        for (int dy = 0; dy < stage.AsciiArtHeight; dy++)
        {
            tx.Print((stage.AsciiArtX, stage.AsciiArtY + dy), AsciiArtsData.Arts[ch][dy]);
            Thread.Sleep(TimeSpan.FromMilliseconds(10));
        }
    }

    private static void DrawLyrics(
        ChannelWriter<OutputMsg> tx,
        string str,
        ref int cursorX,
        ref int cursorY,
        double interval,
        bool newLine)
    {
        foreach (char c in str)
        {
            tx.Print((cursorX, cursorY), c.ToString());
            Thread.Sleep(TimeSpan.FromSeconds(interval));
            if (c != '\0')
            {
                cursorX += 1;
            }
        }

        if (newLine)
        {
            cursorX = 2;
            cursorY += 1;
        }
    }

    private static void ClearLyrics(ChannelWriter<OutputMsg> tx, Stage stage)
    {
        int y = 2;
        for (int i = 0; i < stage.LyricHeight; i++)
        {
            tx.Print((2, y), string.Format("{0}\n", new string(' ', stage.LyricWidth)));
            y++;
        }
    }
}

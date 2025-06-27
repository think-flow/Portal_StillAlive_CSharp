namespace PortalStillAlive.Data;

/*
        Interval: -1 means to calculate based on last
        Mode:   0: Lyric with new line
                1: Lyric without new line
                2: ASCII art
                3: Clear lyrics
                4: Start music
                5: Start credits
                9: END
 */
public record Lyric(object Words, int Time, double Interval, int Mode);

public static class LyricData
{
    public static readonly List<Lyric> Lyrics =
    [
        // Page 1
        new("Forms FORM-29827281-12:", 0, -1, 0),
        new("Test Assessment Report", 200, -1, 0),
        new("\0\0\0\0\0\0\0", 400, -1, 0), // Keep flushing the buffer
        new("", 710, 0, 4), // Music start
        new("This was a triumph.", 730, 2, 0),
        new("", 930, 0, 5), // Credits start
        new("I'm making a note here:", 1123, 2, 0),
        new("HUGE SUCCESS.", 1347, 1.7, 0),
        new("It's hard to overstate", 1627, -1, 0),
        new("my satisfaction.", 1873, 2.6, 0),
        new("Aperture Science", 2350, 1.8, 0),
        new(0, 2350, 0, 2), // ASCII 1
        new("We do what we must", 2733, 1.6, 0),
        new("because we can.", 2910, 1.5, 0),
        new("For the good of all of us.", 3237, -1, 0),
        new(1, 3500, 0, 2), // ASCII 2
        new("Except the ones who are dead.", 3567, -1, 0),
        new("", 3717, 0.05, 0),
        new(0, 3717, 0, 2), // ASCII 1
        new("But there's no sense crying", 3787, -1, 0),
        new("over every mistake.", 3973, 1.77, 0),
        new("You just keep on trying", 4170, -1, 0),
        new("till you run out of cake.", 4370, -1, 0),
        new(2, 4500, 0, 2), // ASCII 3
        new("And the Science gets done.", 4570, -1, 0),
        new("And you make a neat gun.", 4767, -1, 0),
        new(0, 4903, 0, 2), // ASCII 1
        new("For the people who are", 4973, -1, 0),
        new("still alive.", 5110, 1.6, 1),

        // Page 2
        new(0, 5353, 0, 3), // Clear lyrics
        new("Forms FORM-55551-5:", 5413, -1, 0),
        new("Personnel File Addendum:", 5477, 1.13, 0),
        new("", 5650, 0.05, 0),
        new("Dear <<Subject Name Here>>,", 5650, -1, 0),
        new("", 5900, -1, 0),
        new("I'm not even angry.", 5900, 1.86, 0),
        new("I'm being ", 6320, -1, 1),
        new("so ", 6413, -1, 1),
        new("sincere right now.", 6470, 1.9, 0),
        new("Even though you broke ", 6827, -1, 1),
        new(3, 7020, 0, 2), // ASCII 4
        new("my heart.", 7090, -1, 0),
        new("And killed me.", 7170, 1.43, 0),
        new(4, 7300, 0, 2), // ASCII 5
        new("And tore me to pieces.", 7500, 1.83, 0),
        new("And threw every piece ", 7900, -1, 1),
        new("into a fire.", 8080, 1.8, 0),
        new(5, 8080, 0, 2), // ASCII 6
        new("As they burned it hurt because", 8430, -1, 0),
        new(6, 8690, 0, 2), // ASCII 7
        new("I was so happy for you!", 8760, 1.67, 0),
        new("Now, these points of data", 8960, -1, 0),
        new("make a beautiful line.", 9167, -1, 0),
        new("And we're out of beta.", 9357, -1, 0),
        new("We're releasing on time.", 9560, -1, 0),
        new(4, 9700, 0, 2), // ASCII 5
        new("So I'm GLaD I got burned.", 9770, -1, 0),
        new(2, 9913, 0, 2), // ASCII 3
        new("Think of all the things we learned", 9983, -1, 0),
        new(0, 10120, 0, 2), // ASCII 1
        new("For the people who are", 10190, -1, 0),
        new("Still alive.", 10327, 1.8, 0),

        // Page 3
        new(0, 10603, 0, 3), // Clear lyrics
        new("Forms FORM-55551-6:", 10663, -1, 0),
        new("Personnel File Addendum Addendum:", 10710, 1.36, 0),
        new("", 10710, 0.05, 0),
        new("One last thing:", 10910, -1, 0),
        new("", 11130, 0.05, 0),
        new("Go ahead and leave ", 11130, -1, 1),
        new("me.", 11280, 0.5, 0),
        new("I think I'd prefer to stay ", 11507, -1, 1),
        new("inside.", 11787, 1.13, 0),
        new("Maybe you'll find someone else", 12037, -1, 0),
        new("To help you.", 12390, 1.23, 0),
        new("Maybe Black ", 12737, -1, 1),
        new(7, 12787, 0, 2), // ASCII 8
        new("Mesa...", 12857, 2.7, 0),
        new("THAT WAS A JOKE.", 13137, 1.46, 1),
        new(" FAT CHANCE.", 13387, 1.1, 0),
        new("Anyway, ", 13620, -1, 1),
        new(8, 13670, 0, 2), // ASCII 9
        new("this cake is great.", 13740, -1, 0),
        new("It's so delicious and moist.", 13963, -1, 0),
        new(9, 14123, 0, 2), // ASCII 10
        new("Look at me still talking", 14193, -1, 0),
        new(1, 14320, 0, 2), // ASCII 2
        new("when there's science to do.", 14390, -1, 0),
        new(0, 14527, 0, 2), // ASCII 1
        new("When I look out there,", 14597, -1, 0),
        new("It makes me GLaD I'm not you.", 14767, -1, 0),
        new(2, 14913, 0, 2), // ASCII 3
        new("I've experiments to run.", 14983, -1, 0),
        new(4, 15120, 0, 2), // ASCII 5
        new("There is research to be done.", 15190, -1, 0),
        new(0, 15320, 0, 2), // ASCII 1
        new("On the people who are", 15390, -1, 0),
        new("still alive", 15553, 2.0, 1),

        // Page 4
        new(0, 15697, 0, 3), // Clear lyrics
        new("", 15757, 0.05, 0),
        new("", 15757, 0.05, 0),
        new("", 15757, 0.05, 0),
        new("PS: And believe me I am", 15757, -1, 0),
        new("still alive.", 15960, 1.13, 0),
        new("PPS: I'm doing Science and I'm", 16150, -1, 0),
        new("still alive.", 16363, 1.13, 0),
        new("PPPS: I feel FANTASTIC and I'm", 16550, -1, 0),
        new("still alive.", 16760, -1, 0),
        new("", 16860, -1, 0),
        new("FINAL THOUGH:", 16860, -1, 0),
        new("While you're dying I'll be", 16993, -1, 0),
        new("still alive.", 17157, -1, 0),
        new("", 17277, -1, 0),
        new("FINAL THOUGH PS:", 17277, -1, 0),
        new("And when you're dead I will be", 17367, -1, 0),
        new("still alive.", 17550, 1.13, 0),
        new("", 17550, -1, 0),
        new("", 17550, 0.05, 0),
        new("STILL ALIVE", 17760, 1.13, 0),
        new(0, 17900, 0, 3), // Clear lyrics
        new(0, 18500, 0, 3), // Clear lyrics
        new("ENDENDENDENDENDENDENDEND", 18500, 0.05, 9)
    ];
}

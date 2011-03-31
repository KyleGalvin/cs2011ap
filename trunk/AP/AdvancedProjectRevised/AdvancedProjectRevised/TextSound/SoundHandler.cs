using System;
using OpenTK;
using OpenTK.Audio;

public class SoundHandler
{
    public const String EXPLOSION = "../../Sounds/explosion.wav";
    public const String GUNSHOT = "../../Sounds/gunshot.wav";
    public const String SCREAM = "../../Sounds/wilhelm.wav";
    public const String SILENCER = "../../Sounds/gunshotsilencer.wav";
    public const String RELOAD = "../../Sounds/reload.wav";
    public const String INJURED = "../../Sounds/injured1.wav";
    public const String ZOMBIE = "../../Sounds/zombie3.wav";
    public const String BACKGROUND = "../../Sounds/bg1.wav";
    private const int maxSounds = 10;
    private Boolean soundsOn = false;
    public bool pressingF1 = false;

    public int injuredSoundCooldown = 0;

    private int arraySpot = 0;
    int[] bufferList;
    int[] sourceList;
    private int songBuffer;
    private int songSource;
    private AudioReader songReader;

    AudioReader[] audioList;
    AudioContext ac;

    public SoundHandler()
    {
        // initialize buffer, source, AudioReader arrays
        bufferList = new int[maxSounds];
        sourceList = new int[maxSounds];
        audioList = new AudioReader[maxSounds];

        // create ONE AudioContext to handle all sounds
       // ac = new AudioContext();
    }
    public void setSoundState(Boolean state)
    {
        soundsOn = state;
    }
    public Boolean getSoundState()
    {
        return soundsOn;
    }

    // stops the current song (does not affect sound effects)
    public void stopSong()
    {
        // stop playback of the current song and delete its buffer        
        AL.SourceStop(songSource);
    }

    public void continueSong()
    {
        AL.SourcePlay(songSource);
    }

    // plays a full-length song and loops it continuously
    public void playSong(String soundPath)
    {
        if (soundsOn)
        {
            // stop playback of the current song
            AL.SourceStop(songSource);
            AL.DeleteSource(songSource);
            AL.DeleteBuffer(songBuffer);

            // generate a new buffer, source, audioReader
            songBuffer = AL.GenBuffer();
            songSource = AL.GenSource();
            songReader = new AudioReader(soundPath);

            // read the song into the buffer
            AL.BufferData(songBuffer, songReader.ReadToEnd());

            // create a source using this buffer, loop the song
            AL.Source(songSource, ALSourcei.Buffer, songBuffer); // attach the buffer to a source
            AL.Source(songSource, ALSourceb.Looping, true);

            // start playback
            AL.SourcePlay(songSource);
        }
    }

    // plays a sound effect once (no looping)
    public void play(String soundPath)
    {

        if (soundsOn)
        {
            if (audioList[arraySpot] != null)
            {
                // dispose the current AudioReader if it exists
                audioList[arraySpot].Dispose();
            }

            // stop the current sound and delete its source/buffer
            AL.SourceStop(sourceList[arraySpot]);
            AL.DeleteSource(sourceList[arraySpot]);
            AL.DeleteBuffer(bufferList[arraySpot]);

            // genereate a new buffer, source, and AudioReader for the current sound
            bufferList[arraySpot] = AL.GenBuffer();
            sourceList[arraySpot] = AL.GenSource();
            audioList[arraySpot] = new AudioReader(soundPath);

            // read the current sound into the buffer
            AL.BufferData(bufferList[arraySpot], audioList[arraySpot].ReadToEnd());

            // create a source using this buffer
            AL.Source(sourceList[arraySpot], ALSourcei.Buffer, bufferList[arraySpot]); // attach the buffer to a source

            // start playback
            AL.SourcePlay(sourceList[arraySpot]);

            // increment the array location
            arraySpot++;

            // loop back to the beginning of the array to cap number of sounds
            if (arraySpot == maxSounds)
            {
                arraySpot = 0;
            }
        }
    }
}

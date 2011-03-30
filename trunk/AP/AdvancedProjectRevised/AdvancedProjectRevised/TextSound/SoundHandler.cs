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
    public const String INJURED = "../../Sounds/injured3.wav";
    public const String ZOMBIE = "../../Sounds/zombie1.wav";
    private const int maxSounds = 10;
    private Boolean soundsOn = true;

    public int injuredSoundCooldown = 0;
    
    private int arraySpot = 0;
    int[] bufferList;
    int[] sourceList;
    AudioReader[] audioList;
    AudioContext ac;

    public SoundHandler()
    {
        // initialize buffer, source, AudioReader arrays
        bufferList = new int[maxSounds];
        sourceList = new int[maxSounds];
        audioList = new AudioReader[maxSounds];

        // create ONE AudioContext to handle all sounds
        ac = new AudioContext(); 
    }
    public void setSoundState(Boolean state)
    {
        soundsOn = state;
    }
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

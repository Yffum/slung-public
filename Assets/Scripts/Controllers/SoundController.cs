using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    //------SOUNDS-----------------
    [SerializeField] private AudioClip _synthC3;
    [SerializeField] private AudioClip _synthF3;
    [SerializeField] private AudioClip _synthC4;


    [SerializeField] private AudioSource _speakerC3;
    [SerializeField] private AudioSource _speaker;

    /// <summary>
    /// The parent of arppegioOne AudioSources, which have their clips attached
    /// </summary>
    [SerializeField] private Transform _arpeggioOne;



    public SoundController Init()
    {
        //InitializeArpeggios();

        return this;
    }


    /// <summary>
    /// Plays a note at the given index in _arpeggioOne, or the highest index if a higher one is given
    /// </summary>
    public void PlayArpeggioOne(int noteIndex)
    {
        // no negative notes
        if (noteIndex < 0)
        {
            Debug.LogWarning("Arpeggio cannot play negative noteIndex");
        }

        // cap at arpeggio length
        else if (noteIndex >= _arpeggioOne.childCount)
        {
            noteIndex = _arpeggioOne.childCount - 1;
        }

        // get child AudioSource corresponding to noteIndex and then play that AudioSource's attached AudioClip
        AudioSource note = _arpeggioOne.GetChild(noteIndex).GetComponent<AudioSource>();
        note.PlayOneShot(note.clip);
    }

    
    public void PlaySynth()
    {
        _speaker.PlayOneShot(_arpeggioOne.GetChild(1).GetComponent<AudioSource>().clip);
    }


    /*
    /// <summary>
    /// Plays a note at the given index in _arpeggioOne, or the highest index if a higher one is given
    /// </summary>
    public void PlayArpeggioOne(int noteIndex)
    {
        // no negative notes
        if (noteIndex < 0)
        {
            Debug.LogWarning("Arpeggio cannot play negative noteIndex");
        }

        // cap at arpeggio length
        else if (noteIndex >= _arpeggioOne.Length)
        {
            noteIndex = _arpeggioOne.Length - 1;
        }

        // play sound
        _speaker.PlayOneShot(_arpeggioOne[noteIndex]);
    }

    private void InitializeArpeggios()
    {
        _arpeggioOne = new AudioClip[] { _synthC3, _synthF3, _synthC4 };
    }
    */
}

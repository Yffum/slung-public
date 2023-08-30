using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _muteButton;
    [SerializeField] private GameObject _unmuteButton;

    //------SOUNDS-----------------
    [SerializeField] private AudioClip _slingshotSound;
    [SerializeField] private AudioClip _startLevelSound;
    [SerializeField] private AudioClip _endLevelSound;
    [SerializeField] private AudioClip _blipSound;
    [SerializeField] private AudioClip _thudSound;

    /// <summary>
    /// The audio source used for non-explosions
    /// </summary>
    [SerializeField] private AudioSource _speaker;

    [SerializeField] private AudioSource _quietSpeaker;

    /// <summary>
    /// The parent of arppegioOne AudioSources, which have their clips attached
    /// </summary>
    [SerializeField] private Transform _arpeggioOne;

    public SoundController Init()
    {
        //InitializeArpeggios();

        return this;
    }

    public void MuteAll()
    {
        GameController.UserData.IsMuted = true;

        GameController.SaveUserData();

        //_camera.GetComponent<AudioListener>().enabled = false;
        AudioListener.volume = 0f;
        _muteButton.SetActive(false);
        _unmuteButton.SetActive(true);
    }

    public void UnmuteAll()
    {
        GameController.UserData.IsMuted = false;

        GameController.SaveUserData();

        //_camera.GetComponent<AudioListener>().enabled = true;
        AudioListener.volume = 1f;
        _muteButton.SetActive(true);
        _unmuteButton.SetActive(false);

        PlayBlipSound();
    }

    public void PlayStartLevelSound()
    {
        _quietSpeaker.PlayOneShot(_startLevelSound);
    }

    public void PlayEndLevelSound()
    {
        _quietSpeaker.PlayOneShot(_endLevelSound);
    }

    public void PlaySlingshotSound()
    {
        _speaker.PlayOneShot(_slingshotSound);
    }

    public void PlayBlipSound()
    {
        _speaker.PlayOneShot(_blipSound);
    }

    public void PlayThudSound()
    {
        _speaker.PlayOneShot(_thudSound);
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

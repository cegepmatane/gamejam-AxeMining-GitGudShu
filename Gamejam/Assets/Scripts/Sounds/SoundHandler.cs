using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private AudioSource[] _mySounds;

    private AudioSource _die;
    private AudioSource _mine;
    private AudioSource _explosion;
    private AudioSource _stairs;
    void Start()
    {
        _mySounds = GetComponents<AudioSource>();

        _die = _mySounds[0];
        _mine = _mySounds[1];
        _explosion = _mySounds[2];
        _stairs = _mySounds[3];
    }

    public void PlayDie() { _die.Play(); }
    public void PlayMine() { _mine.Play(); }
    public void PlayExplosion() { _explosion.Play(); }
    public void PlayStairs() { _stairs.Play(); }

}

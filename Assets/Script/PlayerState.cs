using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public float moveSpeed = 7.0f;
    public float rotSpeed = 300f;
    public float maxHealth = 100f;

    protected float currentHealth = 0;

    protected enum PlayerState_
    {
        NONE,
        READY,
        RUN,
        DIE
    }

    protected PlayerState_ playerState = PlayerState_.READY;
}

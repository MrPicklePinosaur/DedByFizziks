﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerProfile;
using static EventSystem;
using static PlayerInstance;
using ExitGames.Client.Photon;

[RequireComponent(typeof(PhotonView),typeof(PhotonAnimatorView),typeof(Animator))]
public class NetworkPlayerController : EventListener {

    public PhotonView view;
    protected Animator anim;
    public float moveSpeed;

    public bool isTrapped;

    void Start() {
        base.Start();

        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        isTrapped = false;
    }

    void Update() {

        if (!view.IsMine) return;

        //interact
        if (Input.GetKeyDown(KeyCode.E)) {
            eventSystem.RaiseNetworkEvent(EventCodes.OnPlayerInteractEvent, new object[] { playerProfile.player.ActorNumber });
        }


        if (!isTrapped) {
            HandleMovement();
        }

    }


    protected void HandleMovement() {
        //sprinting
        var sprintMult = 1;
        if (Input.GetKey(KeyCode.LeftShift)) {
            sprintMult *= 2;
        }

        Vector2 inp = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 move = new Vector3(inp.x, 0, inp.y) * moveSpeed * sprintMult;
        transform.position += move * Time.deltaTime;

        //animation stuff
        anim.SetFloat("Forward/Backward Speed", move.z);
        anim.SetFloat("SideToSide Speed", move.x);
    }

    public override void OnEvent(EventData data) {
        object[] payload = (object[])data.CustomData;

        switch (data.Code) {

            case (byte)EventCodes.OnPlayerDeathEvent:
                isTrapped = true;

                break;
            case (byte)EventCodes.OnPlayerReviveEvent:
                isTrapped = false;

                break;
        }
    }
}

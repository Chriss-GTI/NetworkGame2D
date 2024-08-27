using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class Chat : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HelloThere();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GeneralKenobi();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HighGround();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Underestimate();
        }
    }

    private void HelloThere()
    {
        FixedString128Bytes message = new FixedString128Bytes("Hello There!");
        SubmitMessageRpc(message);
    }
    private void GeneralKenobi()
    {
        FixedString128Bytes message = new FixedString128Bytes("General Kenobi!");
        SubmitMessageRpc(message);
    }

    private void HighGround()
    {
        FixedString128Bytes message = new FixedString128Bytes("It's over Anakin, I've got the High Ground!");
        SubmitMessageRpc(message);
    }

    private void Underestimate()
    {
        FixedString128Bytes message = new FixedString128Bytes("Don't Underestimate my Power!");
        SubmitMessageRpc(message);
    }

    [Rpc(SendTo.Server)]
    public void SubmitMessageRpc(FixedString128Bytes message)
    {
        UpdateMessageRpc(message);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateMessageRpc(FixedString128Bytes message)
    {
        text.text = message.ToString();
    }
}
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public struct NetEvent
{
    public byte[] Data { get; internal set; }
}

public class GameManagerScript : MonoBehaviour
{
    public ParticleSystem[] confetti;
    public Animator sceneContainer;

    private ClientWebSocket _socket;
    private ConcurrentQueue<NetEvent> _events;

    private string[] _animMap = new[] { "Host", "Empty", "Contestant", "ContestantScoreboard" };

    // Start is called before the first frame update
    async void Start()
    {
        _events = new ConcurrentQueue<NetEvent>();
        _socket = new ClientWebSocket();
        await _socket.ConnectAsync(new Uri("ws://localhost:12345"), default);
        _ = Task.Run(async () => await SocketLoop());
    }

    async Task SocketLoop()
    {
        var buffer = new byte[1024];
        var stream = new MemoryStream();
        while (_socket.State == WebSocketState.Open)
        {
            stream.SetLength(0);

            WebSocketReceiveResult result;
            do
            {
                result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, default);
                }
                else
                {
                    stream.Write(buffer, 0, result.Count);
                }

            } while (!result.EndOfMessage);

            _events.Enqueue(new NetEvent() { Data = stream.ToArray() });
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (_events.TryDequeue(out var ev))
        {
            // handle event
            if (ev.Data[0] == 1)
            {
                var speed = BitConverter.ToSingle(ev.Data, 4);
                if (ev.Data[1] == 1)
                {
                    sceneContainer.CrossFade(_animMap[ev.Data[2]] + "ToEmpty", speed);
                }
                else
                {
                    sceneContainer.CrossFade(_animMap[ev.Data[1]], speed);
                }
            }

            if (ev.Data[0] == 2)
            {
                foreach (var item in confetti)
                {
                    item.Play();
                }
            }
        }
    }
}

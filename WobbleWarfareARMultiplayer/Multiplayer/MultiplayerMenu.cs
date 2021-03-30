using UnityEngine;
using System;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;

public class MultiplayerMenu : GlobalEventListener
{
    public void StartServer()
    {
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        BoltMatchmaking.CreateSession(sessionID: "test", sceneToLoad: "Game_ARMultiplayer");
    }

    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
    }
}

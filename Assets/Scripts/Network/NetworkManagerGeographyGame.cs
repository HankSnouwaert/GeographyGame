using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Mirror.GeographyGame
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerGeographyGame : NetworkManager
    {
        public Transform dialogPanel;
        public Transform studentTransform1;
        public Transform studentTransform2;
        public Transform studentTransform3;
        public Transform studentTransform4;
        public Text mapSettingsPath;
        private GameObject[] players = new GameObject[4];
        private int playerNum = 0;
        GameObject ball;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform[] studentTransform = { studentTransform1, studentTransform2, studentTransform3, studentTransform4 };

            Transform start = studentTransform[numPlayers];
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation, dialogPanel);
            //player.transform.parent = dialogPanel;
            NetworkServer.AddPlayerForConnection(conn, player);
            players[playerNum] = player;
            playerNum++;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }

        public void OnServerSendStudentsMapSettings()
        {
            for (int i = 0; i <= playerNum; i++)
            {
                Toggle toggle = players[i].GetComponentInChildren(typeof(Toggle)) as Toggle;
                if (toggle.isOn)
                {
                    players[i].GetComponent<Examples.Basic.Student>().createWorld(mapSettingsPath);
                }
                print(toggle.isOn);
            }
        }
    }
}

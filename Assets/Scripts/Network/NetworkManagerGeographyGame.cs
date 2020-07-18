using UnityEngine;

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
        GameObject ball;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform[] studentTransform = { studentTransform1, studentTransform2, studentTransform3, studentTransform4 };

            Transform start = studentTransform[numPlayers];
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            player.transform.parent = dialogPanel;
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}

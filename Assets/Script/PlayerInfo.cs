using System.Net.Sockets;

public class PlayerInfo
{
    public string playerName;
    public int number;
    public Socket socket;

    public PlayerInfo(string _playerName)
    {
        playerName = _playerName;
    }
}
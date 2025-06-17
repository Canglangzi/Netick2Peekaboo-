using System;
using UnityEngine;

public class SimpleNetworkAuthenticator : NetworkAuthenticator
{
    [Header("Simple Network Authenticator")]
    [SerializeField]
    private string serverPassword = "default_password";

    /// <summary>
    /// Returns the connection data that includes the server password encoded in Base64.
    /// </summary>
    /// <returns>Connection data as byte array.</returns>
    public override byte[] GetConnectionData()
    {
        string base64EncodedPassword = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(serverPassword));
        return System.Text.Encoding.ASCII.GetBytes(base64EncodedPassword);
    }

    /// <summary>
    /// Authenticate the connection request based on the provided data.
    /// </summary>
    /// <param name="connectionData">The connection data received with the connection request.</param>
    /// <param name="connectionDataLength">The length of the connection data.</param>
    /// <returns>True if authentication is successful; otherwise, false.</returns>
    public override bool AuthenticateConnection(byte[] connectionData, int connectionDataLength)
    {
        string base64EncodedReceivedPassword = System.Text.Encoding.ASCII.GetString(connectionData, 0, connectionDataLength);
        string receivedPassword = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(base64EncodedReceivedPassword));
        return receivedPassword == serverPassword;
    }
}
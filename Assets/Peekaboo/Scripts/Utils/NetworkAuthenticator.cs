using UnityEngine;

public abstract class NetworkAuthenticator : MonoBehaviour
{
    /// <summary>
    /// This method should return connection data that will be sent with the connection request.
    /// Override this method to provide custom connection data.
    /// </summary>
    /// <returns>Connection data as byte array.</returns>
    public virtual byte[] GetConnectionData()
    {
        // Default implementation, override in derived classes
        return null;
    }

    /// <summary>
    /// This method should authenticate the connection request based on the provided data.
    /// Override this method to implement custom authentication logic.
    /// </summary>
    /// <param name="connectionData">The connection data received with the connection request.</param>
    /// <param name="connectionDataLength">The length of the connection data.</param>
    /// <returns>True if authentication is successful; otherwise, false.</returns>
    public virtual bool AuthenticateConnection(byte[] connectionData, int connectionDataLength)
    {
        // Default implementation, override in derived classes
        return true;
    }
}
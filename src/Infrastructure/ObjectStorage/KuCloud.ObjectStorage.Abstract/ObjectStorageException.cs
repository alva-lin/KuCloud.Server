namespace KuCloud.ObjectStorage.Abstract;

public class ObjectStorageException : Exception
{
    public ObjectStorageException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}

public class ClientException : ObjectStorageException
{
    public ClientException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}

public class ServerException : ObjectStorageException
{
    public ServerException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}

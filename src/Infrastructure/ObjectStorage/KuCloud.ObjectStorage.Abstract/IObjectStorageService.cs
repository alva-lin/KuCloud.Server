namespace KuCloud.ObjectStorage.Abstract;

public interface IObjectStorageService
{
    /// <summary>
    /// check file/folder is exist. 
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Return true if exist, or false if it's not exist</returns>
    Task<bool> IsExistAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// get the metadata of the object.
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="keys">the keys of the information you want to get. If the <paramref name="keys"/> is null, all metadata will be returned. If the <paramref name="keys"/> is a empty array, an empty dictionary will be returned</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Dictionary<string, object?>> GetInfoAsync(string path, string[]? keys = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// get the metadata of the object for the special key.
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="key">the key of the metadata</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the value of the metadata, if it's not exist, return null</returns>
    Task<object?> GetInfoAsync(string path, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// set the metadata by given <paramref name="info"/>.
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="info">the keypair of metadata</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if the setting was successful, false otherwise</returns>
    Task<bool> SetInfoAsync(string path, Dictionary<string, object?> info, CancellationToken cancellationToken = default);

    /// <summary>
    /// generate signal url and return.
    /// </summary>
    /// <param name="urlType">the url type</param>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the signal url, will return empty string if generation fails</returns>
    Task<string> GenerateSignalUrl(SignalUrlType urlType, string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// create folder on given <paramref name="path"/>, and if it is existed, do nothing.
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if success, false otherwise</returns>
    Task<bool> CreateFolderAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// upload file from <paramref name="localPath"/>
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="localPath">the local path of the file waiting to be uploaded</param>
    /// <param name="callback">the callback delegate during upload</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if success, false otherwise</returns>
    Task<bool> UploadAsync(string path, string localPath, ProcessCallback? callback = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// upload file from <paramref name="data"/>
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="data">the data waiting to be uploaded</param>
    /// <param name="callback">the callback delegate during upload</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if success, false otherwise</returns>
    Task<bool> UploadAsync(string path, byte[] data, ProcessCallback? callback = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// upload file from <paramref name="stream"/>
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="stream">the stream waiting to be uploaded</param>
    /// <param name="callback">the callback delegate during upload</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if success, false otherwise</returns>
    Task<bool> UploadAsync(string path, Stream stream, ProcessCallback? callback = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// delete file by <paramref name="path"/> on the StorageService
    /// </summary>
    /// <param name="path">the path of the file on the ObjectStorage. if the <paramref name="path"/> ends with '/', it means it is a folder</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if success, false otherwise</returns>
    Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// delete file by <paramref name="pathArray"/> on the StorageService
    /// </summary>
    /// <param name="pathArray">the path of the files on the ObjectStorage</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if success, false otherwise</returns>
    Task<int> DeleteAsync(string[] pathArray, CancellationToken cancellationToken = default);
}

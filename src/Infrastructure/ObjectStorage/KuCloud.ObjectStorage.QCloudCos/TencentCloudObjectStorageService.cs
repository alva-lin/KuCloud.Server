using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Model.Tag;
using COSXML.Transfer;

using KuCloud.ObjectStorage.Abstract;

using Microsoft.Extensions.Options;

namespace KuCloud.ObjectStorage.QCloudCos;

public class TencentCloudObjectStorageService : IObjectStorageService
{
    private readonly TencentCosOption _option;

    private readonly CosXmlConfig _xmlConfig;

    private readonly QCloudCredentialProvider _credentialProvider;

    private readonly CosXml _cosXml;

    public TencentCloudObjectStorageService(IOptionsSnapshot<TencentCosOption> optionsSnapshot)
    {
        _option = optionsSnapshot.Value;

        _xmlConfig = new CosXmlConfig.Builder()
            .IsHttps(true)
            .SetRegion(_option.Region)
#if DEBUG
            .SetDebugLog(true)
#endif
            .Build();

        _credentialProvider = new DefaultQCloudCredentialProvider(
            _option.SecretId,
            _option.SecretKey,
            _option.DurationSecond);

        _cosXml = new CosXmlServer(_xmlConfig, _credentialProvider);
    }

    public Task<bool> IsExistAsync(string path, CancellationToken cancellationToken = default)
    {
        var request = new DoesObjectExistRequest(_option.Bucket, _option.BasePath + path);

        var task = Task.Run(() => _cosXml.DoesObjectExist(request), cancellationToken);

        return task;
    }

    public async Task<Dictionary<string, object?>> GetInfoAsync(string path, string[]? keys = null, CancellationToken cancellationToken = default)
    {
        var request = new HeadObjectRequest(_option.Bucket, path);

        var result = await Task.Run(() => _cosXml.HeadObject(request), cancellationToken);

        return new()
        {
            { "result", result }
        };
        
        // TODO 添加错误处理
        // TODO 添加单元测试
        // TODO 测试这个接口，并重新规划 GetInfo 的功能
        // TODO 完成所有接口
    }

    public async Task<object?> GetInfoAsync(string path, string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SetInfoAsync(string path, Dictionary<string, object?> info, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateSignalUrl(SignalUrlType urlType, string path, CancellationToken cancellationToken = default)
    {
        var preSignStruct = new PreSignatureStruct
        {
            appid = _option.AppId,
            region = _option.Region,
            bucket = _option.Bucket,
            key = path,
            httpMethod = urlType == SignalUrlType.Download ? "GET" : "PUT",
            isHttps = true,
            signDurationSecond = _option.DurationSecond,
            headers = null,
            queryParameters = null
        };

        return Task.Run(() => _cosXml.GenerateSignURL(preSignStruct), cancellationToken);
    }

    public async Task<bool> CreateFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        path = path.TrimEnd('/') + "/";

        var request = new PutObjectRequest(_option.Bucket, path, Array.Empty<byte>());

        var task = Task.Run(() => _cosXml.PutObject(request), cancellationToken);

        var result = await task;
        
        return result.IsSuccessful();
    }

    public async Task<bool> UploadAsync(string path, string localPath, ProcessCallback? callback = null, CancellationToken cancellationToken = default)
    {
        var transferConfig = new TransferConfig()
        {
            DivisionForUpload  = 10 * 1024 * 1024,
            SliceSizeForUpload = 2  * 1024 * 1024
        };

        var manager = new TransferManager(_cosXml, transferConfig);

        var uploadTask = new COSXMLUploadTask(_option.Bucket, path)
        {
            progressCallback = delegate(long completed, long total)
            {
            
            }
        };
        uploadTask.SetSrcPath(localPath);

        var result = await manager.UploadAsync(uploadTask);

        return result.IsSuccessful();
    }

    public async Task<bool> UploadAsync(string path, byte[] data, ProcessCallback? callback = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UploadAsync(string path, Stream stream, ProcessCallback? callback = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest(_option.Bucket, path);

        var result = await Task.Run(() => _cosXml.DeleteObject(request), cancellationToken);

        return result.IsSuccessful();
    }

    public async Task<int> DeleteAsync(string[] pathArray, CancellationToken cancellationToken = default)
    {
        var request = new DeleteMultiObjectRequest(_option.Bucket);
        request.SetDeleteQuiet(false);
        request.SetObjectKeys(pathArray.ToList());

        var result = await Task.Run(() => _cosXml.DeleteMultiObjects(request), cancellationToken);

        return result.deleteResult.deletedList.Count;
    }
}

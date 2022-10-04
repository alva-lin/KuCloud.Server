using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model;
using COSXML.Model.Object;
using COSXML.Model.Tag;
using COSXML.Transfer;

using KuCloud.ObjectStorage.Abstract;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace KuCloud.ObjectStorage.QCloudCos;

public class QCloudCosService : IObjectStorageService
{
    private readonly QCloudCosOption _option;

    private readonly CosXml _cosXml;

    private readonly ILogger<QCloudCosService> _logger;

    public QCloudCosService(IOptionsSnapshot<QCloudCosOption> optionsSnapshot, ILogger<QCloudCosService> logger)
    {
        _logger = logger;

        _option          = optionsSnapshot.Value;
        _option.BasePath = _option.BasePath.TrimEnd('/') + '/';

        var xmlConfig = new CosXmlConfig.Builder()
            .IsHttps(true)
            .SetRegion(_option.Region)
#if DEBUG
            .SetDebugLog(true)
#endif
            .Build();

        QCloudCredentialProvider credentialProvider = new DefaultQCloudCredentialProvider(
            _option.SecretId,
            _option.SecretKey,
            _option.DurationSecond);

        _cosXml = new CosXmlServer(xmlConfig, credentialProvider);
    }

    private string GetRequestId(CosResult result)
    {
        return result.responseHeaders["x-cos-request-id"].FirstOrDefault(string.Empty);
    }

    public Task<bool> IsExistAsync(string path, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path;
        var request = new DoesObjectExistRequest(_option.Bucket, path);

        var task = Task.Run(() =>
        {
            var isExist = _cosXml.DoesObjectExist(request);
            _logger.LogInformation("{Action} {Path} {IsExist}", "query is exist", path, isExist);
            return isExist;
        }, cancellationToken);

        return task;
    }

    public async Task<Dictionary<string, List<string>>> GetInfoAsync(string path, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path;
        var request = new HeadObjectRequest(_option.Bucket, path);

        var result = await Task.Run(() =>
        {
            var response = _cosXml.HeadObject(request);
            _logger.LogInformation("{Action} {Path} {ETag} {RequestId}", "get info", path, response.eTag, GetRequestId(response));
            return response;
        }, cancellationToken);

        return result.responseHeaders;
    }

    public Task<string> GenerateSignalUrl(SignalUrlType urlType, string path, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path;
        var preSignStruct = new PreSignatureStruct
        {
            appid              = _option.AppId,
            region             = _option.Region,
            bucket             = _option.Bucket,
            key                = path,
            httpMethod         = urlType == SignalUrlType.Download ? "GET" : "PUT",
            isHttps            = true,
            signDurationSecond = _option.DurationSecond,
            headers            = null,
            queryParameters    = null
        };

        return Task.Run(() =>
        {
            var url = _cosXml.GenerateSignURL(preSignStruct);
            _logger.LogInformation("{Action} {Path} {Type} {Url}", "generate signal url", path, urlType.ToString("G"), url);
            return url;
        }, cancellationToken);
    }

    public async Task<bool> CreateFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        path = (_option.BasePath + path).TrimEnd('/') + '/';

        var request = new PutObjectRequest(_option.Bucket, path, Array.Empty<byte>());

        var result = await Task.Run(() => _cosXml.PutObject(request), cancellationToken);

        _logger.LogInformation("{Action} {Path} {ETag} {RequestId}", "create folder", path, result.eTag, GetRequestId(result));

        return result.IsSuccessful();
    }

    public async Task<bool> UploadAsync(string path, string localPath, UploadCallback? callback = null, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path.TrimEnd('/');
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
                callback?.Invoke(path, completed, total, cancellationToken);
            }
        };
        var absPath = Path.GetFullPath(localPath);
        uploadTask.SetSrcPath(absPath);

        try
        {
            var result = await manager.UploadAsync(uploadTask);
            _logger.LogInformation("{Action} {Path} {ETag} {RequestId}", "upload", path, result.eTag, GetRequestId(result));
            return result.IsSuccessful();
        }
        catch (CosClientException clientException)
        {
            if (clientException.InnerException is OperationCanceledException e)
            {
                _logger.LogInformation("{Action} {Path}", "upload canceled", path);
                throw e;
            }
            _logger.LogError(clientException, "{Action} {Path}", "upload", path);
            throw new ClientException("upload error", clientException);
        }
        catch (CosServerException serverException)
        {
            _logger.LogError(serverException, "{Action} {Path}", "upload", path);
            throw new ServerException("upload error", serverException);
        }
    }

    public async Task<bool> UploadAsync(string path, byte[] data, UploadCallback? callback = null, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path.TrimEnd('/');
        var request = new PutObjectRequest(_option.Bucket, path, data);
        request.SetCosProgressCallback(delegate(long completed, long total)
        {
            callback?.Invoke(path, completed, total, cancellationToken);
        });

        try
        {
            var result = await Task.Run(() => _cosXml.PutObject(request), cancellationToken);
            _logger.LogInformation("{Action} {Path} {ETag} {RequestId}", "upload", path, result.eTag, GetRequestId(result));

            return result.IsSuccessful();
        }
        catch (CosClientException clientException)
        {
            if (clientException.InnerException is OperationCanceledException e)
            {
                _logger.LogInformation("{Action} {Path}", "upload canceled", path);
                throw e;
            }
            _logger.LogError(clientException, "{Action} {Path}", "upload", path);
            throw new ClientException("upload error", clientException);
        }
        catch (CosServerException serverException)
        {
            _logger.LogError(serverException, "{Action} {Path}", "upload", path);
            throw new ServerException("upload error", serverException);
        }
    }

    public async Task<bool> UploadAsync(string path, Stream stream, UploadCallback? callback = null, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path.TrimEnd('/');
        var request = new PutObjectRequest(_option.Bucket, path, stream);
        request.SetCosProgressCallback(delegate(long completed, long total)
        {
            callback?.Invoke(path, completed, total, cancellationToken);
        });

        try
        {
            var result = await Task.Run(() => _cosXml.PutObject(request), cancellationToken);
            _logger.LogInformation("{Action} {Path} {ETag} {RequestId}", "upload", path, result.eTag, GetRequestId(result));

            return result.IsSuccessful();
        }
        catch (CosClientException clientException)
        {
            if (clientException.InnerException is OperationCanceledException e)
            {
                _logger.LogInformation("{Action} {Path}", "upload canceled", path);
                throw e;
            }
            _logger.LogError(clientException, "{Action} {Path}", "upload", path);
            throw new ClientException("upload error", clientException);
        }
        catch (CosServerException serverException)
        {
            _logger.LogError(serverException, "{Action} {Path}", "upload", path);
            throw new ServerException("upload error", serverException);
        }
    }

    public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        path = _option.BasePath + path;
        var request = new DeleteObjectRequest(_option.Bucket, path);

        var result = await Task.Run(() => _cosXml.DeleteObject(request), cancellationToken);
        _logger.LogInformation("{Action} {Path} {RequestId}", "delete", path, GetRequestId(result));

        return result.IsSuccessful();
    }

    public async Task<int> DeleteAsync(string[] pathArray, CancellationToken cancellationToken = default)
    {
        pathArray = pathArray.Select(path => _option.BasePath + path).ToArray();

        var request = new DeleteMultiObjectRequest(_option.Bucket);
        request.SetDeleteQuiet(false);
        request.SetObjectKeys(pathArray.ToList());

        var result = await Task.Run(() => _cosXml.DeleteMultiObjects(request), cancellationToken);
        _logger.LogInformation("{Action} {PathArray} {RequestId}", "delete", JsonConvert.SerializeObject(pathArray), GetRequestId(result));

        return result.deleteResult.deletedList.Count;
    }
}

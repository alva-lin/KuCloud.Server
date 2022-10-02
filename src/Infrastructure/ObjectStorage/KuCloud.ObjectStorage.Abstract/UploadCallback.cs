namespace KuCloud.ObjectStorage.Abstract;

public delegate void UploadCallback(string path, long completed, long total, CancellationToken cancellationToken);

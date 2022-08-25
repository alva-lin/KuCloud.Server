using KuCloud.Infrastructure.Enums;

namespace KuCloud.Infrastructure.Exceptions;

public class EntityNotFoundException : KuCloudException
{
    public EntityNotFoundException(Type type, object id)
        : base(KuCloudErrorCode.EntityNotFound, 
            $"can't found entity what id = {id} [{type.FullName}]")
    {
        
    }
}

using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Exceptions;

using System.Linq.Expressions;

namespace KuCloud.Core.Exceptions;

public class EntityNotFoundException : BasicException
{
    public EntityNotFoundException(Type type, object id)
        : base(ErrorCode.EntityNotFound,
            $"can't found entity where id = {id} [{type.FullName}]")
    {

    }

    public EntityNotFoundException(Type type, string msg)
        : base(ErrorCode.EntityNotFound, msg)
    {

    }

    public EntityNotFoundException(Type type, Expression<Func<object, bool>> predicate)
        : base(ErrorCode.EntityNotFound, $"can't found entity where {predicate}")
    {
    }
}

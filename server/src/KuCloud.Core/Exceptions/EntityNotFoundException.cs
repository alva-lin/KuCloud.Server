﻿using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Exceptions;

using System.Linq.Expressions;

namespace KuCloud.Core.Exceptions;

public class EntityNotFoundException : BasicException
{
    public EntityNotFoundException(Type type, object id)
        : base(ResponseCode.EntityNotFound,
            $"can't found entity where id = {id} [{type.FullName}]")
    {

    }
    
    public EntityNotFoundException(Type type, Expression<Func<object, bool>> predicate)
        : base(ResponseCode.EntityNotFound, $"can't found entity where {predicate}") {}
}

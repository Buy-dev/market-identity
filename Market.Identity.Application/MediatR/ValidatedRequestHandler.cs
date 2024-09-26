using MediatR;

namespace Market.Identity.Application.MediatR;

public class ValidatedRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, 
{
    
    
    public Task Handle(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
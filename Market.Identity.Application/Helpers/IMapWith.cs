namespace Market.Identity.Application.Helpers;

public interface IMapWith<TSource, TTarget>
{
    public TTarget Map(TSource source);
}
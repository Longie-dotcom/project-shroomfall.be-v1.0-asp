namespace Application.Interface.GrpcClient
{
    public interface IManagementGrpcClient
    {
        Task RequestGameStartupAsync(
            CancellationToken cancellationToken = default);
    }
}
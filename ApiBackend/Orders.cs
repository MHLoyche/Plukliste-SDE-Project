namespace ApiBackend
{
    public class Orders
    {
        public record OrderRequest(string ProductId, int Amount);
    }
}

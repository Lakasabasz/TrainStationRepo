namespace TrainStationsRepo.API;

public record Post(int Id, string Name, string Type)
{
    private List<GatewayDistances> _distances = [];
    public virtual bool Equals(Post? other) => other?.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();

    public void InitGatewayDistances(List<GatewayDistances> distances)
    {
        _distances = distances;
    }
}

public record GatewayDistances(int Direction, Dictionary<Post, int> Costs);
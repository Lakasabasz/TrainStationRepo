namespace TrainStationsRepo.API;

public record Post(int Id, string Name, string Type)
{
    private List<GatewayDistances> _distances = [];
    public IEnumerable<Post> Directions => _distances.Select(x => x.Direction);
    public virtual bool Equals(Post? other) => other?.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();

    public void InitGatewayDistances(List<GatewayDistances> distances)
    {
        _distances = distances;
    }
    
    public Post? FastestDirectionTo(Post end, List<Post> ignoredPosts)
    {
        (Post direction, int value)? minimum = null;
        foreach (var (direction, costs) in _distances)
        {
            if(ignoredPosts.Contains(direction)) continue;
            var cost = costs[end];
            if(cost == int.MaxValue) continue;
            if (direction == end) return end;
            if (minimum is not null && minimum.Value.value <= cost) continue;
            minimum = (direction, cost);
        }
        return minimum?.direction;
    }
}

public record GatewayDistances(Post Direction, Dictionary<Post, int> Costs);
using System.Data;

namespace TrainStationsRepo.API;

public record Post(string Id, string Name, string Type, HashSet<Post> Neighbours)
{
    public virtual bool Equals(Post? other) => other?.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();

    private Dictionary<Post, CostArray>? _costArrays;
    private CostArray? _publicCosts;
    
    public void GenerateCostArrays(List<Post> allPosts)
    {
        Dictionary<Post, int> values = allPosts.ToDictionary(x => x, _ => int.MaxValue);
        _costArrays = Neighbours.ToDictionary(x => x, _ => new CostArray(values.ToDictionary()));
        _publicCosts = new CostArray(values.ToDictionary());
        foreach (var neighbour in Neighbours) _publicCosts.Values[neighbour] = 0;
    }

    public int swaps = 0;
    public bool ProceedMessages()
    {
        bool costsReduced = false;
        swaps = 0;
        foreach (var (direction, directionCosts) in _costArrays!)
        {
            foreach (var (key, neighbourCost) in direction._publicCosts.Values)
            {
                var currentCost = directionCosts.Values[key];
                if(neighbourCost >= currentCost - 1) continue;
                directionCosts.Values[key] = neighbourCost + 1;
                costsReduced = true;
                swaps++;
            }
        }
        
        return costsReduced;
    }

    public void UpdatePublicCosts()
    {
        if (_costArrays.Count == 0) return;
        foreach (var (key, _) in _publicCosts.Values) 
            _publicCosts.Values[key] = _costArrays!.Select(x => x.Value.Values[key]).Min(x => x);
    }
}
using System.Text.Json;
using TrainStationsRepo.API.Models;

namespace TrainStationsRepo.API;

public record CostArray(Dictionary<Post, int> Values);

class World
{
    private static World? _instance;
    public static World Instance => _instance ??= LoadWorldFromFile("map.json", "data.dat");
    
    public List<Post> Posts { get; }

    public World(List<Post> posts)
    {
        Posts = posts;
    }

    private static World LoadWorldFromFile(string descriptionsFilename, string dataFilename)
    {
        var raw = File.ReadAllText(descriptionsFilename);
        var rawDescription = JsonSerializer.Deserialize<List<descriptionPost>>(raw);
        if (rawDescription is null) throw new InvalidDataException("World loading failed");

        List<Post> posts = rawDescription.Select(x => new Post(x.Id, x.Name, x.Type)).OrderBy(x => x.Id).ToList();

        var rawData = File.ReadAllBytes(dataFilename);
        var intsList = Enumerable.Range(0, rawData.Length / 2)
            .Select(i => BitConverter.ToUInt16(rawData, i * 2))
            .ToList();
        var expectedSize = posts.Count + 2;
        if (intsList.Count % expectedSize != 0) throw new InvalidDataException();

        var sets = Enumerable.Range(0, intsList.Count / expectedSize)
            .Select(i => intsList.Skip(i * expectedSize).Take(expectedSize).ToList());
        
        Dictionary<int, List<GatewayDistances>> distancesList = [];
        foreach (var set in sets)
        {
            Dictionary<Post, int> distances = set.Skip(2).Select((value, i) => (value, i))
                .ToDictionary(
                    indexedValue => posts[indexedValue.i],
                    indexedValue => (int)indexedValue.value
                    );
            var gatewayDistances = new GatewayDistances(set[1], distances);
            if(distancesList.ContainsKey(set[0])) distancesList[set[0]].Add(gatewayDistances);
            else distancesList.Add(set[0], [gatewayDistances]);
        }
        
        foreach (var (owner, value) in distancesList) 
            posts.First(x => x.Id == owner).InitGatewayDistances(value);

        var world = new World(posts);
        
        return world;
    }

    public List<List<Post>> GetAlternatives(int first, int last)
    {
        Post start = Posts.First(x => x.Id == first);
        Post end = Posts.First(x => x.Id == last);

        return [[start, end]];
    }
}
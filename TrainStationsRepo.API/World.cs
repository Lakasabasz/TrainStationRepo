using System.Diagnostics;
using System.Text.Json;
using TrainStationsRepo.API.Models;

namespace TrainStationsRepo.API;

public record CostArray(Dictionary<Post, int> Values);

class World
{
    private static World? _instance;
    public static World Instance => _instance ??= LoadWorldFromFile("2.6.json");
    
    public List<Post> Posts { get; }

    public World(List<Post> posts)
    {
        Posts = posts;
    }

    private static World LoadWorldFromFile(string filename)
    {
        var raw = File.ReadAllText(filename);
        var rawData = JsonSerializer.Deserialize<JsonSourceModel>(raw);
        if (rawData is null) throw new InvalidDataException("World loading failed");

        List<Post> posts = new();
        
        foreach (var change in rawData.changes)
        {
            if (change.type == 0)
            {
                if (change.data.name is null || change.data.discriminant is null)
                    throw new InvalidDataException("World loading failed");
                posts.Add(new Post(change.id, change.data.name, change.data.discriminant, new()));
            }
            else
            {
                if (change.relationStart is null || change.relationEnd is null)
                    throw new InvalidDataException("World loading failed");
                var neighbour = posts.First(x => x.Id == change.relationEnd);
                posts.First(x => x.Id == change.relationStart).Neighbours.Add(neighbour);
            }
        }
        
        foreach (var post in posts) post.GenerateCostArrays(posts);

        var world = new World(posts);
        
        world.FillCostsArrays();

        return world;
    }

    private void FillCostsArrays()
    {
        bool needRestart = true;
        int loop = 0;
        
        while (needRestart)
        {
            var sw = new Stopwatch();
            sw.Start();
            loop++;
            List<Task<bool>> tasks = [];
            tasks.AddRange(Posts.Select(post => new Task<bool>(post.ProceedMessages)));
            foreach (var task in tasks) task.Start();
            int resendCounter = tasks.Sum(task => task.Result ? 1 : 0);

            List<Task> tasks2 = [];
            tasks2.AddRange(Posts.Select(x => new Task(x.UpdatePublicCosts)));
            foreach (var task in tasks2) task.Start();
            foreach (var task in tasks2) task.Wait();

            needRestart = resendCounter > 0;
            Console.WriteLine($"Filling arrays => repeat: {loop}, changes: {resendCounter}, eslaped: {sw.Elapsed}, top swaps: {Posts.Max(x => x.swaps)}");
        }
    }

    public List<List<Post>> GetAlternatives(string first, string last)
    {
        Post start = Posts.First(x => x.Id == first);
        Post end = Posts.First(x => x.Id == last);

        return [[start, end]];
    }
}
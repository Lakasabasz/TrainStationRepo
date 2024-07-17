namespace TrainStationsRepo.API.Models;

public record descriptionPost(int Id, string Name, string Type, IEnumerable<int> Neighbours);


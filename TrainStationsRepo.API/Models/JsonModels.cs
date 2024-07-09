namespace TrainStationsRepo.API.Models;

public record JsonSourceModel(
    string id,
    string changeDate,
    string description,
    string model_version,
    Changes[] changes
);

public record Changes(
    string id,
    int type,
    string label,
    int change,
    Data data,
    string? relationStart,
    string? relationEnd
);

public record Data(
    string? discriminant,
    string? name,
    LineInfo? lineInfo
);

public record LineInfo(
    Dictionary<string, double> lineInfo
);


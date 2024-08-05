namespace TrainStationsRepo.API.Models;

enum ErrorCodes
{
    StartSameAsEnd = 1,
    StationsIdInvalid = 2,
    SWW = 69,
}

record ErrorModel(string error, ErrorCodes ecc);

record Stations(IEnumerable<Station> stations);
record Station(int id, string name, string discriminant);
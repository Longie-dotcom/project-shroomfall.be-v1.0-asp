using Contract.Enum.WorldDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.WorldDomain;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Persistence.Seeder
{
    public class RoomJsonDto
    {
        public RoomDto Room { get; set; }
        public List<CellDto> Cells { get; set; }
        public List<SpawnRuleDto> SpawnRules { get; set; }
        public List<SpawnAreaDto> SpawnAreas { get; set; }
    }

    public class RoomDto
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public LocalizedTextDto LocalizedText { get; set; }
    }

    public class LocalizedTextDto
    {
        public string NameKey { get; set; }
        public string DescriptionKey { get; set; }
    }

    public class CellDto
    {
        public string RoomID { get; set; }
        public string TileID { get; set; }
        public int Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public class SpawnRuleDto
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public string RoomID { get; set; }
        public string EntityID { get; set; }
    }

    public class SpawnAreaDto
    {
        public string ID { get; set; }
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public float Weight { get; set; }
        public string EntitySpawnRuleID { get; set; }
    }

    public static class RoomJsonLoader
    {
        public static async Task<RoomJsonDto> LoadAsync()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "room_player_wood.json");

            var json = await File.ReadAllTextAsync(path);

            return JsonSerializer.Deserialize<RoomJsonDto>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public static async Task SeedRoomAsync(RelationalDB db)
        {
            await db.Set<Room>().ExecuteDeleteAsync();

            var dto = await RoomJsonLoader.LoadAsync();

            var room = RoomMapper.ToDomain(dto);

            await db.Set<Room>().AddAsync(room);

            await db.SaveChangesAsync();
        }
    }

    public static class RoomMapper
    {
        public static Room ToDomain(RoomJsonDto dto)
        {
            var room = new Room(
                dto.Room.ID,
                (RoomType)dto.Room.Type,
                new LocalizedText() {
                    NameKey = dto.Room.LocalizedText.NameKey, 
                    DescriptionKey = dto.Room.LocalizedText.DescriptionKey
                }
            );

            foreach (var c in dto.Cells)
            {
                room.Cells.Add(new Cell(
                    c.RoomID,
                    c.TileID,
                    (CellType)c.Type,
                    c.X,
                    c.Y,
                    c.Z
                ));
            }

            foreach (var r in dto.SpawnRules)
            {
                room.EntitySpawnRules.Add(new EntitySpawnRule(
                    r.ID,
                    (SpawnRuleType)r.Type,
                    r.RoomID,
                    r.EntityID
                ));
            }

            foreach (var a in dto.SpawnAreas)
            {
                var rule = room.EntitySpawnRules.First(x => x.ID == a.EntitySpawnRuleID);

                rule.SpawnAreas.Add(new SpawnArea(
                    a.ID,
                    a.MinX,
                    a.MinY,
                    a.MaxX,
                    a.MaxY,
                    a.MinCount,
                    a.MaxCount,
                    a.Weight,
                    a.EntitySpawnRuleID
                ));
            }

            return room;
        }
    }
}
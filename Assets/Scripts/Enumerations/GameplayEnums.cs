
public enum ToolType
{
    Preparing,
    Seeding,
    Watering,
    Harvesting,
}
public enum WeatherType
{
    Sunny = 1,
    Rainy,
    Windy,
}

public enum GroundType
{
    None,
    Grass,
    Path,
    FallowSoil,
    ArableSoil,
}

public enum TilePosition
{
    NotSwappable,
    TopLeft,
    TopMiddle,
    TopRight,
    Left,
    Middle,
    Right,
    BottomLeft,
    BottomMiddle,
    BottomRight,
    ColumnBottom,
    ColumnMiddle,
    ColumnTop,
    RowMiddle,
    RowRight,
    RowLeft,
    Single
}

public enum PlantStage
{
    Seed,
    Vegetating,
    Budding,
    Flowering,
    Ripening,
    Decaying,

    Count,
}

public enum NeedType
{
    Water,
    Fertilizer,
}
public enum Direction
{
    North,
    East,
    South,
    West
}

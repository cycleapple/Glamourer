Build the plugin first, then run:
dotnet run --project regression-tests/Regression.csproj -c Release

The harness loads the actual built plugin and uses its private color-state factory.
It checks category 23 classification, repeated paste baseline retention for both rings,
material reload comparisons, and pending baseline capture. It does not launch the game.
The default API13 Dalamud directory is ../../../dalamud relative to this project.
Override it with -p:DalamudTestPath=/path/to/api13/dalamud when needed.

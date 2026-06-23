using Godot;
using System;
using System.Collections.Generic;

using Godot.Collections;

public struct WaveEnemiesSpawnDesc
{
    public int count;
    public int currentCount;
    public float spawnInterval;
    public float spawnDelay;
    public Data.EnemyType enemyType;
}

public struct WaveStats
{
    public List<WaveEnemiesSpawnDesc> waveEnemies; // List of tuples containing (count, EnemyType)
}

#nullable enable
public partial class WaveManager : Node
{
    List<WaveStats> waves = new List<WaveStats>();

    public int TotalWaves => waves.Count;
    public int CurrentWave { get; private set; } = 0;

    public bool IsWaveActive { get; private set; } = false;
    private double waveTimer = 0.0;

    public event Action<int>? WaveChanged;
    public event Action? WaveFinishedSpawning;
    public event Action<Data.EnemyType>? CreateEnemy; // Event to create an enemy of a specific type

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Read wave data from a JSON file
        LoadWaves();
    }

    public override void _Process(double delta)
    {
        if (!IsWaveActive) return;

        // For testing purposes, you can trigger the next wave with a key press
        waveTimer += delta;

        for (int i = 0; i < waves[CurrentWave].waveEnemies.Count; ++i)
        {
            var spawnDesc = waves[CurrentWave].waveEnemies[i];
            if (spawnDesc.count == 0 || waveTimer < spawnDesc.spawnDelay || spawnDesc.currentCount >= spawnDesc.count) continue;

            double timeSinceDelayFinished = waveTimer - spawnDesc.spawnDelay;

            if (timeSinceDelayFinished < spawnDesc.spawnInterval * spawnDesc.currentCount) continue;

            // Spawn an enemy of the specified type
            CreateEnemy?.Invoke(spawnDesc.enemyType);
            ++spawnDesc.currentCount;
            waves[CurrentWave].waveEnemies[i] = spawnDesc; // Update the count
        }

        bool allEnemiesSpawned = true;
        for (int i = 0; i < waves[CurrentWave].waveEnemies.Count; ++i)
        {
            var spawnDesc = waves[CurrentWave].waveEnemies[i];
            if (spawnDesc.currentCount < spawnDesc.count)
            {
                allEnemiesSpawned = false;
                break;
            }
        }

        // Check if all enemies for the current wave have been spawned
        if (allEnemiesSpawned)
        {
            IsWaveActive = false;
            WaveFinishedSpawning?.Invoke();
        }
    }

    private void LoadWaves()
    {
        // Load the JSON file
        using var jsonFile = FileAccess.Open("res://Data/waves.json", FileAccess.ModeFlags.Read);
        if (jsonFile == null || !jsonFile.IsOpen())
        {
            GD.PrintErr("Failed to load waves.json");
            return;
        }

        // Read the file content
        string jsonString = jsonFile.GetAsText();
        jsonFile.Close();

        // Parse the JSON data
        Json json = new Json();
        Error error = json.Parse(jsonString);
        if (error != Error.Ok)
        {
            GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} at line {json.GetErrorLine()}");
            return;
        }

        // Convert the JSON data into WaveStats objects
        Array<Dictionary> waveArray = json.Data.AsGodotArray<Dictionary>();
        foreach (var waveDict in waveArray)
        {
            var waveStats = new WaveStats();
            waveStats.waveEnemies = new List<WaveEnemiesSpawnDesc>();

            var enemiesArray = waveDict["enemies"].As<Godot.Collections.Array>();
            foreach (var enemyData in enemiesArray)
            {
                var enemyDict = enemyData.As<Dictionary>();
                var spawnDesc = new WaveEnemiesSpawnDesc
                {
                    count = (int)enemyDict["count"],
                    spawnInterval = (float)enemyDict["spawnInterval"],
                    spawnDelay = (float)enemyDict["spawnDelay"],
                    enemyType = (Data.EnemyType)Enum.Parse(typeof(Data.EnemyType), (string)enemyDict["enemyType"])
                };
                waveStats.waveEnemies.Add(spawnDesc);
            }

            waves.Add(waveStats);
        }

        GD.Print($"Loaded {waves.Count} waves from waves.json");
    }

    public void StartWaves()
    {
        ResetWaves();
        IsWaveActive = true;
        GD.Print($"Starting wave {CurrentWave + 1}");
        WaveChanged?.Invoke(CurrentWave);
    }

    public bool HasMoreWaves()
    {
        return CurrentWave < TotalWaves - 1;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public bool StartNextWave()
    {
        if (!HasMoreWaves())
        {
            GD.Print("All waves completed!");
            return false; // No more waves to start
        }

        IsWaveActive = true;
        CurrentWave++;
        WaveChanged?.Invoke(CurrentWave);
        waveTimer = 0;
        return true;
    }

    public void ResetWaves()
    {
        CurrentWave = 0;
        WaveChanged?.Invoke(CurrentWave);
        for (int i = 0; i < waves.Count; ++i)
        {
            for (int j = 0; j < waves[i].waveEnemies.Count; ++j)
            {
                var spawnDesc = waves[i].waveEnemies[j];
                spawnDesc.currentCount = 0;
                waves[i].waveEnemies[j] = spawnDesc;
            }
        }
        waveTimer = 0;
    }
}

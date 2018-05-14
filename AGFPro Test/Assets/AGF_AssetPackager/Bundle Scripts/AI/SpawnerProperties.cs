using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerProperties : TileProperties {


	public GameObject enemyPrefab;
	public bool spawnInfinite;
	public int numberOfSpawns = 1;
	public int respawnDelay;
	public int spawnDistance = 35;
	public int spawnedEnemyHealth = 100;
	public int spawnedEnemySightRange = 25;

	public string spawnText;
}

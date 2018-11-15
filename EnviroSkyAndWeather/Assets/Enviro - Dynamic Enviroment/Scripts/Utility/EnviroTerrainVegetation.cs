using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviroTerrainVegetation : MonoBehaviour {

	public Terrain terrain;
	public TerrainData tempTerrain;

	public int springDetailIndex = 0;
	public int summerDetailIndex = 1;
	public int autumnDetailIndex = 2;
	public int winterDetailIndex = 3;

	private int[,] DetailLayerSpring;
	private int[,] DetailLayerSummer;
	private int[,] DetailLayerAutumn;
	private int[,] DetailLayerWinter;

	private int[,] noneDetailLayer;

	void Start () 
	{
		if (terrain == null)
		{
			Debug.LogError("Please assign a terrain!");
			return;
		}
		if (tempTerrain == null)
		{
			Debug.LogError("Please copy your TerrainData and assign it!");
			return;
		}
		if (terrain.terrainData.detailPrototypes.Length < springDetailIndex)
		{
			Debug.LogError("Please assign a create your seasonsonal details in terrain and assign correct index for each season!");
			return;
		}
		if (terrain.terrainData.detailPrototypes.Length < summerDetailIndex)
		{
			Debug.LogError("Please assign a create your seasonsonal details in terrain and assign correct index for each season!");
			return;
		}
		if (terrain.terrainData.detailPrototypes.Length < autumnDetailIndex)
		{
			Debug.LogError("Please assign a create your seasonsonal details in terrain and assign correct index for each season!");
			return;
		}
		if (terrain.terrainData.detailPrototypes.Length < winterDetailIndex)
		{
			Debug.LogError("Please assign a create your seasonsonal details in terrain and assign correct index for each season!");
			return;
		}
			
		//save default detaillayer data.
		DetailLayerSpring = terrain.terrainData.GetDetailLayer (0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, springDetailIndex);
		DetailLayerSummer = terrain.terrainData.GetDetailLayer (0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, summerDetailIndex);
		DetailLayerAutumn = terrain.terrainData.GetDetailLayer (0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, autumnDetailIndex);
		DetailLayerWinter = terrain.terrainData.GetDetailLayer (0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, winterDetailIndex);
		noneDetailLayer = terrain.terrainData.GetDetailLayer (0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, springDetailIndex);
		//setup a Layer with zero details.
		for (int y = 0; y < terrain.terrainData.detailHeight; y++) {
			for (int x = 0; x < terrain.terrainData.detailWidth; x++) {
					noneDetailLayer[x, y] = 0;
				}
			}

		terrain.terrainData = tempTerrain;

		EnviroSky.instance.OnSeasonChanged += (EnviroSeasons.Seasons season) =>
		{
			UpdateSeason ();
		};

		UpdateSeason ();
	}

	void UpdateSeason ()
	{
		switch (EnviroSky.instance.Seasons.currentSeasons)
		{
		case EnviroSeasons.Seasons.Spring:
			terrain.terrainData.SetDetailLayer (0, 0, springDetailIndex, DetailLayerSpring);
			terrain.terrainData.SetDetailLayer (0, 0, summerDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, autumnDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, winterDetailIndex, noneDetailLayer);
			break; 

		case EnviroSeasons.Seasons.Summer:
			terrain.terrainData.SetDetailLayer (0, 0, springDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, summerDetailIndex, DetailLayerSummer);
			terrain.terrainData.SetDetailLayer (0, 0, autumnDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, winterDetailIndex, noneDetailLayer);
			break; 

		case EnviroSeasons.Seasons.Autumn:
			terrain.terrainData.SetDetailLayer (0, 0, springDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, summerDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, autumnDetailIndex, DetailLayerAutumn);
			terrain.terrainData.SetDetailLayer (0, 0, winterDetailIndex, noneDetailLayer);
			break;

		case EnviroSeasons.Seasons.Winter:
			terrain.terrainData.SetDetailLayer (0, 0, springDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, summerDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, autumnDetailIndex, noneDetailLayer);
			terrain.terrainData.SetDetailLayer (0, 0, winterDetailIndex, DetailLayerWinter);
			break; 
		}

	}
}

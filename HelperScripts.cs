using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperScripts : MonoBehaviour
{
    // Start is called before the first frame update
    static Terrain[] _terrains = Terrain.activeTerrains;

    public static Terrain GetClosestCurrentTerrain(Vector3 playerPos)
    {
        //Get the closest one to the player
        playerPos.y = 0;
        var center = new Vector3(_terrains[0].transform.position.x + _terrains[0].terrainData.size.x / 2, playerPos.y, _terrains[0].transform.position.z + _terrains[0].terrainData.size.z / 2);
        float lowDist = (center - playerPos).sqrMagnitude;
        var terrainIndex = 0;

        for (int i = 0; i < _terrains.Length; i++)
        {
            center = new Vector3(_terrains[i].transform.position.x + _terrains[i].terrainData.size.x / 2, playerPos.y, _terrains[i].transform.position.z + _terrains[i].terrainData.size.z / 2);

            //Find the distance and check if it is lower than the last one then store it
            var dist = (center - playerPos).sqrMagnitude;
            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }
        return _terrains[terrainIndex];
    }

    public static Vector3 MouseToWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float distToGround = -1f;
        ground.Raycast(ray, out distToGround);
        Vector3 worldPos = ray.GetPoint(distToGround);
        return worldPos;
    }

    


}

using UnityEngine;

namespace BeerNBooze
{
    [DisallowMultipleComponent]
    public class BeerSpawner : MonoBehaviour
    {
        [SerializeField] private Beer beerPrefab = null;

        [ContextMenu("SpawnBeer")]
        public Beer SpawnBeer(float speed, Transform pickUpArea, float size, float destroyHeight)
        {
            Beer beer = Instantiate<Beer>(beerPrefab);
            beer.transform.position = new Vector3(transform.position.x,
                                                  transform.position.y + beerPrefab.transform.position.y,
                                                  transform.position.z);
            beer.beerGraphic.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(-180, 180));
            beer.movementSpeed = speed;
            beer.pickUpArea = pickUpArea;
            beer.size = size;
            beer.destructionHeight = destroyHeight;

            return beer;
        }
    }
}

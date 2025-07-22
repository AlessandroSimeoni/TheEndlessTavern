using UnityEngine;

namespace Tournament
{
    public class TournamentGameOver : MonoBehaviour
    {
        [SerializeField] private GameObject[] elementToBeEnabled = null;
        [SerializeField] private GameObject[] elementToBeDisabled = null;

        public void ToggleElements()
        {
            foreach (GameObject go in elementToBeDisabled)
                go.SetActive(false);

            foreach (GameObject go in elementToBeEnabled)
                go.SetActive(true);
        }
    }
}

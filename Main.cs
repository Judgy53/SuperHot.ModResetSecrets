using UnityEngine;

namespace Mod
{
    public class Main
    {
        public const string GAO_NAME = "ModResetSecrets";

        public void Start()
        {
            if (GameObject.Find(GAO_NAME) == null)
            {
                GameObject gameObject = new GameObject(GAO_NAME);
                gameObject.AddComponent<ModResetSecrets>();
                Object.DontDestroyOnLoad(gameObject);
            }
        }
    }
}

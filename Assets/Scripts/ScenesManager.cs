using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesManager
{
    public static void GoTo(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}

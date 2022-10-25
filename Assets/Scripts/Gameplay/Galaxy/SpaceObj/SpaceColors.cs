using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceColors : MonoBehaviour
{
    public static SpaceColors main;

    [Header("Colors")]
    public Color Black;
    [SerializeField]
    public Color Blue;
    [SerializeField]
    public Color BlueWhite;
    [SerializeField]
    public Color White;
    [SerializeField]
    public Color WhiteYellow;
    [SerializeField]
    public Color Yellow;
    [SerializeField]
    public Color Orange;
    [SerializeField]
    public Color Red;
    [SerializeField]
    public Color Brown;

    private void Start()
    {
        main = this;
    }
}
